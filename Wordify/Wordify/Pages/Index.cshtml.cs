using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wordify.Data;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using Wordify.Data.json;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using Wordify.Extensions;
using Wordify.Models.Interfaces;
using Wordify.Models;

namespace Wordify.Pages
{
    public class IndexModel : PageModel
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private  IBlob _blob;
        private INote _note;
        public static IConfiguration Configuration;

        [BindProperty]
        public string ImageFilePath { get; set; }

        [BindProperty]
        public string ResponseContent { get; set; }

        [BindProperty]
        public string FileName { get; set; }

        [BindProperty]
        public string Title { get; set; }

        [BindProperty]
        public IFormFile FormFile { get; set; }

        public string ResponseString { get; set; }
        public byte[] ByteData { get; set; }


        public IndexModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, 
            IConfiguration configuration, IBlob blob, INote note)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            Configuration = configuration;
            _blob = blob;
            _note = note;
        }

        /// <summary>
        /// OnGet - Runs on page Load
        /// </summary>
        public void OnGet()
        {
        }
        
        /// <summary>
        /// OnPost - Runs when Submit button is pressed
        /// </summary>
        public void OnPost()
        {
            if (FormFile != null)
            {
                ReadHandwrittenText(FormFile).Wait();
            }
            else
            {
                TempData["Error"] = "No File Uploaded.";
            }
        }


        /// <summary>
        /// ReadHandWrittenText - Takes in the uploaded image and, if able, sends it to the API to be converted.
        /// </summary>
        /// <param name="formFile">form from the front end</param>
        /// <returns></returns>
        public async Task ReadHandwrittenText(IFormFile formFile)
        {
            try
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", Configuration["CognitiveServices:subscriptionKey"]);

                string uri = $"{Configuration["CognitiveServices:uriBase"]}?mode=Handwritten";
                HttpResponseMessage response;
                string operationLocation;
                GetImageAsByteArray(formFile);
                using (ByteArrayContent content = new ByteArrayContent(ByteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    response = await client.PostAsync(uri, content);
                }

                if (response.IsSuccessStatusCode)
                {
                    operationLocation = response.Headers.GetValues("Operation-Location").FirstOrDefault();
                }
                else
                {
                    string errorString = await response.Content.ReadAsStringAsync();
                    return;
                }

                string contentString;
                int i = 0;
                do
                {
                    System.Threading.Thread.Sleep(1000);
                    response = await client.GetAsync(operationLocation);
                    contentString = await response.Content.ReadAsStringAsync();
                    ++i;
                }
                while (i < 10 && contentString.IndexOf("\"status\":\"Succeeded\"") == -1);

                if (i == 10 && contentString.IndexOf("\"status\":\"Succeeded\"") == -1)
                {
                    Console.WriteLine("Timeout");
                    return;
                }

                ResponseContent = JToken.Parse(contentString).ToString();
                RootObject ImageText = JsonParse(contentString);
                List<Line> Lines = FilteredJson(ImageText);
                ResponseString = TextString(Lines);

                ImageDisplayExtensions.DisplayImage(ByteData);

                if(_signInManager.IsSignedIn(User))
                {
                    await SaveNoteAsync();
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Something went wrong.";
            }
        }


        /// <summary>
        /// SaveNoteAsync - Saves the current note into Note Database and Uploads Image and Response text to Blob storage.
        /// </summary>
        /// <returns></returns>
        public async Task SaveNoteAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            Note note = new Note()
            {
                UserID = user.Id,
                Date = DateTime.Now,
                Title = Title,
                BlobLength = ByteData.Length,
            };

            await _blob.Upload(note, ResponseString, ByteData);
            await _note.CreateNote(note);
        }


        /// <summary>
        /// JsonParse - Takes in the JSON string and deserialises it based on the RootObject.
        /// </summary>
        /// <param name="jsonString">the JSON string</param>
        /// <returns>Deserialized root object</returns>
        public static RootObject JsonParse(string jsonString)
        {
            RootObject ImageText = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(jsonString);
            return ImageText;
        }


        /// <summary>
        /// FilerteredJson - Filters JSON object into lines of text.
        /// </summary>
        /// <param name="ImagePath"></param>
        /// <returns></returns>
        public static List<Line> FilteredJson(RootObject ImagePath)
        {
            Convertedjson displayJson = new Convertedjson();

            var lines = from l in ImagePath.recognitionResult.lines
                        where l.text != null
                        select l;

            List<Line> text = lines.ToList();      
            return text;
        }


        /// <summary>
        /// TextString - Takes in the List of Lines taken from the JSON and uses string builder to 
        ///     combine them into a single string.
        /// </summary>
        /// <param name="text"> The list of Lines found on the JSON.</param>
        /// <returns> The combined lines as a string.</returns>
        public static string TextString( List<Line> text)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in text)
            {
                sb.AppendLine(item.text);
            }

            return sb.ToString();
        }


        /// <summary>
        /// GetImageAsByteArray - Takes in the image from the front end, and turns it into a byte array so it can 
        ///     be stored and used.
        /// </summary>
        /// <param name="formFile">from the front end input</param>
        public void GetImageAsByteArray(IFormFile formFile)
        {
            ByteData = new byte[formFile.Length];
            using (var ms = new MemoryStream())
            {
                formFile.CopyTo(ms);
                ByteData = ms.ToArray();
            }
        }
    }
}

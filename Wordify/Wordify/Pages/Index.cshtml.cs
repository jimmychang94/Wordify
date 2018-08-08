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


        public IndexModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, 
            IConfiguration configuration, IBlob blob, INote note)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            Configuration = configuration;
            _blob = blob;
            _note = note;
        }

        public void OnGet()
        {
        }
        


        //take in the FormFile from the frontend and start off the process of sending it to the API
        public void OnPost()
        {
            if (FormFile.Length > 0)
            {
                ReadHandwrittenText(FormFile).Wait();
            }
            //if(System.IO.File.Exists(ImageFilePath))
            //{
            //    FileName = Path.GetFileName(ImageFilePath);
            //    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", FileName);
            //    ReadHandwrittenText(ImageFilePath).Wait();
            //    using (var stream = new FileStream(path, FileMode.Create))
            //    {
            //        await FormFile.CopyToAsync(stream);
            //    }
            //}
            else
            {
                // file does not exist
            }
        }

        /// <summary>
        /// Takes in the uploaded image and, if able, sends it to the API to be converted
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

                string requestParameters = "mode=Handwritten";

                string uri = $"{Configuration["CognitiveServices:uriBase"]}?{requestParameters}";

                HttpResponseMessage response;

                string operationLocation;

                byte[] byteData = GetImageAsByteArray(formFile);

                using (ByteArrayContent content = new ByteArrayContent(byteData))
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

                ImageDisplayExtensions.DisplayImage(byteData);

                if(_signInManager.IsSignedIn(User))
                {
                    var user = await _userManager.GetUserAsync(User);
                    Note note = new Note()
                    {
                        UserID = user.Id,
                        Date = DateTime.Now,
                        Title = Title,
                        BlobLength = byteData.Length
                    };
                    await _blob.Upload(note, ResponseString, byteData);
                    _note.CreateNote(note);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// takes in the JSON string and deserialises it based on the RootObject
        /// </summary>
        /// <param name="jsonString">the JSON string</param>
        /// <returns>Deserialized root object</returns>
        public static RootObject JsonParse(string jsonString)
        {
            RootObject ImageText = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(jsonString);
            return ImageText;
        }

        //filter RootObject down to a list of lines.text
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
        /// takes in the List of Lines taken from the JSON and uses string builder to 
        /// combine them into a single string 
        /// </summary>
        /// <param name="text">the list of Lines found on the JSOn</param>
        /// <returns>the combined lines as a string</returns>
        public static string TextString( List<Line> text)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in text)
            {
                sb.AppendLine(item.text);
            }

            string returnString = sb.ToString();

            return returnString;
        }

        /// <summary>
        /// takes in the image from the front end, and turns it into a byte array so it can 
        /// be stored and used.
        /// </summary>
        /// <param name="formFile">from the front end input</param>
        /// <returns>image as a byte array</returns>
        public static byte[] GetImageAsByteArray(IFormFile formFile)
        {
            //using (FileStream fileStream =
            //    new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            //{
            //    BinaryReader binaryReader = new BinaryReader(fileStream);
            //    return binaryReader.ReadBytes((int)fileStream.Length);
            //}
            byte[] fileBytes = new byte[formFile.Length];
            using (var ms = new MemoryStream())
            {
                formFile.CopyTo(ms);
                fileBytes = ms.ToArray();
                return fileBytes;
            }
        }
    }
}

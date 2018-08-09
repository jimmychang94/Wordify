using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wordify.Data;
using Wordify.Extensions;
using Wordify.Models;
using Wordify.Models.Interfaces;

namespace Wordify.Pages
{
    public class NotesModel : PageModel
    {
        private INote _note;
        private IBlob _blob;
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;

        [BindProperty]
        public Note Note { get; set; }

        [BindProperty]
        public string Text { get; set; }

        public NotesModel(INote note, IBlob blob, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _note = note;
            _blob = blob;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public void OnGet(int id)
        {
            Note note = _note.GetNoteByID(id).Result;
            string blobText = _blob.GetText(note).Result;
            Note = note;
            Text = blobText;
            byte[] blobImage = _blob.GetImage(note).Result;
            ImageDisplayExtensions.DisplayImage(blobImage);
        }
    }
}
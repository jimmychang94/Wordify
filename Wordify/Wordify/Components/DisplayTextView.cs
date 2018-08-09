using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wordify.Data;
using Wordify.Extensions;
using Wordify.Models;
using Wordify.Models.Interfaces;

namespace Wordify.Components
{
    public class DisplayTextView : ViewComponent
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private INote _note;
        private IBlob _blob;

        public DisplayTextView(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            INote note, IBlob blob)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _note = note;
            _blob = blob;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            Note note = await _note.GetNoteByID(id);
            string blobText = await _blob.GetText(note);
            DisplayViewModel dvm = new DisplayViewModel()
            {
                Note = note,
                Text = blobText
            };
            byte[] blobImage = await _blob.GetImage(note);
            ImageDisplayExtensions.DisplayImage(blobImage);

            return View(dvm);
        }
    }

}

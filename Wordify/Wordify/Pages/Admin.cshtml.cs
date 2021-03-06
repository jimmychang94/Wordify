﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wordify.Data;
using Wordify.Extensions;
using Wordify.Models;
using Wordify.Models.Interfaces;

namespace Wordify.Pages
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminModel : PageModel
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private INote _note;
        private IBlob _blob;

        [BindProperty]
        public List<ApplicationUser> Users { get; set; }

        [BindProperty]
        public List<AdminViewModel> AVMs { get; set; }

        [BindProperty]
        public Note Note { get; set; }

        [BindProperty]
        public string Text { get; set; }


        public AdminModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            INote note, IBlob blob)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _note = note;
            _blob = blob;
            AVMs = new List<AdminViewModel>();
        }

       
        public void OnGet()
        {
            Users = _userManager.Users.ToList();
            List<Note> Notes = _note.GetAllNotes().Result;
            foreach(Note note in Notes)
            {
                var user = _userManager.FindByIdAsync(note.UserID).Result;
                AVMs.Add(new AdminViewModel()
                {
                    Note = note,
                    UserName = user.UserName
                });
            }
        }
        /// <summary>
        /// based on a users ID, an admin can remove them from the database
        /// </summary>
        /// <param name="id">users ID</param>
        /// <returns>admin page with one less user</returns>
        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);
            return RedirectToPage("/Admin");
        }

        /// <summary>
        /// delete a note based on its ID
        /// </summary>
        /// <param name="id">ID of note to be delted</param>
        /// <returns>admin page</returns>
        public IActionResult OnPostDeleteNote(int id)
        {
            Note note = _note.GetNoteByID(id).Result;
            _blob.DeleteBlob(note);
            _note.DestroyNote(id);
            return RedirectToPage("/Admin");
        }

        /// <summary>
        /// post a note
        /// </summary>
        /// <param name="id">ID of note</param>
        public void OnPostNote(int id)
        {
            try
            {
                Note note = _note.GetNoteByID(id).Result;
                string blobText = _blob.GetText(note).Result;
                Note = note;
                Text = blobText;
                byte[] blobImage = _blob.GetImage(note).Result;
                ImageDisplayExtensions.DisplayImage(blobImage);
                Users = _userManager.Users.ToList();
                List<Note> Notes = _note.GetAllNotes().Result;
                foreach (Note tempNote in Notes)
                {
                    var user = _userManager.FindByIdAsync(tempNote.UserID).Result;
                    AVMs.Add(new AdminViewModel()
                    {
                        Note = tempNote,
                        UserName = user.UserName
                    });
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Something went wrong with the Post Note";
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
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
    /// <summary>
    /// This makes sure the user is logged in to view this page.
    /// It also allows us to manage the profile page from here rather than a controller.
    /// </summary>
    [Authorize (Policy = "MemberOnly")]
    public class ProfileModel : PageModel
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private INote _notes;
        private IBlob _blob;

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Bio")]
        public string Bio { get; set; }

        [BindProperty]
        public List<Note> Notes { get; set; }

        [BindProperty]
        public NoteCardViewModel Ncvm { get; set; }

        public ProfileModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            INote notes, IBlob blob)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _notes = notes;
            _blob = blob;
        }

        /// <summary>
        /// Launch users profile page
        /// </summary>
        /// <returns>profile page</returns>
        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);

            UserName = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Bio = user.Bio;
            Email = user.Email;
            Notes = await _notes.GetNotesByUserID(user.Id);
            return Page();
        }

        /// <summary>
        /// Update user's own account info
        /// </summary>
        /// <returns>relaunch the page</returns>
        public async Task<IActionResult> OnPostUser()
        {
            var user = await _userManager.GetUserAsync(User);

            user.FirstName = FirstName;
            user.LastName = LastName;
            user.Bio = Bio;
            await _userManager.UpdateAsync(user);

            List<Claim> oldClaims = new List<Claim>();
            List<Claim> newClaims = new List<Claim>();

            oldClaims.Add(User.Claims.FirstOrDefault(c => c.Type == "FullName"));
            newClaims.Add(new Claim("FullName", $"{user.FirstName} {user.LastName}"));

            await _userManager.RemoveClaimsAsync(user, oldClaims);
            await _userManager.AddClaimsAsync(user, newClaims);

            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(user, false);
            return RedirectToPage();
        }

        /// <summary>
        /// User views one of their notes based on its ID
        /// </summary>
        /// <param name="id">ID of note the user wants to view</param>
        public void OnPostNote(int id)
        {
            var user = _userManager.GetUserAsync(User).Result;

            UserName = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Bio = user.Bio;
            Email = user.Email;
            Notes = _notes.GetNotesByUserID(user.Id).Result;
            try
            {
                Note note = _notes.GetNoteByID(id).Result;
                if (!Notes.Contains(note))
                {
                    throw new Exception("You are not the owner of that note");
                }
                string blobText = _blob.GetText(note).Result;
                Ncvm.Note = note;
                Ncvm.Text = blobText;
                Ncvm.Title = note.Title;
                byte[] blobImage = _blob.GetImage(note).Result;
                ImageDisplayExtensions.DisplayImage(blobImage);
            }
            catch (Exception)
            {
                TempData["Error"] = "Something went wrong with accessing that note";
            }
        }


        /// <summary>
        /// User deletes their note based on the notes ID
        /// </summary>
        /// <param name="id">ID of not to be deleted</param>
        /// <returns>profile page with one less note, or error</returns>
        public IActionResult OnPostDeleteNote(int id)
        {
            var user = _userManager.GetUserAsync(User).Result;
            Note note = _notes.GetNoteByID(id).Result;
            if (note.UserID != user.Id)
            {
                TempData["Error"] = "You are not the owner of that note";
                return Page();
            }
            _blob.DeleteBlob(note);
            _notes.DestroyNote(id);
            return RedirectToPage("/Profile");
        }

        /// <summary>
        /// Response to user updating their note
        /// </summary>
        /// <param name="id">ID of note the user wants to update</param>
        /// <returns>page with updated note or error</returns>
        public IActionResult OnPostUpdate(int id)
        {
            var user = _userManager.GetUserAsync(User).Result;

            UserName = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Bio = user.Bio;
            Email = user.Email;
            Notes = _notes.GetNotesByUserID(user.Id).Result;
            try
            {
                Note note = _notes.GetNoteByID(id).Result;
                if (!Notes.Contains(note))
                {
                    throw new Exception("You are not the owner of that note");
                }
                note.Title = Ncvm.Title;
                _notes.UpdateNote(note, id);
                _blob.UpdateText(note, Ncvm.Text);
            }
            catch (Exception)
            {
                TempData["Error"] = "You are not the owner of that note";
                return Page();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostClose()
        {
            return RedirectToPage();
        }
    }
}
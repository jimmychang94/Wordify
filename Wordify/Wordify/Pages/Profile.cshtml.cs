using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wordify.Data;

namespace Wordify.Pages
{
    /// <summary>
    /// This makes sure the user is logged in to view this page.
    /// It also allows us to manage the profile page from here rather than a controller.
    /// </summary>
    [Authorize]
    public class ProfileModel : PageModel
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;

        [BindProperty]
        public string FirstName { get; set; }

        [BindProperty]
        public string LastName { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string UserName { get; set; }

        [BindProperty]
        public string Bio { get; set; }


        public ProfileModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;

        }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);

            UserName = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Bio = user.Bio;
            Email = user.Email;
            return Page();
        }

        public async Task<IActionResult> OnPost()
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
            return RedirectToPage("/Index");
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Wordify.Pages
{
    public class IndexModel : PageModel
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;

        public void OnGet()
        {

        }

        public Task<IActionResult> OnPost()
        {
            var user = await _userManager.GetUserAsync(User);


        }
    }
}

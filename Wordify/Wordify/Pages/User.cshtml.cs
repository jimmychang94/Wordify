using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Wordify.Pages
{
    public class UserModel : PageModel
    {
        public string Notes { get; set; }
        public int test { get; set; }

        public void OnGet()
        {
            
        }
    }
}
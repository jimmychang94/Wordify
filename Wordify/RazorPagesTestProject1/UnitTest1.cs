using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Wordify.Data;
using Wordify.Models.Interfaces;
using Wordify.Pages;
using Xunit;

namespace RazorPagesTestProject1
{
    public class UnitTest1
    {
        private static UserManager<ApplicationUser> _userManager;
        private static SignInManager<ApplicationUser> _signInManager;
        private static IBlob _blob;
        private static INote _note;
        public static IConfiguration Configuration;


        //[Fact]
        //public async void IndexOnPostTest()
        //{
        //    var optionBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
        //        .UseInMemoryDatabase("InMemoryDb");

        //    using (var db = new ApplicationDbContext(optionBuilder.Options))
        //    {
        //        var physicalFile = new FileInfo("../../../Assets/Test7.jpg");
        //        IFormFile form = physicalFile.AsMockIFormFile();

        //        var something2 = new IndexModel(_userManager, _signInManager, Configuration, _blob, _note);

        //        await something2.ReadHandwrittenText(form);
        //        string response = "writing\r\nA\r\nPM\r\nOf\r\nAPI\r\nzor Pages\r\nM $ 1\r\n";
        //        Assert.Equal( response, something2.ResponseString);
        //    };           
        //}

        [Fact]
        public void IndexSaveNoteAsyncTest()
        {


        }

        //Profile Page//

        [Fact]
        public void ProfileOnGetTest()
        {


        }

        [Fact]
        public void ProfileOnPostUserTest()
        {


        }

        [Fact]
        public void ProfileOnPostNoteTest()
        {


        }



    }
}

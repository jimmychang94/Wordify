using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Wordify.Data;
using Wordify.Models;
using Wordify.Models.Interfaces;
using Xunit;


namespace RazorPagesTestProject1
{
    public class DevNoteTest
    {

        public static IConfiguration Configuration;


        [Fact]
        public async void CreateNoteTest()
        {
            var optionBuilder = new DbContextOptionsBuilder<DataDbContext>()
               .UseInMemoryDatabase(Guid.NewGuid().ToString());


            using (var db = new DataDbContext(optionBuilder.Options))
            {
                DevNote _note = new DevNote(db, Configuration);
                Note note = new Note();

                await _note.CreateNote(note);
                List<Note> notes = await db.Notes.ToListAsync();

                Assert.Single(notes);
            }
        }


        [Fact]
        public async void DestroyNoteTest()
        {
            var optionBuilder = new DbContextOptionsBuilder<DataDbContext>()
    .UseInMemoryDatabase(Guid.NewGuid().ToString());


            using (var db = new DataDbContext(optionBuilder.Options))
            {
                DevNote _note = new DevNote(db, Configuration);
                Note note = new Note();

                await _note.CreateNote(note);

                 _note.DestroyNote(1);
                List<Note> notes = await db.Notes.ToListAsync();


                Assert.Empty(notes);
            }
        }

        [Fact]
        public async void UpdateNoteTest()
        {
            var optionBuilder = new DbContextOptionsBuilder<DataDbContext>()
    .UseInMemoryDatabase(Guid.NewGuid().ToString());


            using (var db = new DataDbContext(optionBuilder.Options))
            {
                DevNote _note = new DevNote(db, Configuration);
                Note note = new Note()
                {
                    Title = "Hi"
                };

                await _note.CreateNote(note);

                Note newNote = new Note()
                {
                    Title = "Bye"
                };
                 _note.UpdateNote(newNote, 1);
                List<Note> notes = await db.Notes.ToListAsync();

                Assert.Equal("Bye", notes[0].Title);
            }
        }


        [Fact]
        public void GetNotesByUserIDTest()
        {

        }

        [Fact]
        public void GetAllNotesTest()
        {

        }

        [Fact]
        public void GetNoteByIDTest()
        {

        }
    }
}

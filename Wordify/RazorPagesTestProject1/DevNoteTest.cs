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
                                .UseInMemoryDatabase("thatdb");

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
                                .UseInMemoryDatabase("thisdb");

            using (var db = new DataDbContext(optionBuilder.Options))
            {
                DevNote _note = new DevNote(db, Configuration);
                Note note = new Note();

                await _note.CreateNote(note);

                _note.DestroyNote(note.ID);
                List<Note> notes = await db.Notes.ToListAsync();


                Assert.Empty(notes);
            }
        }

        [Fact]
        public async void UpdateNoteTest()
        {
            var optionBuilder = new DbContextOptionsBuilder<DataDbContext>()
                                .UseInMemoryDatabase("mydb");


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
                _note.UpdateNote(newNote, note.ID);
                List<Note> notes = await db.Notes.ToListAsync();

                Assert.Equal("Bye", notes[0].Title);
            }
        }

        [Fact]
        public async void GetNotesByUserIDTest()
        {
            var optionBuilder = new DbContextOptionsBuilder<DataDbContext>()
                                .UseInMemoryDatabase("yourdb");

            using (var db = new DataDbContext(optionBuilder.Options))
            {
                DevNote _note = new DevNote(db, Configuration);
                Note note = new Note()
                {
                    UserID = "UserID"
                };
                Note note2 = new Note()
                {
                    UserID = "UserID"
                };
                Note note3 = new Note()
                {
                    UserID = "23423"
                };
                await _note.CreateNote(note);
                await _note.CreateNote(note2);
                await _note.CreateNote(note3);

                List<Note> notesUser  = await _note.GetNotesByUserID("UserID");

                Assert.Equal(2, notesUser.Count);
            }
        }

        [Fact]
        public async void GetAllNotesTest()
        {
            var optionBuilder = new DbContextOptionsBuilder<DataDbContext>()
                            .UseInMemoryDatabase("yourdb");

            using (var db = new DataDbContext(optionBuilder.Options))
            {
                DevNote _note = new DevNote(db, Configuration);
                Note note = new Note()
                {
                    UserID = "UserID"
                };
                Note note2 = new Note()
                {
                    UserID = "UserID"
                };
                Note note3 = new Note()
                {
                    UserID = "23423"
                };

                foreach (var item in db.Notes)
                {
                     _note.DestroyNote(item.ID);
                }

                await _note.CreateNote(note);
                await _note.CreateNote(note2);
                await _note.CreateNote(note3);

                List<Note> notesUser = await _note.GetAllNotes();

                Assert.Equal(3, notesUser.Count);
            }
        }

        [Fact]
        public async void GetNoteByIDTest()
        {
            var optionBuilder = new DbContextOptionsBuilder<DataDbContext>()
                          .UseInMemoryDatabase("thedb");
            using (var db = new DataDbContext(optionBuilder.Options))
            {
                DevNote _note = new DevNote(db, Configuration);
                Note note = new Note()
                {
                    UserID = "UserID"
                };
                Note note2 = new Note()
                {
                    UserID = "UserID"
                };
                Note note3 = new Note()
                {
                    UserID = "23423"
                };
                await _note.CreateNote(note);
                await _note.CreateNote(note2);
                await _note.CreateNote(note3);

                Note notesUser = await _note.GetNoteByID(note2.ID);

                Assert.Equal(note2.ID, notesUser.ID);
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wordify.Data;
using Wordify.Models.Interfaces;

namespace Wordify.Models
{
    public class DevNote : INote
    {
        private DataDbContext _context;

        private readonly IConfiguration Configuration;

        public DevNote(DataDbContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        /// <summary>
        /// saves a note do the database
        /// </summary>
        /// <param name="newNote">note provided by front end</param>
        /// <returns>nothing</returns>
        public async Task CreateNote(Note newNote)
        {
            await _context.Notes.AddAsync(newNote);
            await _context.SaveChangesAsync();
        }

        //Find a Note by its ID and delete it
        public void DestroyNote(int ID)
        {
            Note note =  _context.Notes.FirstOrDefault(n => n.ID == ID);
            if(note != null)
            {
                _context.Notes.Remove(note);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// gets all of the notes out of the DB
        /// </summary>
        /// <returns>All of the notes</returns>
        public async Task<List<Note>> GetAllNotes()
        {
            List<Note> allNotes = await _context.Notes.ToListAsync();

            return allNotes;
        }

        /// <summary>
        /// Find a note based on its ID
        /// </summary>
        /// <param name="ID">ID of note you want</param>
        /// <returns>the note with the ID or an exception</returns>
        public async Task<Note> GetNoteByID(int ID)
        {
            Note note = await _context.Notes.FirstOrDefaultAsync(n => n.ID == ID);
            if (note != null)
            {
                return note;
            }
            throw new Exception("No note found");
        }

        /// <summary>
        /// Returns every note from the DB that is attached to the provided UserID
        /// </summary>
        /// <param name="userID">ID of the user who's notes you want</param>
        /// <returns>a list of their notes</returns>
        public async Task<List<Note>> GetNotesByUserID(string userID)
        {
            List<Note> userNotes = await _context.Notes.Where(n => n.UserID == userID).ToListAsync();

            return userNotes;
        }

        /// <summary>
        /// takes in a new, altered note, finds its old one based on its ID, and then updates it
        /// </summary>
        /// <param name="note">new note with content to be changed</param>
        /// <param name="ID">ID of note to be changed</param>
        public void UpdateNote(Note note, int ID)
        {
            Note oldNote = _context.Notes.FirstOrDefault(n => n.ID == ID);

            if(oldNote != null)
            {
                oldNote.Title = note.Title;
                _context.Notes.Update(oldNote);
                 _context.SaveChanges();
            }
        }
    }
}

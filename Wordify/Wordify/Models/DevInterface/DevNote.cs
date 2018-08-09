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

        public async Task CreateNote(Note newNote)
        {
            await _context.Notes.AddAsync(newNote);
            await _context.SaveChangesAsync();
        }

        public async void DestroyNote(int ID)
        {
            Note note = await _context.Notes.FirstOrDefaultAsync(n => n.ID == ID);
            if(note != null)
            {
                _context.Notes.Remove(note);
               await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Note>> GetAllNotes()
        {
            List<Note> allNotes = await _context.Notes.ToListAsync();

            return allNotes;
        }

        public async Task<Note> GetNoteByID(int ID)
        {
            Note note = await _context.Notes.FirstOrDefaultAsync(n => n.ID == ID);
            if (note != null)
            {
                return note;
            }
            throw new Exception("No note found");
        }

        public async Task<List<Note>> GetNotesByUserID(string userID)
        {
            List<Note> userNotes = await _context.Notes.Where(n => n.UserID == userID).ToListAsync();

            return userNotes;
        }


        public async void UpdateNote(Note note, int ID)
        {
            Note oldNote = await _context.Notes.FirstOrDefaultAsync(n => n.ID == ID);

            if(oldNote != null)
            {
                oldNote.Title = note.Title;
                _context.Notes.Update(oldNote);
                await _context.SaveChangesAsync();
                System.Threading.Thread.Sleep(500); //purely for testing. probably will refactor

            }
        }
    }
}

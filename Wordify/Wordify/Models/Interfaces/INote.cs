using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wordify.Models.Interfaces
{
    public interface INote
    {
        void CreateNote(Note newNote);
        void DestroyNote(int ID);
        void UpdateNote(Note note, int ID);
        Task<List<Note>> GetNotesByUserID(string userID);
        Task<List<Note>> GetAllNotes();
        Task<Note> GetNoteByID(int ID);
    }
}

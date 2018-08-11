using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wordify.Models
{
    public class NoteCardViewModel
    {
        public Note Note { get; set; }

        public string Text { get; set; } // Used to populate the textarea

        public byte[] ByteData { get; set; } // Used to display the image

        public string Title { get; set; }
    }
}

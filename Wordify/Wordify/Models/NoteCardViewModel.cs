using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wordify.Models
{
    public class NoteCardViewModel
    {
        public Note Note { get; set; }

        public string Text { get; set; }

        public byte[] ByteData { get; set; }

        public string Title { get; set; }
    }
}

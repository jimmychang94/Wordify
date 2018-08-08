using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wordify.Models
{
    public class Note
    {
        public int ID { get; set; }
        public string BlobName { get; set; } // link to location in blob storage
        public int BlobLength { get; set; }
        public string UserID { get; set; }
        public DateTime Date { get; set; } 
    }
}

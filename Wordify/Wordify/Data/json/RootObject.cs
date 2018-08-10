using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wordify.Data.json
{
    public class RootObject
    {
        public string status { get; set; }
        public RecognitionResult recognitionResult { get; set; }
    }
}

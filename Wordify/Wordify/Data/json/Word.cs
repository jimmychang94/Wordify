using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wordify.Data.json
{
    public class Word
    {
        public List<int> boundingBox { get; set; }
        public string text { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wordify.Data.json
{
    public class Line
    {
        public List<int> boundingBox { get; set; }
        public string text { get; set; }
        public List<Word> words { get; set; }
    }
}

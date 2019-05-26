using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HW_06.Models
{
    public class Quotes
    {
        public string Quote { get; set; }
        public string Author { get; set; }
    }

    public class Contents
    {
        public IEnumerable<Quotes> Quotes { get; set; }
    }

    public class QuoteResponse
    {
        public Contents Contents { get; set; }
    }
}

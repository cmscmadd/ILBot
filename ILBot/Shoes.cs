using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILBot
{
    [Serializable]
    public class Shoes
    {/// <summary>
     /// /test
     /// </summary>
        public string Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string ContentUrl { get; set; }
        public string ContentType { get; set; }
        public string Link { get; set; }
        public string Category { get; set; }

        public double Price { get; set; }
        public string Color { get; set; }
    }
}

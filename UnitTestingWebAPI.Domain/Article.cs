using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestingWebAPI.Domain
{
    public class Article
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }
        public string Author { get; set; }
        public string URL { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateEdited { get; set; }

        public int BlogID { get; set; }
        public virtual Blog Blog { get; set; }

        public Article()
        {
        }
    }
}

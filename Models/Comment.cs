using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HW_06.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public string UserName { get; set; }
        public int BlogId { get; set; }
        public BlogPost BlogPost { get; set; }


        public Comment()
        {
        }
    }
}

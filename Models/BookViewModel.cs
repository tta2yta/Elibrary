using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELibrary.Models
{
    public class BookViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Language { get; set; }
        public string ISBN { get; set; }
        public string Image { get; set; }
        public string File { get; set; }
        public string FileName { get; set; }
        public int Size { get; set; }
    }
}
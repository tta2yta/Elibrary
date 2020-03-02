using ELibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELibrary.Models
{
    public class BasicInformation
    {
        public Book Book { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Language> Languages { get; set; }
    }
}
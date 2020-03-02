using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELibrary.Entities
{
    [Table("Book")]
    public class Book
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(255)]
        [Required(ErrorMessage = "Please Enter Title")]
        public string Title { get; set; }

        public string ISBN { get; set; }
        public int CategoryId { get; set; }
        public int LanguageId { get; set; }
        public Category Category { get; set; }
        public Language Language { get; set; }
        public virtual ICollection<FileDetail> FileDetails { get; set; }
        public virtual ICollection<Author> Authors { get; set; }
    }
}
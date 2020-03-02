using ELibrary.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELibrary.Entities
{
    [Table("FileDetail")]
    public class FileDetail
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int FileSize { get; set; }

        [Required]
        [MaxLength(255)]
        public string Extension { get; set; }

        public FileType? FileType { get; set; }
        public int BookId { get; set; }
        public virtual Book Book { get; set; }
    }
}
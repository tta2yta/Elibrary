using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ELibrary.Models
{
    public class FileInformation
    {
        [Display(Name = "Upload File")]
        // [ValidateFile]
        public HttpPostedFileBase File { get; set; }

        [Display(Name = "Upload Image")]
        // [ValidateImage]
        public HttpPostedFileBase Image { get; set; }
    }
}
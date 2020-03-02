using ELibrary.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ELibrary.Models
{
    public class BookFormViewModel
    {
        public Book Book { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Language> Languages { get; set; }


        [Display(Name = "Upload File")]
        // [ValidateFile]
        public HttpPostedFileBase File { get; set; }

        [Display(Name = "Upload Image")]
        // [ValidateImage]
        public HttpPostedFileBase Image { get; set; }

    }

    //Customized data annotation validator for uploading file
    public class ValidateFileAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            int MaxContentLength = 1024 * 1024 * 3; //3 MB
            string[] AllowedFileExtensions = new string[] { ".pdf" };

            var file = value as HttpPostedFileBase;


            if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower()))
            {
                ErrorMessage = "Please upload Your File of type: " + string.Join(", ", AllowedFileExtensions);
                return false;
            }
            else if (file.ContentLength > MaxContentLength)
            {
                ErrorMessage = "Your File is too large, maximum allowed size is : " + (MaxContentLength / 1024).ToString() + "MB";
                return false;
            }
            else
                return true;
        }
    }
    //Customized data annotation validator for uploading file
    public class ValidateImageAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            int MaxContentLength = 1024 * 1024 * 3; //3 MB
            string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png", "jpeg" };

            var file = value as HttpPostedFileBase;

            /* if (file == null)
                 return false;*/

            if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower()))
            {
                ErrorMessage = "Please upload am image of type: " + string.Join(", ", AllowedFileExtensions);
                return false;
            }
            else if (file.ContentLength > MaxContentLength)
            {
                ErrorMessage = "Your Photo is too large, maximum allowed size is : " + (MaxContentLength / 1024).ToString() + "MB";
                return false;
            }
            else
                return true;
        }
    }
}
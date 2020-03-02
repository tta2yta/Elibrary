//using ELibrary.Entities;
//using ELibrary.Models;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace ELibrary.Controllers
//{
//    public class BookWizardController : Controller
//    {
//        // GET: BookWizard
//        public ActionResult Index()
//        {
//            return View("BasicInformation");
//        }

//        private Book GetBook()
//        {
//            if (Session["book"] == null)
//            {
//                Session["book"] = new Book();
//            }
//            return (Book)Session["book"];
//        }

//        private void RemoveBook()
//        {
//            Session.Remove("book");
//        }

//        [HttpPost]
//        public ActionResult BasicInformation(BasicInformation data, string prevBtn, string nextBtn)
//        {
//            if (nextBtn != null)
//            {
//                if (ModelState.IsValid)
//                {
//                    Book obj = GetBook();
//                    obj.Book.Title = data.Book.Title;
//                    obj.Book.ISBN = data.Book.ISBN;
//                    obj.Book.CategoryId = data.Book.CategoryId;
//                    obj.Book.LanguageId = data.Book.LanguageId;
//                    return View("FileInformation");
                   
//                }
//            }
//            return View();
//        }

//        [HttpPost]
//        public ActionResult FileInformation(FileInformation data,string prevBtn, string nextBtn)
//        {
//            BookFormViewModelWizard obj = GetBook();
//            if (prevBtn != null)
//            {
//                BasicInformation binfo = new BasicInformation();
//                binfo.Book.Title = obj.Book.Title;
//                binfo.Book.ISBN = obj.Book.ISBN;
//                binfo.Book.CategoryId = obj.Book.CategoryId;
//                binfo.Book.LanguageId = obj.Book.LanguageId;
//                return View("BasicDetails", binfo);
//            }
//            if (nextBtn != null)
//            {
//                if (ModelState.IsValid)
//                {
//                    List<FileDetail> fileDetails = new List<FileDetail>();

//                    if (data.File != null && data.File.ContentLength > 0)
//                    {
//                        var fileName = Path.GetFileName(data.File.FileName);

//                        var bookFileDetail = new FileDetail
//                        {
//                            Id = Guid.NewGuid(),
//                            FileName = fileName,
//                            Extension = Path.GetExtension(fileName),
//                            FileSize = data.File.ContentLength,
//                            FileType = FileType.Pdf
//                        };

//                        fileDetails.Add(bookFileDetail);
//                        obj.pdfFilePath = Path.Combine(Server.MapPath("~/Uploads/"), bookFileDetail.Id + "-" + fileName);
//                        //bookViewModel.File.SaveAs(pdfFilePath);
//                        return View("AuthorInformaion");
//                    }

//                    if (data.Image != null && data.Image.ContentLength > 0)
//                    {
//                        var imageName = Path.GetFileName(data.Image.FileName);

//                        var imageFileDetail = new FileDetail
//                        {
//                            Id = Guid.NewGuid(),
//                            FileName = imageName,
//                            Extension = Path.GetExtension(imageName),
//                            FileSize = data.Image.ContentLength,
//                            FileType = FileType.Photo
//                        };

//                        fileDetails.Add(imageFileDetail);
//                        obj.imageFilePath = Path.Combine(Server.MapPath("~/Uploads/"), imageFileDetail.Id + "-" + imageName);
//                        //bookViewModel.Image.SaveAs(imageFilePath);
//                    }
//                }
//            }
//            return View();
//        }
//}
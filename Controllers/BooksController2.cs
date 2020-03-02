using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ELibrary.Entities;
using ELibrary.Models;
using System.IO;

namespace ELibrary.Controllers
{
    public class BooksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Books
        public ActionResult Index()
        {
            var books = db.Books.Include(b => b.Category).Include(b => b.Language);
            return View(books.ToList());
        }

        // GET: Books/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Include(b => b.FileDetails).SingleOrDefault(b => b.Id == id);
            if (book == null)
            {
                return HttpNotFound();
            }

            /*var file = book.FileDetails.Where(f => f.FileType == FileType.Pdf).Select(f => new { f.FileName, f.Id }).First();
            var image = book.FileDetails.Where(f => f.FileType == FileType.Photo).Select(f => new { f.FileName, f.Id }).First();
            ViewBag.ImageAddress = image.Id + "-" + image.FileName;
            string strImage = image.Id + "-" + image.FileName;
            ViewBag.FileAddress = file.Id + "-" + file.FileName;
            string strFile = file.Id + "-" + file.FileName;
            var bookDetails = (from b in db.Books.Include(b => b.FileDetails).Where(b => b.Id == id)
                               join category in db.Categories on b.CategoryId equals category.Id
                               join language in db.Languages on b.LanguageId equals language.Id
                               select new
                               {
                                   Id = b.Id,
                                   Title = b.Title,
                                   Category = category.Name,
                                   Language = language.Name,
                                   ISBN = b.ISBN,
                                   Image = strImage,
                                   File = strFile
                               }
                               ).ToList();*/

            var bookView = (from b in db.Books.Where(b => b.Id == id)
                            join fd in db.FileDetails on b.Id equals fd.BookId
                            join c in db.Categories on b.CategoryId equals c.Id
                            join l in db.Languages on b.LanguageId equals l.Id
                            select new
                            {
                                Id = b.Id,
                                Title = b.Title,
                                Category = c.Name,
                                Language = l.Name,
                                ISBN = b.ISBN,
                                FileDetailId = fd.Id,
                                FileName = fd.FileName,
                                File = fd.Id + "-" + fd.FileName,
                                FieType = fd.FileType,
                                FileSize = fd.FileSize
                            }).ToList();

            var model = new BookViewModel
            {
                Id = bookView.First().Id,
                Title = bookView.First().Title,
                Category = bookView.First().Category,
                Language = bookView.First().Language,
                ISBN = bookView.First().ISBN,
            };
            foreach (var item in bookView)
            {
                if (item.FieType == FileType.Pdf)
                {
                    model.FileName = item.FileName;
                    model.File = item.File;
                    model.Size = bookView.First().FileSize;

                }
                else
                {
                    model.Image = item.File;
                }

            }

            // return View(book);
            return View(model);
        }

        // GET: Books/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            ViewBag.LanguageId = new SelectList(db.Languages, "Id", "Name");
            return View();
        }

        public ActionResult UploadBook()
        {
            var categories = db.Categories.ToList();
            var languages = db.Languages.ToList();

            var viewModel = new BookFormViewModel()
            {
                Categories = categories,
                Languages = languages
            };
            return View("UploadBook", viewModel);
        }

        [HttpPost]
        public ActionResult SaveAs(BookFormViewModel bookViewModel)
        {
            //Check server side validation using data annotation
            try
            {
                if (ModelState.IsValid)
                {
                    List<FileDetail> fileDetails = new List<FileDetail>();

                    //Uploading the Book file (pdf file)
                    if (bookViewModel.File != null && bookViewModel.File.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(bookViewModel.File.FileName);

                        var bookFileDetail = new FileDetail
                        {
                            Id = Guid.NewGuid(),
                            FileName = fileName,
                            Extension = Path.GetExtension(fileName),
                            FileSize = bookViewModel.File.ContentLength,
                            FileType = FileType.Pdf

                        };

                        fileDetails.Add(bookFileDetail);
                        var path = Path.Combine(Server.MapPath("~/Uploads/"), bookFileDetail.Id + "-" + fileName);
                        bookViewModel.File.SaveAs(path);
                    }

                    //Uploading the Book image file
                    if (bookViewModel.Image != null && bookViewModel.Image.ContentLength > 0)
                    {
                        var imageName = Path.GetFileName(bookViewModel.Image.FileName);

                        var imageFileDetail = new FileDetail
                        {
                            Id = Guid.NewGuid(),
                            FileName = imageName,
                            Extension = Path.GetExtension(imageName),
                            FileSize = bookViewModel.Image.ContentLength,
                            FileType = FileType.Photo
                        };

                        fileDetails.Add(imageFileDetail);
                        var path = Path.Combine(Server.MapPath("~/Uploads/"), imageFileDetail.Id + "-" + imageName);
                        bookViewModel.Image.SaveAs(path);
                    }
                    /*var book = new Book
                    {
                        Title = bookViewModel.Book.Title,
                        ISBN = bookViewModel.Book.ISBN,
                        CategoryId = bookViewModel.Book.CategoryId,
                        LanguageId = bookViewModel.Book.LanguageId,

                    };*/

                    bookViewModel.Book.FileDetails = fileDetails;
                    db.Books.Add(bookViewModel.Book);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }

            var categories = db.Categories.ToList();
            var languages = db.Languages.ToList();

            var viewModel = new BookFormViewModel()
            {
                Categories = categories,
                Languages = languages
            };
            return View("UploadBook", viewModel);
        }
        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,ISBN,CategoryId,LanguageId")] Book book, HttpPostedFileBase file, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                List<FileDetail> fileDetails = new List<FileDetail>();

                //Getting the Book file (pdf file)
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);

                    var bookFileDetail = new FileDetail
                    {
                        Id = Guid.NewGuid(),
                        FileName = fileName,
                        Extension = Path.GetExtension(fileName),
                        FileSize = file.ContentLength,
                        FileType = FileType.Pdf

                    };

                    fileDetails.Add(bookFileDetail);
                    var path = Path.Combine(Server.MapPath("~/Uploads/"), bookFileDetail.Id + "-" + fileName);
                    file.SaveAs(path);

                }
                if (image != null && image.ContentLength > 0)
                {
                    var imageName = Path.GetFileName(image.FileName);

                    var imageFileDetail = new FileDetail
                    {
                        Id = Guid.NewGuid(),
                        FileName = imageName,
                        Extension = Path.GetExtension(imageName),
                        FileSize = 20,
                        FileType = FileType.Photo
                    };

                    fileDetails.Add(imageFileDetail);
                    var path = Path.Combine(Server.MapPath("~/Uploads/"), imageFileDetail.Id + "-" + imageName);
                    image.SaveAs(path);

                }

                book.FileDetails = fileDetails;
                db.Books.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", book.CategoryId);
            ViewBag.LanguageId = new SelectList(db.Languages, "Id", "Name", book.LanguageId);
            return View(book);
        }

        // GET: Books/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Include(b => b.FileDetails).SingleOrDefault(b => b.Id == id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", book.CategoryId);
            ViewBag.LanguageId = new SelectList(db.Languages, "Id", "Name", book.LanguageId);
            return View(book);
        }

        //Second Edit method
        public ActionResult Edit2(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Include(b => b.FileDetails).SingleOrDefault(b => b.Id == id);
            if (book == null)
            {
                return HttpNotFound();
            }
            var categories = db.Categories.ToList();
            var languages = db.Languages.ToList();

            var viewModel = new BookFormViewModel()
            {
                Book = book,
                Categories = categories,
                Languages = languages
            };
            return View("Edit2", viewModel);
        }

        [HttpPost]
        public ActionResult Save(BookFormViewModel bookViewModel)
        {
            if (ModelState.IsValid)
            {
                var bookInDB = db.Books.Include(b => b.FileDetails).Where(b => b.Id == bookViewModel.Book.Id).Single();
                var tempFileDetails = bookInDB.FileDetails;
                if (bookInDB.FileDetails.Count >= 0)
                {
                    if (bookViewModel.File != null)
                    {
                        //Delete the pdf record in the database and update with the new content
                        FileDetail fileDetail = bookInDB.FileDetails.Where(f => f.FileType == FileType.Pdf).FirstOrDefault();

                        //file still exists in database and file system
                        if (fileDetail != null)
                        {
                            var id = fileDetail.Id;
                            var fileName = fileDetail.FileName;
                            var fileInDisk = id + "-" + fileName;

                            //Remove from database
                            db.FileDetails.Remove(fileDetail);
                            db.SaveChanges();

                            //Remove from file system

                            var path = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }
                        }
                        var newFileName = Path.GetFileName(bookViewModel.File.FileName);

                        var bookFileDetail = new FileDetail
                        {
                            Id = Guid.NewGuid(),
                            FileName = newFileName,
                            Extension = Path.GetExtension(newFileName),
                            FileSize = bookViewModel.File.ContentLength,
                            FileType = FileType.Pdf

                        };

                        tempFileDetails.Add(bookFileDetail);
                        var newPath = Path.Combine(Server.MapPath("~/Uploads/"), bookFileDetail.Id + "-" + newFileName);
                        bookViewModel.File.SaveAs(newPath);
                    }

                    if (bookViewModel.Image != null)
                    {
                        //Delete the image record in the database and update with the new content
                        FileDetail fileDetail = bookInDB.FileDetails.Where(f => f.FileType == FileType.Photo).FirstOrDefault();

                        //file still exists in database and file system
                        if (fileDetail != null)
                        {
                            var id = fileDetail.Id;
                            var fileName = fileDetail.FileName;
                            var fileInDisk = id + "-" + fileName;

                            //Remove from database
                            db.FileDetails.Remove(fileDetail);
                            db.SaveChanges();

                            //Remove from file system

                            var path = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }
                        }
                        var newImageName = Path.GetFileName(bookViewModel.Image.FileName);

                        var imageFileDetail = new FileDetail
                        {
                            Id = Guid.NewGuid(),
                            FileName = newImageName,
                            Extension = Path.GetExtension(newImageName),
                            FileSize = bookViewModel.Image.ContentLength,
                            FileType = FileType.Photo

                        };

                        tempFileDetails.Add(imageFileDetail);
                        var newPath = Path.Combine(Server.MapPath("~/Uploads/"), imageFileDetail.Id + "-" + newImageName);
                        bookViewModel.Image.SaveAs(newPath);
                    }
                    bookInDB.FileDetails = tempFileDetails;
                }
                // Update the contents of the Book Header
                bookInDB.Title = bookViewModel.Book.Title;
                bookInDB.ISBN = bookViewModel.Book.ISBN;
                bookInDB.CategoryId = bookViewModel.Book.CategoryId;
                bookInDB.LanguageId = bookViewModel.Book.LanguageId;

                //Save content to the database
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            return View("Edit2", bookViewModel);
        }
        [HttpPost]
        public JsonResult DeleteFile(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Result = "Error" });
            }
            try
            {
                Guid guid = new Guid(id);
                FileDetail fileDetail = db.FileDetails.Find(guid);
                var fileName = fileDetail.Id + "-" + fileDetail.FileName;
                if (fileDetail == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { Result = "Error" });
                }

                //Remove from database
                db.FileDetails.Remove(fileDetail);
                db.SaveChanges();

                //Delete file from the file system
                var path = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        // Action method used to dowload a file
        public FileResult Download(String p, String d)
        {
            return File(Path.Combine(Server.MapPath("~/Uploads/"), p), System.Net.Mime.MediaTypeNames.Application.Octet, d);
        }
        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,ISBN,CategoryId,LanguageId")] Book book, HttpPostedFileBase file, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", book.CategoryId);
            ViewBag.LanguageId = new SelectList(db.Languages, "Id", "Name", book.LanguageId);
            return View(book);
        }

        // GET: Books/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Book book = db.Books.Find(id);
            db.Books.Remove(book);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

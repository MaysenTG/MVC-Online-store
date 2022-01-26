using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Lab_Example.Models;
using assignment_2_20010223.Models;
using System.Web.Helpers;
using Lab_Example;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace assignment_2_20010223.Controllers
{
    public class ProductImagesController : Controller
    {
        private StoreContext db = new StoreContext();

        // GET: ProductImages
        public ActionResult Index()
        {
            return View(db.ProductImages.ToList());
        }

        // GET: ProductImages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductImage productImage = db.ProductImages.Find(id);
            if (productImage == null)
            {
                return HttpNotFound();
            }
            return View(productImage);
        }

        // GET: ProductImages/Upload
        public ActionResult Upload()
        {
            return View();
        }

        // POST: ProductImages/Upload
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase[] files)
        {
            bool allValid = true;
            string inValidFiles = "";
            if (files[0] != null)
            {
                if(files.Length <= 10)
                {
                    foreach(var file in files)
                    {
                        if(!ValidateFile(file))
                        {
                            allValid = false;
                            inValidFiles = ", " + file.FileName;
                        }
                    }
                    if(allValid)
                    {
                        foreach(var file in files)
                        {
                            try
                            {
                                SaveFileToDisk(file);
                            }

                            catch (Exception)
                            {
                                ModelState.AddModelError("Filename", "Sorry, an error occured saving the file to the disk: ");
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("Filename", "All files must be a gif, jpg, jpeg, png or less than 2mb");
                }
            }
            else
            {
                ModelState.AddModelError("Filename", "Please upload a file");
            }


            if (ModelState.IsValid)
            {
                bool duplicates = false;
                bool otherDbError = false;
                string duplicateFiles = "";

                foreach (var file in files)
                {
                    var productToAdd = new ProductImage { FileName = file.FileName };

                    try
                    {
                        db.ProductImages.Add(productToAdd);
                        db.SaveChanges();
                    } catch (DbUpdateException e)
                    {
                        SqlException innerException = e.InnerException.InnerException as SqlException;

                        if(innerException != null && innerException.Number == 2601)
                        {
                            duplicateFiles += ", " + file.FileName;
                            duplicates = true;
                        } else
                        {
                            otherDbError = true;
                        }
                    }

                }
                if (duplicates)
                {
                    ModelState.AddModelError("Filename", "All files uploaded except " + duplicateFiles + ", which already exist in the database");
                    return View();
                }
                else if (otherDbError)
                {
                    ModelState.AddModelError("Filename", "Sorry, an error occured saving to the database");
                    return View();
                }


                
                return RedirectToAction("Index");
            }

            return View();
        }

        // GET: ProductImages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductImage productImage = db.ProductImages.Find(id);
            if (productImage == null)
            {
                return HttpNotFound();
            }
            return View(productImage);
        }

        // POST: ProductImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FileName")] ProductImage productImage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productImage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productImage);
        }

        // GET: ProductImages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductImage productImage = db.ProductImages.Find(id);
            if (productImage == null)
            {
                return HttpNotFound();
            }
            return View(productImage);
        }

        // POST: ProductImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductImage productImage = db.ProductImages.Find(id);
            db.ProductImages.Remove(productImage);
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

        private bool ValidateFile(HttpPostedFileBase file)
        {
            string fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();
            string[] allowedFileTypes = { ".gif", ".png", ".jpeg", ".jpg" };

            // Bigger than 0 and smaller than 2mb
            if(file.ContentLength > 0 && file.ContentLength < 2097152 && allowedFileTypes.Contains(fileExtension))
            {
                return true;
            }
            return false;
        }

        private void SaveFileToDisk(HttpPostedFileBase file)
        {
            WebImage img = new WebImage(file.InputStream);

            if(img.Width > 200)
            {
                img.Resize(200, img.Height);
            }
            img.Save(Constants.ProductImagePath + file.FileName);

            if (img.Width > 100 || img.Height > 200)
            {
                img.Resize(100, 100);
            }
            img.Save(Constants.ProductThumbnailPath + file.FileName);
        }
    }
}

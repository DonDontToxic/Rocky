using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rocky_DataAccess.Data;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Ultility;

namespace Rocky.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        
        // GET
        public IActionResult Index()
        {
            IEnumerable<Product> objList = _db.Product
                .Include(u=>u.Category)
                .Include(u=>u.ApplicationType);

            // foreach (var obj in objList)
            // {
            //     obj.Category = _db.Category.FirstOrDefault(
            //         u => u.Id == obj.CategoryId);
            //     obj.ApplicationType = _db.ApplicationType.FirstOrDefault(
            //         u => u.Id == obj.ApplicationId);
            // };
            
            return View(objList);
        }
        
        // GET - UPSERT
        public IActionResult Upsert(int? id)
        {
            
            // IEnumerable<SelectListItem> CategoryDropDown = _db.Category.Select(i => new SelectListItem
            // {
            //     Text = i.Name,
            //     Value = i.Id.ToString()
            // });
            // ViewBag.CategoryDropDown = CategoryDropDown;
            // var product = new Product();
            
            var productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _db.Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                ApplicationSelectList = _db.ApplicationType.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
            
            if (id == null)
            {
                // This is for create
                return View(productVM);
            }
            
            // This is for update
            productVM.Product = _db.Product.Find(id);
            if (productVM.Product == null) return NotFound();
            return View(productVM);
        }
        // POST - UPSERT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                productVM.CategorySelectList = _db.Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                productVM.ApplicationSelectList = _db.ApplicationType.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                return View(productVM);
            };
            var files = HttpContext.Request.Form.Files;
            var webRootPath = _webHostEnvironment.WebRootPath;

            if (productVM.Product.Id == 0)
            {
                // Create
                var upload = webRootPath + WC.ImagePath;
                var fileName = Guid.NewGuid().ToString();
                var extension = Path.GetExtension(files[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(upload,
                    fileName + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                };
                productVM.Product.Image = fileName + extension;
                _db.Product.Add(productVM.Product);
                _db.SaveChanges();
            }
            else
            {
                // Update
                var objFromDb = _db.Product.AsNoTracking().FirstOrDefault(
                    u => u.Id == productVM.Product.Id);
                if (files.Count > 0)
                {
                    var upload = webRootPath + WC.ImagePath;
                    var fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(files[0].FileName);
                    
                    // Handle the old file
                    if (objFromDb != null)
                    {
                        var oldFile = Path.Combine(upload, objFromDb.Image);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }
                    }

                    // Handle the new uploaded file
                    using (var fileStream = new FileStream(Path.Combine(upload,
                        fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    };
                    productVM.Product.Image = fileName + extension;
                }
                else
                { 
                    if (objFromDb != null) productVM.Product.Image = objFromDb.Image;
                }
                _db.Product.Update(productVM.Product);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null) return NotFound();
            
            var product = _db.Product
                .Include(u=>u.Category)
                .Include(u=>u.ApplicationType)
                .FirstOrDefault(u=>u.Id == id); // Only find for the primary key
            if (product == null) return NotFound();
            
            return View(product);        
        }
        
        // POST - DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var product = _db.Product.Find(id);
            if (product == null) return NotFound();
            
            // Handle the product image on local
            var webRootPath = _webHostEnvironment.WebRootPath;
            var upload = webRootPath + WC.ImagePath;
            var oldFile = Path.Combine(upload, product.Image);
            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }
            // Handle the product on db
            _db.Product.Remove(product);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
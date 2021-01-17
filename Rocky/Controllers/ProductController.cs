using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;

namespace Rocky.Controllers
{
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
            IEnumerable<Product> objList = _db.Product;

            foreach (var obj in objList)
            {
                obj.Category = _db.Category.FirstOrDefault(
                    u => u.Id == obj.CategoryId);
            };
            
            

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
                })
            };
            
            if (id == null)
            {
                // This is for create
                return View(productVM);
            }
            else
            {
                // This is for update
                productVM.Product = _db.Product.Find(id);
                if (productVM.Product == null) return NotFound();
                return View(productVM);
            }
            return View();
        }
        // POST - UPSERT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (!ModelState.IsValid) return View(productVM);
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

        public IActionResult Delete()
        {
            throw new System.NotImplementedException();
        }
    }
}
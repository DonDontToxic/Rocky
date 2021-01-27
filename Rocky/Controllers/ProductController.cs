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
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Ultility;

namespace Rocky.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _prodRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IProductRepository db, IWebHostEnvironment webHostEnvironment)
        {
            _prodRepo = db;
            _webHostEnvironment = webHostEnvironment;
        }
        
        // GET
        public IActionResult Index()
        {
            IEnumerable<Product> objList = _prodRepo.GetAll(
                includeProperties: "Category,ApplicationType");


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
                CategorySelectList = _prodRepo.GetAllDropDownList(WC.CategoryName),
                ApplicationSelectList = _prodRepo.GetAllDropDownList(WC.ApplicationTypeName),
            };
            
            if (id == null)
            {
                // This is for create
                return View(productVM);
            }
            
            // This is for update
            productVM.Product = _prodRepo.Find(id.GetValueOrDefault());
            if (productVM.Product == null)
            {
                TempData[WC.Error] = "Action failed...Please try again!!!";

                return NotFound();
            }

            return View(productVM);
        }
        // POST - UPSERT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                productVM.CategorySelectList = _prodRepo.GetAllDropDownList(WC.CategoryName);
                productVM.ApplicationSelectList = _prodRepo.GetAllDropDownList(WC.ApplicationTypeName);
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
                _prodRepo.Add(productVM.Product);
                _prodRepo.Save();
                TempData[WC.Success] = "Product created successfully";
            }
            else
            {
                // Update
                var objFromDb = _prodRepo.FirstOrDefault(
                    u => u.Id == productVM.Product.Id, isTracking:false);
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
                _prodRepo.Update(productVM.Product);
                _prodRepo.Save();
                TempData[WC.Success] = "Product edited successfully";

            }
            return RedirectToAction("Index");
        }

        // GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null) return NotFound();

            var product = _prodRepo
                .FirstOrDefault(u => u.Id == id, includeProperties: "Category,ApplicationType");
            if (product == null) return NotFound();
            
            return View(product);        
        }
        
        // POST - DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var product = _prodRepo.Find(id.GetValueOrDefault());
            if (product == null)
            {
                TempData[WC.Error] = "Action failed...Please try again!!!";
                return NotFound();
            }
            
            // Handle the product image on local
            var webRootPath = _webHostEnvironment.WebRootPath;
            var upload = webRootPath + WC.ImagePath;
            var oldFile = Path.Combine(upload, product.Image);
            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }
            // Handle the product on db
            _prodRepo.Remove(product);
            _prodRepo.Save();
            TempData[WC.Success] = "Action completed...";
            return RedirectToAction("Index");
        }
    }
}
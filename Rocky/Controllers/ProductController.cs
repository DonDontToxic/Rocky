using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;

namespace Rocky.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;
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
        public IActionResult Upsert(Product product)
        {
            if (!ModelState.IsValid) return View(product);
            _db.Product.Update(product);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete()
        {
            throw new System.NotImplementedException();
        }
    }
}
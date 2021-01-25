using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;

namespace Rocky.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        
        // GET
        public IActionResult Index()
        {
            IEnumerable<Category> objList = _db.Category;
            
            return View(objList);
        }
        
        //GET - CREATE
        public IActionResult Create()
        {
            return View();
        }
        // POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid) return View(category);
            _db.Category.Add(category);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET - EDIT
        public IActionResult Edit(int? id)
        {
            if (id == 0 || id == null) return NotFound();
            
            var category = _db.Category.Find(id); // Only find for the primary key
            
            if (category == null) return NotFound();
            return View(category);
        }
        // POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid) return View(category);
            _db.Category.Update(category);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        // GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null) return NotFound();
            
            var category = _db.Category.Find(id); // Only find for the primary key
            
            if (category == null) return NotFound();
            return View(category);        
        }
        // POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var category = _db.Category.Find(id);
            if (category == null) return NotFound();
            _db.Category.Remove(category);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
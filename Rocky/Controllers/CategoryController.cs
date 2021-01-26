using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky;
using Rocky_DataAccess.Data;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Ultility;

namespace Rocky.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _catRepo;

        public CategoryController(ICategoryRepository db)
        {
            _catRepo = db;
        }
        
        // GET
        public IActionResult Index()
        {
            IEnumerable<Category> objList = _catRepo.GetAll();
            
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
            _catRepo.Add(category);
            _catRepo.Save();
            return RedirectToAction("Index");
        }

        // GET - EDIT
        public IActionResult Edit(int? id)
        {
            if (id == 0 || id == null) return NotFound();
            
            var category = _catRepo.Find(id.GetValueOrDefault()); // Only find for the primary key
            
            if (category == null) return NotFound();
            return View(category);
        }
        // POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid) return View(category);
            _catRepo.Update(category);
            _catRepo.Save();
            return RedirectToAction("Index");
        }
        
        // GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null) return NotFound();
            
            var category = _catRepo.Find(id.GetValueOrDefault()); // Only find for the primary key
            
            if (category == null) return NotFound();
            return View(category);        
        }
        // POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var category = _catRepo.Find(id.GetValueOrDefault());
            if (category == null) return NotFound();
            _catRepo.Remove(category);
            _catRepo.Save();
            return RedirectToAction("Index");
        }
    }
}
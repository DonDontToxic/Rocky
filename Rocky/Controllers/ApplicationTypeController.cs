using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;

namespace Rocky.Controllers
{
    public class ApplicationTypeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ApplicationTypeController(ApplicationDbContext db)
        {
            _db = db;
        }
        
        // GET
        public IActionResult Index()
        {
            IEnumerable<ApplicationType> typeList = _db.ApplicationType;
            return View(typeList);
        }
        
        // GET - CREATE
        public IActionResult Create()
        {
            return View();
        }
        // POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType type)
        {
            if (!ModelState.IsValid) return View(type);
            _db.ApplicationType.Add(type);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        // GET - EDIT
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) return NotFound();
            var typeObj = _db.ApplicationType.Find(id);
            if (typeObj == null) return NotFound();
            return View(typeObj);
        }
        // POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationType type)
        {
            if (!ModelState.IsValid) return View(type);
            _db.ApplicationType.Update(type);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        // GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) return NotFound();
            var typeObj = _db.ApplicationType.Find(id);
            if (typeObj == null) return NotFound();
            return View(typeObj);
        }
        // POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {            
            if (id == null || id == 0) return NotFound();
            var typeObj = _db.ApplicationType.Find(id);
            if (typeObj == null) return NotFound();
            _db.ApplicationType.Remove(typeObj);
            _db.SaveChanges();
            return RedirectToAction("Index");        
        }
    }
}
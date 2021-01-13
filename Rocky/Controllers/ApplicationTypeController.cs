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
    }
}
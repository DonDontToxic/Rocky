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
    public class ApplicationTypeController : Controller
    {
        private readonly IApplicationTypeRepository _appRepo;

        public ApplicationTypeController(IApplicationTypeRepository appRepo)
        {
            _appRepo = appRepo;
        }
        
        // GET
        public IActionResult Index()
        {
            IEnumerable<ApplicationType> typeList = _appRepo.GetAll();
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
            _appRepo.Add(type);
            _appRepo.Save();
            return RedirectToAction("Index");
        }
        
        // GET - EDIT
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) return NotFound();
            var typeObj = _appRepo.Find(id.GetValueOrDefault());
            if (typeObj == null) return NotFound();
            return View(typeObj);
        }
        // POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationType type)
        {
            if (!ModelState.IsValid) return View(type);
            _appRepo.Update(type);
            _appRepo.Save();
            return RedirectToAction("Index");
        }
        
        // GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) return NotFound();
            var typeObj = _appRepo.Find(id.GetValueOrDefault());
            if (typeObj == null) return NotFound();
            return View(typeObj);
        }
        // POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {            
            if (id == null || id == 0) return NotFound();
            var typeObj = _appRepo.Find(id.GetValueOrDefault());
            if (typeObj == null) return NotFound();
            _appRepo.Remove(typeObj);
            _appRepo.Save();
            return RedirectToAction("Index");        
        }
    }
}
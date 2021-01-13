﻿using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;

namespace Rocky.Controllers
{
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
    }
}
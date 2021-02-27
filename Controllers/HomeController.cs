using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProductsAndCategories.Models;

namespace ProductsAndCategories.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;
        public HomeController(MyContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Redirect()
        {
            return RedirectToAction("Index");
        }

        [HttpGet("products")]
        public IActionResult Index()
        {
            Wrapper wrapper = new Wrapper();
            wrapper.AllProducts = _context.Products.ToList();
            return View("Index", wrapper);
        }

        [HttpPost("products")]
        public IActionResult CreateProduct(Wrapper Form)
        {
            if (ModelState.IsValid)
            {
                _context.Add(Form.Product);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return Index();
            }
        }

        [HttpGet("categories")]
        public IActionResult Categories()
        {
            Wrapper wrapper = new Wrapper();
            wrapper.AllCategories = _context.Categories.ToList();
            return View("Categories", wrapper);
        }

        [HttpPost("categories")]
        public IActionResult CreateCategory(Wrapper Form)
        {
            if (ModelState.IsValid)
            {
                _context.Add(Form.Category);
                _context.SaveChanges();
                return RedirectToAction("Categories");
            }
            else
            {
                return Categories();
            }
        }

        [HttpGet("products/{id}")]
        public IActionResult ProductDetails(int id)
        {
            Wrapper wrapper = new Wrapper();
            wrapper.Product = _context.Products.Include(p => p.Categories).ThenInclude(a => a.Category).FirstOrDefault(p => p.ProductId == id);
            wrapper.AllCategories = _context.Categories.Include(c => c.Products).Where(c => !c.Products.Any(p => p.ProductId == wrapper.Product.ProductId)).ToList();
            return View("ProductDetails", wrapper);
        }

        [HttpPost("products/{id}")]
        public IActionResult AddCategory(int id, Wrapper Form)
        {
            Form.Association.ProductId = id;
            _context.Add(Form.Association);
            _context.SaveChanges();
            return RedirectToAction("ProductDetails", id);
        }

        [HttpGet("categories/{id}")]
        public IActionResult CategoryDetails(int id)
        {
            Wrapper wrapper = new Wrapper();
            wrapper.Category = _context.Categories.Include(c => c.Products).ThenInclude(a => a.Product).FirstOrDefault(c => c.CategoryId == id);
            wrapper.AllProducts = _context.Products.Include(p => p.Categories).Where(p => !p.Categories.Any(c => c.CategoryId == wrapper.Category.CategoryId)).ToList();
            return View("CategoryDetails", wrapper);
        }

        [HttpPost("categories/{id}")]
        public IActionResult AddProduct(int id, Wrapper Form)
        {
            Form.Association.CategoryId = id;
            _context.Add(Form.Association);
            _context.SaveChanges();
            return RedirectToAction("CategoryDetails", id);
        }
    }
}
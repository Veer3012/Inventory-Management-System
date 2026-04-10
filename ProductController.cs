using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        private void LogAction(string action, string details)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail") ?? "System";
            var log = new AuditLog
            {
                Action = action,
                EntityName = "Product",
                Details = details,
                PerformedBy = userEmail,
                Timestamp = DateTime.Now
            };
            _context.AuditLogs.Add(log);
            _context.SaveChanges();
        }

        public IActionResult Index() => View(_context.Products.ToList());

        public IActionResult Add() => View();

        [HttpPost]
        public IActionResult Add(Product product)
        {
            product.DateAdded = DateTime.Now;
            _context.Products.Add(product);
            _context.SaveChanges();

            LogAction("Create", $"Added new product: {product.ProductName}");
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id) => View(_context.Products.Find(id));

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();

            LogAction("Update", $"Modified product ID: {product.ProductId}");
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                LogAction("Delete", $"Deleted product: {product.ProductName}");
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // Action to view history
        public IActionResult History()
        {
            var logs = _context.AuditLogs.OrderByDescending(l => l.Timestamp).ToList();
            return View(logs);
        }
    }
}
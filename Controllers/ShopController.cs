using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlantShopApp.Data;

namespace PlantShopApp.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string category = null)
        {
            ViewBag.CurrentCategory = category ?? "All";
            
            var query = _context.Products.AsQueryable();
            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                query = query.Where(p => p.Category == category);
            }
            
            var products = await query.ToListAsync();
            return View(products);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using PlantShopApp.Data;
using PlantShopApp.Models;
using System.Text.Json;

namespace PlantShopApp.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string CartSessionKey = "CartSession";

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        private List<CartItem> GetCart()
        {
            var sessionData = HttpContext.Session.GetString(CartSessionKey);
            return sessionData == null ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(sessionData)!;
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
        }

        public IActionResult Index()
        {
            return View(GetCart());
        }

        [HttpPost]
        public IActionResult Add(int id, int quantity = 1)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                var cart = GetCart();
                var item = cart.FirstOrDefault(c => c.ProductId == id);
                if (item != null)
                {
                    item.Quantity += quantity;
                }
                else
                {
                    cart.Add(new CartItem
                    {
                        ProductId = product.ProductId,
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = quantity,
                        ImageUrl = product.ImageUrl
                    });
                }
                SaveCart(cart);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Remove(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == id);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

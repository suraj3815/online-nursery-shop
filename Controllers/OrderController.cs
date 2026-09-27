using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlantShopApp.Data;
using PlantShopApp.Models;
using System.Text.Json;

namespace PlantShopApp.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private const string CartSessionKey = "CartSession";

        public OrderController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private List<CartItem> GetCart()
        {
            var sessionData = HttpContext.Session.GetString(CartSessionKey);
            return sessionData == null ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(sessionData)!;
        }

        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Any()) return RedirectToAction("Index", "Cart");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string address)
        {
            var cart = GetCart();
            if (!cart.Any()) return RedirectToAction("Index", "Cart");

            var user = await _userManager.GetUserAsync(User);
            var order = new Order
            {
                UserId = user?.Id,
                Address = address,
                TotalAmount = cart.Sum(c => c.Subtotal),
                Status = "Pending"
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cart)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                };
                _context.OrderItems.Add(orderItem);
            }
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove(CartSessionKey);
            return RedirectToAction(nameof(MyOrders));
        }

        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == user!.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }
    }
}

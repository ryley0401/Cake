using CakeShop.Data;
using CakeShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CakeShop.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var cartItems = await _context.ShoppingCartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Cake)
                .ToListAsync();

            decimal total = cartItems.Sum(item => (item.Cake?.Price ?? 0) * item.Quantity);
            ViewBag.CartTotal = total;

            return View(cartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int cakeId, int quantity)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            if (quantity <= 0)
            {
                TempData["CartMessage"] = "數量必須大於0";
                return RedirectToAction("Details", "Cakes", new { id = cakeId });
            }

            var cartItem = await _context.ShoppingCartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.CakeId == cakeId);

            var cake = await _context.Cakes.FindAsync(cakeId);
            if (cake == null)
            {
                return NotFound("找不到此蛋糕");
            }

            if (cartItem == null)
            {
                cartItem = new ShoppingCartItem
                {
                    CakeId = cakeId,
                    UserId = userId,
                    Quantity = quantity
                };
                _context.ShoppingCartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            await _context.SaveChangesAsync();
            TempData["CartMessage"] = $"已將 {quantity} 個 「{cake.Name}」加入購物車";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var cartItem = await _context.ShoppingCartItems.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (cartItem != null)
            {
                _context.ShoppingCartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
                TempData["CartMessage"] = "已成功移除商品";
            }
            else
            {
                TempData["CartMessage"] = "找不到要移除的商品或權限不足";
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQnantity(int id, int quantity)
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            if (quantity <= 0)
            {
                return await RemoveFromCart(id);
            }

            var cartItem = await _context.ShoppingCartItems.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                await _context.SaveChangesAsync();
                TempData["CartMessage"] = "購物車數量已更新";
            }
            else
            {
                TempData["CartMessage"] = "找不到要更新的商品或權限不足";
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

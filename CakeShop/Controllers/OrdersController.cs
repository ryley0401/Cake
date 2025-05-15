using CakeShop.Data;
using CakeShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CakeShop.Controllers
{
    [Authorize] // 需要登入才能訪問訂單相關頁面
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

        // GET: Orders/Checkout (顯示結帳頁面，預填用戶資料)

        public async Task<IActionResult> Checkout()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Challenge();

            var user = await _userManager.GetUserAsync(User); // 取得目前登入的使用者物件
            if (user == null) return NotFound("找不到使用者資訊");

            // 取得購物車內容，確認有東西才顯示結帳頁
            var cartItems = await _context.ShoppingCartItems
                                          .Where(c => c.UserId == userId)
                                          .Include(c => c.Cake)
                                          .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "您的購物車是空的，無法結帳。";
                return RedirectToAction("Index", "ShoppingCart");
            }

            // 計算總金額
            decimal total = cartItems.Sum(item => (item.Cake?.Price ?? 0) * item.Quantity);

            // 建立一個 ViewModel 來傳遞預設的訂單資訊
            var checkoutViewModel = new CheckoutViewModel
            {
                RecipientName = user.Name,
                ShippingAddress = user.Address,
                RecipientPhone = user.PhoneNumber ?? "", // User 可能沒有電話
                CartItems = cartItems, // 把購物車內容也傳過去顯示
                TotalAmount = total
            };

            return View(checkoutViewModel); // 將 ViewModel 傳遞給 View
        }

        public IActionResult Index()
        {
            return View();
        }
    }
        // 可以獨立建立一個 ViewModel 檔案 (例如在 ViewModels 資料夾)
        public class CheckoutViewModel
        {
            [Required(ErrorMessage = "收件人姓名為必填項")]
            [StringLength(50)]
            [Display(Name = "收件人姓名")]
            public string RecipientName { get; set; } = string.Empty;

            [Required(ErrorMessage = "收貨地址為必填項")]
            [StringLength(200)]
            [Display(Name = "收貨地址")]
            public string ShippingAddress { get; set; } = string.Empty;

            [Required(ErrorMessage = "聯絡電話為必填項")]
            [Phone(ErrorMessage = "請輸入有效的電話號碼")]
            [Display(Name = "聯絡電話")]
            public string RecipientPhone { get; set; } = string.Empty;

            // 顯示用，不需要 Post 回來
            public IEnumerable<ShoppingCartItem> CartItems { get; set; } = new List<ShoppingCartItem>();
            public decimal TotalAmount { get; set; }
        }
    }
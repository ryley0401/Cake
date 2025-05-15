using CakeShop.Data;
using CakeShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // 需要登入才能操作購物車
using System.Security.Claims; // 用來取得使用者 ID
using Microsoft.AspNetCore.Identity; // UserManager


namespace CakeShop.Controllers
{
    public class CakesController : Controller
    {
        /*   readonly (唯讀的):這是一個修飾詞，專門用於欄位它表示這個欄位的值
             只能在兩個地方被設定：
           1. 在宣告欄位的同時進行初始化
           2. 在該類別的建構函式內部進行
           _context 這個欄位只能在包含它的那個類別（例如 HomeController）內部被存取 (private)。
           _context 這個欄位的值只能在物件被建立時（在建構函式中）被賦值一次，之後就不能再改變它所引用的物件
           (readonly)。
         */

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager; // 用於取得使用者資訊

        public CakesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Cakes (蛋糕列表 - 所有人可看)
        [AllowAnonymous] // 明確標示允許匿名訪問
        public async Task<IActionResult> Index()
        {
            var cakes = await _context.Cakes.ToListAsync();
            return View(cakes);
        }

        // GET: Cakes/Details/5 (蛋糕詳情 - 所有人可看)
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cake = await _context.Cakes.FirstOrDefaultAsync(m => m.Id == id);
            if (cake == null)
            {
                return NotFound();
            }
            // 可以建立一個 ViewModel 來傳遞蛋糕資訊和數量選擇
            ViewBag.Quantity = 1; // 預設數量
            return View(cake);
        }


        // --- 如果需要管理功能 ---
        // GET: Cakes/Create (需要管理員角色，此處簡化，先不做角色)
        // [Authorize] // 可以加上角色限制 [Authorize(Roles="Admin")]
        // public IActionResult Create() { ... }

        // POST: Cakes/Create
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // [Authorize]
        // public async Task<IActionResult> Create(...) { ... }

        // ... 其他 CRUD (Edit, Delete) 方法 ...
    }
}
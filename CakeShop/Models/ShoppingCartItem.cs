using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CakeShop.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }

        public int CakeId { get; set; } // 外鍵到 Cake
        [ForeignKey("CakeId")]
        public virtual Cake? Cake { get; set; }

        [Range(1, 100, ErrorMessage = "數量必須介於 1 和 100 之間")]
        [Display(Name = "數量")]
        public int Quantity { get; set; }

        // 連結到使用者 (哪個使用者的購物車)
        public string? UserId { get; set; } // 外鍵到 ApplicationUser (Id 是 string 類型)
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        // 或者，如果允許未登入用戶有暫時購物車，可以加入 SessionId
        // public string? ShoppingCartId { get; set; } // 用 SessionId 或 Cookie 值識別
    }
}
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CakeShop.Models
{
    // 繼承 IdentityUser 來擴充使用者資訊
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "姓名為必填項")]
        [StringLength(50)]
        [Display(Name = "姓名")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "地址為必填項")]
        [StringLength(200)]
        [Display(Name = "地址")]
        public string Address { get; set; } = string.Empty;

        // IdentityUser 已經有 PhoneNumber 屬性，但預設可能為 nullable 或未啟用
        // 我們可以在註冊時強制要求或在此處覆寫
        // [Required(ErrorMessage = "電話為必填項")] // 如果需要強制
        // [Phone]
        // public override string PhoneNumber { get; set; }

        // 導覽屬性 (使用者可以有多筆訂單) - 可選，但有助於 ORM
        public virtual ICollection<Order>? Orders { get; set; }
        public virtual ICollection<ShoppingCartItem>? ShoppingCartItems { get; set; } // 使用者的購物車項目
    }
}
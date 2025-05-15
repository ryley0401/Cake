using CakeShop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CakeShop.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // 檢查資料庫是否存在，如果不存在則建立
            context.Database.EnsureCreated();

            // 先清空現有資料
            context.Cakes.RemoveRange(context.Cakes);
            context.SaveChanges();

            var cakes = new Cake[]
            {
                new Cake{Name="6吋蘋果蜂蜜鮮奶蛋糕", Description="濕潤又帶有彈性的原味蛋糕，抹入滑順不膩口的鮮奶油，另外再加上香甜的蜂蜜丁與帶有些許酸甜感的蘋果丁，水果和蜜香的結合，是款清爽的蛋糕。", Price=950.00m, ImageUrl="/images/Apple.jpg"},
                new Cake{Name="6吋藍莓三重奏起司蛋糕", Description="三種不同口感的藍莓乳酪一起呈現，由底部的餅乾塔皮往上，從濃郁、綿密和滑順的口感，搭配漸層感的外觀，帶出一些夢幻的感覺。", Price=1080.00m, ImageUrl="/images/Blueberry.jpg"},
                new Cake{Name="6吋經典法式千層薄餅", Description="新升級的配方，讓千層餅皮更加柔軟，內餡添加瑞穗鮮乳，提升奶香與滑順口感。", Price=900.00m, ImageUrl="/images/Classic.jpg"},
                new Cake{Name="5吋綿雲起司蛋糕", Description="柔軟輕柔的蛋糕體，中心灌入香濃北海道四葉奶油乳酪內餡，表面淋上一層起司醬後再撒上帕馬森起司。是一款起司風味濃郁，帶有鹹香感的特殊蛋糕。", Price=520.00m, ImageUrl="/images/Cotton.jpg"},
                new Cake{Name="6吋草莓流芯輕乳蛋糕", Description="香甜的草莓慕斯，中間夾入一層流芯感的草莓果醬，以清爽的輕乳蛋糕搭配，最上層撒上草莓白巧克力包裹的脆片餅乾增添口感。", Price=108.00m, ImageUrl="/images/Strawberry.jpg"}
            };

            foreach (Cake c in cakes)
            {
                context.Cakes.Add(c);
            }
            context.SaveChanges(); // 儲存蛋糕資料

            // 可以在這裡加入其他的初始資料，例如管理員帳號等
        }
    }
}
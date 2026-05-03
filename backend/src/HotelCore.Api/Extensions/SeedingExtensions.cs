using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Entities.Images;
using HotelCore.Domain.Entities.Users.Restaurant;
using HotelCore.Infrastructure.Persistence;

namespace HotelCore.Api.Extensions;

public static class SeedingExtensions
{
    public static async Task SeedRolesAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        foreach (var role in Enum.GetValues<UserRole>())
        {
            var roleName = role.ToString();

            if (role == UserRole.Guest) continue;

            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
        }
    }

    public static async Task SeedRestaurantDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (await db.ProductCategories.AnyAsync()) return;

        static MyImage Img(string key, string url, int w, int h) => new()
        {
            StorageKey = key,
            Url = url,
            Width = w,
            Height = h,
            SizeBytes = w * h / 5,
            AspectRatio = Math.Round((double)w / h, 4),
        };

        var breakfast = new ProductCategory { Name = "Breakfast" };
        var mains = new ProductCategory { Name = "Main Courses" };
        var salads = new ProductCategory { Name = "Salads" };
        var desserts = new ProductCategory { Name = "Desserts" };
        var drinks = new ProductCategory { Name = "Drinks" };

        breakfast.Products = new List<Product>
        {
            new() { Name = "Continental Breakfast", Description = "Assorted pastries, butter, jam, orange juice, and your choice of tea or coffee.", Price = 18.00m, Category = breakfast, Image = Img("mock/continental-breakfast", "https://images.unsplash.com/photo-1484723091739-30a097e8f929?w=800&q=80", 800, 534) },
            new() { Name = "Full English Breakfast", Description = "Two eggs any style, bacon, sausage, grilled tomato, baked beans, toast, and tea or coffee.", Price = 22.00m, Category = breakfast, Image = Img("mock/full-english", "https://images.unsplash.com/photo-1533089860892-a7c6f0a88666?w=800&q=80", 800, 534) },
            new() { Name = "Avocado Toast", Description = "Sourdough toast topped with smashed avocado, cherry tomatoes, poached egg, and chilli flakes.", Price = 16.00m, Category = breakfast, Image = Img("mock/avocado-toast", "https://images.unsplash.com/photo-1541519227354-08fa5d50c820?w=800&q=80", 800, 534) },
            new() { Name = "Pancake Stack", Description = "Three fluffy buttermilk pancakes served with maple syrup, fresh berries, and whipped cream.", Price = 14.00m, Category = breakfast, Image = Img("mock/pancakes", "https://images.unsplash.com/photo-1567620905732-2d1ec7ab7445?w=800&q=80", 800, 534) },
            new() { Name = "Greek Yogurt & Granola", Description = "Thick Greek yogurt layered with house-made granola, honey, and seasonal fruit.", Price = 12.00m, Category = breakfast, Image = Img("mock/yogurt-granola", "https://images.unsplash.com/photo-1490645935967-10de6ba17061?w=800&q=80", 800, 534) },
        };

        mains.Products = new List<Product>
        {
            new() { Name = "Grilled Salmon", Description = "Atlantic salmon fillet with lemon butter sauce, seasonal vegetables, and roasted new potatoes.", Price = 32.00m, Category = mains, Image = Img("mock/grilled-salmon", "https://images.unsplash.com/photo-1467003909585-2f8a72700288?w=800&q=80", 800, 534) },
            new() { Name = "Beef Tenderloin", Description = "200g tenderloin cooked to your liking, served with truffle mashed potato and red wine jus.", Price = 48.00m, Category = mains, Image = Img("mock/beef-tenderloin", "https://images.unsplash.com/photo-1546833998-877b37c2e5c6?w=800&q=80", 800, 534) },
            new() { Name = "Chicken Piccata", Description = "Pan-seared chicken breast in a lemon caper sauce, served with angel hair pasta and fresh herbs.", Price = 28.00m, Category = mains, Image = Img("mock/chicken-piccata", "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=800&q=80", 800, 534) },
            new() { Name = "Wild Mushroom Risotto", Description = "Creamy Arborio rice with a blend of wild mushrooms, Parmesan, truffle oil, and fresh thyme.", Price = 24.00m, Category = mains, Image = Img("mock/mushroom-risotto", "https://images.unsplash.com/photo-1476124369491-e7addf5db371?w=800&q=80", 800, 534) },
            new() { Name = "Fish & Chips", Description = "Beer-battered cod fillet with hand-cut chips, mushy peas, tartar sauce, and a lemon wedge.", Price = 26.00m, Category = mains, Image = Img("mock/fish-and-chips", "https://images.unsplash.com/photo-1580217593608-a0db1cd27e5a?w=800&q=80", 800, 534) },
            new() { Name = "Margherita Pizza", Description = "Stone-baked pizza base with San Marzano tomato sauce, buffalo mozzarella, and fresh basil.", Price = 20.00m, Category = mains, Image = Img("mock/margherita-pizza", "https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=800&q=80", 800, 534) },
        };

        salads.Products = new List<Product>
        {
            new() { Name = "Caesar Salad", Description = "Crisp romaine lettuce, shaved Parmesan, house-made Caesar dressing, and crunchy croutons.", Price = 14.00m, Category = salads, Image = Img("mock/caesar-salad", "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=800&q=80", 800, 534) },
            new() { Name = "Greek Salad", Description = "Cucumber, tomato, Kalamata olives, red onion, and feta cheese dressed with extra virgin olive oil.", Price = 13.00m, Category = salads, Image = Img("mock/greek-salad", "https://images.unsplash.com/photo-1540420773420-3366772f4999?w=800&q=80", 800, 534) },
            new() { Name = "Nicoise Salad", Description = "Seared tuna, green beans, boiled egg, olives, and potatoes on a bed of mixed leaves.", Price = 18.00m, Category = salads, Image = Img("mock/nicoise-salad", "https://images.unsplash.com/photo-1547592166-23ac45744acd?w=800&q=80", 800, 534) },
            new() { Name = "Garden Salad", Description = "Mixed seasonal greens, cherry tomatoes, cucumber, and radish with a choice of dressing.", Price = 11.00m, Category = salads, Image = Img("mock/garden-salad", "https://images.unsplash.com/photo-1559181567-c3190438f2e7?w=800&q=80", 800, 534) },
        };

        desserts.Products = new List<Product>
        {
            new() { Name = "Crème Brûlée", Description = "Classic vanilla custard with a caramelised sugar crust, served with fresh seasonal berries.", Price = 12.00m, Category = desserts, Image = Img("mock/creme-brulee", "https://images.unsplash.com/photo-1470124182917-cc6e71b22ecc?w=800&q=80", 800, 534) },
            new() { Name = "Chocolate Lava Cake", Description = "Warm dark chocolate fondant with a molten centre, served with vanilla bean ice cream.", Price = 13.00m, Category = desserts, Image = Img("mock/lava-cake", "https://images.unsplash.com/photo-1606313564200-e75d5e30476c?w=800&q=80", 800, 534) },
            new() { Name = "Tiramisu", Description = "Layers of espresso-soaked ladyfingers and mascarpone cream, dusted with cocoa powder.", Price = 11.00m, Category = desserts, Image = Img("mock/tiramisu", "https://images.unsplash.com/photo-1571877227200-a0d98ea607e9?w=800&q=80", 800, 534) },
            new() { Name = "Cheese Board", Description = "A curated selection of three artisan cheeses with crackers, grapes, and fig preserve.", Price = 16.00m, Category = desserts, Image = Img("mock/cheese-board", "https://images.unsplash.com/photo-1558961363-fa8fdf82db35?w=800&q=80", 800, 534) },
        };

        drinks.Products = new List<Product>
        {
            new() { Name = "Fresh Orange Juice", Description = "Freshly squeezed orange juice (330 ml).", Price = 6.00m, Category = drinks, Image = Img("mock/orange-juice", "https://images.unsplash.com/photo-1621506289937-a8e4df240d0b?w=800&q=80", 800, 534) },
            new() { Name = "Sparkling Water", Description = "Chilled sparkling mineral water (500 ml).", Price = 4.00m, Category = drinks, Image = Img("mock/sparkling-water", "https://images.unsplash.com/photo-1560963805-6c64a8f47b44?w=800&q=80", 800, 534) },
            new() { Name = "Still Water", Description = "Chilled still mineral water (500 ml).", Price = 4.00m, Category = drinks, Image = Img("mock/still-water", "https://images.unsplash.com/photo-1548839140-29a749e1cf4d?w=800&q=80", 800, 534) },
            new() { Name = "Americano", Description = "Double shot of espresso topped with hot water.", Price = 5.00m, Category = drinks, Image = Img("mock/americano", "https://images.unsplash.com/photo-1509042239860-f550ce710b93?w=800&q=80", 800, 534) },
            new() { Name = "Cappuccino", Description = "Espresso with steamed milk and a thick layer of foam.", Price = 5.50m, Category = drinks, Image = Img("mock/cappuccino", "https://images.unsplash.com/photo-1534778101976-62847782c213?w=800&q=80", 800, 534) },
            new() { Name = "English Breakfast Tea", Description = "Classic black tea served with milk on the side.", Price = 4.50m, Category = drinks, Image = Img("mock/tea", "https://images.unsplash.com/photo-1544787219-7f47ccb76574?w=800&q=80", 800, 534) },
            new() { Name = "Lemonade", Description = "House-made lemonade with fresh mint and a splash of sparkling water (400 ml).", Price = 6.50m, Category = drinks, Image = Img("mock/lemonade", "https://images.unsplash.com/photo-1523677011781-c91d1bbe2f9e?w=800&q=80", 800, 534) },
            new() { Name = "House Red Wine", Description = "A glass of the chef's selected red wine (175 ml).", Price = 9.00m, Category = drinks, Image = Img("mock/red-wine", "https://images.unsplash.com/photo-1553361371-9b22f78e8b1d?w=800&q=80", 800, 534) },
            new() { Name = "House White Wine", Description = "A glass of the chef's selected white wine (175 ml).", Price = 9.00m, Category = drinks, Image = Img("mock/white-wine", "https://images.unsplash.com/photo-1510812431401-41d2bd2722f3?w=800&q=80", 800, 534) },
        };

        db.ProductCategories.AddRange(breakfast, mains, salads, desserts, drinks);
        await db.SaveChangesAsync();
    }
}

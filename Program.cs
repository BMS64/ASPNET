using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineMarketplace.Data;
using OnlineMarketplace.Data.Repository;
using OnlineMarketplace.Data.Services;
using OnlineMarketplace.Models;

namespace OnlineMarketplace;

public class Program
{
    public async static Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        
        // Cookies, Session, Cache
        builder.Services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
        builder.Services.AddSession(options => {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true; // For GDPR compliance
        });

        builder.Services.AddDbContext<ShopContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
        });

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireDigit = false;
        }).AddEntityFrameworkStores<ShopContext>();
        
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<ICustomerService, CustomerService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<ICartService, CartService>();
        builder.Services.AddScoped<IReviewService, ReviewService>();
        builder.Services.AddScoped<IPaymentService, PaymentService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
        }
        app.UseStaticFiles();

        app.UseSession();

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        #region Seed Database
        /// Seed the database
        //using (var scope = app.Services.CreateScope())
        //{
        //    var context = scope.ServiceProvider.GetRequiredService<ShopContext>();
        //    Seed(context); // Call seed method
        //}
        #endregion
        #region Ensure Roles Exist
        //var ser = app.Services.CreateScope().ServiceProvider;
        //var roleManager = ser.GetRequiredService<RoleManager<IdentityRole>>();
        //await EnsureRolesExist(roleManager);
        #endregion

        app.Run();
    }

    private static async Task EnsureRolesExist(RoleManager<IdentityRole> roleManager)
    {
        // Ensure the User role exists
        if (!await roleManager.RoleExistsAsync(UserRoles.User))
        {
            await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
        }

        // Ensure the Admin role exists (if you need this role)
        if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
        {
            await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        }

        if (!await roleManager.RoleExistsAsync(UserRoles.Guest))
        {
            await roleManager.CreateAsync(new IdentityRole(UserRoles.Guest));
        }
    }

//    private static void Seed(ShopContext context)
//    {
//            var products = new List<Product>
//            {
//new Product { Name = "Wireless Mouse", Description = "Ergonomic wireless mouse with 2.4GHz connectivity.", UnitsInStock = 150, Price = 29.99m, ImageURL = "images/mouse.jpg", CategoryId = 1 },
//new Product { Name = "Gaming Keyboard", Description = "Mechanical keyboard with RGB lighting.", UnitsInStock = 100, Price = 79.99m, ImageURL = "images/keyboard.jpg", CategoryId = 1 },
//new Product { Name = "4K Monitor", Description = "Ultra HD 4K monitor with 27-inch screen.", UnitsInStock = 50, Price = 299.99m, ImageURL = "images/monitor.jpg", CategoryId = 1 },
//new Product { Name = "USB-C Hub", Description = "7-in-1 USB-C hub with multiple ports.", UnitsInStock = 200, Price = 49.99m, ImageURL = "images/usb-hub.jpg", CategoryId = 1 },
//new Product { Name = "Portable SSD", Description = "1TB portable solid-state drive for fast storage.", UnitsInStock = 80, Price = 149.99m, ImageURL = "images/ssd.jpg", CategoryId = 1 },
//new Product { Name = "Smartwatch", Description = "Fitness tracking smartwatch with heart rate monitoring.", UnitsInStock = 120, Price = 199.99m, ImageURL = "images/smartwatch.jpg", CategoryId = 1 },
//new Product { Name = "Bluetooth Headphones", Description = "Noise-cancelling over-ear Bluetooth headphones.", UnitsInStock = 90, Price = 129.99m, ImageURL = "images/headphones.jpg", CategoryId = 1 },
//new Product { Name = "Desk Lamp", Description = "LED desk lamp with adjustable brightness.", UnitsInStock = 300, Price = 24.99m, ImageURL = "images/lamp.jpg", CategoryId = 1 },
//new Product { Name = "Office Chair", Description = "Ergonomic office chair with lumbar support.", UnitsInStock = 70, Price = 179.99m, ImageURL = "images/chair.jpg", CategoryId = 4 },
//new Product { Name = "Power Bank", Description = "10,000mAh portable power bank with fast charging.", UnitsInStock = 150, Price = 29.99m, ImageURL = "images/powerbank.jpg", CategoryId = 1 },
//new Product { Name = "Action Camera", Description = "4K action camera with waterproof casing.", UnitsInStock = 50, Price = 249.99m, ImageURL = "images/action-camera.jpg", CategoryId = 1 },
//new Product { Name = "Electric Scooter", Description = "Foldable electric scooter with 20-mile range.", UnitsInStock = 30, Price = 499.99m, ImageURL = "images/scooter.jpg", CategoryId = 1 },
//new Product { Name = "Wireless Earbuds", Description = "True wireless earbuds with 30 hours playtime.", UnitsInStock = 110, Price = 59.99m, ImageURL = "images/earbuds.jpg", CategoryId = 1 },
//new Product { Name = "Smartphone", Description = "5G-enabled smartphone with AMOLED display.", UnitsInStock = 40, Price = 699.99m, ImageURL = "images/smartphone.jpg", CategoryId = 1 },
//new Product { Name = "Tablet", Description = "10-inch tablet with 64GB storage.", UnitsInStock = 60, Price = 329.99m, ImageURL = "images/tablet.jpg", CategoryId = 1 },
//new Product { Name = "Backpack", Description = "Durable laptop backpack with multiple compartments.", UnitsInStock = 180, Price = 39.99m, ImageURL = "images/backpack.jpg", CategoryId = 8 },
//new Product { Name = "External Hard Drive", Description = "2TB external hard drive for backups.", UnitsInStock = 90, Price = 89.99m, ImageURL = "images/hard-drive.jpg", CategoryId = 1 },
//new Product { Name = "Webcam", Description = "1080p webcam with autofocus and microphone.", UnitsInStock = 140, Price = 49.99m, ImageURL = "images/webcam.jpg", CategoryId = 1 },
//new Product { Name = "Graphics Tablet", Description = "Drawing tablet with pen pressure sensitivity.", UnitsInStock = 40, Price = 199.99m, ImageURL = "images/graphics-tablet.jpg", CategoryId = 1 },
//new Product { Name = "Smart TV", Description = "50-inch 4K UHD Smart TV with HDR.", UnitsInStock = 35, Price = 449.99m, ImageURL = "images/smart-tv.jpg", CategoryId = 1 },
//new Product { Name = "Wireless Charger", Description = "Fast wireless charging pad for smartphones.", UnitsInStock = 100, Price = 29.99m, ImageURL = "images/wireless-charger.jpg", CategoryId = 1 },
//new Product { Name = "The Great Gatsby", Description = "Classic novel by F. Scott Fitzgerald.", UnitsInStock = 50, Price = 14.99m, ImageURL = "images/great-gatsby.jpg", CategoryId = 2 },
//new Product { Name = "Cookbook", Description = "Healthy recipes for everyday meals.", UnitsInStock = 30, Price = 19.99m, ImageURL = "images/cookbook.jpg", CategoryId = 2 },
//new Product { Name = "Programming with C#", Description = "Comprehensive guide for C# developers.", UnitsInStock = 40, Price = 49.99m, ImageURL = "images/programming-csharp.jpg", CategoryId = 2 },
//new Product { Name = "Men's T-Shirt", Description = "Cotton crewneck T-shirt in multiple colors.", UnitsInStock = 200, Price = 9.99m, ImageURL = "images/tshirt.jpg", CategoryId = 3 },
//new Product { Name = "Women's Jeans", Description = "Skinny fit denim jeans for women.", UnitsInStock = 120, Price = 39.99m, ImageURL = "images/jeans.jpg", CategoryId = 3 },
//new Product { Name = "Blender", Description = "High-speed blender with multiple settings.", UnitsInStock = 60, Price = 79.99m, ImageURL = "images/blender.jpg", CategoryId = 4 },
//new Product { Name = "Coffee Maker", Description = "Automatic coffee maker with timer.", UnitsInStock = 70, Price = 49.99m, ImageURL = "images/coffee-maker.jpg", CategoryId = 4 },
//new Product { Name = "Dinnerware Set", Description = "16-piece ceramic dinnerware set.", UnitsInStock = 40, Price = 59.99m, ImageURL = "images/dinnerware.jpg", CategoryId = 4 },
//new Product { Name = "Notebook", Description = "College-ruled spiral notebook.", UnitsInStock = 300, Price = 2.99m, ImageURL = "images/notebook.jpg", CategoryId = 8 },
//new Product { Name = "Ballpoint Pens", Description = "Pack of 20 ballpoint pens in assorted colors.", UnitsInStock = 500, Price = 7.99m, ImageURL = "images/pens.jpg", CategoryId = 8 },
//new Product { Name = "Desk Organizer", Description = "Multi-compartment desk organizer for supplies.", UnitsInStock = 100, Price = 19.99m, ImageURL = "images/desk-organizer.jpg", CategoryId = 8 },
//new Product { Name = "Building Blocks", Description = "500-piece building block set.", UnitsInStock = 120, Price = 29.99m, ImageURL = "images/building-blocks.jpg", CategoryId = 9 },
//new Product { Name = "RC Car", Description = "Remote-controlled racing car.", UnitsInStock = 80, Price = 49.99m, ImageURL = "images/rc-car.jpg", CategoryId = 9 },
//new Product { Name = "Plush Toy", Description = "Soft plush teddy bear.", UnitsInStock = 200, Price = 14.99m, ImageURL = "images/plush-toy.jpg", CategoryId = 9 },
//new Product { Name = "Organic Apples", Description = "Fresh organic apples (1kg).", UnitsInStock = 150, Price = 4.99m, ImageURL = "images/apples.jpg", CategoryId = 10 },
//new Product { Name = "Chicken Breast", Description = "Boneless skinless chicken breast (1kg).", UnitsInStock = 100, Price = 8.99m, ImageURL = "images/chicken.jpg", CategoryId = 10 },
//new Product { Name = "Cheddar Cheese", Description = "Block of aged cheddar cheese (500g).", UnitsInStock = 60, Price = 5.99m, ImageURL = "images/cheese.jpg", CategoryId = 10 }
//            };
//            context.Products.AddRange(products);
//            context.SaveChanges();
//    }
}

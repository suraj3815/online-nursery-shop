using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlantShopApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

// Add session for cart
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection(); // Removed to prevent the harmless warning
app.UseStaticFiles();

app.UseRouting();
app.UseSession(); // Important for cart

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Seed Admin Role and User
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate(); // This automatically creates the tables!

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }
    
    var adminUser = await userManager.FindByEmailAsync("admin@plantshop.com");
    if (adminUser == null)
    {
        adminUser = new IdentityUser { UserName = "admin@plantshop.com", Email = "admin@plantshop.com" };
        await userManager.CreateAsync(adminUser, "Admin@123");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
    if (!context.Products.Any(p => p.Category == "Indoor" && p.Name == "Monstera Deliciosa"))
    {
        context.Products.AddRange(
            new PlantShopApp.Models.Product { Name = "Monstera Deliciosa", Price = 45.99m, Description = "Famous for its natural leaf holes, this tropical beauty adds instant jungle vibes to any room.", Stock = 10, Category = "Indoor", ImageUrl = "/images/monstera.jpg" },
            new PlantShopApp.Models.Product { Name = "Snake Plant", Price = 24.50m, Description = "One of the toughest houseplants around. It purifies the air and thrives on neglect.", Stock = 15, Category = "Indoor", ImageUrl = "/images/snake_plant.jpg" },
            new PlantShopApp.Models.Product { Name = "Fiddle Leaf Fig", Price = 65.00m, Description = "A stunning statement plant with large, violin-shaped leaves. Needs bright, indirect light.", Stock = 5, Category = "Indoor", ImageUrl = "/images/fiddle_leaf_fig.jpg" },
            new PlantShopApp.Models.Product { Name = "Aloe Vera", Price = 18.99m, Description = "A handy medicinal succulent that is easy to care for and loves a sunny windowsill.", Stock = 20, Category = "Indoor", ImageUrl = "/images/aloe_vera.jpg" }
        );
        await context.SaveChangesAsync();
    }

    // 10 New Indoor Plants
    if (!context.Products.Any(p => p.Name == "Pothos"))
    {
        context.Products.AddRange(
            new PlantShopApp.Models.Product { Name = "Pothos", Price = 12.99m, Description = "A very easy trailing plant that looks great on shelves.", Stock = 30, Category = "Indoor", ImageUrl = "/images/lemon_balm.jpg" },
            new PlantShopApp.Models.Product { Name = "Peace Lily", Price = 24.99m, Description = "Produces elegant white flowers and purifies indoor air.", Stock = 15, Category = "Indoor", ImageUrl = "/images/chamomile.jpg" },
            new PlantShopApp.Models.Product { Name = "Spider Plant", Price = 14.50m, Description = "Fun, grassy leaves with small 'babies' that hang down.", Stock = 25, Category = "Indoor", ImageUrl = "/images/rosemary.jpg" },
            new PlantShopApp.Models.Product { Name = "Rubber Plant", Price = 35.00m, Description = "A bold plant with thick, glossy, dark green leaves.", Stock = 10, Category = "Indoor", ImageUrl = "/images/sage.jpg" },
            new PlantShopApp.Models.Product { Name = "Philodendron", Price = 18.00m, Description = "A classic, heart-shaped leaf houseplant that grows quickly.", Stock = 20, Category = "Indoor", ImageUrl = "/images/peppermint.jpg" },
            new PlantShopApp.Models.Product { Name = "Calathea", Price = 28.50m, Description = "Known for its stunning, patterned foliage that moves at night.", Stock = 12, Category = "Indoor", ImageUrl = "/images/holy_basil.jpg" },
            new PlantShopApp.Models.Product { Name = "Cast Iron Plant", Price = 30.00m, Description = "Virtually indestructible, tolerates very low light.", Stock = 15, Category = "Indoor", ImageUrl = "/images/echinacea.jpg" },
            new PlantShopApp.Models.Product { Name = "Chinese Evergreen", Price = 22.99m, Description = "Beautiful silver-patterned leaves, thrives in low light.", Stock = 18, Category = "Indoor", ImageUrl = "/images/calendula.jpg" },
            new PlantShopApp.Models.Product { Name = "Parlor Palm", Price = 19.50m, Description = "Brings a tropical feel to any room, pet-friendly.", Stock = 22, Category = "Indoor", ImageUrl = "/images/ashwagandha.jpg" },
            new PlantShopApp.Models.Product { Name = "Bird of Paradise", Price = 55.00m, Description = "A massive, dramatic plant with huge leaves.", Stock = 5, Category = "Indoor", ImageUrl = "/images/ginger_plant.jpg" }
        );
        await context.SaveChangesAsync();
    }
    
    // Seed Outdoor Plants
    if (!context.Products.Any(p => p.Name == "Lavender Bush" && p.Category == "Outdoor"))
    {
        context.Products.AddRange(
            new PlantShopApp.Models.Product { Name = "Lavender Bush", Price = 35.00m, Description = "A fragrant outdoor plant perfect for gardens and patios. Needs full sun.", Stock = 12, Category = "Outdoor", ImageUrl = "/images/lavender.jpg" },
            new PlantShopApp.Models.Product { Name = "Rose Bush", Price = 42.99m, Description = "Classic red roses for your garden. Beautiful blooms that attract butterflies.", Stock = 8, Category = "Outdoor", ImageUrl = "/images/rose_bush.jpg" },
            new PlantShopApp.Models.Product { Name = "Bougainvillea", Price = 55.00m, Description = "A vibrant, climbing outdoor plant that blooms with brilliant pink and magenta colors.", Stock = 15, Category = "Outdoor", ImageUrl = "/images/bougainvillea.jpg" }
        );
        await context.SaveChangesAsync();
    }

    // 10 New Outdoor Plants
    if (!context.Products.Any(p => p.Name == "Hydrangea"))
    {
        context.Products.AddRange(
            new PlantShopApp.Models.Product { Name = "Hydrangea", Price = 45.00m, Description = "Huge, beautiful clusters of flowers that change color based on soil.", Stock = 12, Category = "Outdoor", ImageUrl = "/images/echinacea.jpg" },
            new PlantShopApp.Models.Product { Name = "Peony", Price = 38.50m, Description = "Lush, fragrant flowers that bloom in late spring.", Stock = 10, Category = "Outdoor", ImageUrl = "/images/calendula.jpg" },
            new PlantShopApp.Models.Product { Name = "Japanese Maple", Price = 85.00m, Description = "A stunning ornamental tree with deep red leaves.", Stock = 4, Category = "Outdoor", ImageUrl = "/images/rosemary.jpg" },
            new PlantShopApp.Models.Product { Name = "Boxwood", Price = 25.00m, Description = "Classic evergreen shrub, perfect for hedges.", Stock = 30, Category = "Outdoor", ImageUrl = "/images/thyme.jpg" },
            new PlantShopApp.Models.Product { Name = "Lilac Bush", Price = 40.00m, Description = "Incredibly fragrant purple flowers in early spring.", Stock = 8, Category = "Outdoor", ImageUrl = "/images/sage.jpg" },
            new PlantShopApp.Models.Product { Name = "Hibiscus", Price = 32.99m, Description = "Large, tropical-looking flowers in bright colors.", Stock = 15, Category = "Outdoor", ImageUrl = "/images/holy_basil.jpg" },
            new PlantShopApp.Models.Product { Name = "Clematis", Price = 28.00m, Description = "A beautiful climbing vine that produces starry flowers.", Stock = 14, Category = "Outdoor", ImageUrl = "/images/chamomile.jpg" },
            new PlantShopApp.Models.Product { Name = "Hosta", Price = 15.50m, Description = "Lush foliage plant perfect for shady garden spots.", Stock = 25, Category = "Outdoor", ImageUrl = "/images/lemon_balm.jpg" },
            new PlantShopApp.Models.Product { Name = "Jasmine", Price = 35.99m, Description = "A climbing plant with incredibly fragrant white flowers.", Stock = 12, Category = "Outdoor", ImageUrl = "/images/peppermint.jpg" },
            new PlantShopApp.Models.Product { Name = "Wisteria", Price = 50.00m, Description = "Dramatic, cascading purple flowers on a vigorous vine.", Stock = 6, Category = "Outdoor", ImageUrl = "/images/turmeric_plant.jpg" }
        );
        await context.SaveChangesAsync();
    }
    else 
    {
        // Fix existing broken links
        var l = context.Products.FirstOrDefault(p => p.Name == "Lavender Bush"); if (l != null) l.ImageUrl = "/images/lavender.jpg";
        var r = context.Products.FirstOrDefault(p => p.Name == "Rose Bush"); if (r != null) r.ImageUrl = "/images/rose_bush.jpg";
        var b = context.Products.FirstOrDefault(p => p.Name == "Bougainvillea"); if (b != null) b.ImageUrl = "/images/bougainvillea.jpg";
        await context.SaveChangesAsync();
    }
    
    // Seed Medicinal Plants
    if (!context.Products.Any(p => p.Name == "Peppermint" && p.Category == "Medicinal"))
    {
        context.Products.AddRange(
            new PlantShopApp.Models.Product { Name = "Peppermint", Price = 12.99m, Description = "Great for digestive health and makes a refreshing tea.", Stock = 30, Category = "Medicinal", ImageUrl = "/images/peppermint.jpg" },
            new PlantShopApp.Models.Product { Name = "Chamomile", Price = 14.50m, Description = "Famous for its calming effects and use in sleep teas.", Stock = 25, Category = "Medicinal", ImageUrl = "/images/chamomile.jpg" },
            new PlantShopApp.Models.Product { Name = "Echinacea", Price = 18.00m, Description = "A powerful immune system booster with beautiful purple flowers.", Stock = 20, Category = "Medicinal", ImageUrl = "/images/echinacea.jpg" },
            new PlantShopApp.Models.Product { Name = "Holy Basil (Tulsi)", Price = 16.99m, Description = "Known as the 'Queen of Herbs', used for stress relief and vitality.", Stock = 40, Category = "Medicinal", ImageUrl = "/images/holy_basil.jpg" },
            new PlantShopApp.Models.Product { Name = "Ginger", Price = 15.00m, Description = "Excellent for nausea relief and reducing inflammation.", Stock = 35, Category = "Medicinal", ImageUrl = "/images/ginger_plant.jpg" },
            new PlantShopApp.Models.Product { Name = "Turmeric", Price = 19.99m, Description = "A potent anti-inflammatory and antioxidant plant.", Stock = 20, Category = "Medicinal", ImageUrl = "/images/turmeric_plant.jpg" },
            new PlantShopApp.Models.Product { Name = "Rosemary", Price = 13.50m, Description = "Enhances memory, focus, and adds great flavor to cooking.", Stock = 25, Category = "Medicinal", ImageUrl = "/images/rosemary.jpg" },
            new PlantShopApp.Models.Product { Name = "Thyme", Price = 11.99m, Description = "Good for respiratory health and a staple culinary herb.", Stock = 30, Category = "Medicinal", ImageUrl = "/images/thyme.jpg" },
            new PlantShopApp.Models.Product { Name = "Sage", Price = 14.00m, Description = "Packed with antioxidants and used for digestive issues.", Stock = 20, Category = "Medicinal", ImageUrl = "/images/sage.jpg" },
            new PlantShopApp.Models.Product { Name = "Lemon Balm", Price = 12.50m, Description = "A calming herb that reduces stress and promotes sleep.", Stock = 40, Category = "Medicinal", ImageUrl = "/images/lemon_balm.jpg" },
            new PlantShopApp.Models.Product { Name = "Calendula", Price = 15.99m, Description = "Known for its skin-healing properties and bright orange flowers.", Stock = 15, Category = "Medicinal", ImageUrl = "/images/calendula.jpg" },
            new PlantShopApp.Models.Product { Name = "Ginseng", Price = 25.00m, Description = "An adaptogen that boosts energy and lowers blood sugar.", Stock = 10, Category = "Medicinal", ImageUrl = "/images/ginseng_plant.jpg" },
            new PlantShopApp.Models.Product { Name = "Ashwagandha", Price = 22.00m, Description = "An ancient medicinal herb that reduces anxiety and stress.", Stock = 18, Category = "Medicinal", ImageUrl = "/images/ashwagandha.jpg" },
            new PlantShopApp.Models.Product { Name = "Neem", Price = 20.00m, Description = "A powerful antibacterial and antifungal plant.", Stock = 12, Category = "Medicinal", ImageUrl = "/images/lemon_balm.jpg" },
            new PlantShopApp.Models.Product { Name = "Gotu Kola", Price = 18.50m, Description = "Supports brain health and promotes longevity.", Stock = 15, Category = "Medicinal", ImageUrl = "/images/sage.jpg" }
        );
        await context.SaveChangesAsync();
    }

    // Seed Succulents & Cacti
    if (!context.Products.Any(p => p.Category == "Succulents & Cacti"))
    {
        context.Products.AddRange(
            new PlantShopApp.Models.Product { Name = "Jade Plant", Price = 19.99m, Description = "Symbol of good luck and prosperity. Very easy to care for.", Stock = 20, Category = "Succulents & Cacti", ImageUrl = "/images/lemon_balm.jpg" },
            new PlantShopApp.Models.Product { Name = "Zebra Haworthia", Price = 14.99m, Description = "Striking striped leaves. Perfect for desks and small spaces.", Stock = 15, Category = "Succulents & Cacti", ImageUrl = "/images/sage.jpg" },
            new PlantShopApp.Models.Product { Name = "String of Pearls", Price = 22.50m, Description = "Beautiful hanging succulent with pearl-like leaves.", Stock = 10, Category = "Succulents & Cacti", ImageUrl = "/images/rosemary.jpg" },
            new PlantShopApp.Models.Product { Name = "Burro's Tail", Price = 24.00m, Description = "Thick trailing leaves, looks stunning in a hanging basket.", Stock = 12, Category = "Succulents & Cacti", ImageUrl = "/images/peppermint.jpg" },
            new PlantShopApp.Models.Product { Name = "Echeveria", Price = 12.99m, Description = "Classic rosette shape. Comes in stunning pastel colors.", Stock = 30, Category = "Succulents & Cacti", ImageUrl = "/images/chamomile.jpg" },
            new PlantShopApp.Models.Product { Name = "Crown of Thorns", Price = 26.00m, Description = "Beautiful small flowers year-round if kept in bright light.", Stock = 8, Category = "Succulents & Cacti", ImageUrl = "/images/calendula.jpg" },
            new PlantShopApp.Models.Product { Name = "Christmas Cactus", Price = 18.50m, Description = "Blooms brilliantly in the winter. A holiday favorite.", Stock = 25, Category = "Succulents & Cacti", ImageUrl = "/images/echinacea.jpg" },
            new PlantShopApp.Models.Product { Name = "Bunny Ears Cactus", Price = 16.00m, Description = "Cute flat pads that look like bunny ears. Handle with care!", Stock = 18, Category = "Succulents & Cacti", ImageUrl = "/images/turmeric_plant.jpg" },
            new PlantShopApp.Models.Product { Name = "Panda Plant", Price = 15.50m, Description = "Fuzzy soft leaves with brown spotted edges.", Stock = 14, Category = "Succulents & Cacti", ImageUrl = "/images/ashwagandha.jpg" },
            new PlantShopApp.Models.Product { Name = "ZZ Plant", Price = 28.99m, Description = "Virtually indestructible. Thrives in very low light conditions.", Stock = 22, Category = "Succulents & Cacti", ImageUrl = "/images/holy_basil.jpg" }
        );
        await context.SaveChangesAsync();
    }
    else
    {
        // One-time repair logic to update old categories if they are still "Indoor" by default
        var fixes = context.Products.ToList();
        foreach (var p in fixes)
        {
            if (p.Category == "Indoor" && (p.Name == "Lavender Bush" || p.Name == "Rose Bush" || p.Name == "Bougainvillea")) p.Category = "Outdoor";
            if (p.Category == "Indoor" && (p.Name == "Peppermint" || p.Name == "Chamomile" || p.Name == "Echinacea" || p.Name == "Holy Basil (Tulsi)" || p.Name == "Ginger" || p.Name == "Turmeric" || p.Name == "Rosemary" || p.Name == "Thyme" || p.Name == "Sage" || p.Name == "Lemon Balm" || p.Name == "Calendula" || p.Name == "Ginseng" || p.Name == "Ashwagandha" || p.Name == "Neem" || p.Name == "Gotu Kola")) p.Category = "Medicinal";
        }
        await context.SaveChangesAsync();
    }
}

app.Run();

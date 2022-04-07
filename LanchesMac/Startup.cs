using LanchesMac.Context;
using LanchesMac.Models;
using LanchesMac.Repositories;
using LanchesMac.Repositories.Interfaces;
using LanchesMac.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LanchesMac;
public class Startup {
    public Startup(IConfiguration configuration) {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services) {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        // IdentityUser gerenciar usuários, IdentityRole gerenciar perfil
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()// Recuperar informações para o contexto
            .AddDefaultTokenProviders();


        //services.Configure<IdentityOptions>(options =>
        //{
        //    // Default Password settings.
        //    options.Password.RequireDigit = false;
        //    options.Password.RequireLowercase = false;
        //    options.Password.RequireNonAlphanumeric = false;
        //    options.Password.RequireUppercase = false;
        //    options.Password.RequiredLength = 3;
        //    options.Password.RequiredUniqueChars = 1;
        //});

        services.AddTransient<ILancheRepository, LancheRepository>();
        services.AddTransient<ICategoriaRepository, CategoriaRepository>();
        services.AddTransient<IPedidoRepository, PedidoRepository>();


        // Criando usuários e administradores
        services.AddScoped<ISeedUserRoleInitial, SeedUserRoleInitial>();
        services.AddAuthorization(options => {
            options.AddPolicy("Admin",
            politica => {
                politica.RequireRole("Admin");
            });
        });


        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped(sp => CarrinhoCompra.GetCarrinho(sp));

        services.AddControllersWithViews();

        services.AddMemoryCache();
        services.AddSession();

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ISeedUserRoleInitial seedUserRoleInitial) {

        if (env.IsDevelopment()) {

            app.UseDeveloperExceptionPage();

        } else {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();


        // Cria os perfis
        seedUserRoleInitial.SeedRoles();
        // Cria os usuários e atribui aos perfis
        seedUserRoleInitial.SeedUsers();


        app.UseSession();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => {


            // Novo endpoint de Areas
            endpoints.MapControllerRoute(
                name: "AdminArea",
                pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");


            endpoints.MapControllerRoute(
               name: "categoriaFiltro",
               pattern: "Lanche/{action}/{categoria?}",
               defaults: new { Controller = "Lanche", action = "List" });

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}
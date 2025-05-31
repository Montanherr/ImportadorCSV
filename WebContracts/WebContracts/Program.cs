using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebContracts.Data;

var builder = WebApplication.CreateBuilder(args);

// Configura o AppDbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adiciona os serviços de autenticação com cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";           // Caminho para a tela de login
        options.LogoutPath = "/Auth/Logout";         // Caminho para logout
        options.AccessDeniedPath = "/Auth/AccessDenied"; // Caminho para acesso negado (não finalizado)

        // TOKEN DA APLICAÇÃO

        // Por padrão vai expirar após 30 minutos
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;

        // E depois direcionar para /Auth/Login?expired=true 
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                if (context.Request.Path != "/Auth/Login" && context.Response.StatusCode == 200)
                {
                    context.Response.Redirect("/Auth/Login?expired=true");
                }
                return Task.CompletedTask;
            }
        };
    });

// Autorização (requerido após autenticação)
builder.Services.AddAuthorization();

// Adiciona os serviços MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configura o pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Middleware de autenticação e autorização 
app.UseAuthentication(); // Necessário ser antes do UseAuthorization por conta da ordem
app.UseAuthorization();

// Mapeamento de rotas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

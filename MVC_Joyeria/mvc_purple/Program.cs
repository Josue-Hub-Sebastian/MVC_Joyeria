using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using mvc_purple.Services;
using System;
using System.Net.Http.Headers;
using mvc_purple.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);



// ================================
builder.Services.AddDbContext<JoyeriaDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));


// ================================
// CONFIGURACIÓN GENERAL MVC
// ================================
builder.Services.AddControllersWithViews();

// 🔹 Sesión (para almacenar token JWT)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromHours(8);
});

// 🔹 IHttpContextAccessor (necesario para el TokenHandler)
builder.Services.AddHttpContextAccessor();

// ================================
// 🔐 AUTENTICACIÓN POR COOKIES
// ================================
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Cliente/Login";
        options.LogoutPath = "/Cliente/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name = "PurpleSkyAuth";
    });

// ================================
// CONFIGURACIÓN DE CONEXIÓN API
// ================================

// 🔸 Base URL del API
var apiBase = builder.Configuration.GetValue<string>("Api:BaseUrl")?.TrimEnd('/')
              ?? "http://localhost:5229";

// 🔸 Token handler (adjunta Authorization header si hay token en sesión)
builder.Services.AddTransient<TokenHandler>();

// 🔸 HttpClient para cada servicio (con TokenHandler)
builder.Services.AddHttpClient<IProductoApiService, ProductoApiService>(c =>
{
    c.BaseAddress = new Uri(apiBase + "/api/");
    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
})
.AddHttpMessageHandler<TokenHandler>();

builder.Services.AddHttpClient<IPedidoApiService, PedidoApiService>(c =>
{
    c.BaseAddress = new Uri(apiBase + "/api/");
    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
})
.AddHttpMessageHandler<TokenHandler>();

builder.Services.AddHttpClient<IClienteApiService, ClienteApiService>(c =>
{
    c.BaseAddress = new Uri(apiBase + "/api/");
    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
})
.AddHttpMessageHandler<TokenHandler>();

// ================================
// AUTORIZACIÓN POR POLÍTICAS
//LAMAMADELAMAMADELAMAMADELAMAMA
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("EsAdmin", "True"));
});
//ASEGURA QUE SOLO USUARIOS CON EL CLAIM EsAdmin=TRUE PUEDEN ACCEDER A RECURSOS PROTEGIDOS CON ESTA POLÍTICA





// ================================
// CONFIGURACIÓN DEL PIPELINE HTTP
// ================================
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection(); // No la usas si tu API está en HTTP

app.UseStaticFiles();

app.UseRouting();

// 🔹 Middleware orden correcto
app.UseSession();
app.UseAuthentication();  // ✅ necesario antes de UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using mvc_purple.Services;
using System;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

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
// CONFIGURACIÓN DE CONEXIÓN API
// ================================

// 🔸 Base URL del API (ajustada a tu API que corre en HTTP)
var apiBase = builder.Configuration.GetValue<string>("Api:BaseUrl")?.TrimEnd('/')
              ?? "http://localhost:5229";

// 🔸 Token handler (adjunta Authorization header si hay token en sesión)
builder.Services.AddTransient<TokenHandler>();

// 🔸 HttpClient por servicio (cada uno usa TokenHandler para enviar token JWT)
builder.Services.AddHttpClient<IProductoApiService, ProductoApiService>(c =>
{
    c.BaseAddress = new Uri(apiBase + "/api/");
    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    // 🔧 Aceptar cualquier certificado (por si algún día cambias a HTTPS)
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
// CONFIGURACIÓN DEL PIPELINE HTTP
// ================================
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// 🔹 No redirigimos a HTTPS porque tu API está en HTTP
// app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// 🔹 Session antes de autorización
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

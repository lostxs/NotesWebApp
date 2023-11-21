using Microsoft.EntityFrameworkCore;
using NotesApp_Postgre.Context;
using NotesApp_Postgre.Models;
using NotesApp_Postgre.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<ReminderService>();

builder.Services.AddSignalR();

builder.Services.AddTransient<NotificationHub>();

builder.Services.AddControllersWithViews();

builder.Services.AddSwaggerGen();


//Добавление CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {


            builder.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
        });
});


//Регистрация базы данных
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});




var app = builder.Build();

app.UseSwagger().UseSwaggerUI();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=notes}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();

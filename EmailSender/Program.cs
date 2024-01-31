using EmailSender.ApplicationDBContext;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Please check the connection details in the appsettings.json the DB name will be EmailSender

/*
  >>>>>>>>> If this is the first time to run the APP,then before Start the App you will need to create the DB <<<<<<<<<<<<<<

1) -> Run "Update-Database" on the Package Manager Console
2) -> Run "Add-Migration <YourMigrationTitle>" on the Package Manager Console
3) -> Run "Update-Database" on the Package Manager Console
 */

var dbConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDBContext>(options => options.UseSqlServer(dbConnection));

builder.Services.AddMvc().AddNToastNotifyToastr(new ToastrOptions()
{
    ProgressBar = true,
    PositionClass = ToastPositions.TopRight,
    PreventDuplicates = true,
    CloseButton = true,
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Messages}/{action=CreateAndShowMessages}/{id?}");

app.Run();

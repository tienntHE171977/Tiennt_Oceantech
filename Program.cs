using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Tiennthe171977_Oceanteach.Business;
using Tiennthe171977_Oceanteach.Models;
using Tiennthe171977_Oceanteach.Service;
using System.Reflection;
using Tiennthe171977_Oceanteach.Validators;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.WebHost.UseUrls("http://localhost:5000");
builder.Services.AddDbContext<OceantechContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IEmployeeBusiness, EmployeeBusiness>();
builder.Services.AddScoped<ILocationBusiness, LocationBusiness>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddFluentValidation(fv =>
{
    fv.RegisterValidatorsFromAssemblyContaining<EmployeeValidator>();
    fv.DisableDataAnnotationsValidation = true;
    fv.ImplicitlyValidateChildProperties = true;
    fv.AutomaticValidationEnabled = false;
});
builder.Services.AddLogging();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Employee}/{action=Index}/{id?}");

app.Run();
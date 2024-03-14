
using FindYourStoryApp.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


//Adds sessions to the container so that sessions can be used in this project.
//The options of this session are:
//-> If session remains inactive for 10 minutes, the session expires.
//-> Sets the cookie settings, HttpOnly and IsEssential to true (Microsoft, 2023).
//FreeCode Spot (2020) demonstrates adding sessions for Firebase Authentication.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//Adds the DBContext for Find Your Story DB.
builder.Services.AddDbContext<FindYourStoryDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Find_Your_Story_DB")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

//Uses the session.
//FreeCode Spot (2020) demonstrates using sessions for Firebase Authentication.
app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

//REFERENCE LIST:
//FreeCode Spot. 2020. How to Integrate Firebase in ASP NET Core MVC, 2020 (Version 1.0)
//[Source code] https://www.freecodespot.com/blog/firebase-in-asp-net-core-mvc/
//(Accessed 8 March 2024).
//Microsoft. 2023. Session and state management in ASP.NET Core, 11 April 2023 (Version 1.0)
//[Online]. Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-8.0
//[Accessed 8 March 2024].

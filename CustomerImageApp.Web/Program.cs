using CustomerImageApp.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();


builder.Services.AddHttpClient("CustomerAPI", client =>
{
    client.BaseAddress = new Uri("http://ecommerce112-001-site5.qtempurl.com/");
    client.Timeout = TimeSpan.FromSeconds(30);
});


builder.Services.AddScoped<ICustomerImageService, CustomerImageService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Customers}/{action=List}/{id?}");

app.Run();
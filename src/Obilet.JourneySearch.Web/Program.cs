var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

#region Configure ObiletApi HttpClient

builder.Services.AddHttpClient("ObiletApi", client =>
{
    client.BaseAddress = new Uri(
        EnvConfiguration.OBILET_API_BASE_URL.TrimEnd('/') + "/");

    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));

    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Basic", EnvConfiguration.OBILET_API_CLIENT_TOKEN);

    client.Timeout = TimeSpan.FromSeconds(30);
});

#endregion

#region Configure Session

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".ObiletJourneySearch.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(20);
});

#endregion

#region Configure ObiletApiClient and Services

builder.Services.AddTransient<ObiletApiClient>();
builder.Services.AddScoped<IObiletSessionService,ObiletSessionService>();
builder.Services.AddScoped<IBusLocationService,BusLocationService>();
builder.Services.AddTransient<IJourneyService, JourneyService>();

#endregion

// Bütün servis kayıtları tamamlandıktan sonra Build çağrılır.
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
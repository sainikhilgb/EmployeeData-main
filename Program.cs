var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// Add session services with configuration.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout duration
    options.Cookie.HttpOnly = true;                // Secure session cookie
    options.Cookie.IsEssential = true;             // Required for GDPR compliance
});

// Build the app.
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error"); // Use custom error handling in production
    app.UseHsts();                     // Enforce strict transport security (HSTS)
}

app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
app.UseStaticFiles();      // Serve static files (e.g., CSS, JS, images)

app.UseRouting();          // Enable endpoint routing

app.UseSession();          // Enable session handling (must come before authorization)

// Add WebSocket support (if required for your app).
app.UseWebSockets();

// Authorization middleware (if you have authentication logic).
app.UseAuthorization();
app.MapControllers();

// Map Razor Pages to endpoints.
app.MapRazorPages();

// Run the application.
app.Run();

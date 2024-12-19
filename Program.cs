var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Configure CORS
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowMyOrigin",
    builder =>
    {
      builder.WithOrigins(
        "http://localhost:5116/Registration/RegistrationployeeList?handler=ProjectName&projectCode={id}"
      )
      .AllowAnyMethod()
      .AllowAnyHeader();
    });
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

app.UseCors("AllowMyOrigin"); // Add CORS middleware after UseRouting

app.UseAuthorization();

app.MapRazorPages();

app.UseWebSockets();


app.Run();

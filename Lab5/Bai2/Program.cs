using Bai2.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddOpenApi();

builder.Services.AddSingleton<IRepository, Repository>();

builder.Services.AddSingleton<IProductRepository, ProductRepository>();

var app = builder.Build();
 
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
 
app.MapControllers(); 

app.Run();
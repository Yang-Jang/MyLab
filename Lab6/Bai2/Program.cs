using Microsoft.EntityFrameworkCore;
using Bai2.Models;
using Bai2.Data;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ReservationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

// app.MapGet("/", () => "Hello World!");

app.Run();

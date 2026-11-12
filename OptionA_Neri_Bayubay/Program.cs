
using OptionA_Neri_Bayubay.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Register one shared AssetService instance
builder.Services.AddSingleton<AssetService>();

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

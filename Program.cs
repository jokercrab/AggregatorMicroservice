using Aggregator.AuxiliaryServices;
using Aggregator.DataFetchers;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.AspNetCore.Server.Kestrel.Core;




var builder = WebApplication.CreateBuilder(args);
//CORS
var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy  =>
                      {
                          policy.WithOrigins("http://10.42.0.207:3000",
                                                "http://10.42.0.1",
                                                "http://localhost:3000");
                      });
});
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddScoped<Animevost>();
builder.Services.AddScoped<Anilibria>();
builder.Services.AddSingleton<PeriodicTask>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<PeriodicTask>());
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors(MyAllowSpecificOrigins);

app.MapControllers();

app.Run();

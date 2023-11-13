using Aggregator.Services;
using Aggregator.DataFetchers;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Aggregator.DataStructs;
using System.Net.Http.Headers;




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
//builder.Services.AddHttpClient();
builder.Services.AddConfig(builder.Configuration)
                .AddDependencyGroup();
builder.Services.AddHttpClient();

var app = builder.Build();
app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors(MyAllowSpecificOrigins);

app.MapControllers();



app.Run();

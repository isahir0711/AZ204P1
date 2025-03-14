using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Project1.Services;
var builder = WebApplication.CreateBuilder(args);

//Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options =>
    {
        builder.Configuration.Bind("AzureAd", options);
        options.TokenValidationParameters.NameClaimType = "name";
    }, options => { builder.Configuration.Bind("AzureAd", options); });

// Configure authorization
builder.Services.AddAuthorization(config =>
{
    config.AddPolicy("AuthZPolicy", policy =>
        policy.RequireRole("Forecast.Read"));
});

builder.Services.AddScoped<FileStorage>();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

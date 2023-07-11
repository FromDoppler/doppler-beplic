using System.Globalization;
using DopplerBeplic.DopplerSecurity;
using DopplerBeplic.Models.Config;
using DopplerBeplic.Models.DTO;
using DopplerBeplic.Services.Classes;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// It is if you want to override the configuration in your
// local environment, `*.Secret.*` files will not be
// included in git.
builder.Configuration.AddJsonFile("appsettings.Secret.json",
    optional: true,
    reloadOnChange: true);

// It is to override configuration using Docker's services.
// Probably this will be the way of overriding the
// configuration in our Swarm stack.
builder.Configuration.AddJsonFile("/run/secrets/appsettings.Secret.json",
    optional: true,
    reloadOnChange: true);

// It is to override configuration using a different file
// for each configuration entry. For example, you can create
// the file `/run/secrets/Logging__LogLevel__Default` with
// the content `Trace`. See:
// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0#key-per-file-configuration-provider
builder.Configuration.AddKeyPerFile("/run/secrets",
    optional: true,
    reloadOnChange: true);

builder.Services.Configure<ApiConnectionOptions>(
    builder.Configuration.GetSection(ApiConnectionOptions.Connection));
builder.Services.Configure<DefaulValuesOptions>(
    builder.Configuration.GetSection(DefaulValuesOptions.Values));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDopplerSecurity();
builder.Services.AddSingleton<BeplicSdk>();
builder.Services.AddSingleton<IBeplicService, BeplicService>();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter the token into field as 'Bearer {token}'",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme },
                },
                Array.Empty<string>()
            }
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/account", Results<Created<string>, BadRequest<string>> (
    IBeplicService beplicService,
    [FromBody] UserCreationDTO body) =>
{
    var response = beplicService.CreateUser(body);

    return response.Success ?
        TypedResults.Created("/", string.Format(CultureInfo.InvariantCulture, "User created with ID:{0}", response.CustomerId))
        : TypedResults.BadRequest(
            string.Format(
                CultureInfo.InvariantCulture,
                "Failed to create user. ErrorStatus: {0} Error:{1}",
                response.ErrorStatus,
                response.Error));
})
.WithName("CreateUser")
.WithOpenApi()
.RequireAuthorization(Policies.OnlySuperuser);

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }

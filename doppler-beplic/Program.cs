using System.Globalization;
using DopplerBeplic.Models.DTO;
using DopplerBeplic.Services.Classes;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<BeplicService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/account", Results<Created<string>, BadRequest<string>> (
    BeplicService beplicService,
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
.WithOpenApi();

app.Run();

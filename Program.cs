using FluentValidation;
using FluentValidation.AspNetCore;
using FormBuilder.Api.Data;
using FormBuilder.Api.Services;
using FormBuilder.Api.Validators;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<HtmlFormParser>();
builder.Services.AddScoped<FormService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateFormRequestValidator>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=formbuilder.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Angular");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using ECommerceAPI.Application.Validators.Products;
using ECommerceAPI.Infrastructure;
using ECommerceAPI.Infrastructure.Filters;
using ECommerceAPI.Infrastructure.Services.Storage.Azure;
using ECommerceAPI.Persistence;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistenceServices();
builder.Services.AddInfrastructureServices();

//builder.Services.AddStorage<LocalStorage>(); // LocalStorage'ý DI Container'a ekliyoruz
builder.Services.AddStorage<AzureStorage>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
                {
                    policy.WithOrigins("https://localhost:4200", "http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
});

builder.Services.AddControllers(options => options.Filters.Add<ValidationFilter>())
    .AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<CreateProductValidator>())
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true); //Mevcut .netteki filterlarý bu þekilde baskýlarýz ve tüm filter kontrolü artýk bizde olur.
    
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(); //wwwroot'u kullanabilmek için çaðýrmamýz gerekiyor

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

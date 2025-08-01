using ECommerceAPI.API.Configurations.ColumnWriter;
using ECommerceAPI.API.Extensions;
using ECommerceAPI.Application;
using ECommerceAPI.Application.Validators.Products;
using ECommerceAPI.Infrastructure;
using ECommerceAPI.Infrastructure.Filters;
using ECommerceAPI.Infrastructure.Services.Storage.Azure;
using ECommerceAPI.Infrastructure.Services.Storage.Local;
using ECommerceAPI.Persistence;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Sinks.PostgreSQL;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistenceServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();

//builder.Services.AddStorage<LocalStorage>(); // LocalStorage'� DI Container'a ekliyoruz
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

//Loglama ile ilgili yap�land�rmay� burada yap�yoruz
Logger log = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/logs.txt")
    .WriteTo.PostgreSQL(builder.Configuration.GetConnectionString("PostgreSQL"), "logs", needAutoCreateTable: true, 
     columnOptions: new Dictionary<string, ColumnWriterBase>
    {
         {"message", new RenderedMessageColumnWriter()},
         {"message_template", new MessageTemplateColumnWriter()},
         {"level", new LevelColumnWriter() },
         {"time_stamp", new TimestampColumnWriter() },
         {"exception", new ExceptionColumnWriter() },
         {"log_event", new LogEventSerializedColumnWriter() },
         {"user_name", new UsernameColumnWriter() }
    })
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();

// bu �ekilde uygulamaya at�lan requestleri de loglayabiliriz.
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-ch-ua");
    logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
    logging.CombineLogs = true;
});

builder.Host.UseSerilog(log); // bunu �a��rd���m�z zaman default olan log mekanizmas�yla Serilog'u de�i�tirir 

builder.Services.AddControllers(options => options.Filters.Add<ValidationFilter>())
    .AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<CreateProductValidator>())
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true); //Mevcut .netteki filterlar� bu �ekilde bask�lar�z ve t�m filter kontrol� art�k bizde olur.
    
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Token do�rulama
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Admin",options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Token:Issuer"],
            ValidAudience = builder.Configuration["Token:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
            // bu �ekilde, olu�turdu�umuz token'�n s�resinin do�ru �al��mas�n� sa�lad�k
            LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false,

            NameClaimType = ClaimTypes.Name // bu sayede kullan�c�n�n Name property'sine eri�ebiliriz.
        };
    });




var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureExceptionHandler<Program>(app.Services.GetRequiredService<ILogger<Program>>()); // bu �ekilde bir middleware olu�turup o metodu da burada kullanabiliriz.

app.UseStaticFiles(); //wwwroot'u kullanabilmek i�in �a��rmam�z gerekiyor

app.UseSerilogRequestLogging(); //bunun alt�ndaki yap�land�rmalar loglan�r

app.UseCors();

app.UseHttpsRedirection();

app.UseHttpLogging();

app.UseAuthentication(); // yapt���m�z in�aada Authorization mekanizmas�n� kullanabilmemiz i�in bu middleware'� buraya eklemeliyiz. A�a��daki otomatik geldi.
app.UseAuthorization();

// username'i alabilmek i�in middleware yazd�k
app.Use(async (context, next) =>
{
    var username = context.User?.Identity?.IsAuthenticated != null || true ? context.User.Identity.Name : null;
    LogContext.PushProperty("user_name", username);
    await next();
});

app.MapControllers();

app.Run();

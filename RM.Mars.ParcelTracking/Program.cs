using RM.Mars.ParcelTracking.Application.Services.AuditTrail;
using RM.Mars.ParcelTracking.Application.Services.Parcels;
using RM.Mars.ParcelTracking.Application.Services.StatusValidator;
using RM.Mars.ParcelTracking.Application.Services.TimeCalculator;
using RM.Mars.ParcelTracking.Application.Services.Validation;
using RM.Mars.ParcelTracking.Common.Utils.DateTimeProvider;
using RM.Mars.ParcelTracking.Infrastructure.Repositories.Parcels;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<TimeCalculatorOptions>(builder.Configuration.GetSection("TimeCalculator"));
builder.Services.AddSingleton<IParcelsRepository, ParcelsRepository>();
builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
builder.Services.AddTransient<IParcelService, ParcelService>();
builder.Services.AddTransient<ITimeCalculatorService, TimeCalculatorService>();
builder.Services.AddTransient<IStatusValidation, StatusValidation>();
builder.Services.AddTransient<IAuditTrailService, AuditTrailService>();
builder.Services.AddTransient<IParcelRequestValidation, ParcelRequestValidation>();
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

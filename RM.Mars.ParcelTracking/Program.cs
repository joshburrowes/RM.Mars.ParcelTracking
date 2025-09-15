using RM.Mars.ParcelTracking.Repositories.Parcels;
using RM.Mars.ParcelTracking.Services.AuditTrail;
using RM.Mars.ParcelTracking.Services.Parcels;
using RM.Mars.ParcelTracking.Services.StatusValidator;
using RM.Mars.ParcelTracking.Services.TimeCalculator;
using RM.Mars.ParcelTracking.Services.Validation;
using RM.Mars.ParcelTracking.Utils.DateTimeProvider;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IParcelsRepository, ParcelsRepository>();
builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
builder.Services.AddTransient<IParcelService, ParcelService>();
builder.Services.AddTransient<ITimeCalculatorService, TimeCalculatorService>();
builder.Services.AddTransient<IStatusValidator, StatusValidator>();
builder.Services.AddTransient<IAuditTrailService, AuditTrailService>();
builder.Services.AddTransient<IParcelRequestValidation, ParcelRequestValidation>();
builder.Services.AddControllers();
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

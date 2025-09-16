using System.Text.Json;
using System.Text.Json.Serialization;
using RM.Mars.ParcelTracking.Api.Models.Response;
using RM.Mars.ParcelTracking.Application.Enums;
using RM.Mars.ParcelTracking.Application.Models.Parcel;
using RM.Mars.ParcelTracking.Application.Services.AuditTrail;
using RM.Mars.ParcelTracking.Common.Utils.DateTimeProvider;
using RM.Mars.ParcelTracking.Infrastructure.Models;

namespace RM.Mars.ParcelTracking.Infrastructure.Repositories.Parcels;

public class ParcelsRepository : IParcelsRepository
{
    private readonly string _filePath;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IAuditTrailService _auditTrailService;

    public ParcelsRepository(IDateTimeProvider dateTimeProvider, IAuditTrailService auditTrailService)
    {
        _dateTimeProvider = dateTimeProvider;
        _auditTrailService = auditTrailService;
        string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
        _filePath = Path.Combine(projectRoot, @"Infrastructure\Database", "DocumentDb.json");
    }

    public async Task AddParcelAsync(ParcelCreatedResponse parcelResponse)
    {
        if (parcelResponse == null)
        {
            throw new ArgumentNullException(nameof(parcelResponse));
        }

        DatabaseDocument document = GetDocument();

        ParcelDto parcel = new ParcelDto
        {
            Status = parcelResponse.Status,
            Barcode = parcelResponse.Barcode,
            LaunchDate = parcelResponse.LaunchDate.Date,
            Sender = parcelResponse.Sender,
            Origin = parcelResponse.Origin,
            Contents = parcelResponse.Contents,
            EstimatedArrivalDate = parcelResponse.EstimatedArrivalDate.Date,
            Destination = parcelResponse.Destination,
            Recipient = parcelResponse.Recipient,
            LastUpdated = _dateTimeProvider.UtcNow,
            History = _auditTrailService.UpdateStatusHistory(null, parcelResponse.Status)
        };

        document.Parcels.Add(parcel);

        string json = JsonSerializer.Serialize(document, CreateSerializerOptions());
        await File.WriteAllTextAsync(_filePath, json);
    }

    public Task<ParcelDto?> GetParcelByBarcodeAsync(string barcode)
    {
        if (string.IsNullOrEmpty(barcode))
        {
            throw new ArgumentNullException(nameof(barcode));
        }

        ParcelDto? parcel = GetParcels().Find(p => p.Barcode == barcode);

        return Task.FromResult(parcel);
    }

    public Task UpdateParcelAsync(ParcelDto parcel, ParcelStatus newStatus)
    {
        DatabaseDocument document = GetDocument();

        ParcelDto? existing = document.Parcels.Find(p => p.Barcode == parcel.Barcode);

        if (existing == null)
        {
            throw new FileNotFoundException($"Parcel with barcode: '{parcel.Barcode}' not found in database.");
        }

        existing.Status = newStatus;
        existing.Destination = parcel.Destination;
        existing.Origin = parcel.Origin;
        existing.Sender = parcel.Sender;
        existing.Recipient = parcel.Recipient;
        existing.EstimatedArrivalDate = parcel.EstimatedArrivalDate.Date;
        existing.LaunchDate = parcel.LaunchDate.Date;
        existing.Contents = parcel.Contents;
        existing.LastUpdated = _dateTimeProvider.UtcNow;
        existing.History = _auditTrailService.UpdateStatusHistory(existing.History, newStatus);

        string json = JsonSerializer.Serialize(document, CreateSerializerOptions());
        return File.WriteAllTextAsync(_filePath, json);
    }

    private DatabaseDocument GetDocument()
    {
        if (!File.Exists(_filePath))
            return new DatabaseDocument();

        string json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<DatabaseDocument>(json, CreateSerializerOptions()) ?? new DatabaseDocument();
    }

    private static JsonSerializerOptions CreateSerializerOptions() => new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private List<ParcelDto> GetParcels() => GetDocument().Parcels;
}
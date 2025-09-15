using System.Text.Json;
using RM.Mars.ParcelTracking.Models;
using RM.Mars.ParcelTracking.Models.Parcel;
using RM.Mars.ParcelTracking.Models.Response;
using RM.Mars.ParcelTracking.Services.AuditTrail;
using RM.Mars.ParcelTracking.Utils.DateTimeProvider;

namespace RM.Mars.ParcelTracking.Repositories.Parcels;

/// <summary>
/// Provides methods for managing parcels, including retrieving, adding, updating, and querying parcels stored in a
/// JSON-based document database.
/// </summary>
/// <remarks>N.B.: This is a simple local document db for the purposes of the coding challenge. This repository interacts with a JSON file to persist parcel data. It supports operations such as
/// retrieving all parcels, adding new parcels, updating existing parcels, and querying parcels by barcode. The
/// repository ensures that parcel data is serialized and deserialized in a structured format.</remarks>
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
        _filePath = Path.Combine(projectRoot, "Database", "DocumentDb.json");
    }

    public List<ParcelDto> GetParcels() => GetDocument().Parcels;

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
            LaunchDate = parcelResponse.LaunchDate,
            Sender = parcelResponse.Sender,
            Origin = parcelResponse.Origin,
            Contents = parcelResponse.Contents,
            EstimatedArrivalDate = parcelResponse.EstimatedArrivalDate,
            EtaDays = parcelResponse.EtaDays,
            Destination = parcelResponse.Destination, 
            Recipient = parcelResponse.Recipient,
            LastUpdated = _dateTimeProvider.UtcNow,
            History = _auditTrailService.UpdateStatusHistory(null, parcelResponse.Status)
        };

        document.Parcels.Add(parcel);

        JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(document, options);

        await File.WriteAllTextAsync(_filePath, json);
    }

    public Task<ParcelDto?> GetParcelByBarcode(string barcode)
    {
        if (string.IsNullOrEmpty(barcode))
        {
            throw new ArgumentNullException(nameof(barcode));
        }

        ParcelDto? parcel = GetParcels().Find(p => p.Barcode == barcode);

        return Task.FromResult(parcel);
    }

    public Task UpdateParcelAsync(ParcelDto parcel)
    {
        if (parcel == null)
        {
            throw new ArgumentNullException(nameof(parcel));
        }

        DatabaseDocument document = GetDocument();

        ParcelDto? existing = document.Parcels.Find(p => p.Barcode == parcel.Barcode);
        if (existing == null)
        {
            throw new InvalidOperationException($"Parcel with barcode '{parcel.Barcode}' not found.");
        }

        existing.Status = parcel.Status;
        existing.Destination = parcel.Destination;
        existing.Origin = parcel.Origin;
        existing.Sender = parcel.Sender;
        existing.Recipient = parcel.Recipient;
        existing.EstimatedArrivalDate = parcel.EstimatedArrivalDate;
        existing.LaunchDate = parcel.LaunchDate;
        existing.EtaDays = parcel.EtaDays;
        existing.Contents = parcel.Contents;
        existing.LastUpdated = _dateTimeProvider.UtcNow;
        existing.History = _auditTrailService.UpdateStatusHistory(existing.History, parcel.Status);

        JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(document, options);
        return File.WriteAllTextAsync(_filePath, json);
    }

    private DatabaseDocument GetDocument()
    {
        if (!File.Exists(_filePath))
            return new DatabaseDocument();

        string json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<DatabaseDocument>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new DatabaseDocument();
    }
}
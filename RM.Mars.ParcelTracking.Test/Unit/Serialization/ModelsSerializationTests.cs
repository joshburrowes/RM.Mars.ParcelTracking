using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using RM.Mars.ParcelTracking.Application.Enums;
using RM.Mars.ParcelTracking.Api.Models.Requests;
using RM.Mars.ParcelTracking.Api.Models.Response;
using RM.Mars.ParcelTracking.Application.Models.AuditTrail;
using RM.Mars.ParcelTracking.Application.Models.Validation;
using RM.Mars.ParcelTracking.Common.Utils.Json;

namespace RM.Mars.ParcelTracking.Test.Unit.Serialization;

[TestFixture]
public class ModelsSerializationTests
{
    private JsonSerializerOptions _options = null!;

    [SetUp]
    public void SetUp()
    {
        _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
        _options.Converters.Add(new JsonStringEnumConverter());
        _options.Converters.Add(new DateOnlyDateTimeConverter());
    }

    [Test]
    public void CreateParcelRequest_Deserializes_FromCamelCaseJson()
    {
        string json = "{\"barcode\":\"RMARS1234567890123456789M\",\"deliveryService\":\"Express\",\"sender\":\"S\",\"recipient\":\"R\",\"contents\":\"Book\"}";
        CreateParcelRequest? req = JsonSerializer.Deserialize<CreateParcelRequest>(json, _options);
        req.Should().NotBeNull();
        req!.Barcode.Should().Be("RMARS1234567890123456789M");
        req.DeliveryService.Should().Be(nameof(DeliveryServiceEnum.Express));
        req.Sender.Should().Be("S");
        req.Recipient.Should().Be("R");
        req.Contents.Should().Be("Book");
    }

    [Test]
    public void UpdateParcelStatusRequest_Deserializes_FromCamelCaseJson()
    {
        string json = "{\"newStatus\":\"OnRocketToMars\"}";
        UpdateParcelStatusRequest? req = JsonSerializer.Deserialize<UpdateParcelStatusRequest>(json, _options);
        req.Should().NotBeNull();
        req!.NewStatus.Should().Be(nameof(ParcelStatus.OnRocketToMars));
    }

    [Test]
    public void StatusValidationResponse_Serializes_EnumAsString()
    {
        var resp = new StatusValidationResponse
        {
            Valid = true,
            Reason = string.Empty,
            NewParcelStatus = ParcelStatus.LandedOnMars
        };
        string json = JsonSerializer.Serialize(resp, _options);
        json.Should().Contain("\"valid\":true")
            .And.Contain("\"newParcelStatus\":\"LandedOnMars\"")
            .And.Contain("\"reason\":\"\"");
    }

    [Test]
    public void ValidationResponse_Serializes_NestedRequest()
    {
        var nestedReq = new CreateParcelRequest
        {
            Barcode = "RMARS1234567890123456789M",
            DeliveryService = nameof(DeliveryServiceEnum.Standard),
            Sender = "S",
            Recipient = "R",
            Contents = "C"
        };
        var resp = new ValidationResponse
        {
            IsValid = false,
            ErrorMessage = "Barcode invalid",
            CreateParcelRequest = nestedReq
        };
        string json = JsonSerializer.Serialize(resp, _options);
        json.Should().Contain("\"isValid\":false")
            .And.Contain("\"errorMessage\":\"Barcode invalid\"")
            .And.Contain("\"createParcelRequest\":{")
            .And.Contain("\"barcode\":\"RMARS1234567890123456789M\"")
            .And.Contain("\"deliveryService\":\"Standard\"");
    }

    [Test]
    public void StatusAuditTrail_List_Serializes_StatusAsString()
    {
        var list = new List<StatusAuditTrail>
        {
            new StatusAuditTrail { Status = ParcelStatus.Created, TimeStamp = "2025-08-20" },
            new StatusAuditTrail { Status = ParcelStatus.OnRocketToMars, TimeStamp = "2025-09-03" }
        };
        string json = JsonSerializer.Serialize(list, _options);
        json.Should().Be("[{\"status\":\"Created\",\"timeStamp\":\"2025-08-20\"},{\"status\":\"OnRocketToMars\",\"timeStamp\":\"2025-09-03\"}]");
    }

    [Test]
    public void ParcelCreatedResponse_Serializes_InExpectedOrder_AndFormat()
    {
        ParcelCreatedResponse response = new ParcelCreatedResponse
        {
            Barcode = "RMARS1234567890123456789M",
            Status = ParcelStatus.Created,
            LaunchDate = new DateTime(2025, 9, 3, 12, 0, 0, DateTimeKind.Utc),
            EtaDays = 90,
            EstimatedArrivalDate = new DateTime(2025, 12, 2, 18, 0, 0, DateTimeKind.Utc),
            Origin = "Starport Thames Estuary",
            Destination = "New London",
            Sender = "Anders Hejlsberg",
            Recipient = "Elon Musk",
            Contents = "Signed C# language specification and a Christmas card"
        };

        string json = JsonSerializer.Serialize(response, _options);

        string expected = "{" +
                          "\"barcode\":\"RMARS1234567890123456789M\"," +
                          "\"status\":\"Created\"," +
                          "\"launchDate\":\"2025-09-03\"," +
                          "\"etaDays\":90," +
                          "\"estimatedArrivalDate\":\"2025-12-02\"," +
                          "\"origin\":\"Starport Thames Estuary\"," +
                          "\"destination\":\"New London\"," +
                          "\"sender\":\"Anders Hejlsberg\"," +
                          "\"recipient\":\"Elon Musk\"," +
                          "\"contents\":\"Signed C# language specification and a Christmas card\"" +
                          "}";

        json.Should().Be(expected);
    }
}

using System.Text;
using RM.Mars.ParcelTracking.Common.Extensions;
using RM.Mars.ParcelTracking.Application.Enums;
using RM.Mars.ParcelTracking.Api.Models.Requests;
using RM.Mars.ParcelTracking.Api.Models.Response;

namespace RM.Mars.ParcelTracking.Application.Services.Validation;

/// <summary>
/// Service for validating parcel creation requests and business rules.
/// </summary>
public class ParcelRequestValidation : IParcelRequestValidation
{
    /// <inheritdoc/>
    public ValidationResponse Validate(CreateParcelRequest request)
    {
        StringBuilder sb = CreateErrorMessageBuilder(request);

        ValidationResponse response = new()
        {
            ErrorMessage = sb.ToString(),
            CreateParcelRequest = request
        };

        response.IsValid = string.IsNullOrEmpty(response.ErrorMessage);
        return response;
    }

    private static StringBuilder CreateErrorMessageBuilder(CreateParcelRequest request)
    {
        var stringBuilder = new StringBuilder();

        if (string.IsNullOrEmpty(request.Barcode))
        {
            stringBuilder.AppendLine($"{nameof(request.Barcode)} is required. {Environment.NewLine}");
        }

        if (!request.Barcode.IsValidBarcode())
        {
            stringBuilder.AppendLine($"{nameof(request.Barcode)} is in an invalid format.{Environment.NewLine}");
        }

        if (request.DeliveryService != nameof(DeliveryServiceEnum.Express) &&
            request.DeliveryService != nameof(DeliveryServiceEnum.Standard))
        {
            stringBuilder.AppendLine(
                $"{nameof(request.DeliveryService)} must be either '{nameof(DeliveryServiceEnum.Express)}' or '{nameof(DeliveryServiceEnum.Standard)}'{Environment.NewLine}");
        }

        return stringBuilder;
    }

}
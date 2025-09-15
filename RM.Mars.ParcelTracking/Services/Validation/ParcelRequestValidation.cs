using RM.Mars.ParcelTracking.Models.Requests;
using RM.Mars.ParcelTracking.Models.Validation;
using System.Text;
using RM.Mars.ParcelTracking.Enums;
using RM.Mars.ParcelTracking.Extensions;

namespace RM.Mars.ParcelTracking.Services.Validation;

public class ParcelRequestValidation : IParcelRequestValidation
{
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
        StringBuilder stringBuilder = new StringBuilder();

        if (string.IsNullOrEmpty(request.Barcode))
        {
            stringBuilder.AppendLine($"{nameof(request.Barcode)} is required. {Environment.NewLine}");
        }

        if (!request.Barcode.IsValidBarcode())
        {
            stringBuilder.AppendLine($"{nameof(request.Barcode)} is in an invalid format.{Environment.NewLine}");
        }

        if (request.DeliveryService != DeliveryServiceEnum.Express.ToString() &&
            request.DeliveryService != DeliveryServiceEnum.Standard.ToString())
        {
            stringBuilder.AppendLine(
                $"{nameof(request.DeliveryService)} must be either '{DeliveryServiceEnum.Express.ToString()}' or '{DeliveryServiceEnum.Standard.ToString()}'{Environment.NewLine}");
        }

        return stringBuilder;
    }

}
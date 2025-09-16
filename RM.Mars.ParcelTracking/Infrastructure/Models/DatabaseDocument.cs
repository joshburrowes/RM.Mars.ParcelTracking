using RM.Mars.ParcelTracking.Application.Models.Parcel;

namespace RM.Mars.ParcelTracking.Infrastructure.Models;

public record DatabaseDocument
{
    public List<ParcelDto> Parcels { get; set; } = [];
}
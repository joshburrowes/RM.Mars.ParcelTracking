using Microsoft.AspNetCore.Mvc;

namespace RM.Mars.ParcelTracking.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParcelController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateParcel([FromBody] CreateParcelRequest request)
        {
            // create logic
            return CreatedAtAction(nameof(GetParcel), new { id = newParcel.Id }, newParcel);
        }

        [HttpGet("{barcode}")]
        public async Task<IActionResult> GetParcel(string barcode)
        {
            return Ok(parcel);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateParcelStatusRequest request)
        {
            return NoContent();
        }
    }
}

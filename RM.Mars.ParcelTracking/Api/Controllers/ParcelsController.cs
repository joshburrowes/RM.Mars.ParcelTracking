using Microsoft.AspNetCore.Mvc;
using RM.Mars.ParcelTracking.Application.Services.Parcels;
using RM.Mars.ParcelTracking.Application.Services.StatusValidator;
using RM.Mars.ParcelTracking.Application.Services.Validation;
using RM.Mars.ParcelTracking.Api.Models.Requests;
using RM.Mars.ParcelTracking.Api.Models.Response;
using RM.Mars.ParcelTracking.Application.Models.Parcel;
using RM.Mars.ParcelTracking.Application.Models.Validation;

namespace RM.Mars.ParcelTracking.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParcelsController(
        IParcelService parcelsService,
        ILogger<ParcelsController> logger,
        IParcelRequestValidation parcelRequestValidation,
        IStatusValidation validation)
        : ControllerBase
    {
        /// <summary>
        /// Create a new parcel.
        /// </summary>
        /// <param name="request">Parcel creation request.</param>
        /// <returns>Created parcel details.</returns>
        /// <response code="201">Parcel created successfully.</response>
        /// <response code="400">Validation failed or parcel already exists.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ParcelCreatedResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateParcel([FromBody] CreateParcelRequest request)
        {
            logger.LogInformation("CreateParcel triggered.");

            ValidationResponse validationResponse = parcelRequestValidation.Validate(request);

            if (!validationResponse.IsValid)
            {
                return BadRequest(validationResponse.ErrorMessage);
            }

            ParcelCreatedResponse? parcel = await parcelsService.ProcessParcelRequestAsync(request).ConfigureAwait(false);

            if (parcel == null)
            {
                return BadRequest("Parcel already exists");
            }

            return CreatedAtAction(nameof(CreateParcel), new { id = parcel.Barcode }, parcel);
        }

        /// <summary>
        /// Get parcel details by barcode.
        /// </summary>
        /// <param name="barcode">Parcel barcode.</param>
        /// <returns>Parcel details.</returns>
        /// <response code="200">Parcel found.</response>
        /// <response code="400">Barcode is required.</response>
        /// <response code="404">Parcel not found.</response>
        [HttpGet("{barcode}")]
        [ProducesResponseType(typeof(ParcelDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetParcel(string barcode)
        {
            if (string.IsNullOrEmpty(barcode))
            {
                return BadRequest("Barcode is required.");
            }

            logger.LogInformation("GetParcel triggered with barcode:{Barcode}.", barcode);

            ParcelDto? parcel = await parcelsService.GetParcelByBarcodeAsync(barcode).ConfigureAwait(false);

            if (parcel == null)
            {
                return NotFound($"Parcel with barcode: '{barcode}' not found.");
            }

            return Ok(parcel);
        }

        /// <summary>
        /// Update the status of a parcel.
        /// </summary>
        /// <param name="barcode">Parcel barcode.</param>
        /// <param name="request">Status update request.</param>
        /// <returns>Status update result.</returns>
        /// <response code="200">Status updated successfully.</response>
        /// <response code="400">Invalid request or status transition.</response>
        /// <response code="404">Parcel not found.</response>
        /// <response code="500">Update failed due to server error.</response>
        [HttpPatch("{barcode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateStatus(string barcode, [FromBody] UpdateParcelStatusRequest request)
        {
            if (string.IsNullOrEmpty(barcode))
            {
                return BadRequest("Barcode is required.");
            }
            
            ParcelDto? parcel = await parcelsService.GetParcelByBarcodeAsync(barcode).ConfigureAwait(false);
            if (parcel == null)
            {
                return NotFound($"Parcel with barcode: '{barcode}' not found.");
            }

            if (string.IsNullOrEmpty(request.NewStatus))
            {
                return BadRequest("newStatus must have a value.");
            }

            StatusValidationResponse statusValidation = validation.ValidateStatus(parcel, request.NewStatus);

            if (!statusValidation.Valid)
            {
                return BadRequest(statusValidation.Reason);
            }

            if (await parcelsService.UpdateParcelStatus(parcel, statusValidation.NewParcelStatus))
            {
                return Ok();
            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Update Failed: An error occurred while updating the parcel status.");
        }
    }
}

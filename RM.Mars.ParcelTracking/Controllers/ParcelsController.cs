using Microsoft.AspNetCore.Mvc;
using RM.Mars.ParcelTracking.Models.Requests;
using RM.Mars.ParcelTracking.Models.Parcel;
using RM.Mars.ParcelTracking.Models.Response;
using RM.Mars.ParcelTracking.Models.Validation;
using RM.Mars.ParcelTracking.Services.Parcels;
using RM.Mars.ParcelTracking.Services.StatusValidator;
using RM.Mars.ParcelTracking.Services.Validation;

namespace RM.Mars.ParcelTracking.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParcelsController : ControllerBase
    {
        private readonly IParcelService _parcelsService;
        private readonly ILogger<ParcelsController> _logger;
        private readonly IParcelRequestValidation _parcelRequestValidation;
        private readonly IStatusValidation _statusValidation;

        public ParcelsController(IParcelService parcelsService, ILogger<ParcelsController> logger, IParcelRequestValidation parcelRequestValidation, IStatusValidation statusValidation)
        {
            _parcelsService = parcelsService;
            _logger = logger;
            _parcelRequestValidation = parcelRequestValidation;
            _statusValidation = statusValidation;
        }

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
            _logger.LogInformation("CreateParcel triggered.");

            ValidationResponse validationResponse = _parcelRequestValidation.Validate(request);

            if (!validationResponse.IsValid)
            {
                return BadRequest(validationResponse.ErrorMessage);
            }

            ParcelCreatedResponse? parcel = await _parcelsService.ProcessParcelRequestAsync(request).ConfigureAwait(false);

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

            _logger.LogInformation("GetParcel triggered with barcode:{Barcode}.", barcode);

            ParcelDto? parcel = await _parcelsService.GetParcelByBarcodeAsync(barcode).ConfigureAwait(false);

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
            
            ParcelDto? parcel = await _parcelsService.GetParcelByBarcodeAsync(barcode).ConfigureAwait(false);
            if (parcel == null)
            {
                return NotFound($"Parcel with barcode: '{barcode}' not found.");
            }

            if (string.IsNullOrEmpty(request.NewStatus))
            {
                return BadRequest("newStatus must have a value.");
            }

            StatusValidationResponse statusValidation = _statusValidation.ValidateStatus(parcel, request.NewStatus);

            if (!statusValidation.Valid)
            {
                return BadRequest(statusValidation.Reason);
            }

            if (await _parcelsService.UpdateParcelStatus(parcel, statusValidation.NewParcelStatus))
            {
                return Ok();
            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Update Failed: An error occurred while updating the parcel status.");
        }
    }
}

using AdvertisingPlatforms.Core.Services.Abstractions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AdvertisingPlatforms.Presentation.API.Controllers;

[Route("api/v1/advertising_platforms")]
[ApiController]
public class AdvertisingPlatformsController : ControllerBase
{
    private readonly IAdvertisingPlatformsService _service;
    private readonly IValidator<string> _validator;

    public AdvertisingPlatformsController(IAdvertisingPlatformsService service, IValidator<string> validator)
    {
        _service = service;
        _validator = validator;
    }

    [Route("load_ad_platforms_from_file")]
    [HttpPost]
    public async Task<ActionResult> LoadingAdPlatformsFromFileAsync()
    {
        await _service.LoadingAdPlatformsFromFileAsync();
        return Ok();
    }

    [Route("ad_platforms/{location}")]
    [HttpGet]
    public ActionResult FindAdPlatformsByLocationAsync(string location)
    {
        // Если используется Swagger
        location = location.Replace("%2F", "/");
        
        var result = _validator.Validate(location);
        if (result.IsValid)
        {
            return Ok(_service.FindAdPlatformsByLocation(location));
        }

        var answer = result.Errors.Aggregate(string.Empty, 
            (current, validationResult) => current + (validationResult.ErrorMessage + "\n"));

        return BadRequest(answer);
    }
}
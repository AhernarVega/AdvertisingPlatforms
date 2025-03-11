using AdvertisingPlatforms.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdvertisingPlatforms.Presentation.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AdvertisingPlatformsController : ControllerBase
{
    private readonly IAdvertisingPlatformsService _service;
    
    public AdvertisingPlatformsController(IAdvertisingPlatformsService service)
    {
        _service = service;
    }

    [Route("ad_platforms_from_file")]
    [HttpGet]
    public async Task<ActionResult> LoadingAdPlatformsFromFileAsync()
    {
        await _service.LoadingAdPlatformsFromFileAsync();
        return Ok();
    }

    [Route("ad_platforms/{location}")]
    [HttpGet]
    public ActionResult FindAdPlatformsByLocationAsync(string location)
    {
        return Ok(_service.FindAdPlatformsByLocation(location)) ;
    }
}
using Microsoft.AspNetCore.Mvc;

namespace AdvertisingPlatforms.Presentation.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AdvertisingPlatformsController : ControllerBase
{
    // TODO: прописать используемые сервисы
    
    public AdvertisingPlatformsController()
    {
        // TODO: инициализировать сервисы
    }

    [Route("ad_platforms_from_file")]
    [HttpGet]
    public async Task<ActionResult> LoadingAdPlatformsFromFileAsync()
    {
        // TODO: вызов сервиса для загрузки данных о рекламных
        return Ok();
    }

    [Route("ad_platforms/{location}")]
    [HttpGet]
    public async Task<ActionResult> FindAdPlatformsByLocationAsync(string location)
    {
        // TODO: вызов сервиса для поиска рекламных площадок по локации
        return Ok();
    }
}
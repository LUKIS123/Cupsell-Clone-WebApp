using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CupsellCloneAPI.Controllers;

[ApiController]
[Authorize]
[Route("cupsellclone/graphics")]
public class GraphicController : ControllerBase
{
}
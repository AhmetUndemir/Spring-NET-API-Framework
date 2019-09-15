using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SimpleSprint.Controllers
{
    [Authorize]
    [ApiController]
    public class BaseController : ControllerBase
    {
        
    }
}
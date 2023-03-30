using Microsoft.AspNetCore.Mvc;

namespace Jobby.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
       
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IEnumerable<dynamic> Get()
        {
            return default;
        }
    }
}
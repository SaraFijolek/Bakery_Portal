using Microsoft.AspNetCore.Mvc;
using WebApplication2.Backery.Services.Services.Interfaces;
using WebApplication2.Backery.Services.Services;

namespace WebApplication2.Backery.API.Controllers
{
    [ApiController]
    [Route("api/statistics")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _service;

        public StatisticsController(IStatisticsService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var statistics = _service.GetStatistics();
            return Ok(statistics);
        }
    }

}

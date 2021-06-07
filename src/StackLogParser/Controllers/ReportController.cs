using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StackLogParser.Abstractions;
using StackLogParser.Entities;
using StackLogParser.Models;

namespace StackLogParser.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly ILogger<ReportController> _logger;
        private readonly ILogReaderService _logReaderService;
        private readonly IReportService _reportService;

        public ReportController(ILogger<ReportController> logger, ILogReaderService logReaderService, IReportService reportService)
        {
            _logger = logger;
            _logReaderService = logReaderService;
            _reportService = reportService;
        }

        /// <summary>
        /// A method that will return a report based on the requested input parameters
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IReport>> GetReport([FromQuery]ReportRequest reportRequest, CancellationToken cancellationToken)
        {
            // validate our reporting input
            bool isValid;
            var msg = reportRequest.IsValid(out isValid);
            _logger.LogError(msg);
            if (!isValid)
            {
                return new BadRequestObjectResult(msg);
            }

            return new OkObjectResult(await _reportService.GetReportAsync(reportRequest, _logReaderService.ReadAsync(reportRequest.LogFile), cancellationToken));
        }
    }
}

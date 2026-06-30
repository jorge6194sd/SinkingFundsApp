using Microsoft.AspNetCore.Mvc;
using SinkingFunds.Web.Dtos;
using SinkingFunds.Application.Services;

namespace SinkingFunds.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnvelopesController : ControllerBase
    {
        private readonly EnvelopeService _envelopeService;
        public EnvelopesController(EnvelopeService envelopeService)
        {
            _envelopeService = envelopeService;
        }
        
        // GET api/envelope/5
        [HttpGet("{id}/balance")]
        public IActionResult GetBalance(Guid id)
        {
            decimal balance = _envelopeService.GetEnvelopeBalance(id);
            return Ok(balance);
        }

        // POST /api/envelopes/
        [HttpPost]
        public IActionResult CreateEnvelope([FromBody] CreateRequest request)
        {
            var envelope = _envelopeService.CreateEnvelope(request.Name);
            return Ok(envelope.Id);
        }

        // POST /api/envelopes/{id}/withdraw
        [HttpPost("{id}/withdraw")]
        public IActionResult Withdraw(Guid id, [FromBody] WithdrawRequest request)
        {
            _envelopeService.WithdrawFromEnvelope(id, request.Description, request.Amount, DateTime.UtcNow);
            return NoContent();
        }

        // POST api/envelopes/{id}/deposit
        [HttpPost("{id}/deposit")]
        public IActionResult Deposit(Guid id, [FromBody] DepositRequest request)
        {
            _envelopeService.DepositToEnvelope(id, request.Description, request.Amount, DateTime.UtcNow);
            return NoContent();
        }

        [HttpGet]
        public IActionResult GetAllEnvelopes()
        {
            var envelopeSummaries = _envelopeService.GetAllEnvelopeSummaries();
            return Ok(envelopeSummaries);
        }
    }
}

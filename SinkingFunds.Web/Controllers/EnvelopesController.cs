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
        
        // GET: api/<EnvelopesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            throw new NotImplementedException();
        }

        // GET api/<EnvelopesController>/5
        [HttpGet("{id}")]
        public IActionResult GetBalance(Guid id)
        {
            throw new NotImplementedException();
           //_envelopeService.get
        }

        // POST api/<EnvelopesController>/5
        [HttpPost("{id}/withdraw")]
        public IActionResult Withdraw(Guid id, [FromBody] WithdrawRequest request)
        {
            _envelopeService.WithdrawFromEnvelope(id, request.Description, request.Amount, DateTime.UtcNow);
            return NoContent();
        }

        // POST api/<EnvelopesController>/5
        [HttpPost("{id}/deposit")]
        public IActionResult Deposit(Guid id, [FromBody] DepositRequest request)
        {
            _envelopeService.DepositToEnvelope(id, request.Description, request.Amount, DateTime.UtcNow);
            return NoContent();
        }

        // DELETE api/<EnvelopesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}

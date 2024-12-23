using LoanManagement.Services.Loans.Contracts.DTOs;

namespace LoanManagement.restApi.Controllers.Loans
{
    [Route("api/Loans")]
    [ApiController]
    public class LoansController(
        LoanService service,
        LoanQuery query) : ControllerBase
    {
        [HttpPost("create")]
        public async Task<int> Create([FromBody] AddLoanDto dto)
        {
            return await service.Create(dto);
        }

        [HttpGet("all_with_optional_short_term_filter")]
        public async Task<HashSet<GetLoanDto>> GetLoansWithOptionalTermFilter(
            [FromQuery] bool? isShortTerm = null)
        {
            return await query.GetLoansWithOptionalTermFilter(isShortTerm);
        }
    }
}
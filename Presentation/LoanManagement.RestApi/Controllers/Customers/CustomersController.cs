namespace LoanManagement.restApi.Controllers.Customers
{
    [Route("api/Customers")]
    [ApiController]
    public class CustomersController(
        CustomerService service,
        CustomerQuery query,
        FinancialInformationService financialInformationService,
        UpdateCustomerFinancialInformationHandler
            updateCustomerFinancialInformationHandler)
        : ControllerBase
    {
        [HttpPost("Create_customer")]
        public async Task<int> Create([FromBody] AddCustomerDto dto)
        {
            return await service.Create(dto);
        }

        [HttpPut("'update_customer")]
        public async Task Update([FromBody] UpdateCustomerDto dto)
        {
            await service.Update(dto);
        }

        [HttpPatch("verification_by/{id}")]
        public async Task Verification([FromRoute] int id)
        {
            await service.EnableVerificationById(id);
        }

        [HttpGet("with_optional_verification_filter")]
        public async Task<HashSet<GetCustomerAndFinancialInfoDto>>
            GetCustomersWithOptionalVerificationFilter(
                [FromQuery] bool? verificationFilter = null)
        {
            return await query
                .GetCustomersWithOptionalVerificationFilter(
                    verificationFilter);
        }

        [HttpPost("create_financial_info_by_customer_id")]
        public async Task<int> CreateByCustomerId([FromQuery] int customerId,
            [FromBody] AddFinancialInformationDto dto)
        {
            return await financialInformationService.CreateByCustomerId(
                customerId, dto);
        }

        [HttpPut("update_financial_information")]
        public async Task<int> Handle(
            [FromBody] UpdateFinancialInformationCommand command)
        {
            return await updateCustomerFinancialInformationHandler.Handle(
                command);
        }
    }
}
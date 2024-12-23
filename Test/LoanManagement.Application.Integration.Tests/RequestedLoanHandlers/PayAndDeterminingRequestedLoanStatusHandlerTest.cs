using LoanManagement.Applications.RequestedLoans.Handlers.
    PayAndDeterminingRequestedLoanStatus;

namespace LoanManagement.Application.Integration.Test.RequestedLoanHandlers;

public class
    PayAndDeterminingRequestedLoanStatusHandlerTest : BusinessIntegrationTest
{
    private readonly PayAndDeterminingRequestedLoanStatusHandler _sut;
    private readonly Mock<RequestedLoanService> _requestedLoanServiceMock;
    private readonly Mock<InstallmentService> _installmentServiceMock;

    public PayAndDeterminingRequestedLoanStatusHandlerTest()
    {
        _requestedLoanServiceMock = new Mock<RequestedLoanService>();
        _installmentServiceMock = new Mock<InstallmentService>();
        Mock<UnitOfWork> unitOfWorkMock = new();
        _sut = new PayAndDeterminingRequestedLoanStatusCommandHandler(
            unitOfWorkMock.Object,
            _requestedLoanServiceMock.Object,
            _installmentServiceMock.Object);
    }

    [Fact]
    public async Task
        Handle_pay_first_payable_instalment_and_determining_requested_loan_status_properly()
    {
        var requestedLoanId = 1;

        await _sut.Handle(requestedLoanId);

        _installmentServiceMock.Verify(s =>
            s.UpdatePaymentDateAndAmountByRequestId(requestedLoanId));
        _requestedLoanServiceMock.Verify(s =>
            s.UpdateReceivedLoanStatusById(requestedLoanId));
    }
}
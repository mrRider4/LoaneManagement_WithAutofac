namespace LoanManagement.Application.Integration.Test.RequestedLoanHandlers;

public class ApproveRequestAndCreateInstallmentsCommandHandlerTest
{
    private readonly ApproveRequestAndCreateInstallmentsHandler _sut;
    private readonly Mock<RequestedLoanService> _requestedLoanServiceMock;
    private readonly Mock<InstallmentService> _installmentServiceMock;

    public ApproveRequestAndCreateInstallmentsCommandHandlerTest()
    {
        Mock<UnitOfWork> unitOfWorkMock = new();
        _requestedLoanServiceMock = new Mock<RequestedLoanService>();
        _installmentServiceMock = new Mock<InstallmentService>();
        _sut = new ApproveRequestAndCreateInstallmentsCommandHandler(
            unitOfWorkMock.Object,
            _requestedLoanServiceMock.Object,
            _installmentServiceMock.Object);
    }

    [Fact]
    public async Task
        Handle_change_pending_status_to_approved_and_create_all_installments_by_requestedLoanId_and_loanId_properly()
    {
        var requestedLoanId = 1;
        

        await _sut.Handel(requestedLoanId);

        _requestedLoanServiceMock.Verify(r => r.ApproveById(requestedLoanId));
        _installmentServiceMock.Verify(i =>
            i.CreatRangeByRequestedLoanId(requestedLoanId));
    }
}
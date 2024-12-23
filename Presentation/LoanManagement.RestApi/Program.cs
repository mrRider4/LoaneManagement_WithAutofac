using LoanManagement.Applications.RequestedLoans.Handlers.ApproveRequestAndCreateInstallments;
using LoanManagement.Applications.RequestedLoans.Handlers.PayAndDeterminingRequestedLoanStatus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<EfDataContext>
(option => option.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection"
    )));

builder.Services.AddScoped<UnitOfWork, EfUnitOfWork>();
builder.Services.AddScoped<DateOnlyService, DateOnlyAppService>();

builder.Services.AddScoped<CustomerService, CustomerAppService>();
builder.Services.AddScoped<CustomerQuery, EfCustomerQuery>();
builder.Services.AddScoped<CustomerRepository, EfCustomerRepository>();

builder.Services
    .AddScoped<FinancialInformationService, FinancialInformationAppService>();
builder.Services
    .AddScoped<FinancialInformationQuery, EfFinancialInformationQuery>();
builder.Services
    .AddScoped<FinancialInformationRepository,
        EfFinancialInformationRepository>();
builder.Services
    .AddScoped<UpdateCustomerFinancialInformationHandler,
        UpdateCustomerFinancialInformationCommandHandler>();

builder.Services.AddScoped<LoanService, LoanAppService>();
builder.Services.AddScoped<LoanQuery, EfLoanQuery>();
builder.Services.AddScoped<LoanRepository, EfLoanRepository>();
builder.Services.AddScoped<LoanCalculator, LoanCalculatorTool>();

builder.Services.AddScoped<RequestedLoanService, RequestedLoanAppService>();
builder.Services
    .AddScoped<RequestedLoanRepository, EfRequestedLoanRepository>();
builder.Services.AddScoped<RequestedLoanQuery, EfRequestedLoanQuery>();
builder.Services
    .AddScoped<RequestedLoanCalculator, RequestedLoanCalculatorTool>();

builder.Services.AddScoped<InstallmentService, InstallmentAppService>();
builder.Services.AddScoped<InstallmentQuery, EfInstallmentQuery>();
builder.Services.AddScoped<InstallmentRepository, EfInstallmentRepository>();

builder.Services
    .AddScoped<ApproveRequestAndCreateInstallmentsHandler,
        ApproveRequestAndCreateInstallmentsCommandHandler>();
builder.Services
    .AddScoped<PayAndDeterminingRequestedLoanStatusHandler,
        PayAndDeterminingRequestedLoanStatusCommandHandler>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
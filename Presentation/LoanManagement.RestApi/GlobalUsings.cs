// Global using directives

global using System.Reflection;
global using Autofac;
global using Autofac.Extensions.DependencyInjection;
global using Contracts.Interfaces;
global using LoanManagement.Applications;
global using LoanManagement.Applications.Customers.Handler.UpdateCustomerFinancialInformation;
global using LoanManagement.Applications.Customers.Handler.UpdateCustomerFinancialInformation.Contracts;
global using LoanManagement.Applications.Customers.Handler.UpdateCustomerFinancialInformation.Contracts.Commands;
global using LoanManagement.Applications.RequestedLoans.Handlers.ApproveRequestAndCreateInstallments;
global using LoanManagement.Applications.RequestedLoans.Handlers.ApproveRequestAndCreateInstallments.Contracts;
global using LoanManagement.Applications.RequestedLoans.Handlers.PayAndDeterminingRequestedLoanStatus;
global using LoanManagement.Persistence.Ef;
global using LoanManagement.Persistence.Ef.Customers;
global using LoanManagement.Persistence.Ef.EfContext;
global using LoanManagement.Persistence.Ef.EfUnitOfWorks;
global using LoanManagement.Persistence.Ef.FinancialInformations;
global using LoanManagement.Persistence.Ef.Installments;
global using LoanManagement.Persistence.Ef.Loans;
global using LoanManagement.Persistence.Ef.RequestLoans;
global using LoanManagement.Services;
global using LoanManagement.Services.Customers;
global using LoanManagement.Services.Customers.Contracts;
global using LoanManagement.Services.DateOnly;
global using LoanManagement.Services.FinancialInformations;
global using LoanManagement.Services.FinancialInformations.Contracts;
global using LoanManagement.Services.Installments;
global using LoanManagement.Services.Installments.Contracts;
global using LoanManagement.Services.Loans;
global using LoanManagement.Services.Loans.Contracts;
global using LoanManagement.Services.Loans.Contracts.CalculatTools;
global using LoanManagement.Services.RequestedLoans;
global using LoanManagement.Services.RequestedLoans.CalculateTools;
global using LoanManagement.Services.RequestedLoans.Contracts;
global using LoanManagement.Services.UnitOfWorks.Contracts;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
﻿// Global using directives

global using System.Transactions;
global using LoanManagement.Entities.Customers;
global using LoanManagement.Entities.FinancialInformations;
global using LoanManagement.Entities.FinancialInformations.Enums;
global using LoanManagement.Entities.Loans;
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
global using LoanManagement.Services.Loans.Contracts;
global using LoanManagement.Services.Loans.Contracts.CalculatTools;
global using LoanManagement.Services.RequestedLoans;
global using LoanManagement.Services.RequestedLoans.CalculateTools;
global using LoanManagement.Services.RequestedLoans.Contracts;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Xunit;
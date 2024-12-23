namespace LoanManagement.Services.Customers;

public class CustomerAppService(
    UnitOfWork unitOfWork,
    CustomerRepository repository,
    FinancialInformationRepository financialInfoRepository) : CustomerService
{
    public async Task<int> Create(AddCustomerDto dto)
    {
        if (dto.NationalCode.Trim().Length != 10)
        {
            throw new InvalidNationalCodeLengthException();
        }

        if (dto.PhoneNumber.Trim().Length != 10)
        {
            throw new InvalidPhoneNumberLengthException();
        }

        if (await repository.IsExistByNationalCode(dto.NationalCode))
        {
            throw new CustomerNationalCodeDuplicateException();
        }

        var customer = new Customer()
        {
            Name = dto.Name,
            LastName = dto.LastName,
            Email = dto.Email,
            NationalCode = dto.NationalCode,
            PhoneNumber = dto.PhoneNumber,
            IsVerified = false
        };
        await repository.Add(customer);
        await unitOfWork.Save();
        return customer.Id;
    }

    public async Task Update(UpdateCustomerDto dto)
    {
        var customer = await repository.FindById(dto.Id);
        StopIfCustomerNotFound(customer);
        StopIfNothingChanges(customer!, dto);
        if (!string.IsNullOrWhiteSpace(dto.NationalCode))
        {
            if (dto.NationalCode.Length != 10)
            {
                throw new InvalidNationalCodeLengthException();
            }

            if (customer!.NationalCode != dto.NationalCode &&
                await repository.IsExistByNationalCode(dto.NationalCode))
            {
                throw new CustomerNationalCodeDuplicateException();
            }
        }

        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber) &&
            dto.PhoneNumber.Length != 10)
        {
            throw new InvalidPhoneNumberLengthException();
        }

        customer!.Name = string.IsNullOrWhiteSpace(dto.Name)
            ? customer.Name
            : dto.Name;
        customer.LastName = string.IsNullOrWhiteSpace(dto.LastName)
            ? customer.LastName
            : dto.LastName;
        customer.NationalCode = string.IsNullOrWhiteSpace(dto.NationalCode)
            ? customer.NationalCode
            : dto.NationalCode;
        customer.PhoneNumber = string.IsNullOrWhiteSpace(dto.PhoneNumber)
            ? customer.PhoneNumber
            : dto.PhoneNumber;
        customer.Email = string.IsNullOrWhiteSpace(dto.Email)
            ? customer.Email
            : dto.Email;
        customer.IsVerified = false;

        await unitOfWork.Save();
    }


    public async Task EnableVerificationById(int id)
    {
        var customer = await repository.FindById(id);
        StopIfCustomerNotFound(customer);
        if (!await financialInfoRepository
                .IsExistByCustomerIdAndIsDeletedFilter(id))
        {
            throw new FinancialInformationNotFoundException();
        }

        customer!.IsVerified = true;
        await unitOfWork.Save();
    }

    public async Task DisableVerificationById(int id)
    {
        var customer = await repository.FindById(id);
        StopIfCustomerNotFound(customer);
        customer!.IsVerified = false;
        await unitOfWork.Save();
    }

    private static void StopIfCustomerNotFound(Customer? customer)
    {
        if (customer is null)
            throw new CustomerNotFoundException();
    }

    private void StopIfNothingChanges(Customer customer, UpdateCustomerDto dto)
    {
        if ((dto.Name == null || dto.Name == customer.Name) &&
            (dto.LastName == null || dto.LastName == customer.LastName) &&
            (dto.NationalCode == null ||
             dto.NationalCode == customer.NationalCode) &&
            (dto.PhoneNumber == null ||
             dto.PhoneNumber == customer.PhoneNumber) &&
            (dto.Email == null || dto.Email == customer.Email))
        {
            throw new NothingToChangeCustomerException();
        }
    }
}
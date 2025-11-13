using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarRental.Application.DTOs;
using CarRental.Application.Exceptions;
using CarRental.Application.Interfaces.Infrastructure;
using CarRental.Application.Interfaces.Repositories;
using CarRental.Application.Interfaces.Services;
using CarRental.Domain.Entities;
using CarRental.Domain.ValueObjects;

namespace CarRental.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger _logger;

        public CustomerService(ICustomerRepository customerRepository, ILogger logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public async Task<IReadOnlyList<CustomerDto>> GetAllAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            return customers.Select(MapToDto).ToArray();
        }

        public async Task<CustomerDto> GetByIdAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer is null)
                throw new NotFoundException(nameof(Customer), id);

            return MapToDto(customer);
        }

        public async Task<CustomerDto> AddAsync(string name, string email)
        {
            ValidateCustomerInput(name, email);

            var emailVo = Email.Create(email);
            var customer = new Customer(name, emailVo);

            await _customerRepository.AddAsync(customer);

            _logger.Info($"Customer added: {customer.Name} ({customer.Email})");

            return MapToDto(customer);
        }

        public async Task UpdateAsync(int id, string name, string email)
        {
            ValidateCustomerInput(name, email);

            var existing = await _customerRepository.GetByIdAsync(id);
            if (existing is null)
                throw new NotFoundException(nameof(Customer), id);

            var emailVo = Email.Create(email);
            var updated = new Customer(id, name, emailVo);

            await _customerRepository.UpdateAsync(updated);
            _logger.Info($"Customer updated: ID={id}");
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _customerRepository.GetByIdAsync(id);
            if (existing is null)
                throw new NotFoundException(nameof(Customer), id);

            await _customerRepository.DeleteAsync(id);
            _logger.Info($"Customer deleted: ID={id}");
        }

        public async Task<IReadOnlyList<CustomerDto>> SearchByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new List<CustomerDto>();

            var customers = await _customerRepository.SearchByNameAsync(name.Trim());
            return customers.Select(MapToDto).ToArray();
        }

        private static void ValidateCustomerInput(string name, string email)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(name))
                errors.Add("Name is required.");

            if (string.IsNullOrWhiteSpace(email))
                errors.Add("Email is required.");

            if (errors.Count > 0)
                throw new ValidationException(errors);
        }

        private static CustomerDto MapToDto(Customer customer) =>
            new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email.Value
            };
    }
}

using AutoMapper;
using BuildingBlocks.Core.Domain.ValueObjects;
using ECommerce.Services.Customers.Customers.Dtos;
using ECommerce.Services.Customers.Customers.Features.CreatingCustomersReadModels;
using ECommerce.Services.Customers.Customers.Features.UpdatingCustomerReadsModel;
using ECommerce.Services.Customers.Customers.Models;
using ECommerce.Services.Customers.Customers.Models.Reads;
using ECommerce.Services.Customers.Customers.ValueObjects;

namespace ECommerce.Services.Customers.Customers;

public class CustomersMapping : Profile
{
    public CustomersMapping()
    {
        CreateMap<Customer, CustomerDto>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id.Value))
            .ForMember(x => x.Country, opt => opt.MapFrom(x => x.Address == Address.Null ? "" : x.Address!.City))
            .ForMember(x => x.City, opt => opt.MapFrom(x => x.Address == Address.Null ? "" : x.Address!.City))
            .ForMember(
                x => x.DetailAddress,
                opt => opt.MapFrom(x => x.Address == Address.Null ? "" : x.Address!.Detail))
            .ForMember(
                x => x.Nationality,
                opt => opt.MapFrom(x => x.Nationality == Nationality.Null ? null : x.Nationality!.Value))
            .ForMember(x => x.Email, opt => opt.MapFrom(x => x.Email.Value))
            .ForMember(
                x => x.BirthDate,
                opt => opt.MapFrom(x => x.BirthDate == BirthDate.Null ? null : x.BirthDate!.Value as DateTime?))
            .ForMember(
                x => x.PhoneNumber,
                opt => opt.MapFrom(x => x.PhoneNumber == PhoneNumber.Null ? "" : x.PhoneNumber!.Value));

        CreateMap<Customer, CreateCustomerReadModels>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id.Value))
            .ForMember(x => x.Country, opt => opt.MapFrom(x => x.Address == Address.Null ? "" : x.Address!.City))
            .ForMember(x => x.City, opt => opt.MapFrom(x => x.Address == Address.Null ? "" : x.Address!.City))
            .ForMember(
                x => x.DetailAddress,
                opt => opt.MapFrom(x => x.Address == Address.Null ? "" : x.Address!.Detail))
            .ForMember(
                x => x.Nationality,
                opt => opt.MapFrom(x => x.Nationality == Nationality.Null ? null : x.Nationality!.Value))
            .ForMember(x => x.Email, opt => opt.MapFrom(x => x.Email.Value))
            .ForMember(
                x => x.BirthDate,
                opt => opt.MapFrom(x => x.BirthDate == BirthDate.Null ? null : x.BirthDate!.Value as DateTime?))
            .ForMember(
                x => x.PhoneNumber,
                opt => opt.MapFrom(x => x.PhoneNumber == PhoneNumber.Null ? "" : x.PhoneNumber!.Value))
            .ForMember(x => x.FirstName, opt => opt.MapFrom(x => x.Name.FirstName))
            .ForMember(x => x.LastName, opt => opt.MapFrom(x => x.Name.LastName))
            .ForMember(x => x.FullName, opt => opt.MapFrom(x => x.Name.FullName));

        CreateMap<Customer, UpdateCustomerReadsModel>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id.Value))
            .ForMember(x => x.Country, opt => opt.MapFrom(x => x.Address == Address.Null ? "" : x.Address!.City))
            .ForMember(x => x.City, opt => opt.MapFrom(x => x.Address == Address.Null ? "" : x.Address!.City))
            .ForMember(
                x => x.DetailAddress,
                opt => opt.MapFrom(x => x.Address == Address.Null ? "" : x.Address!.Detail))
            .ForMember(
                x => x.Nationality,
                opt => opt.MapFrom(x => x.Nationality == Nationality.Null ? null : x.Nationality!.Value))
            .ForMember(x => x.Email, opt => opt.MapFrom(x => x.Email.Value))
            .ForMember(
                x => x.BirthDate,
                opt => opt.MapFrom(x => x.BirthDate == BirthDate.Null ? null : x.BirthDate!.Value as DateTime?))
            .ForMember(
                x => x.PhoneNumber,
                opt => opt.MapFrom(x => x.PhoneNumber == PhoneNumber.Null ? "" : x.PhoneNumber!.Value))
            .ForMember(x => x.FirstName, opt => opt.MapFrom(x => x.Name.FirstName))
            .ForMember(x => x.LastName, opt => opt.MapFrom(x => x.Name.LastName))
            .ForMember(x => x.FullName, opt => opt.MapFrom(x => x.Name.FullName));

        CreateMap<CreateCustomerReadModels, CustomerReadModel>();
        CreateMap<UpdateCustomerReadsModel, CustomerReadModel>();
    }
}

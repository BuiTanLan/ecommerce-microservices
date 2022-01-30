using Ardalis.GuardClauses;
using AutoMapper;
using BuildingBlocks.CQRS.Query;
using BuildingBlocks.Exception;
using ECommerce.Services.Identity.Shared.Exceptions;
using ECommerce.Services.Identity.Shared.Models;
using ECommerce.Services.Identity.Users.Dtos;
using FastEndpoints.Validation;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Services.Identity.Users.Features.GettingUerByEmail;

public record GetUserByEmail(string Email) : IQuery<GetUserByEmailResult>;

internal class GetUserByIdValidator : AbstractValidator<GetUserByEmail>
{
    public GetUserByIdValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email address is not valid");
    }
}

internal class GetUserByEmailHandler : IQueryHandler<GetUserByEmail, GetUserByEmailResult>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public GetUserByEmailHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = Guard.Against.Null(userManager, nameof(userManager));
        _mapper = Guard.Against.Null(mapper, nameof(mapper));
    }

    public async Task<GetUserByEmailResult> Handle(GetUserByEmail query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var identityUser = await _userManager.FindByIdAsync(query.Email);

        Guard.Against.NotFound(identityUser, new UserNotFoundException(query.Email));

        var userDto = _mapper.Map<IdentityUserDto>(identityUser);

        return new GetUserByEmailResult(userDto);
    }
}

public record GetUserByEmailResult(IdentityUserDto IdentityUser);

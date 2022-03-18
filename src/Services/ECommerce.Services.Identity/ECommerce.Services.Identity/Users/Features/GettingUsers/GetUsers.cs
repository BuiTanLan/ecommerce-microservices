using AutoMapper;
using ECommerce.Services.Identity.Shared.Models;
using ECommerce.Services.Identity.Users.Dtos;
using FluentValidation;
using MicroBootstrap.Abstractions.CQRS.Query;
using MicroBootstrap.Core.Persistence.EfCore;
using MicroBootstrap.Core.Types;
using MicroBootstrap.CQRS.Query;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services.Identity.Users.Features.GettingUsers;

public record GetUsers : ListQuery<GetUsersResult>;


public class GetUsersValidator : AbstractValidator<GetUsers>
{
    public GetUsersValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page should at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize should at least greater than or equal to 1.");
    }
}

public class GetUsersHandler : IQueryHandler<GetUsers, GetUsersResult>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public GetUsersHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<GetUsersResult> Handle(GetUsers request, CancellationToken cancellationToken)
    {
        var customer = await _userManager.Users
            .OrderByDescending(x => x.CreatedAt)
            .ApplyIncludeList(request.Includes)
            .ApplyFilterList(request.Filters)
            .AsNoTracking()
            .PaginateAsync<ApplicationUser, IdentityUserDto>(
                _mapper.ConfigurationProvider,
                request.Page,
                request.PageSize,
                cancellationToken: cancellationToken);

        return new GetUsersResult(customer);
    }
}

public record GetUsersResult(ListResultModel<IdentityUserDto> IdentityUsers);

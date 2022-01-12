using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Web
{
    [ApiController]
    public abstract class BaseController : Controller
    {
        protected const string BaseApiPath = Constants.BaseApiPath;

        private IMediator _mediator;
        private IMapper _mapper;

        protected IMediator Mediator =>
            _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        protected IMapper Mapper => _mapper ??= HttpContext.RequestServices.GetService<IMapper>();
    }
}

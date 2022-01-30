using System.Net;
using BuildingBlocks.Exception.Types;

namespace ECommerce.Services.Customers.Customers.Features.CreatingCustomer;

public class CreatingUserForCustomerException : AppException
{
    public CreatingUserForCustomerException()
        : base("There is an error in creating a user account for customer.", HttpStatusCode.InternalServerError)
    {
    }
}

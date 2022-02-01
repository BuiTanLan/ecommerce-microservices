using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Model;
using BuildingBlocks.Exception;
using ECommerce.Services.Catalogs.Products.Exceptions.Domain;

namespace ECommerce.Services.Catalogs.Products.Models.ValueObjects;

// https://github.com/NimblePros/ValueObjects
public class Dimensions : ValueObject
{
    public int Height { get; private set; }
    public int Width { get; private set; }
    public int Depth { get; private set; }

    public Dimensions(int width, int height, int depth)
    {
        Guard.Against.NegativeOrZero(height, new ProductDomainException("Height must be greater than zero"));
        Guard.Against.NegativeOrZero(width, new ProductDomainException("Width must be greater than zero"));
        Guard.Against.NegativeOrZero(depth, new ProductDomainException("Depth must be greater than zero"));

        Height = height;
        Width = width;
        Depth = depth;
    }

    public string FormattedDescription()
    {
        return $"HxWxD: {Height} x {Width} x {Depth}";
    }

    public override string ToString()
    {
        return FormattedDescription();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Width;
        yield return Height;
        yield return Depth;
    }
}

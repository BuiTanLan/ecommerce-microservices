namespace BuildingBlocks.Caching;

using System;

public class ExpirationOptions
{
    public ExpirationOptions(DateTimeOffset absoluteExpiration)
    {
        AbsoluteExpiration = absoluteExpiration;
    }

    public DateTimeOffset AbsoluteExpiration { get; }
}

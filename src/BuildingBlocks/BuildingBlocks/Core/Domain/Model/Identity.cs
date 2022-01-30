namespace BuildingBlocks.Core.Domain.Model;

public abstract class Identity<TId> : IEquatable<Identity<TId>>, IIdentity<TId>
{
    protected Identity(TId value) => Value = value;

    public TId Value { get; protected set; }

    public static implicit operator TId(Identity<TId> identityId)
        => identityId.Value;

    public bool Equals(Identity<TId> other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value.Equals(other.Value);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals(obj as Identity<TId>);
    }

    public override int GetHashCode()
    {
        return (GetType().GetHashCode() ^ 93) + Value.GetHashCode();
    }

    public override string ToString()
    {
        return $"{GetType().Name} [Id={Value}]";
    }

    public static bool operator ==(Identity<TId> left, Identity<TId> right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(Identity<TId> left, Identity<TId> right)
    {
        return !(left == right);
    }

    protected IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

public abstract class Identity : Identity<long>
{
    protected Identity(long value) : base(value)
    {
    }
}

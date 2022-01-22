namespace BuildingBlocks.Core.Domain.Model;

/// <inheritdoc cref="IIdentity{TId}" />
public abstract class Identity<TId> : IEquatable<Identity<TId>>, IIdentity<TId>
{
    /// <inheritdoc />
    public TId Id { get; }

    /// <inheritdoc />
    public bool Equals(Identity<TId> other)
    {
        if (ReferenceEquals(this, other))
            return true;

        return !ReferenceEquals(null, other) && Id.Equals(other.Id);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return Equals(obj as Identity<TId>);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return (GetType().GetHashCode() ^ 93) + Id.GetHashCode();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{GetType().Name} [Id={Id}]";
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
}

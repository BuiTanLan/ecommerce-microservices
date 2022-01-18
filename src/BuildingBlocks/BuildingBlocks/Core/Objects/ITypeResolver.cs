namespace BuildingBlocks.Core.Objects;

public interface ITypeResolver
{
    Type Resolve(string typeName);
    void Register(Type type);
}

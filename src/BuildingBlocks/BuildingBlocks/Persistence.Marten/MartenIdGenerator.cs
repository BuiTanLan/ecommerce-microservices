using BuildingBlocks.Core.IdsGenerator;
using Marten;
using Marten.Schema.Identity;

namespace BuildingBlocks.Persistence.Marten;

public class MartenIdGenerator : IIdGenerator<Guid>
{
    private readonly IDocumentSession _documentSession;

    public MartenIdGenerator(IDocumentSession documentSession)
    {
        _documentSession = documentSession ?? throw new ArgumentNullException(nameof(documentSession));
    }

    public Guid New() => CombGuidIdGeneration.NewGuid();
}

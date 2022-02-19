namespace BuildingBlocks.Abstractions.CQRS.Command;

public abstract record TxInternalCommand : InternalCommand, ITxInternalCommand;

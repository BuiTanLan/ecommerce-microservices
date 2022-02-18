namespace BuildingBlocks.CQRS.Command;

public abstract record TxInternalCommand : InternalCommand, ITxInternalCommand;

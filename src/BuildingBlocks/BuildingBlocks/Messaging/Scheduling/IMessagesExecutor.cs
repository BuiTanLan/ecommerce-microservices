namespace BuildingBlocks.Messaging.Scheduling;

public interface IMessagesExecutor
{
    Task ExecuteCommand(MessageSerializedObject messageSerializedObject);
}

using Com.Pinz.Client.DomainModel;
using Prism.Events;

namespace Com.Pinz.Client.Module.TaskManager.Events
{
    public class TaskDeletedEvent : PubSubEvent<Task>
    {
    }
}

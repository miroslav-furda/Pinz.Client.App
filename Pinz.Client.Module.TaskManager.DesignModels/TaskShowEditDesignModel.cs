using Com.Pinz.Client.DomainModel;
using Com.Pinz.DomainModel;

namespace Com.Pinz.Client.Module.TaskManager.DesignModels
{
    public class TaskShowEditDesignModel
    {
        public Task Task { get; private set; }
        public bool EditMode { get; private set; }

        public TaskShowEditDesignModel()
        {
            EditMode = true;
            Task = new Task()
            {
                TaskName = "Task name 1-A",
                Body = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu.",
                IsComplete = false,
                ActualWork = 50,
                Status = TaskStatus.TaskInProgress,
                StartDate = System.DateTime.Today,
                CreationTime = System.DateTime.Today,
                DueDate = System.DateTime.Today

            };
        }
    }
}

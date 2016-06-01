using Com.Pinz.Client.Module.TaskManager.Models.Category;

namespace Com.Pinz.Client.Module.TaskManager.Models.Task
{
    public class TaskModel : DomainModel.Task
    {
        private CategoryModel _category;

        public TaskModel()
        {
        }

        public TaskModel(DomainModel.Task task, CategoryModel category)
        {
            TaskId = task.TaskId;
            TaskName = task.TaskName;
            Body = task.Body;
            IsComplete = task.IsComplete;
            CreationTime = task.CreationTime;
            DateCompleted = task.DateCompleted;
            StartDate = task.StartDate;
            DueDate = task.DueDate;
            ActualWork = task.ActualWork;
            Status = task.Status;
            Priority = task.Priority;
            CategoryId = task.CategoryId;
            UserId = task.UserId;
            _category = category;
        }

        public CategoryModel Category
        {
            get { return _category; }
            set { SetProperty(ref _category, value); }
        }
    }
}
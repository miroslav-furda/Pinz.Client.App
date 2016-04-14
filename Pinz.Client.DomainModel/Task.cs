using Com.Pinz.Client.Commons.Prism;
using Com.Pinz.Client.DomainModel.Properties;
using Com.Pinz.DomainModel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Com.Pinz.Client.DomainModel
{
    public class Task : BindableValidationBase, ITask
    {
        public Guid TaskId { get; set; }

        private string _taskName;
        [Required(ErrorMessageResourceName = "Task_TaskName_Required", ErrorMessageResourceType = typeof(Resources))]
        public string TaskName
        {
            get { return _taskName; }
            set { SetProperty(ref this._taskName, value); }
        }

        private string _body;
        public string Body
        {
            get { return _body; }
            set { SetProperty(ref this._body, value); }
        }

        private bool _isComplete;
        public bool IsComplete
        {
            get { return _isComplete; }
            set { SetProperty(ref this._isComplete, value); }
        }

        private DateTime _creationTime;
        [Required(ErrorMessageResourceName = "Task_CreationTime_Required", ErrorMessageResourceType = typeof(Resources))]
        public DateTime CreationTime
        {
            get { return _creationTime; }
            set { SetProperty(ref this._creationTime, value); }
        }

        private DateTime? _dateCompleted;
        public DateTime? DateCompleted
        {
            get { return _dateCompleted; }
            set { SetProperty(ref this._dateCompleted, value); }
        }

        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get { return _startDate; }
            set { SetProperty(ref this._startDate, value); }
        }

        private DateTime? _dueDate;
        public DateTime? DueDate
        {
            get { return _dueDate; }
            set { SetProperty(ref this._dueDate, value); }
        }

        private int _actualWork;
        [Range(minimum: 0, maximum: 100)]
        public int ActualWork
        {
            get { return _actualWork; }
            set { SetProperty(ref this._actualWork, value); }
        }

        private TaskStatus _status;
        [Required(ErrorMessageResourceName = "Task_TaskStatus_Required", ErrorMessageResourceType = typeof(Resources))]
        public TaskStatus Status
        {
            get { return _status; }
            set { SetProperty(ref this._status, value); }
        }

        private TaskPriority? _priority;
        public TaskPriority? Priority
        {
            get { return _priority; }
            set { SetProperty(ref this._priority, value); }
        }

        private Guid _categoryId;
        [Required(ErrorMessageResourceName = "Task_Category_Required", ErrorMessageResourceType = typeof(Resources))]
        public Guid CategoryId
        {
            get { return _categoryId; }
            set { SetProperty(ref this._categoryId, value); }
        }

        private Guid? _userId;
        public Guid? UserId
        {
            get { return _userId; }
            set { SetProperty(ref this._userId, value); }
        }

        public override string ToString()
        {
            return string.Format("Task[TaskId:{0}, TaskName:{1}, UserId:{2}", TaskId, TaskName, UserId);
        }

    }
}

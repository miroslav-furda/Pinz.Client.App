using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.Model.Service;
using Com.Pinz.Client.Module.TaskManager.Events;
using Ninject;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    public class CategoryShowEditModel : BindableBase
    {
        public Category Category { get; set; }

        private bool _isEditorEnabled = false;
        public bool IsEditorEnabled
        {
            get { return _isEditorEnabled; }
            set
            {
                SetProperty(ref this._isEditorEnabled, value);
            }
        }

        public DelegateCommand StartEditCategory { get; private set; }
        public DelegateCommand CancelEditCategory { get; private set; }
        public DelegateCommand UpdateCategory { get; private set; }

        private ITaskClientService service;
        private IEventAggregator eventAggregator;
        private string originalCategoryName;

        [Inject]
        public CategoryShowEditModel(ITaskClientService service, IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.service = service;
            IsEditorEnabled = false;

            StartEditCategory = new DelegateCommand(OnStartEditCategory);
            CancelEditCategory = new DelegateCommand(OnCancelEditCategory);
            UpdateCategory = new DelegateCommand(OnUpdateCategory);

            CategoryEditStartedEvent categoryEditStartedEvent = eventAggregator.GetEvent<CategoryEditStartedEvent>();
            categoryEditStartedEvent.Subscribe(CategoryEditStartedEventHandler, ThreadOption.UIThread, false, c => c != Category);

            TaskEditStartedEvent taskEditStartedEvent = eventAggregator.GetEvent<TaskEditStartedEvent>();
            taskEditStartedEvent.Subscribe(CategoryEditStartedEventHandler);
        }

        private void CategoryEditStartedEventHandler(object category)
        {
            if (IsEditorEnabled)
                OnCancelEditCategory();
        }

        private void OnUpdateCategory()
        {
            service.UpdateCategory(Category);
            IsEditorEnabled = false;
        }

        private void OnCancelEditCategory()
        {
            Category.Name = originalCategoryName;
            IsEditorEnabled = false;
        }

        private void OnStartEditCategory()
        {
            eventAggregator.GetEvent<CategoryEditStartedEvent>().Publish(Category);

            originalCategoryName = Category.Name;
            IsEditorEnabled = true;
        }
    }
}

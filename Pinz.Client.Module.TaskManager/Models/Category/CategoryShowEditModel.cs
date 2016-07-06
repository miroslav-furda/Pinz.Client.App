using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Com.Pinz.Client.Module.TaskManager.Events;
using Com.Pinz.Client.Module.TaskManager.Models.Category;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Com.Pinz.DomainModel;
using Ninject;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Com.Pinz.Client.Commons.Prism;

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    public class CategoryShowEditModel : BindableBase
    {
        private bool _isEditorEnabled;
        private CategoryModel category;
        private readonly IEventAggregator eventAggregator;

        private bool isDeleteEnabled;
        private string originalCategoryName;

        private readonly ITaskRemoteService service;

        [Inject]
        public CategoryShowEditModel(ITaskRemoteService service, IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.service = service;
            IsEditorEnabled = false;

            StartEditCategory = new DelegateCommand(OnStartEditCategory);
            CancelEditCategory = new DelegateCommand(OnCancelEditCategory);
            UpdateCategory = new AwaitableDelegateCommand(OnUpdateCategory);
            DeleteCategory = new AwaitableDelegateCommand(OnDeleteCategory);

            var categoryEditStartedEvent = eventAggregator.GetEvent<CategoryEditStartedEvent>();
            categoryEditStartedEvent.Subscribe(CategoryEditStartedEventHandler, ThreadOption.UIThread, false, c => c != Category);

            var taskEditStartedEvent = eventAggregator.GetEvent<TaskEditStartedEvent>();
            taskEditStartedEvent.Subscribe(CategoryEditStartedEventHandler);
        }

        public CategoryModel Category
        {
            get { return category; }
            set
            {
                if (SetProperty(ref category, value))
                {
                    category.PropertyChanged += Category_PropertyChanged;                    
                    IsDeleteEnabled = Category.Tasks?.All(nav => nav.Status == TaskStatus.TaskComplete) ?? true;
                }
            }
        }

        public bool IsEditorEnabled
        {
            get { return _isEditorEnabled; }
            set { SetProperty(ref _isEditorEnabled, value); }
        }

        public bool IsDeleteEnabled
        {
            get { return isDeleteEnabled; }
            set { SetProperty(ref isDeleteEnabled, value); }
        }

        public DelegateCommand StartEditCategory { get; private set; }
        public DelegateCommand CancelEditCategory { get; private set; }
        public AwaitableDelegateCommand UpdateCategory { get; private set; }
        public AwaitableDelegateCommand DeleteCategory { get; private set; }

        private void Tasks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsDeleteEnabled = Category.Tasks.All(nav => nav.Status == TaskStatus.TaskComplete);
        }

        private void Category_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Tasks")
            {
                if (Category.Tasks != null)
                {
                    category.Tasks.CollectionChanged += Tasks_CollectionChanged;
                    IsDeleteEnabled = Category.Tasks.All(nav => nav.Status == TaskStatus.TaskComplete);
                }                                
            }
        }

        private void CategoryEditStartedEventHandler(object category)
        {
            if (IsEditorEnabled)
                OnCancelEditCategory();
        }

        private async System.Threading.Tasks.Task OnUpdateCategory()
        {
            await service.UpdateCategoryAsync(Category);
            IsEditorEnabled = false;
        }

        private void OnCancelEditCategory()
        {
            Category.Name = originalCategoryName;
            IsEditorEnabled = false;
        }

        private async System.Threading.Tasks.Task OnDeleteCategory()
        {
            if (IsDeleteEnabled)
            {
                IsEditorEnabled = false;
                await service.DeleteCategoryAsync(Category);
                var project = Category.Project;
                project.Categories.Remove(Category);                
            }
        }

        private void OnStartEditCategory()
        {
            eventAggregator.GetEvent<CategoryEditStartedEvent>().Publish(Category);

            originalCategoryName = Category.Name;
            IsEditorEnabled = true;
        }
    }
}
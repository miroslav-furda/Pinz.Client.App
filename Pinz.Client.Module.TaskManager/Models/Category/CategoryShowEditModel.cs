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
using Prism.Interactivity.InteractionRequest;

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    public class CategoryShowEditModel : BindableBase
    {
        private CategoryModel _category;
        public CategoryModel Category
        {
            get { return _category; }
            set
            {
                if (SetProperty(ref _category, value))
                {
                    _category.PropertyChanged += Category_PropertyChanged;
                    IsDeleteEnabled = Category.Tasks?.All(nav => nav.Status == TaskStatus.TaskComplete) ?? true;
                }
            }
        }

        private bool _isEditorEnabled;
        public bool IsEditorEnabled
        {
            get { return _isEditorEnabled; }
            set { SetProperty(ref _isEditorEnabled, value); }
        }

        private bool _isDeleteEnabled;
        public bool IsDeleteEnabled
        {
            get { return _isDeleteEnabled; }
            set { SetProperty(ref _isDeleteEnabled, value); }
        }

        public DelegateCommand StartEditCategory { get; private set; }
        public DelegateCommand CancelEditCategory { get; private set; }
        public AwaitableDelegateCommand UpdateCategory { get; private set; }
        public AwaitableDelegateCommand DeleteCategory { get; private set; }
        public InteractionRequest<IConfirmation> DeleteConfirmation { get; private set; }

        private readonly IEventAggregator _eventAggregator;
        private readonly ITaskRemoteService _service;
        private string _originalCategoryName;

        [Inject]
        public CategoryShowEditModel(ITaskRemoteService service, IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
            this._service = service;
            IsEditorEnabled = false;

            StartEditCategory = new DelegateCommand(OnStartEditCategory);
            CancelEditCategory = new DelegateCommand(OnCancelEditCategory);
            UpdateCategory = new AwaitableDelegateCommand(OnUpdateCategory);
            DeleteCategory = new AwaitableDelegateCommand(OnDeleteCategory);

            var categoryEditStartedEvent = eventAggregator.GetEvent<CategoryEditStartedEvent>();
            categoryEditStartedEvent.Subscribe(CategoryEditStartedEventHandler, ThreadOption.UIThread, false, c => c != Category);

            var taskEditStartedEvent = eventAggregator.GetEvent<TaskEditStartedEvent>();
            taskEditStartedEvent.Subscribe(CategoryEditStartedEventHandler);
            this.DeleteConfirmation = new InteractionRequest<IConfirmation>();
        }


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
                    _category.Tasks.CollectionChanged += Tasks_CollectionChanged;
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
            if (Category.ValidateModel())
            {
                await _service.UpdateCategoryAsync(Category);
                IsEditorEnabled = false;
            }
        }

        private void OnCancelEditCategory()
        {
            Category.Name = _originalCategoryName;
            IsEditorEnabled = false;
        }

        private async System.Threading.Tasks.Task OnDeleteCategory()
        {
            if (IsDeleteEnabled)
            {
                this.DeleteConfirmation.Raise(new Confirmation
                {
                    Title = Properties.Resources.DeleteConfirmation_Title,
                    Content = Properties.Resources.DeleteConfirmation_Content
                }, async (dialog) =>
                {
                    if (dialog.Confirmed)
                    {
                        IsEditorEnabled = false;
                        await _service.DeleteCategoryAsync(Category);
                        var project = Category.Project;
                        project.Categories.Remove(Category);
                    }
                });
            }
        }

        private void OnStartEditCategory()
        {
            _eventAggregator.GetEvent<CategoryEditStartedEvent>().Publish(Category);

            _originalCategoryName = Category.Name;
            IsEditorEnabled = true;
        }
    }
}
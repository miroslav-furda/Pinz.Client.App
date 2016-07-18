using System.Collections.ObjectModel;
using Com.Pinz.Client.Module.TaskManager.Models.Category;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Ninject;
using Prism.Commands;
using Prism.Mvvm;
using Com.Pinz.Client.Commons.Prism;
using Prism.Events;
using System;
using Com.Pinz.Client.Commons.Event;

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    public class CategoryListModel : BindableBase
    {
        private ProjectModel _project;
        public ProjectModel Project
        {
            get { return _project; }
            set
            {
                if (SetProperty(ref _project, value))
                    LoadCategories();
            }
        }

        private ObservableCollection<CategoryModel> _categories;
        public ObservableCollection<CategoryModel> Categories
        {
            get { return _categories; }
            set { SetProperty(ref _categories, value); }
        }

        public AwaitableDelegateCommand CreateCategory { get; private set; }

        private readonly ITaskRemoteService _taskService;
        private readonly IAdministrationRemoteService _adminService;
        private readonly IEventAggregator _eventAggregator;

        [Inject]
        public CategoryListModel(ITaskRemoteService taskService, IAdministrationRemoteService adminService, IEventAggregator eventAggregator)
        {
            this._taskService = taskService;
            this._adminService = adminService;
            this._eventAggregator = eventAggregator;
            CreateCategory = new AwaitableDelegateCommand(OnCreateCategory);
            Categories = new ObservableCollection<CategoryModel>();
        }

        private async System.Threading.Tasks.Task OnCreateCategory()
        {
            try
            {
                DomainModel.Category newCategory = await _taskService.CreateCategoryInProjectAsync(Project);
                Categories.Add(new CategoryModel(newCategory, Project));
            }
            catch (TimeoutException timeoutEx)
            {
                _eventAggregator.GetEvent<TimeoutErrorEvent>().Publish(timeoutEx);
            }
        }

        private async System.Threading.Tasks.Task LoadCategories()
        {
            Categories.Clear();
            if (Project != null)
            {
                try
                {
                    Project.Categories = Categories;
                    var categories = await _taskService.ReadAllCategoriesByProjectAsync(Project);
                    foreach (var category in categories)
                    {
                        Categories.Add(new CategoryModel(category, Project));
                    }

                    var users = await _adminService.ReadAllUsersByProjectAsync(Project);
                    Project.ProjectUsers.Clear();
                    foreach (var user in users)
                    {
                        Project.ProjectUsers.Add(user);
                    }
                }
                catch (TimeoutException timeoutEx)
                {
                    _eventAggregator.GetEvent<TimeoutErrorEvent>().Publish(timeoutEx);
                }
            }
        }
    }
}
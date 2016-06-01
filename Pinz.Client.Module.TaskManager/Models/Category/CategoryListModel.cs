using System.Collections.ObjectModel;
using Ninject;
using Prism.Mvvm;
using Com.Pinz.Client.DomainModel;
using Prism.Commands;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using System.Collections.Generic;
using Com.Pinz.Client.Module.TaskManager.Models.Category;

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    public class CategoryListModel : BindableBase
    {
        private ProjectModel project;
        public ProjectModel Project
        {
            get
            {
                return project;
            }
            set
            {
                if (SetProperty(ref this.project, value))
                    LoadCategories();
            }
        }

        private ObservableCollection<CategoryModel> _categories;
        public ObservableCollection<CategoryModel> Categories
        {
            get
            {
                return _categories;
            }
            set
            {
                SetProperty(ref this._categories, value);
            }
        }

        public DelegateCommand CreateCategory { get; private set; }

        private ITaskRemoteService taskService;
        private IAdministrationRemoteService adminService;

        [Inject]
        public CategoryListModel(ITaskRemoteService taskService, IAdministrationRemoteService adminService)
        {
            this.taskService = taskService;
            this.adminService = adminService;
            Categories = new ObservableCollection<CategoryModel>();
            CreateCategory = new DelegateCommand(OnCreateCategory);
        }

        private void OnCreateCategory()
        {
            taskService.CreateCategoryInProject(Project);
        }

        private async void LoadCategories()
        {
            Categories.Clear();
            if (Project != null)
            {
                var categories = await System.Threading.Tasks.Task.Run(() => taskService.ReadAllCategoriesByProject(Project));
                foreach (var category in categories)
                {
                    Categories.Add(new CategoryModel(category, Project));
                }                

                var users = await System.Threading.Tasks.Task.Run(() => adminService.ReadAllUsersByProject(Project));
                Project.ProjectUsers.Clear();
                foreach (var user in users)
                {
                    Project.ProjectUsers.Add(new UserModel(user));
                }
            }
        }
    }
}

using System.Collections.ObjectModel;
using Com.Pinz.Client.Module.TaskManager.Models.Category;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Ninject;
using Prism.Commands;
using Prism.Mvvm;

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    public class CategoryListModel : BindableBase
    {        
        private readonly IAdministrationRemoteService adminService;
        private ProjectModel project;

        private readonly ITaskRemoteService taskService;

        [Inject]
        public CategoryListModel(ITaskRemoteService taskService, IAdministrationRemoteService adminService)
        {
            this.taskService = taskService;
            this.adminService = adminService;            
            CreateCategory = new DelegateCommand(OnCreateCategory);
        }

        public ProjectModel Project
        {
            get { return project; }
            set
            {
                if (SetProperty(ref project, value))
                    LoadCategories();
            }
        }

        public ObservableCollection<CategoryModel> Categories => project?.Categories;

        public DelegateCommand CreateCategory { get; private set; }

        private void OnCreateCategory()
        {
            taskService.CreateCategoryInProject(Project);
        }

        private async void LoadCategories()
        {                        
            if (Project != null)
            {
                Project.Categories = new ObservableCollection<CategoryModel>();
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
using System.Collections.ObjectModel;
using Com.Pinz.Client.Module.TaskManager.Models.Category;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Ninject;
using Prism.Commands;
using Prism.Mvvm;
using Com.Pinz.Client.Commons.Prism;

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
            CreateCategory = new AwaitableDelegateCommand(OnCreateCategory);
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

        public AwaitableDelegateCommand CreateCategory { get; private set; }

        private async System.Threading.Tasks.Task OnCreateCategory()
        {
            await taskService.CreateCategoryInProjectAsync(Project);
        }

        private async System.Threading.Tasks.Task LoadCategories()
        {                        
            if (Project != null)
            {
                Project.Categories = new ObservableCollection<CategoryModel>();
                var categories = await taskService.ReadAllCategoriesByProjectAsync(Project);
                foreach (var category in categories)
                {
                    Categories.Add(new CategoryModel(category, Project));
                }

                var users = await adminService.ReadAllUsersByProjectAsync(Project);
                Project.ProjectUsers.Clear();
                foreach (var user in users)
                {
                    Project.ProjectUsers.Add(new UserModel(user));
                }
            }
        }
    }
}
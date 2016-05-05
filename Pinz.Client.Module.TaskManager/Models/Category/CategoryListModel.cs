using System.Collections.ObjectModel;
using Ninject;
using Prism.Mvvm;
using Com.Pinz.Client.DomainModel;
using Prism.Commands;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using System.Collections.Generic;

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    public class CategoryListModel : BindableBase
    {
        private Project _project;
        public Project Project
        {
            get
            {
                return _project;
            }
            set
            {
                SetProperty(ref this._project, value);
                LoadCategories();
            }
        }

        private ObservableCollection<Category> _categories;
        public ObservableCollection<Category> Categories
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

        [Inject]
        public CategoryListModel(ITaskRemoteService taskService)
        {
            this.taskService = taskService;
            Categories = new ObservableCollection<Category>();
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
                List<Category> categories = await System.Threading.Tasks.Task.Run(() => taskService.ReadAllCategoriesByProject(Project));
                categories.ForEach(Categories.Add);
            }
        }
    }
}

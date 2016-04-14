using System.Collections.ObjectModel;
using Ninject;
using System.Linq;
using Prism.Mvvm;
using Com.Pinz.Client.DomainModel;
using Prism.Commands;
using Com.Pinz.Client.Model.Service;

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
                Categories = taskService.ReadAllCategoriesByProject(value);
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

        private ITaskClientService taskService;

        [Inject]
        public CategoryListModel(ITaskClientService taskService)
        {
            this.taskService = taskService;
            CreateCategory = new DelegateCommand(OnCreateCategory);
        }

        private void OnCreateCategory()
        {
            taskService.CreateCategoryInProject(Project);
        }
    }
}

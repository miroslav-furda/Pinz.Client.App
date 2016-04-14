using Com.Pinz.Client.DomainModel;
using System;
using System.Collections.ObjectModel;

namespace Com.Pinz.Client.Module.TaskManager.DesignModels
{
    public class CategoryListDesignModel
    {
        public Project Project { get; set; }

        public ObservableCollection<Category> Categories { get; private set; }

        public CategoryListDesignModel()
        {
            Project = new Project()
            {
                ProjectId = Guid.NewGuid(),
                CompanyId = Guid.NewGuid(),
                Name = "Project1",
                Description = "Project description"
            };

            Categories = new ObservableCollection<Category>();
            Categories.Add(new Category()
            {
                CategoryId = Guid.NewGuid(),
                ProjectId = Project.ProjectId,
                Name = "Category1"
            });

            Categories.Add(new Category()
            {
                CategoryId = Guid.NewGuid(),
                ProjectId = Project.ProjectId,
                Name = "Category2"
            });

            Categories.Add(new Category()
            {
                CategoryId = Guid.NewGuid(),
                ProjectId = Project.ProjectId,
                Name = "Category3"
            });
        }
    }
}

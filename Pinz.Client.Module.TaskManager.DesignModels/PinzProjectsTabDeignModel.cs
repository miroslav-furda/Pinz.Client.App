using Com.Pinz.Client.DomainModel;
using System;
using System.Collections.ObjectModel;

namespace Com.Pinz.Client.Module.TaskManager.DesignModels
{
    public class PinzProjectsTabDeignModel
    {
        public ObservableCollection<Project> Projects { get; private set; }

        public PinzProjectsTabDeignModel()
        {
            Projects = new ObservableCollection<Project>();

            Company company = new Company()
            {
                CompanyId = Guid.NewGuid(),
                Name = "Company"
            };

            Projects.Add(new Project()
            {
                ProjectId = Guid.NewGuid(),
                CompanyId = company.CompanyId,
                Name = "Project1",
                Description = "Project description"
            });
            Projects.Add(new Project()
            {
                ProjectId = Guid.NewGuid(),
                CompanyId = company.CompanyId,
                Name = "Project2",
                Description = "Project2 description"
            });
        }
    }
}

using Com.Pinz.Client.DomainModel;
using Com.Pinz.DomainModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Com.Pinz.Client.Model.Service
{
    public interface ITaskClientService
    {
        ObservableCollection<Project> Projects { get; }

        ObservableCollection<Task> ReadAllTasksByCategory(Category category);

        ObservableCollection<Category> ReadAllCategoriesByProject(Project project);

        ObservableCollection<Project> ReadAllProjectsForCurrentUser();

        void MoveTaskToCategory(Task task, Category category);

        void ChangeTaskStatus(Task task, TaskStatus newStatus);

        Task CreateTaskInCategory(Category category);

        void UpdateTask(Task task);

        void DeleteTask(Task task);

        Category CreateCategoryInProject(Project project);

        void UpdateCategory(Category category);

        void DeleteCategory(Category category);
    }
}

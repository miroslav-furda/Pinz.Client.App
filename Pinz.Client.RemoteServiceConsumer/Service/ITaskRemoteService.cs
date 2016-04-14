using Com.Pinz.Client.DomainModel;
using Com.Pinz.DomainModel;
using System.Collections.Generic;

namespace Com.Pinz.Client.RemoteServiceConsumer.Service
{
    public interface ITaskRemoteService
    {
        List<Task> ReadAllTasksByCategory(Category category);

        List<Category> ReadAllCategoriesByProject(Project project);

        List<Project> ReadAllProjectsForCurrentUser();

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

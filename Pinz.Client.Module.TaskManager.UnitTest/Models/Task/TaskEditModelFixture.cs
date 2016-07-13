using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.Pinz.Client.Module.TaskManager.Events;
using Prism.Events;
using AutoMapper;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using System;
using System.Collections.ObjectModel;

namespace Com.Pinz.Client.Module.TaskManager.Models.Task
{
    [TestClass]
    public class TaskEditModelFixture
    {
        private TaskEditModel model;
        private Mock<ITaskRemoteService> taskService;
        private Mock<TaskEditStartedEvent> taskEditStartEvent;
        private TaskModel _task;

        [TestInitialize]
        public void SetUpFixture()
        {
            taskService = new Mock<ITaskRemoteService>();
            var eventAgregator = new Mock<IEventAggregator>();

            taskEditStartEvent = new Mock<TaskEditStartedEvent>();
            var categoryEditStartedEvent = new Mock<CategoryEditStartedEvent>();
            eventAgregator.Setup(x => x.GetEvent<TaskEditStartedEvent>()).Returns(taskEditStartEvent.Object);
            eventAgregator.Setup(x => x.GetEvent<CategoryEditStartedEvent>()).Returns(categoryEditStartedEvent.Object);

            var mapper = new Mock<IMapper>();

            model = new TaskEditModel(taskService.Object, eventAgregator.Object, mapper.Object);

            _task = new TaskModel
            {
                TaskId = Guid.NewGuid(),
                TaskName = "Test",
                CreationTime = DateTime.Today,
                Status = Pinz.DomainModel.TaskStatus.TaskNotStarted,
                CategoryId = Guid.NewGuid(),
                Category = new Category.CategoryModel()
                {
                    Project = new ProjectModel()
                    {
                        ProjectUsers = new ObservableCollection<UserModel>()
                    }
                }
            };
        }

        [TestMethod]
        public void InitializationSetsValues()
        {
            bool changedPropertyNameTask = false;
            model.PropertyChanged += (o, e) =>
            {
                if ("Task" == e.PropertyName)
                    changedPropertyNameTask = true;
            };

            model.Task = _task;
            Assert.IsTrue(changedPropertyNameTask);
            Assert.IsFalse(model.EditMode);
        }

        [TestMethod]
        public void OkEditCommand_Executes_Update()
        {
            model.Task = _task;
            model.EditMode = true;

            model.OkCommand.ExecuteAsync(this);

            taskService.Verify(m => m.UpdateTaskAsync(It.IsAny<DomainModel.Task>()));
            Assert.IsFalse(model.EditMode);
        }

        [TestMethod]
        public void OkEditCommand_Fail_Validation()
        {
            _task.TaskName = "";
            model.Task = _task;
            model.EditMode = true;

            model.OkCommand.ExecuteAsync(this);

            taskService.Verify(m => m.UpdateTaskAsync(It.IsAny<DomainModel.Task>()), Times.Never);
            Assert.IsTrue(model.EditMode);
        }

        [TestMethod]
        public void CancelEditCommand_No_Update()
        {
            model.CancelCommand.Execute();

            taskService.Verify(m => m.UpdateTaskAsync(It.IsAny<DomainModel.Task>()), Times.Never);
            Assert.IsFalse(model.EditMode);
        }
    }
}

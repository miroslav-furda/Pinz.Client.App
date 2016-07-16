using Com.Pinz.Client.Module.TaskManager.Events;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Com.Pinz.DomainModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using System;

namespace Com.Pinz.Client.Module.TaskManager.Models.Task
{
    [TestClass]
    public class TaskShowEditModelFixture
    {
        private TaskShowEditModel model;
        private Mock<ITaskRemoteService> taskService;
        private Mock<TaskEditFinishedEvent> taskEditFinishedEvent;

        [TestInitialize]
        public void SetUpFixture()
        {
            taskService = new Mock<ITaskRemoteService>();
            var eventAgregator = new Mock<IEventAggregator>();

            taskEditFinishedEvent = new Mock<TaskEditFinishedEvent>();
            taskEditFinishedEvent.Setup(
                x => x.Subscribe(It.IsAny<Action<object>>(), It.IsAny<ThreadOption>(), It.IsAny<bool>(), It.IsAny<Predicate<object>>()))
                .Returns(It.IsAny<SubscriptionToken>);

            eventAgregator.Setup(x => x.GetEvent<TaskEditFinishedEvent>()).Returns(taskEditFinishedEvent.Object);
            model = new TaskShowEditModel(taskService.Object, eventAgregator.Object);
        }

        [TestMethod]
        public void InitializationSetsValues()
        {
            string changedPropertyName = null;
            model.PropertyChanged += (o, e) =>
            {
                changedPropertyName = e.PropertyName;
            };

            model.Task = new DomainModel.Task { TaskName = "Test" };
            Assert.AreEqual("Task", changedPropertyName);
            Assert.IsFalse(model.EditMode);
        }

        [TestMethod]
        public void OnEditChangesEditFlag()
        {
            model.EditCommand.Execute();

            Assert.IsTrue(model.EditMode);
        }

        [TestMethod]
        public void OnStartCallsService()
        {
            model.Task = new DomainModel.Task { TaskName = "Test" };

            model.StartCommand.ExecuteAsync(this);

            taskService.Verify(m => m.ChangeTaskStatusAsync(model.Task, TaskStatus.TaskInProgress));
        }

        [TestMethod]
        public void OnComplete_True_Calls_CompleteTask()
        {
            model.Task = new DomainModel.Task { TaskName = "Test" };

            model.CompleteCommand.Execute(true);

            taskService.Verify(m => m.ChangeTaskStatusAsync(model.Task, TaskStatus.TaskComplete));
        }

        [TestMethod]
        public void OnComplete_False_Calls_ReopenTask()
        {
            model.Task = new DomainModel.Task { TaskName = "Test" };

            model.CompleteCommand.Execute(false);

            taskService.Verify(m => m.ChangeTaskStatusAsync(model.Task, TaskStatus.TaskNotStarted));
        }

        [TestMethod]
        public void OnComplete_Null_Calls_Nothing()
        {
            model.CompleteCommand.Execute(null);

            taskService.Verify(m => m.ChangeTaskStatusAsync(It.IsAny<DomainModel.Task>(), It.IsAny<TaskStatus>()), Times.Never);
        }
    }
}

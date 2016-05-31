using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.Pinz.Client.Module.TaskManager.Events;
using Prism.Events;
using AutoMapper;
using Com.Pinz.Client.RemoteServiceConsumer.Service;

namespace Com.Pinz.Client.Module.TaskManager.Models.Task
{
    [TestClass]
    public class TaskEditModelFixture
    {
        private TaskEditModel model;
        private Mock<ITaskRemoteService> taskService;
        private Mock<TaskEditStartedEvent> taskEditStartEvent;

        [TestInitialize]
        public void SetUpFixture()
        {
            taskService = new Mock<ITaskRemoteService>();
            var eventAgregator = new Mock<IEventAggregator>();

            taskEditStartEvent = new Mock<TaskEditStartedEvent>();
            /*taskEditStartEvent.Setup(
                x => x.Subscribe(It.IsAny<Action<object>>(), It.IsAny<ThreadOption>(), It.IsAny<bool>(), It.IsAny<Predicate<object>>()))
                .Returns(It.IsAny<SubscriptionToken>);
                */
            var categoryEditStartedEvent = new Mock<CategoryEditStartedEvent>();
            eventAgregator.Setup(x => x.GetEvent<TaskEditStartedEvent>()).Returns(taskEditStartEvent.Object);
            eventAgregator.Setup(x => x.GetEvent<CategoryEditStartedEvent>()).Returns(categoryEditStartedEvent.Object);

            var mapper = new Mock<IMapper>();

            model = new TaskEditModel(taskService.Object, eventAgregator.Object, mapper.Object);
        }

        [TestMethod]
        public void InitializationSetsValues()
        {
            string changedPropertyName = null;
            model.PropertyChanged += (o, e) =>
            {
                changedPropertyName = e.PropertyName;
            };

            model.Task = new TaskModel{ TaskName = "Test" };
            Assert.AreEqual("Task", changedPropertyName);
            Assert.IsFalse(model.EditMode);
        }

        [TestMethod]
        public void OkEditCommand_Executes_Update()
        {
            model.OkCommand.Execute();

            taskService.Verify(m => m.UpdateTask(It.IsAny<DomainModel.Task>()));
            Assert.IsFalse(model.EditMode);
        }

        [TestMethod]
        public void CancelEditCommand_No_Update()
        {
            model.CancelCommand.Execute();

            taskService.Verify(m => m.UpdateTask(It.IsAny<DomainModel.Task>()), Times.Never);
            Assert.IsFalse(model.EditMode);
        }
    }
}

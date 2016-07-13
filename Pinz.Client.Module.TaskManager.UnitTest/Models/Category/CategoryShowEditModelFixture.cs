using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Events;
using Com.Pinz.Client.Module.TaskManager.Events;
using Com.Pinz.Client.RemoteServiceConsumer.Service;

namespace Com.Pinz.Client.Module.TaskManager.Models.Category
{
    [TestClass]
    public class CategoryShowEditModelFixture
    {
        private CategoryShowEditModel _model;
        private Mock<ITaskRemoteService> _taskService;

        [TestInitialize]
        public void SetUpFixture()
        {
            _taskService = new Mock<ITaskRemoteService>();

            var eventAgregator = new Mock<IEventAggregator>();
            var categoryEditStartedEvent = new Mock<CategoryEditStartedEvent>();
            eventAgregator.Setup(x => x.GetEvent<CategoryEditStartedEvent>()).Returns(categoryEditStartedEvent.Object);
            var taskEditStartEvent = new Mock<TaskEditStartedEvent>();
            eventAgregator.Setup(x => x.GetEvent<TaskEditStartedEvent>()).Returns(taskEditStartEvent.Object);

            _model = new CategoryShowEditModel(_taskService.Object, eventAgregator.Object);
            _model.Category = new CategoryModel{ Name = "Category1" };
        }

        [TestMethod]
        public void InitializationSetsValues()
        {
            Assert.IsFalse(_model.IsEditorEnabled);
            Assert.IsNotNull(_model.Category);
        }

        [TestMethod]
        public void ValidateCategoryOnUpdate()
        {
            _model.StartEditCategory.Execute();
            _model.Category.Name = "";

            _model.UpdateCategory.ExecuteAsync(this);

            Assert.IsTrue(_model.IsEditorEnabled);
            _taskService.Verify(m => m.UpdateCategoryAsync(_model.Category),Times.Never);
        }

        [TestMethod]
        public void UpdateOnServiceCalledOnEditOK()
        {
            _model.StartEditCategory.Execute();
            _model.Category.Name = "New category name";

            _model.UpdateCategory.ExecuteAsync(this);
            _taskService.Verify(m => m.UpdateCategoryAsync(_model.Category));
        }

        [TestMethod]
        public void RessetCategoryNameOnCancel()
        {
            _model.StartEditCategory.Execute();
            _model.Category.Name = "New category name";

            _model.CancelEditCategory.Execute();

            Assert.AreSame("Category1", _model.Category.Name);
        }

        [TestMethod]
        public void PropertyChangeTriggeredOnIsEditorEnabled()
        {
            string changedPropertyName = string.Empty;
            _model.PropertyChanged += (o, e) =>
            {
                changedPropertyName = e.PropertyName;
            };
            _model.StartEditCategory.Execute();
            Assert.AreEqual("IsEditorEnabled", changedPropertyName);
            Assert.IsTrue(_model.IsEditorEnabled);
        }
    }
}

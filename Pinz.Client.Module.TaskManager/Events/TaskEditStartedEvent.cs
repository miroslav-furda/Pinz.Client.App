﻿using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.Module.TaskManager.Models;
using Com.Pinz.Client.Module.TaskManager.Models.Task;
using Prism.Events;

namespace Com.Pinz.Client.Module.TaskManager.Events
{
    public class TaskEditStartedEvent : PubSubEvent<TaskModel>
    {
    }
}

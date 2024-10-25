using Microsoft.Extensions.Logging;

namespace ARWNI2S.Engine.Infrastructure
{

    internal class FrameTaskScheduler : TaskScheduler
    {
        private struct Statistics
        {
            private ulong _totalTaskCount;

            ulong TotalTaskCount { get { return _totalTaskCount; } }


        }

        private readonly List<Task> _nextFrameTasks = [];
        private readonly Queue<Task> _pendingTasks = [];
        private readonly List<Task> _frameTasks = [];
        private readonly List<Task> _runningTasks = [];
        private readonly ILogger<FrameTaskScheduler> _logger;
        private readonly int MAX_PARALLEL = 8;

        public override int MaximumConcurrencyLevel => base.MaximumConcurrencyLevel;

        public FrameTaskScheduler(ILogger<FrameTaskScheduler> logger)
        {
            _logger = logger;

            _frameTasks.Clear();
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return _frameTasks;
        }

        protected override void QueueTask(Task task)
        {
            if (task == null || task.IsCompleted)
                return;

            _logger.LogDebug("Enqueue Task: {task}", task.Id);

            lock (_nextFrameTasks)
            {
                _nextFrameTasks.Add(task);
            }

            // task.CreationOptions;
            // task.Exception;
            // task.Id;
            // task.IsCanceled;
            // task.IsCompleted;
            // task.IsCompletedSuccessfully;
            // task.IsFaulted;
            // task.Status;
        }

        //private void ProcessTask(Task task)
        //{
        //    TryExecuteTask(task);
        //}

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            _logger.LogDebug("ExecuteTaskInline: {task}", task.Id);
            return TryExecuteTask(task);
        }

        // Método para ejecutar tareas por lotes
        public void RunFrameTasks()
        {
            if (_pendingTasks.Count > 0 && _runningTasks.Count < MAX_PARALLEL)
            {
                lock (_runningTasks)
                {
                    lock (_pendingTasks)
                    {
                        for (int i = _runningTasks.Count() - 1; i < MAX_PARALLEL; i++)
                        {
                            var next = _pendingTasks.Dequeue();
                            _runningTasks.Add(next);
                            // Ejecutar todas las tareas
                            Parallel.ForEach(_runningTasks, task =>
                            {
                                TryExecuteTask(task);
                            });
                        }
                    }
                }
            }

            lock (_frameTasks)
            {
                var tasksToExecute = _frameTasks.ToArray();
                _frameTasks.Clear();

                // Ejecutar todas las tareas
                Parallel.ForEach(tasksToExecute, task =>
                {
                    _logger.LogDebug("TryExecuteTask: {task}", task.Id);
                    TryExecuteTask(task);
                });
            }

        }

        public void PrepareFrameTasks()
        {
            lock (_runningTasks)
            {
                lock (_pendingTasks)
                {
                    var running = _runningTasks.Where(t => !t.IsCompleted || !t.IsFaulted || !t.IsCanceled);
                    _runningTasks.Clear();
                    _runningTasks.AddRange(running);
                }
            }
            lock (_nextFrameTasks)
            {
                // Ejecutar todas las tareas
                Parallel.ForEach(_nextFrameTasks, task =>
                {
                    if (task.IsCompleted || task.IsFaulted || task.IsCanceled)
                        return;
                    if (task.CreationOptions == TaskCreationOptions.LongRunning)
                    {
                        lock (_pendingTasks)
                        {
                            _pendingTasks.Enqueue(task);
                        }
                    }
                    else
                    {
                        lock (_frameTasks)
                        {
                            _frameTasks.Add(task);
                        }
                    }
                });
            }
        }
    }
}

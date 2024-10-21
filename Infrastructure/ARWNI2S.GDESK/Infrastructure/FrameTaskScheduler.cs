namespace ARWNI2S.Engine.NewFolder
{
    internal class FrameTaskScheduler : TaskScheduler
    {
        private readonly List<Task> _taskQueue = [];

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return _taskQueue;
        }

        // Sobrescribir para inyectar lógica personalizada para la simulación
        protected override void QueueTask(Task task)
        {
            lock (_taskQueue)
            {
                _taskQueue.Add(task);
            }

            ThreadPool.QueueUserWorkItem(_ => ProcessTask(task));
        }

        private void ProcessTask(Task task)
        {
            TryExecuteTask(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return TryExecuteTask(task);
        }

        // Método para ejecutar tareas por lotes
        public void RunScheduledTasks()
        {
            lock (_taskQueue)
            {
                var tasksToExecute = _taskQueue.ToArray();
                _taskQueue.Clear();

                // Ejecutar todas las tareas
                Parallel.ForEach(tasksToExecute, task =>
                {
                    TryExecuteTask(task);
                });
            }
        }
    }
}

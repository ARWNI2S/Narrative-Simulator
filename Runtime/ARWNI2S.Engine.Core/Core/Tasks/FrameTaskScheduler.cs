
namespace ARWNI2S.Engine.Core.Tasks
{
    internal class FrameTaskScheduler : TaskScheduler
    {
        public override int MaximumConcurrencyLevel => base.MaximumConcurrencyLevel;

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return default;
        }

        protected override void QueueTask(Task task)
        {

        }

        protected override bool TryDequeue(Task task)
        {
            return base.TryDequeue(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }
    }
}

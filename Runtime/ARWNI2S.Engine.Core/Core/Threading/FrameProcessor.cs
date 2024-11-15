namespace ARWNI2S.Engine.Core.Threading
{
    internal class FrameProcessor
    {










        private readonly FrameSynchronizationContext _syncContext = new();

        public void Run()
        {
            SynchronizationContext.SetSynchronizationContext(_syncContext);

            while (true)
            {
                // Ejecutar el ciclo de actualización
                UpdateCycle();

                // Ejecutar todas las tareas encoladas en el FrameSynchronizationContext
                _syncContext.ExecuteFrame();
            }
        }

        private void UpdateCycle()
        {
            // Lógica de actualización del ciclo
            Console.WriteLine("Ejecutando ciclo de actualización.");
        }

        private async Task HandleAsyncOperation(Func<Task> asyncOperation)
        {
            var tcs = new TaskCompletionSource<bool>();

            // Lanzar la operación asíncrona y manejar el callback
            _ = Task.Run(async () =>
            {
                await asyncOperation();
                tcs.SetResult(true);
            });

            // Insertar el resultado de vuelta en el UpdateRing
            await tcs.Task;
            AddEventToGameThread(tcs.Task);
        }

        private void AddEventToGameThread(Task<bool> task)
        {

        }
    }
}

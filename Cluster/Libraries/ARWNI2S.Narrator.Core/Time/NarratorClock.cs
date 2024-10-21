using ARWNI2S.Runtime.Simulation.Time;

namespace ARWNI2S.Node.Core.Time
{
    public class NarratorClock : Grain, INI2SClock
    {
        private TimeSpan _currentTime;
        private bool _isSynchronized;

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _currentTime = TimeSpan.Zero; // Inicializa el reloj
            _isSynchronized = false;
            return base.OnActivateAsync(cancellationToken);
        }

        // Obtener el tiempo actual del reloj de forma asíncrona
        public Task<TimeSpan> GetCurrentTimeAsync()
        {
            return Task.FromResult(_currentTime);
        }

        // Propuesta de tiempo externo para sincronización (asincrónica)
        public Task ProposeTimeAsync(TimeSpan proposedTime)
        {
            // Lógica de consenso: ajustar la sincronización basada en la mayoría
            // Por simplicidad, aquí simplemente actualizamos el tiempo
            _currentTime = proposedTime;
            _isSynchronized = true;
            return Task.CompletedTask;
        }

        // Sincronizar el tiempo localmente
        public void Synchronize(TimeSpan externalTime)
        {
            _currentTime = externalTime;
        }

        // Resetear el reloj
        public void Reset(TimeSpan startTime)
        {
            _currentTime = startTime;
        }

        // Iniciar el reloj (ej. empezaría a incrementar en tiempo real)
        public void Start()
        {
            // Iniciar el reloj, que normalmente estaría gestionado por un ciclo de tiempo
        }

        // Pausar el reloj
        public void Pause()
        {
            // Pausar el ciclo del reloj
        }

        // Consultar si está sincronizado
        public Task<bool> IsSynchronizedAsync()
        {
            return Task.FromResult(_isSynchronized);
        }
    }

}

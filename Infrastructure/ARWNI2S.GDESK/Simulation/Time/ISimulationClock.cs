namespace ARWNI2S.Engine.Simulation.Time
{
    public interface ISimulationClock
    {
        // Obtiene el tiempo actual del reloj en ticks o en una estructura TimeSpan.
        Task<TimeSpan> GetCurrentTimeAsync();

        // Sincroniza el reloj con un valor externo (del consenso de otros relojes).
        void Synchronize(TimeSpan time);

        // Resetea o ajusta el reloj a un nuevo tiempo base.
        void Reset(TimeSpan startTime);

        // Inicia el reloj, permitiendo que el tiempo fluya.
        void Start();

        // Pausa el reloj.
        void Pause();
    }
}

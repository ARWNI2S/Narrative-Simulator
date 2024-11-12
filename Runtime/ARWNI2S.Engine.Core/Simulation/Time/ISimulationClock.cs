namespace ARWNI2S.Engine.Simulation.Time
{
    public interface ISimulationClock
    {
        /// <summary>
        /// Obtiene la resolucion del reloj de simulacion.
        /// </summary>
        /// <returns>Milisegundos por unidad de tiempo.</returns>
        internal double GetResolution();

        /// <summary>
        /// Obtiene el tiempo actual del reloj de la simulacion.
        /// </summary>
        /// <returns>Tiempo de la simulacion en milisegundos.</returns>
        internal ulong GetTimeUnits();

        /// <summary>
        /// Obtiene el tiempo actual del reloj de la simulacion.
        /// </summary>
        /// <returns>Tarea asincrona que contiene el Tiempo Transcurrido.</returns>
        Task<TimeSpan> GetTimeAsync();

        /// <summary>
        /// Inicia el reloj, permitiendo que el tiempo fluya.
        /// </summary>
        void Start();

        /// <summary>
        /// Avanza el reloj de simulacion.
        /// </summary>
        /// <param name="time">Unidades de tiempo a avanzar.</param>
        void Advance(double time);

        /// <summary>
        /// Pausa el reloj.
        /// </summary>
        void Pause();
    }
}

using ARWNI2S.Infrastructure.Memory;

namespace ARWNI2S.Engine.Simulation.Kernel.Events
{
    internal interface IEventPool : IPool<SimulationEvent>, IDisposable
    {
        /// <summary>
        /// Devuelve el numero de objetos en la reserva de memoria.
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Devuelve el tamaño maximo de la reserva de memoria.
        /// </summary>
        int MaxSize { get; }
        /// <summary>
        /// Devuelve el numero de objetos en memoria.
        /// </summary>
        int TotalObjects { get; }

        /// <summary>
        /// Rellena la reserva de memoria hasta alcanzar el tamaño maximo de la reserva.
        /// </summary>
        void Fill();

        /// <summary>
        /// Rellena la reserva de memoria con un numero de objetos determinado por el parametro <paramref name="nbrNewObjects"/>, hasta alcanzar el tamaño maximo de la reserva.
        /// </summary>
        /// <param name="nbrNewObjects">Indica el numero de objetos a añadir a la reserva de memoria.</param>
        void Fill(int nbrNewObjects);
    }
}

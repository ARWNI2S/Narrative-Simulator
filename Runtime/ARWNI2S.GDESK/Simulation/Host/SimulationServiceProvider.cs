namespace ARWNI2S.Engine.Simulation.Host
{
    public static class SimulationServiceProvider
    {
        public static IServiceProvider Instance { get; private set; }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            if (Instance != null)
                throw new InvalidOperationException("El SimulationServiceProvider ya ha sido inicializado.");

            Instance = serviceProvider;
        }
    }
}

namespace ARWNI2S.Engine.Simulation.Entities.Scene
{
    /** Type of tick we wish to perform on the world */
    enum SceneTickType
    {
        /** Update the world time only. */
        TimeOnly = 0,
        /** Update time and models. */
        ModelsOnly = 1,
        /** Update all. */
        All = 2,
        /** Delta time is zero, we are paused. Components don't tick. */
        Pause = 3,
    };
}

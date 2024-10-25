using ARWNI2S.Infrastructure;

namespace ARWNI2S.Engine.Simulation.LOD
{
    public struct LODDescriptor
    {
        /// <summary>
        /// Level Of Detail 
        /// </summary>
        public int LOD { get; private set; }

        public int DesiredFramerate { get; private set; }

        public double Resolution { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lod"></param>
        /// <param name="desiredFps"></param>
        public LODDescriptor(int lod, int desiredFps)
        {
            LOD = lod;

            if (desiredFps < Constants.MINIMUM_DESIRED_FRAMERATE)
                desiredFps = Constants.MINIMUM_DESIRED_FRAMERATE;

            DesiredFramerate = desiredFps;
            Resolution = 1000 / desiredFps;
        }
    }

    public class SimulationLOD
    {
        public static readonly LODDescriptor LOD_0 = new(0, Constants.MINIMUM_DESIRED_FRAMERATE);
        public static readonly LODDescriptor LOD_1 = new(1, 20);
        public static readonly LODDescriptor LOD_2 = new(2, 25);
        public static readonly LODDescriptor LOD_3 = new(3, 30);
        public static readonly LODDescriptor LOD_4 = new(4, 35);
        public static readonly LODDescriptor LOD_5 = new(5, 40);
        public static readonly LODDescriptor LOD_6 = new(6, 45);
        public static readonly LODDescriptor LOD_7 = new(7, 50);
        public static readonly LODDescriptor LOD_8 = new(8, 55);
        public static readonly LODDescriptor LOD_9 = new(9, 60);
        public static readonly LODDescriptor LOD_MAX = new(int.MaxValue, int.MaxValue);
    }
}

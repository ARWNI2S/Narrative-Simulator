namespace ARWNI2S.Framework.World.Coordinates
{
    public static class CoordinateConverter
    {

        //public static (int X, int Y) ConvertSubsectorHexToSectorHex(string subsectorCode, string subsectorHex)
        //{
        //    // Obtener la posición del subsector dentro del sector (usando letras A-P en subsectorCode)
        //    int subsectorPositionX = (subsectorCode[0] - 'A') % 4;
        //    int subsectorPositionY = (subsectorCode[0] - 'A') / 4;

        //    // Obtener las coordenadas X, Y dentro del subsector
        //    int hexX = int.Parse(subsectorHex.Substring(0, 2));
        //    int hexY = int.Parse(subsectorHex.Substring(2, 2));

        //    // Calcular la posición en el sector completo
        //    int sectorHexX = subsectorPositionX * SubsectorSize + hexX;
        //    int sectorHexY = subsectorPositionY * SubsectorSize + hexY;

        //    return new(sectorHexX, sectorHexY);
        //}

        public static (int X, int Y) ConvertSubsectorHexToSectorHex(string subsectorCode, (int X, int Y) subsectorHex)
        {
            const int SubsectorSizeX = 8; // Tamaño de cada subsector (8x10 hexes)
            const int SubsectorSizeY = 10; // Tamaño de cada subsector (8x10 hexes)

            // Obtener la posición del subsector dentro del sector (usando letras A-P en subsectorCode)
            int subsectorPositionX = (subsectorCode[0] - 'A') % 4;
            int subsectorPositionY = (subsectorCode[0] - 'A') / 4;

            // Calcular la posición en el sector completo
            int sectorHexX = subsectorPositionX * SubsectorSizeX + subsectorHex.X;
            int sectorHexY = subsectorPositionY * SubsectorSizeY + subsectorHex.Y;

            return new(sectorHexX, sectorHexY);
        }

    }
}

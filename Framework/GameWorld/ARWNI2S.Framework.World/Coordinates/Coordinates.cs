namespace ARWNI2S.Framework.World.Coordinates
{
    //// Clase base genérica para cualquier sistema de coordenadas
    //public abstract class Coord<T>
    //{
    //    public T[] Valores { get; protected set; }

    //    // Constructor para definir las coordenadas
    //    public Coord(params T[] valores)
    //    {
    //        Valores = valores;
    //    }
    //}

    //// Ejemplo de una clase que extiende el sistema de coordenadas para un espacio 2D cartesiano
    //public class CoordenadaCartesiana2D : Coord<double>
    //{
    //    public CoordenadaCartesiana2D(double x, double y) : base(x, y) { }

    //    // Implementación del cálculo de distancia para un sistema cartesiano 2D
    //    public override double CalcularDistancia(Coord<double> otroPunto)
    //    {
    //        if (otroPunto.Valores.Length != 2)
    //            throw new ArgumentException("El otro punto debe tener dos coordenadas para un sistema 2D.");

    //        double dx = this.Valores[0] - otroPunto.Valores[0];
    //        double dy = this.Valores[1] - otroPunto.Valores[1];

    //        return Math.Sqrt(dx * dx + dy * dy);
    //    }
    //}

    //// Ejemplo de una clase que extiende el sistema de coordenadas para un espacio 3D cartesiano
    //public class CoordenadaCartesiana3D : Coord<double>
    //{
    //    public CoordenadaCartesiana3D(double x, double y, double z) : base(x, y, z) { }

    //    // Implementación del cálculo de distancia para un sistema cartesiano 3D
    //    public override double CalcularDistancia(Coord<double> otroPunto)
    //    {
    //        if (otroPunto.Valores.Length != 3)
    //            throw new ArgumentException("El otro punto debe tener tres coordenadas para un sistema 3D.");

    //        double dx = this.Valores[0] - otroPunto.Valores[0];
    //        double dy = this.Valores[1] - otroPunto.Valores[1];
    //        double dz = this.Valores[2] - otroPunto.Valores[2];

    //        return Math.Sqrt(dx * dx + dy * dy + dz * dz);
    //    }
    //}

    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        // Crear puntos en un sistema de coordenadas 2D
    //        var punto1 = new CoordenadaCartesiana2D(2, 3);
    //        var punto2 = new CoordenadaCartesiana2D(5, 7);

    //        Console.WriteLine($"Punto 1: {punto1}");
    //        Console.WriteLine($"Punto 2: {punto2}");
    //        Console.WriteLine($"Distancia entre Punto 1 y Punto 2: {punto1.CalcularDistancia(punto2)}");

    //        // Crear puntos en un sistema de coordenadas 3D
    //        var punto3D_1 = new CoordenadaCartesiana3D(1, 2, 3);
    //        var punto3D_2 = new CoordenadaCartesiana3D(4, 6, 8);

    //        Console.WriteLine($"Punto 3D 1: {punto3D_1}");
    //        Console.WriteLine($"Punto 3D 2: {punto3D_2}");
    //        Console.WriteLine($"Distancia entre Punto 3D 1 y Punto 3D 2: {punto3D_1.CalcularDistancia(punto3D_2)}");
    //    }
    //}
}

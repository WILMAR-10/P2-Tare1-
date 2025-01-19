using System;
using System.Collections.Generic;

namespace P2.Clases_Tarea_1
{
    public abstract class Shape
    {
        public double Ancho { get; set; }
        public double Alto { get; set; }

        protected Shape(double ancho, double alto)
        {
            Ancho = ancho;
            Alto = alto;
        }

        public abstract double CalcularArea();
    }

    public class Rectangulo : Shape
    {
        public Rectangulo(double ancho, double alto) : base(ancho, alto) { }

        public override double CalcularArea()
        {
            return Ancho * Alto;
        }
    }

    public class Triangulo : Shape
    {
        public Triangulo(double ancho, double alto) : base(ancho, alto) { }

        public override double CalcularArea()
        {
            return (Ancho * Alto) / 2;
        }
    }

    public class Circulo : Shape
    {
        public Circulo(double radio) : base(radio, radio) { }

        public override double CalcularArea()
        {
            return Math.PI * Math.Pow(Ancho, 2);
        }
    }
}

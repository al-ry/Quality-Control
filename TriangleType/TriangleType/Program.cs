using System;
using System.Globalization;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;


namespace TriangleType
{
    public class Program
    {

        const int ARGUMENT_COUNT = 3;

        private enum TriangleType
        {
            common,
            none,
            equilateral,
            isosceles
        }
        public static void Main(string[] args)
        {
            try
            {
                float[] vertices = ParseArgs(args);
                TriangleType type = DefineTriangleType(vertices[0], vertices[1], vertices[2]);
                PrintTriangleType(type);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw;
            }
        }
        private static float[] ParseArgs(string[] args)
        {
            if (args.Length != ARGUMENT_COUNT)
            {
                throw new ArgumentException("Incorrect arguments count were specified. "
                    + "Arguments should be: triangle.exe <vertex1><vertex2><vertex3>");
            }
            try
            {
                float[] vertices = new float[3];

                vertices[0] = float.Parse(args[0], CultureInfo.InvariantCulture);
                vertices[1] = float.Parse(args[1], CultureInfo.InvariantCulture);
                vertices[2] = float.Parse(args[2], CultureInfo.InvariantCulture);
                if (vertices[0] <= 0 || vertices[1] <= 0 || vertices[2] <= 0)
                {
                    throw new Exception("Vertices must be greater than 0");
                }
                return vertices;
            }
            catch (FormatException)
            {
                throw new FormatException("Arguments should be float numbers");
            }
        }
        private static TriangleType DefineTriangleType(float vertex1, float vertex2, float vertex3)
        {
            float a = vertex1;
            float b = vertex2;
            float c = vertex3;
            if (a + b > c && b + c > a && a + c > b)
            {
                if (a == b && b == c)
                {
                    return TriangleType.equilateral;
                }
                if (a == b || b == c || c == a)
                {
                    return TriangleType.isosceles;
                }
                return TriangleType.common;
            }
            else
            {
                return TriangleType.none;
            }
        }
        private static void PrintTriangleType(TriangleType type)
        {
            if (type == TriangleType.common)
            {
                Console.WriteLine("Triangle is common");
            }
            else if (type == TriangleType.none)
            {
                Console.WriteLine("That is not triangle");
            }
            else if (type == TriangleType.isosceles)
            {
                Console.WriteLine("Triangle is isosceles");
            }
            else if (type == TriangleType.equilateral)
            {
                Console.WriteLine("Triangle is equilateral");
            }
        }
    }
}

using CommandLine;
using System;

namespace AerialodSlopefield
{
    class Program
    {
        public class Options
        {
            [Option("x1", Required = true, HelpText = "The X coordinate of the first corner of the area to render.")]
            public double X1 { get; set; }

            [Option("y1", Required = true, HelpText = "The Y coordinate of the first corner of the area to render.")]
            public double Y1 { get; set; }

            [Option("x2", Required = true, HelpText = "The X coordinate of the second corner of the area to render.")]
            public double X2 { get; set; }

            [Option("y2", Required = true, HelpText = "The Y coordinate of the second corner of the area to render.")]
            public double Y2 { get; set; }

            [Option("step", Required = true, HelpText = "The X and Y step.")]
            public double GridStep { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o => GenerateSlopeField(o));
        }

        private static void GenerateSlopeField(Options o)
        {
            var xmin = Math.Min(o.X1, o.X2);
            var xmax = Math.Max(o.X1, o.X2);
            var ymin = Math.Min(o.Y1, o.Y2);
            var ymax = Math.Max(o.Y1, o.Y2);
            var xsteps = (int)Math.Floor((xmax - xmin) / o.GridStep);
            var ysteps = (int)Math.Floor((ymax - ymin) / o.GridStep);
            double[,] results = new double[xsteps, ysteps];

            int currentXStep = 0;
            int currentYStep = 0;

            double minVal = double.MaxValue;
            double maxVal = double.MinValue;
            double lastResult;

            for (var y = ymin; y < ymax; y += o.GridStep)
            {
                for (var x = xmin; x < xmax; x += o.GridStep)
                {
                    results[currentXStep, currentYStep] = lastResult = MyFunc(x, y);
                    if (double.IsNormal(lastResult))
                    {
                        if (lastResult > maxVal)
                        {
                            maxVal = lastResult;
                        }
                        if (lastResult < minVal)
                        {
                            minVal = lastResult;
                        }
                    }
                    currentXStep++;
                }
                currentYStep++;
                currentXStep = 0;
            }

            var delta = maxVal - minVal;

            Console.WriteLine("ncols " + ysteps);
            Console.WriteLine("nrows " + xsteps);
            Console.WriteLine("xllcorner " + xmin);
            Console.WriteLine("yllcorner " + ymin);
            Console.WriteLine("cellsize " + o.GridStep);
            Console.WriteLine("NODATA_value -1");

            for (var y = 0; y < ysteps; y++)
            {
                for (var x = 0; x < xsteps; x++)
                {
                    if (double.IsNormal(results[x, y]))
                    {
                        results[x, y] = (results[x, y] - minVal) / delta;
                        Console.Write(results[x, y] + " ");
                    }
                    else
                    {
                        Console.Write("-1 ");
                    }
                }
                Console.WriteLine();
            }
        }

        private static double MyFunc(double x, double y)
        {
            return Math.Cos(x / y);
        }
    }
}

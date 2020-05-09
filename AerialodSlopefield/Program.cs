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

            [Option("raw", Required = false, HelpText = "If specified, the final arctan() function is not applied, so you basically get the graph of your function.")]
            public bool RenderRaw { get; set; }
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

            double minVal = double.MaxValue;
            double maxVal = double.MinValue;
            double lastResult;

            for (var yindex = 0; yindex < ysteps; yindex++)
            {
                for (var xindex = 0; xindex < xsteps; xindex++)
                {
                    var xpos = xmin + xindex * o.GridStep;
                    var ypos = ymin + yindex * o.GridStep;
                    if (o.RenderRaw)
                    {
                        results[xindex, yindex] = lastResult = MyFunc(xpos, ypos);
                    }
                    else
                    {
                        results[xindex, yindex] = lastResult = Math.Atan(MyFunc(xpos, ypos));
                    }
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
                }
            }

            var delta = maxVal - minVal;

            Console.WriteLine("ncols " + ysteps);
            Console.WriteLine("nrows " + xsteps);
            Console.WriteLine("xllcorner " + xmin);
            Console.WriteLine("yllcorner " + ymin);
            Console.WriteLine("cellsize " + o.GridStep);
            Console.WriteLine("NODATA_value -1");

            for (var yindex = 0; yindex < ysteps; yindex++)
            {
                for (var xindex = 0; xindex < xsteps; xindex++)
                {
                    if (double.IsNormal(results[xindex, yindex]))
                    {
                        results[xindex, yindex] = (results[xindex, yindex] - minVal) / delta;
                        Console.Write(results[xindex, yindex] + " ");
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
            return x * x / y;
        }
    }
}

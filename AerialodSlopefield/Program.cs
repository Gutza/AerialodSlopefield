using AerialodSlopefield.Outputs;
using CommandLine;
using System;
using System.Text;

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

            [Option('f', "filename", Required = false, HelpText = "The name of the file to write to. If the parameter is not specified, the output is sent to the console. You probably want to specify an ASC extension.")]
            public string Filename { get; set; }
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

            using IOutput output = InitializeOutput(o.Filename);
            output.WriteLine("ncols " + ysteps);
            output.WriteLine("nrows " + xsteps);
            output.WriteLine("xllcorner " + xmin);
            output.WriteLine("yllcorner " + ymin);
            output.WriteLine("cellsize " + o.GridStep);
            output.WriteLine("NODATA_value -1");

            for (var yindex = 0; yindex < ysteps; yindex++)
            {
                var line = new StringBuilder(xsteps * 10);
                for (var xindex = 0; xindex < xsteps; xindex++)
                {
                    if (double.IsNormal(results[xindex, yindex]))
                    {
                        results[xindex, yindex] = (results[xindex, yindex] - minVal) / delta;
                        line.Append(results[xindex, yindex] + " ");
                    }
                    else
                    {
                        line.Append("-1 ");
                    }
                }
                output.WriteLine(line.ToString());
            }
        }

        private static IOutput InitializeOutput(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return new ConsoleOutput();
            }

            return new FileOutput(filename);
        }

        private static double MyFunc(double x, double y)
        {
            return x * x / y;
        }
    }
}

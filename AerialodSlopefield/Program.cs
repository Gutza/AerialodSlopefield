using AerialodSlopefield.ConsoleOutput;
using AerialodSlopefield.FileOutput;
using CommandLine;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace AerialodSlopefield
{
    class Program
    {
        public class Options
        {
            [Option("x1", Default = -10, HelpText = "The X coordinate of the first corner of the area to render.")]
            public double X1 { get; set; }

            [Option("x2", Default = 10, HelpText = "The X coordinate of the second corner of the area to render.")]
            public double X2 { get; set; }

            [Option('x', "xscale", Default = 1, HelpText = "Resize the X range by this factor. For instance, if you use the default x1 and x2 values, and you set xscale to 1.3 then you will end up rendering the area between x1=-13.0 and x2=13.0")]
            public double XScale { get; set; }

            [Option("y1", Default = -10, HelpText = "The Y coordinate of the first corner of the area to render.")]
            public double Y1 { get; set; }

            [Option("y2", Default = 10, HelpText = "The Y coordinate of the second corner of the area to render.")]
            public double Y2 { get; set; }

            [Option('y', "yscale", Default = 1, HelpText = "Resize the Y range by this factor. For instance, if you use the default y1 value, you set y2 to 100, and you set yscale to 3.14 then you will end up rendering the area between y1=-31.4 and y2=314.0")]
            public double YScale { get; set; }

            [Option('s', "step", Default = 0.1, HelpText = "The X and Y step.")]
            public double GridStep { get; set; }

            [Option('r', "raw", Default = false, HelpText = "If specified, the final arctan() function is not applied, so you basically get the graph of your function.")]
            public bool RenderRaw { get; set; }

            [Option('f', "filename", Required = false, HelpText = "The name of the file to write to. If the parameter is not specified, the output is sent to the console. You probably want to specify an ASC extension.")]
            public string Filename { get; set; }

            [Option('p', "precision", Default = 4, HelpText = "The precision of the conversion between internally computed values and their ASCII representation in the output file.")]
            public int Precision { get; set; }

            [Option('d', "denormalize", HelpText = "Do not normalize the output values. By default, the output is normalized between 0 and 1; use this option if you want the actual values returned by your function.")]
            public bool Denormalize { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o => GenerateSlopeField(o));
        }

        private static void GenerateSlopeField(Options o)
        {
            o.X1 *= o.XScale;
            o.X2 *= o.XScale;
            o.Y1 *= o.YScale;
            o.Y2 *= o.YScale;
            var xmin = Math.Min(o.X1, o.X2);
            var xmax = Math.Max(o.X1, o.X2);
            var ymin = Math.Min(o.Y1, o.Y2);
            var ymax = Math.Max(o.Y1, o.Y2);
            var xsteps = 1 + (int)Math.Floor((xmax - xmin) / o.GridStep);
            var ysteps = 1 + (int)Math.Floor((ymax - ymin) / o.GridStep);
            double[][] resultMatrix = new double[ysteps][];

            double minVal = double.MaxValue;
            double maxVal = double.MinValue;

            var consoleOutput = InitializeConsoleOutput(o.Filename);

            var stopwatch = new Stopwatch();
            consoleOutput.Write("Starting computations [(" + xmin + "," + ymin + ")..(" + xmax + "," + ymax + ")@" + o.GridStep + "]...");
            stopwatch.Start();
            for (var yindex = 0; yindex < ysteps; yindex++)
            {
                var computation = ComputeRow(yindex, xsteps, xmin, ymin, o.GridStep, o.RenderRaw);
                resultMatrix[yindex] = computation.RowData;
                minVal = Math.Min(minVal, computation.MinValue);
                maxVal = Math.Max(maxVal, computation.MaxValue);
            }
            stopwatch.Stop();
            consoleOutput.WriteLine(" finished in " + stopwatch.Elapsed);

            var delta = maxVal - minVal;
            if (delta == 0)
            {
                /*
                 * Fringe case: some smartass is using a constant render function, so the delta is zero.
                 * Fake a delta equal to their constant value, so we output constant zeroes all over.
                 * That will teach them, and they shalt be smartasses no more, from henceforth to kingdom come.
                 * Because that's how smartasses work.
                 */
                delta = minVal;
            }

            using IOutput output = InitializeOutput(o.Filename);
            output.WriteLine("ncols " + xsteps);
            output.WriteLine("nrows " + ysteps);
            output.WriteLine("xllcorner " + xmin);
            output.WriteLine("yllcorner " + ymin);
            output.WriteLine("cellsize " + o.GridStep);
            output.WriteLine("NODATA_value -1");

            consoleOutput.Write("Starting output...");
            stopwatch.Restart();
            for (var yindex = 0; yindex < ysteps; yindex++)
            {
                var line = new StringBuilder(xsteps * (o.Precision + 2)); // o.Precision + period + space
                for (var xindex = 0; xindex < xsteps; xindex++)
                {
                    if (double.IsFinite(resultMatrix[yindex][xindex]))
                    {
                        if (!o.Denormalize)
                        {
                            resultMatrix[yindex][xindex] = (resultMatrix[yindex][xindex] - minVal) / delta;
                        }
                        line.Append(ValueRenderer(resultMatrix[yindex][xindex], o) + " ");
                    }
                    else
                    {
                        line.Append("-1 ");
                    }
                }
                output.WriteLine(line.ToString());
            }
            stopwatch.Stop();
            consoleOutput.WriteLine(" finished in " + stopwatch.Elapsed);
        }

        private static string ValueRenderer(double value, Options o)
        {
            return value.ToString("F" + o.Precision, CultureInfo.InvariantCulture);
            /*
            if (value>0.1 && (!o.RenderRaw || !o.Denormalize))
            {
                // TODO: Pre-compute the format string
                return value.ToString("F" + o.Precision, CultureInfo.InvariantCulture);
            }

            return RoundToSignificantDigits(value, o.Precision);
            */
        }

        static string RoundToSignificantDigits(double d, int significantDigits)
        {
            /*
            TODO: This is not currently used because it's buggy.
            It doesn't work for negative values, and it also returns one too few decimals for numbers less than 1
            Test it with function return (x+1000)*x/y/(y+1000) AND with function return (x+1000)*x/y/(y+1000)
            */
            if (d == 0.0)
            {
                return "0";
            }

            var totalDigits = (int)Math.Log10(d);
            var format = "F" + Math.Max(0, (significantDigits - totalDigits - 1));
            return d.ToString(format, CultureInfo.InvariantCulture);
        }

        private static IConsoleOutput InitializeConsoleOutput(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return new VoidConsoleOutput();
            }

            return new RealConsoleOutput();
        }

        class ComputationDTO
        {
            internal double[] RowData;
            internal double MinValue;
            internal double MaxValue;
        }

        private static ComputationDTO ComputeRow(int yindex, int xsteps, double xmin, double ymin, double gridStep, bool renderRaw)
        {
            var result = new double[xsteps];

            double minVal = double.MaxValue;
            double maxVal = double.MinValue;
            double lastResult;

            var ypos = ymin + yindex * gridStep;
            for (var xindex = 0; xindex < xsteps; xindex++)
            {
                var xpos = xmin + xindex * gridStep;
                if (renderRaw)
                {
                    lastResult = RenderFunction.MyFunction(xpos, ypos, xindex, yindex);
                }
                else
                {
                    lastResult = Math.Atan(RenderFunction.MyFunction(xpos, ypos, xindex, yindex));
                }
                result[xindex] = lastResult;
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

            return new ComputationDTO()
            {
                RowData = result,
                MinValue = minVal,
                MaxValue = maxVal,
            };
        }

        private static IOutput InitializeOutput(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return new FileOutput.ConsoleOutput();
            }

            return new FileOutput.FileOutput(filename);
        }
    }
}

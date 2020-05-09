﻿using AerialodSlopefield.ConsoleOutput;
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
            double[][] resultMatrix = new double[ysteps][];

            double minVal = double.MaxValue;
            double maxVal = double.MinValue;

            var consoleOutput = InitializeConsoleOutput(o.Filename);

            var stopwatch = new Stopwatch();
            consoleOutput.Write("Starting computations...");
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

            using IOutput output = InitializeOutput(o.Filename);
            output.WriteLine("ncols " + ysteps);
            output.WriteLine("nrows " + xsteps);
            output.WriteLine("xllcorner " + xmin);
            output.WriteLine("yllcorner " + ymin);
            output.WriteLine("cellsize " + o.GridStep);
            output.WriteLine("NODATA_value -1");

            consoleOutput.Write("Starting output...");
            stopwatch.Restart();
            for (var yindex = 0; yindex < ysteps; yindex++)
            {
                var line = new StringBuilder(xsteps * 4);
                for (var xindex = 0; xindex < xsteps; xindex++)
                {
                    if (double.IsFinite(resultMatrix[yindex][xindex]))
                    {
                        resultMatrix[yindex][xindex] = (resultMatrix[yindex][xindex] - minVal) / delta;
                        line.Append(resultMatrix[yindex][xindex].ToString("F3", CultureInfo.InvariantCulture) + " ");
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

            for (var xindex = 0; xindex < xsteps; xindex++)
            {
                var xpos = xmin + xindex * gridStep;
                var ypos = ymin + yindex * gridStep;
                if (renderRaw)
                {
                    result[xindex] = lastResult = RenderFunction.MyFunction(xpos, ypos);
                }
                else
                {
                    result[xindex] = lastResult = Math.Atan(RenderFunction.MyFunction(xpos, ypos));
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

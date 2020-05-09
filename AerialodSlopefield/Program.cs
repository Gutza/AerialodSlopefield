using CommandLine;
using System;

namespace AerialodSlopefield
{
    class Program
    {
        public class Options
        {
            [Option("minx", Required =true, HelpText ="The minimum X value to render.")]
            public float MinX { get; set; }

            [Option("maxx", Required = true, HelpText = "The maximum X value to render.")]
            public float MaxX { get; set; }

            [Option("stepx", Required = true, HelpText = "The step for X values.")]
            public float StepX { get; set; }

            [Option("miny", Required = true, HelpText = "The minimum Y value to render.")]
            public float MinY { get; set; }

            [Option("maxy", Required = true, HelpText = "The maximum Y value to render.")]
            public float MaxY { get; set; }

            [Option("stepy", Required = true, HelpText = "The step for Y values.")]
            public float StepY { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o => GenerateSlopeField(o));
        }

        private static void GenerateSlopeField(Options o)
        {
            throw new NotImplementedException();
        }
    }
}

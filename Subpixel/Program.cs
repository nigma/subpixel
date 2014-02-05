using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Text;
using CommandLine;
using CommandLine.Text;

namespace Subpixel
{

    class Options {
        [Option('t', "text", Required = true, HelpText = "Icon text.")]
        public String Text { get; set; }

        [Option('f', "font", DefaultValue = "Calibri", HelpText = "Font name.")]
        public String Font { get; set; }

        [Option('w', "width", DefaultValue = 16, HelpText = "Icon width.")]
        public int Width { get; set; }

        [Option('h', "height", DefaultValue = 16, HelpText = "Icon height.")]
        public int Height { get; set; }

        [Option('c', "color", DefaultValue = "#FFFFFF", HelpText = "Font color.")]
        public String FontColor { get; set; }

        [Option('b', "background", DefaultValue = "#000000", HelpText = "Background color.")]
        public String BackgroundColor { get; set; }

        [Option('r', "rendering", DefaultValue = TextRenderingHint.AntiAliasGridFit,
            HelpText = "Rendering: AntiAlias, AntiAliasGridFit, ClearTypeGridFit, " +
                       "SingleBitPerPixel, SingleBitPerPixelGridFit.")]
        public TextRenderingHint Rendering { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output image file.")]
        public String Output { get; set; }

        [Option("padding-v", DefaultValue = 1, HelpText = "Vertical padding.")]
        public int PaddingV { get; set; }

        [Option("padding-h", DefaultValue = 1, HelpText = "Horizontal padding.")]
        public int PaddingH { get; set; }

        [Option('s', "style", DefaultValue = FontStyle.Regular, 
            HelpText = "Font style: Regular, Bold, Italic, Strikeout, Underline.")]
        public FontStyle FontStyle { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
    
    class Program
    {

        static void Test()
        {
            Size iconSize = new Size(16, 16);
            SizeF padding = new SizeF(1, 1);
            Color fontColor = ColorTranslator.FromHtml("#FFFFFF");
            Color backgroundColor = ColorTranslator.FromHtml("#71bf4c");

            var icon = new FaviconDrawing(iconSize, padding);
            icon.DrawFavicon(
                "S", "Calibri", fontColor, backgroundColor,
                FontStyle.Bold, TextRenderingHint.AntiAlias
            );
            icon.Save("test.png");
        }

        static void Run(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                Size iconSize = new Size(options.Width, options.Height);
                SizeF padding = new SizeF(options.PaddingH, options.PaddingV);
                Color fontColor = ColorTranslator.FromHtml(options.FontColor);
                Color backgroundColor = ColorTranslator.FromHtml(options.BackgroundColor);

                var icon = new FaviconDrawing(iconSize, padding);
                icon.DrawFavicon(
                    options.Text, options.Font, fontColor, backgroundColor,
                    options.FontStyle, options.Rendering
                );
                icon.Save(options.Output);
            } 
        }

        static void Main(string[] args)
        {
            Run(args);
        }
    }
}

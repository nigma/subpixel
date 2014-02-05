using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Subpixel
{
    struct TextBoxInfo
    {
        public String text;
        public Font font;
        public float fontSize;
        public RectangleF trueBox;

        public TextBoxInfo(String text, Font font, float fontSize, RectangleF trueBox)
        {
            this.text = text;
            this.font = font;
            this.fontSize = fontSize;
            this.trueBox = trueBox;
        }
    }

    class TextFitting
    {
        private static int white = Color.White.ToArgb();

        public static SizeF MeasureString(Graphics graphic, String textToFit, Font fontTry, StringFormat format) {
            return MeasureRealStringBoundaries(graphic, textToFit, fontTry, format).Size;
        }

        /*
         * Find an optimal font size for text to fit in the given box.
         */
        public static TextBoxInfo FitTextSize(
            Graphics graphic, String textToFit, SizeF boxSize, String fontName, 
            FontStyle fontStyle, StringFormat format, 
            float guessSize = 16, float step=0.05f)
        {

            var fontTry = GetFont(fontName, guessSize, fontStyle);

            /* Initial measure */
            SizeF measuredSize = MeasureString(graphic, textToFit, fontTry, format);

            float currentWidth = measuredSize.Width;
            float currentHeight = measuredSize.Height;
            float desiredWidth = boxSize.Width;
            float desiredHeight = boxSize.Height;

            /* Estimate size */
            float estimatedFontSize = Math.Min(
                currentWidth * (desiredWidth / currentWidth),
                currentHeight * (desiredHeight / currentHeight)
            );

            /* Refine size */
            float refinedSize = estimatedFontSize;
            while (refinedSize > 1)
            {
                fontTry = GetFont(fontName, refinedSize, fontStyle);
                measuredSize = MeasureString(graphic, textToFit, fontTry, format);

                if (measuredSize.Width <= desiredWidth && measuredSize.Height <= desiredHeight)
                    break;
                refinedSize -= step;
            }
            
            var textBoxInfo = new TextBoxInfo(
                textToFit,
                GetFont(fontName, refinedSize, fontStyle),
                refinedSize,
                MeasureRealStringBoundaries(graphic, textToFit, fontTry, format)
            );

            return textBoxInfo;
        }

        private static Font GetFont(String fontName, float fontSize, FontStyle fontStyle)
        {
            FontFamily fontFamily = new FontFamily(fontName);
            return new Font(fontFamily, fontSize, fontStyle);
        }

        /*
         Several methods of measuring rendered string size
         */

        /* Size estimation */
        static public SizeF GraphicMeasureString(Graphics graphics, string text, Font font, StringFormat format) {
            return graphics.MeasureString(text, font, new SizeF(int.MaxValue, int.MaxValue), format);
        }

        static public SizeF MeasureTextRendererString(Graphics graphics, string text, Font font, StringFormat format)
        {
            TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.Left
                | TextFormatFlags.Top | TextFormatFlags.RightToLeft | TextFormatFlags.SingleLine;
            return TextRenderer.MeasureText(graphics, text, font, new Size(int.MaxValue, int.MaxValue), flags);
        }

        static public RectangleF MeasureDisplayStringWidth(Graphics graphics, string text, Font font, StringFormat format)
        {
            StringFormat strFormat = new StringFormat(format);
            RectangleF rect = new RectangleF(0, 0, 1000, 1000);
            CharacterRange[] ranges = { new CharacterRange(0, text.Length) };
            Region[] regions = new Region[1];
            strFormat.SetMeasurableCharacterRanges(ranges);
            regions = graphics.MeasureCharacterRanges(text, font, rect, strFormat);
            rect = regions[0].GetBounds(graphics);
            return rect;
        }

        /* Only drawing the actual text and then measuring 
         * non-empty region seems a reliable method of figuring
         * out text boundaries.
         * See: http://stackoverflow.com/a/12780074
         */
        public static RectangleF MeasureRealStringBoundaries(Graphics graphics, string text, Font font, StringFormat format)
        {
            /*
             3x3 matrix, affine transformation (skew - used by rotation)
             [ X scale,     Y skew,      0 ]
             [ X skew,      Y scale,     0 ]
             [ X translate, Y translate, 1 ]

             indices (0, ...): X scale, Y skew, Y skew, X scale, X translate, Y translate
             */
            var scale = new SizeF(graphics.Transform.Elements[0], graphics.Transform.Elements[3]);

            SizeF sizef = GraphicMeasureString(graphics, text, font, format);
            
            sizef.Width *= scale.Width;
            sizef.Height *= scale.Height;
            Size size = sizef.ToSize();

            RectangleF boundaries;

            using (Bitmap image = new Bitmap(size.Width, size.Height))
            {
                image.SetResolution(graphics.DpiX, graphics.DpiY);

                /* Draw text and find boundaries of non-empty region */
                using (Graphics g = Graphics.FromImage(image))
                {
                    g.SmoothingMode = graphics.SmoothingMode;
                    g.TextRenderingHint = graphics.TextRenderingHint;
                    g.Clear(Color.White);
                    g.ScaleTransform(scale.Width, scale.Height);
                    g.DrawString(text, font, Brushes.Black, new PointF(0, 0), format);
                }
                boundaries = GetNonEmptyBoundaries(image);
            }
            return new RectangleF(boundaries.X / scale.Width, boundaries.Y / scale.Height,
                boundaries.Width / scale.Width, boundaries.Height / scale.Height);
        }

        // Find boundaries of non-empty region
        public static RectangleF GetNonEmptyBoundaries(Bitmap image)
        {
            int left = 0;
            int right = image.Size.Width - 1;
            int top = 0;
            int bottom = image.Size.Height - 1;

            for (; left < right; left++)
                if (ColNonEmpty(image, left)) break;

            for (; right > left; right--)
                if (ColNonEmpty(image, right)) break;

            for (; top < bottom; top++)
                if (RowNonEmpty(image, top)) break;

            for (; bottom > top; bottom--)
                if (RowNonEmpty(image, bottom)) break;

            return new RectangleF(left, top, right - left + 1, bottom - top + 1);
        }

        public static bool ColNonEmpty(Bitmap image, int col)
        {
            int max = image.Size.Height;
            for (int y = 0; y < max; y++)
                if (image.GetPixel(col, y).ToArgb() != white)
                    return true;
            return false;
        }

        public static bool RowNonEmpty(Bitmap image, int row)
        {
            int max = image.Size.Width;
            for (int x = 0; x < max; x++)
                if (image.GetPixel(x, row).ToArgb() != white)
                    return true;
            return false;
        }
    }
}

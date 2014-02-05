using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Subpixel
{
    class FaviconDrawing
    {
        private Size size;
        private SizeF padding;
        private Bitmap bitmap = null;

        public FaviconDrawing(Size size, SizeF padding)
        {
            this.size = size;
            this.padding = padding;
            this.bitmap = new Bitmap(this.size.Width, this.size.Height);
            //bitmap.MakeTransparent();
        }

        private SizeF RealEstateSize() {
            return new SizeF(this.size.Width - 2 * this.padding.Width,
                this.size.Height - 2 * this.padding.Height);
        }

        private PointF TextPosition(TextBoxInfo info) {
            return new PointF(
                - info.trueBox.Left + (this.size.Width - info.trueBox.Width) / 2,
                - info.trueBox.Top + (this.size.Height - info.trueBox.Height) / 2
            );
        }

        private StringFormat GetStringFormat() {
            var format = StringFormat.GenericTypographic;
            //format = StringFormat.GenericDefault;
            //format.FormatFlags |= StringFormatFlags.NoWrap | StringFormatFlags.NoFontFallback;
            return format;
        }

        private void DrawDebugBox(Graphics graphics, TextBoxInfo info) {
            graphics.DrawRectangle(
                new Pen(Color.Blue, 1),
                this.padding.Width, this.padding.Height,
                this.RealEstateSize().Width, this.RealEstateSize().Height
            );

            graphics.DrawRectangle(
                new Pen(Color.Red, 1), 
                (this.size.Width - info.trueBox.Width) / 2, (this.size.Height - info.trueBox.Height) / 2,
                info.trueBox.Width, info.trueBox.Height
            );
        }

        private Brush GetBrush(Color color)
        {
            return new SolidBrush(color);
        }

        private void FillBackground(Graphics graphics, Color color) {
            graphics.Clear(color);
        }

        private void DrawText(Graphics graphics, String text, String fontName, Color fontColor, FontStyle fontStyle)
        {
            var format = GetStringFormat();

            var boxInfo = TextFitting.FitTextSize(
                graphics, text, RealEstateSize(), fontName, fontStyle, format);

            //DrawDebugBox(graphics, boxInfo);

            // boxInfo.font is a font object selected based on text fitting params
            graphics.DrawString(text, boxInfo.font, GetBrush(fontColor), TextPosition(boxInfo), format);
        }

        public void DrawFavicon(String text, String fontName, Color fontColor, Color backgroundColor,
            FontStyle fontStyle, TextRenderingHint hint)
        {
            using (var graphics = Graphics.FromImage(this.bitmap))
            {
                graphics.TextRenderingHint = hint;
                FillBackground(graphics, backgroundColor);
                DrawText(graphics, text, fontName, fontColor, fontStyle);                
            }
        }

        public void Save(String path)
        {
            this.bitmap.Save(path);
        }

    }
}

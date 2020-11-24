using Lib;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace IoTMcuEdit
{
    class GenShow
    {
        public static readonly Dictionary<FontSelfColor, Brush> BrushesSave = new()
        {
            { FontSelfColor.RED, Brushes.Red },
            { FontSelfColor.BULE, Brushes.Blue },
            { FontSelfColor.MIX, Brushes.Lime }
        };
        public static Bitmap Gen(ShowObj show, LcdObj lcd)
        {
            Bitmap ShowImg = new(lcd.X, lcd.Y);
            Graphics graphics = Graphics.FromImage(ShowImg);
            graphics.SmoothingMode = SmoothingMode.None;
            graphics.InterpolationMode = InterpolationMode.Low;
            graphics.PageUnit = GraphicsUnit.Pixel;
            graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
            graphics.Clear(Color.White);

            graphics.DrawString(show.Text, new Font(show.FontType, show.Size, GraphicsUnit.Pixel),
                BrushesSave[show.Color], show.X, show.Y);

            return ShowImg;
        }
    }
}

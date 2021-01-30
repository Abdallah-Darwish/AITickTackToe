using Avalonia;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITickTackToe
{
    static class Extensions
    {
        public static Point Scale(this in Point p, in Size scale) => new Point(p.X * scale.Width, p.Y * scale.Height);
        public static IBitmap Resize(this Bitmap bmp, PixelSize sz)
        {
            var res = new RenderTargetBitmap(sz);
            using var ctx = res.CreateDrawingContext(null);
            ctx.DrawImage(bmp.PlatformImpl, 1.0, new Rect(bmp.Size), new Rect(res.Size));
            return res;
        }
    }
}

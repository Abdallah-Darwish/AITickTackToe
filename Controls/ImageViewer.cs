//TODO: preserve aspect ratio

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System.Linq;
using System;
using Avalonia.Media.Imaging;
using Avalonia.Metadata;

namespace AITickTackToe.Controls
{
    public class ImageViewer : Control
    {
        private IBitmap? _source;
        [Content]
        ///<summary>
        ///Source Image for this control to render
        ///</summary>
        public IBitmap? Source
        {
            get => _source;
            set
            {
                if (!SetAndRaise(SourceProperty, ref _source, value)) { return; }
                SourceTopLeft = default;
                ScaleFactor = 1.0;
            }
        }
        public static readonly DirectProperty<ImageViewer, IBitmap?> SourceProperty = AvaloniaProperty.RegisterDirect<ImageViewer, IBitmap?>(nameof(Source), o => o.Source, (o, v) => o.Source = v);

        private double _scaleFactor = 1.0;
        ///<summary>
        /// How much to scale this image when rendering it
        ///</summary>
        public double ScaleFactor
        {
            get => _scaleFactor;
            set
            {
                if (Source == null || Bounds.Width == 0 || Bounds.Height == 0)
                {
                    SetAndRaise(ScaleFactorProperty, ref _scaleFactor, value);
                    return;
                }
                if (value < 1.0) { throw new ArgumentOutOfRangeException(nameof(ScaleFactor), value, "Value must be >= 1."); }

                if (!SetAndRaise(ScaleFactorProperty, ref _scaleFactor, value)) { return; }

                var srcRect = new Rect(default, Source.Size);
                var newSrcRect = new Rect(SourceTopLeft, Source.Size / ScaleFactor);
                if (!srcRect.Contains(newSrcRect))
                {
                    SourceTopLeft = new Point(Math.Max(Math.Min(srcRect.Width - newSrcRect.Size.Width, SourceTopLeft.X), 0), Math.Max(Math.Min(srcRect.Height - newSrcRect.Size.Height, SourceTopLeft.Y), 0));
                }
            }
        }
        public static readonly DirectProperty<ImageViewer, double> ScaleFactorProperty = AvaloniaProperty.RegisterDirect<ImageViewer, double>(nameof(ScaleFactor), o => o.ScaleFactor, (o, v) => o.ScaleFactor = v, enableDataValidation: true);

        private Point _sourceTopLeft = default;
        //made it source not "scaled source" cause I don't want to handle it after scaling
        public Point SourceTopLeft
        {
            get => _sourceTopLeft;
            set
            {
                if (Source != null && Bounds.Width != 0 && Bounds.Height != 0)
                {
                    var srcRect = new Rect(default, Source.Size);
                    var newSrcRect = new Rect(value, Source.Size / ScaleFactor);
                    if (value.X < 0 || value.Y < 0 || !srcRect.Contains(newSrcRect))
                    {
                        throw new ArgumentOutOfRangeException(nameof(SourceTopLeft), value, "This value will go outside of source bounds.");
                    }
                }
                SetAndRaise(ScaledSourceTopLeftProperty, ref _sourceTopLeft, value);
            }
        }
        public static readonly DirectProperty<ImageViewer, Point> ScaledSourceTopLeftProperty = AvaloniaProperty.RegisterDirect<ImageViewer, Point>(nameof(ScaledSourceTopLeftProperty), o => o.SourceTopLeft, (o, v) => o.SourceTopLeft = v, enableDataValidation: true);
        #region InteractionHandling
        protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
        {
            base.OnPointerWheelChanged(e);
            if (Source == null) { return; }
            var delta = e.Delta.Y * 0.1;
            try
            {
                ScaleFactor += delta;
            }
            catch { }
        }

        private bool _captured = false;
        private Point _pointerLastLocation;
        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);
            if (!_captured) { return; }
            var p = e.GetPosition(this);
            try
            {
                var movment = (p - _pointerLastLocation) * ScaleFactor;
                try { SourceTopLeft -= new Point(movment.X, 0); }
                catch { }
                try { SourceTopLeft -= new Point(0, movment.Y); }
                catch { }
                _pointerLastLocation = p;
            }
            catch { }
        }
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            if (Source == null) { return; }
            _captured = true;
            _pointerLastLocation = e.GetPosition(this);
        }
        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);
            _captured = false;
        }
        #endregion
        static ImageViewer()
        {
            AffectsRender<ImageViewer>(SourceProperty, ScaledSourceTopLeftProperty, ScaleFactorProperty);
        }
        public override void Render(DrawingContext ctx)
        {
            base.Render(ctx);
            if (Source == null) { return; }
            var srcRect = new Rect(SourceTopLeft, Source.Size / ScaleFactor);
            /*
            Find if max(width, height) for the Source, then 
            max = its equivalent in the control
            calculate min
            */
            double dstWidth, dstHeight;
            if(Source.PixelSize.Width > Source.PixelSize.Height)
            {
                dstWidth = Bounds.Width;
                dstHeight = dstWidth * (Source.Size.Height / Source.Size.Width);
            }
            else
            {
                dstHeight = Bounds.Height;
                dstWidth = dstHeight * (Source.Size.Width / Source.Size.Height);
            }
            var dstRect = new Rect(default, new Size(dstWidth, dstHeight));

            ctx.DrawImage(Source, 1.0, srcRect, dstRect);
        }
    }
}
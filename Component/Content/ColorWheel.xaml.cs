using System;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Component.Helpers;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using TouchTracking;
using Utmdev.Xf.Components.Models;
using Xamarin.Forms;

namespace Utmdev.Xf.Components
{
    public partial class ColorWheel : ContentView
    {
        #region Fields

        // Info
        private CanvasInfo _colorsCanvasInfo;

        private CanvasInfo _pickerCanvasInfo;

        private float _angle;

        private float _innerCircleX;

        private float _innerCircleY;

        private SKPoint _center;

        private SKPoint _pixelPoint;

        private SKPoint _previousTouchPoint;

        private SKRect _innerCircleRect;

        private long _touchId = -1;

        private SKColor[] _colors;

        private SKMatrix _matrix;

        private int _lastHue;

        private int _currentHue;

        private double _hueStep = 1.0;

        // Paints
        private SKPaint _outerCirclePaint;

        private SKPaint _innerCirclePaint;

        private float _outerCircleRadius;

        private float _innerCircleRadius;

        private float _outerCircleWidth;

        private float _saturation = 100;

        private float _luminosity = 50;

        #endregion

        #region Bindable Properties

        public static readonly BindableProperty HueProperty = BindableProperty
            .Create(nameof(Hue), typeof(int), typeof(ColorWheel), -1);

        public int Hue
        {
            get => (int)GetValue(HueProperty);

            set => SetValue(HueProperty, value);
        }

        #endregion

        #region Commands

        public static readonly BindableProperty ReleasedCommandProperty = BindableProperty
            .Create(nameof(ReleasedCommand), typeof(ICommand), typeof(ColorWheel));

        public ICommand ReleasedCommand
        {
            get => (ICommand)GetValue(ReleasedCommandProperty);
            set => SetValue(ReleasedCommandProperty, value);
        }

        public static readonly BindableProperty ValueChangedCommandProperty = BindableProperty
            .Create(nameof(ValueChangedCommand), typeof(ICommand), typeof(ColorWheel));

        public ICommand ValueChangedCommand
        {
            get => (ICommand)GetValue(ValueChangedCommandProperty);
            set => SetValue(ValueChangedCommandProperty, value);
        }

        #endregion

        public ColorWheel()
        {
            InitializeComponent();
            InitializePaints();
        }

        #region Methods

        private void InitializePaints()
        {
            _colorsCanvasInfo = new CanvasInfo();
            _pickerCanvasInfo = new CanvasInfo();

            // Outer circle paint
            _outerCirclePaint = new SKPaint
            {
                Color = SKColors.Red,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 5
            };

            // Inner circle paint
            _innerCirclePaint = new SKPaint
            {
                Color = SKColors.White,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 5
            };

            // Define an array of rainbow colors
            _colors = new SKColor[8];

            for (var i = 0; i < _colors.Length; i++) _colors[i] = SKColor
                    .FromHsl(i * 360f / 7, _saturation, _luminosity);

            // Inner circle shadow
            _innerCirclePaint.ImageFilter = SKImageFilter
                .CreateDropShadow(0, 0, 5, 5, SKColor.Parse("#070E24")
                    , SKDropShadowImageFilterShadowMode.DrawShadowAndForeground);
        }

        private void CalculateAngle(SKPoint sKPoint)
        {
            _angle = (float)Math.Atan2(-_center.Y - -sKPoint.Y
                , -_center.X - -sKPoint.X);
        }

        private float FromRadiansToDegrees(float radians)
        {
            return (float)(radians * (180 / Math.PI));
        }

        private float FromDegreesRadians(float degrees)
        {
            return (float)(degrees * (Math.PI / 180));
        }

        private void ExecuteReleasedCommand()
        {
            Hue = CalculateHue();

            // Execute command
            XamarinHelper.ExecuteCommand(ReleasedCommand, GetColor());
        }

        private int CalculateHue()
        {
            // Calculate hue
            if (_angle < 0)
                return (int)(360 - Math.Abs(FromRadiansToDegrees(_angle)));
            return (int)FromRadiansToDegrees(_angle);
        }

        private void ExecuteValueChangedCommand()
        {
            _lastHue = _currentHue;

            _currentHue = (int)(Math.Round(CalculateHue() / _hueStep) * _hueStep);

            if (_lastHue != _currentHue)
            {
                Hue = CalculateHue();

                // Execute value changed command
                XamarinHelper.ExecuteCommand(ValueChangedCommand, GetColor());
            }
        }

        private string GetColor()
            => SKColor.FromHsl(Hue, _saturation, _luminosity).ToString();

        #endregion

        #region Events

        private void ColorsCanvas_PaintSurface
            (object sender, SKPaintSurfaceEventArgs e)
        {
            // Get info
            _colorsCanvasInfo.ImageInfo = e.Info;
            _colorsCanvasInfo.Canvas = e.Surface.Canvas;
            _colorsCanvasInfo.Surface = e.Surface;

            // Clear
            _colorsCanvasInfo.Canvas.Clear(SKColors.Transparent);

            // Calc radius
            _outerCircleRadius = _colorsCanvasInfo.ImageInfo.Height / 2.5f;

            _outerCircleWidth = _colorsCanvasInfo.ImageInfo.Width * 0.0925f;

            _outerCirclePaint.StrokeWidth = _outerCircleWidth;

            _innerCircleRadius = _outerCircleWidth / 2.5f;

            _center = new SKPoint(_colorsCanvasInfo.ImageInfo.Rect.MidX
                , _colorsCanvasInfo.ImageInfo.Rect.MidY);

            // Create sweep gradient
            _outerCirclePaint.Shader = SKShader
                .CreateSweepGradient(_center, _colors, null);

            // Draw colors circle
            _colorsCanvasInfo.Canvas.DrawCircle
                (_center, _outerCircleRadius, _outerCirclePaint);

            // Picker invalidate surface event
            pickerCanvas.PaintSurface += PickerCanvas_PaintSurface;

            pickerCanvas.InvalidateSurface();
        }

        private void PickerCanvas_PaintSurface
            (object sender, SKPaintSurfaceEventArgs e)
        {
            // Get info
            _pickerCanvasInfo.ImageInfo = e.Info;
            _pickerCanvasInfo.Canvas = e.Surface.Canvas;
            _pickerCanvasInfo.Surface = e.Surface;

            // Clear
            _pickerCanvasInfo.Canvas.Clear(SKColors.Transparent);

            // Calculate inner circle center
            _innerCircleX = _center.X + (float)Math.Cos(_angle) * _outerCircleRadius;
            _innerCircleY = _center.Y + (float)Math.Sin(_angle) * _outerCircleRadius;

            // Draw inner circle
            _pickerCanvasInfo.Canvas
                .DrawCircle(_innerCircleX, _innerCircleY, _innerCircleRadius, _innerCirclePaint);

            // Set matrix
            _pickerCanvasInfo.Canvas.SetMatrix(_matrix);
        }

        private void TouchEffect_TouchAction(object sender, TouchActionEventArgs args)
        {
            // Convert touch point to pixel point
            _pixelPoint = new SKPoint((float)(pickerCanvas.CanvasSize.Width * args.Location.X / pickerCanvas.Width)
                , (float)(pickerCanvas.CanvasSize.Height * args.Location.Y / pickerCanvas.Height));

            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    _matrix = SKMatrix.MakeIdentity();

                    // Get innrer circle rect
                    _innerCircleRect = new SKRect
                    (_innerCircleX - _innerCircleRadius, _innerCircleY - _innerCircleRadius
                        , _innerCircleX + _innerCircleRadius, _innerCircleY + _innerCircleRadius);

                    _innerCircleRect = _matrix.MapRect(_innerCircleRect);

                    // Check if the inner circle was touched
                    if (_innerCircleRect.Contains(_pixelPoint))
                    {
                        _touchId = args.Id;
                        _previousTouchPoint = _pixelPoint;
                    }
                    else
                    {
                        CalculateAngle(_pixelPoint);
                        pickerCanvas.InvalidateSurface();
                    }

                    break;

                case TouchActionType.Moved:
                    if (_touchId == args.Id)
                    {
                        // Calculate inner circle angle
                        CalculateAngle(_pixelPoint);

                        // Adjust the matrix for the new position
                        _matrix.TransX += _pixelPoint.X - _previousTouchPoint.X;
                        _matrix.TransY += _pixelPoint.Y - _previousTouchPoint.Y;
                        _previousTouchPoint = _pixelPoint;

                        pickerCanvas.InvalidateSurface();

                        ExecuteValueChangedCommand();
                    }

                    break;

                case TouchActionType.Released:
                case TouchActionType.Cancelled:
                    {
                        _touchId = -1;

                        ExecuteReleasedCommand();
                    }
                    break;
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            // Hue
            if (propertyName == HueProperty.PropertyName)
            {
                // Convert to radians
                _angle = FromDegreesRadians(Hue);
                pickerCanvas.InvalidateSurface();
            }
        }

        #endregion
    }
}

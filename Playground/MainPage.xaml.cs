using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace Playground
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            // Color changed command
            xfColorWheel.ValueChangedCommand = new Command<string>((color) =>
            {
                wheelColor.TextColor = Color.FromHex(color);
                wheelColor.Text = color.ToUpper();
            });

            // Released command
            xfColorWheel.ReleasedCommand = new Command<string>((color) =>
            {
                wheelColor.TextColor = Color.FromHex(color);
                wheelColor.Text = color.ToUpper();
            });
        }
    }
}

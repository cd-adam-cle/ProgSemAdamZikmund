using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Interactivity;
using System;
using System.Linq;
using Avalonia.Input;

namespace RGB_type; 

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        sldRed.ValueChanged += (s, e) => { txtRed.Text = ((int)sldRed.Value).ToString(); UpdateColor(); };
        sldGreen.ValueChanged += (s, e) => { txtGreen.Text = ((int)sldGreen.Value).ToString(); UpdateColor(); };
        sldBlue.ValueChanged += (s, e) => { txtBlue.Text = ((int)sldBlue.Value).ToString(); UpdateColor(); };

        txtRed.TextChanged += (s, e) => SyncTextToSlider(txtRed, sldRed);
        txtGreen.TextChanged += (s, e) => SyncTextToSlider(txtGreen, sldGreen);
        txtBlue.TextChanged += (s, e) => SyncTextToSlider(txtBlue, sldBlue);

        txtRed.AddHandler(InputElement.TextInputEvent, Integer, RoutingStrategies.Tunnel);
        txtGreen.AddHandler(InputElement.TextInputEvent, Integer, RoutingStrategies.Tunnel);
        txtBlue.AddHandler(InputElement.TextInputEvent, Integer, RoutingStrategies.Tunnel);

        UpdateColor();
    }

    private void Integer(object? sender, TextInputEventArgs e)
    {
        e.Handled = (e.Text != null && !e.Text.All(char.IsDigit));
    }

    private async void SyncTextToSlider(TextBox tb, Slider sld)
    {
        if (int.TryParse(tb.Text, out int val))
        {
            if (val > 255)
            {
                tb.Text = "255";
                val = 255;
            
                var box = new Window
                {
                    Title = "Chyba",
                    Width = 300, Height = 150,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Content = new TextBlock { 
                        Text = "Můžete zadávat jen hodnoty v rozmezí 0-255", 
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Avalonia.Thickness(20)
                    }
                };
                await box.ShowDialog(this);
            }
            sld.Value = val;
            UpdateColor();
        }
    }

    private void UpdateColor()
    {
        byte r = (byte)sldRed.Value;
        byte g = (byte)sldGreen.Value;
        byte b = (byte)sldBlue.Value;

        rectColor.Fill = new SolidColorBrush(Color.FromRgb(r, g, b));
        lblHex.Text = $"#{r:X2}{g:X2}{b:X2}";
    }
}
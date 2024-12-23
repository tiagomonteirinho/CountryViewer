using System.Windows;

namespace CountryViewer;

/// <summary>
/// Interaction logic for AboutWindow.xaml
/// </summary>
public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
    }

    private void button_return_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}

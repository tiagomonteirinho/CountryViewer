using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CountryViewer.Services;

namespace CountryViewer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private NetworkService _networkService;
    private ApiService _apiService;
    private DialogService _dialogService;
    private DataService _dataService;
    private List<Country> _countries;

    public Country SelectedCountry;

    public MainWindow()
    {
        InitializeComponent();

        _networkService = new NetworkService();
        _apiService = new ApiService();
        _dialogService = new DialogService();
        _dataService = new DataService();

        var progress = new Progress<int>(value => progressBar_load.Value = value);
        LoadCountriesAsync(progress);
    }

    private async void LoadCountriesAsync(IProgress<int> progress)
    {
        progressBar_load.Value = 0;

        var connection = _networkService.CheckConnection();
        if (connection.IsSuccessful)
        {
            await LoadApiCountriesAsync(progress);
            if (_countries != null)
            {
                foreach (var country in _countries)
                {
                    country.DefineGini();
                }

                _dataService.DeleteData();
                _dataService.SaveData(_countries);
                label_load.Content = $"{_countries.Count} countries updated successfully";
                textBox_search.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Unable to connect to the server. Looking for local data...");
                await LoadLocalCountriesAsync(progress);
                if (_countries != null && _countries.Count != 0)
                {
                    label_load.Content = $"{_countries.Count} countries loaded successfully";
                    textBox_search.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("Unable to connect to the server and no local data was found. Please check your connection and try again.");
                    Environment.Exit(0);
                }
            }
        }
        else
        {
            MessageBox.Show("Unable to connect to the Internet. Looking for local data...");
            await LoadLocalCountriesAsync(progress);
            if (_countries != null && _countries.Count != 0)
            {
                label_load.Content = $"{_countries.Count} countries loaded successfully";
                textBox_search.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Unable to connect to the Internet and no local data was found. Please connect to the Internet and try again.");
                Environment.Exit(0);
            }
        }

        listBox_countries.ItemsSource = _countries.OrderBy(c => c.Name.Common).ToList();
        listBox_countries.DisplayMemberPath = "Name.Common";
        progressBar_load.Value = 100;
    }

    private async Task LoadApiCountriesAsync(IProgress<int> progress)
    {
        var response = await _apiService.GetCountries("https://restcountries.com", "/v3.1/all");
        _countries = (List<Country>)response.Result;

        if (_countries != null)
        {
            int totalCountries = _countries.Count;
            for (int i = 0; i < totalCountries; i++)
            {
                await Task.Delay(1);

                progress.Report((i + 1) * 100 / totalCountries);
                label_load.Content = $"Loaded {i} countries of {_countries.Count}";
            }
        }
    }

    private async Task LoadLocalCountriesAsync(IProgress<int> progress)
    {
        _countries = _dataService.GetData();

        if (_countries != null)
        {
            int totalCountries = _countries.Count;
            for (int i = 0; i < totalCountries; i++)
            {
                await Task.Delay(1);

                progress.Report((i + 1) * 100 / totalCountries);
                label_load.Content = $"Loaded {i} countries of {_countries.Count}";
            }
        }
    }

    private void listBox_countries_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SelectedCountry = (Country)listBox_countries.SelectedItem;

        if (SelectedCountry != null)
        {
            textBox_officialName.Text = SelectedCountry.GetOfficialName();
            textBox_capitals.Text = SelectedCountry.GetCapitals();
            textBox_region.Text = SelectedCountry.GetRegion();
            textBox_subregion.Text = SelectedCountry.GetSubregion();
            textBox_population.Text = SelectedCountry.GetPopulation();
            textBox_gini.Text = SelectedCountry.GetGini();

            var connection = _networkService.CheckConnection();
            if (connection.IsSuccessful)
            {
                image_flag.Source = new BitmapImage(new Uri(SelectedCountry.GetFlag()));
            }
            else
            {
                image_flag.Source = new BitmapImage(new Uri("pack://application:,,,/CountryViewer;component/Models/unavailable-image.jpg"));
            }
        }
    }

    private void textBox_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_countries.Count > 0)
        {
            listBox_countries.ItemsSource = _countries.Where(c => c.Name.Common.Contains(textBox_search.Text)).OrderBy(c => c.Name.Common).ToList();
        }
    }

    private void button_about_Click(object sender, RoutedEventArgs e)
    {
        AboutWindow aboutWindow = new AboutWindow();
        aboutWindow.Show();
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        Environment.Exit(0);
    }
}
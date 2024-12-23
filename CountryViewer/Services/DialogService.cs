using System.Windows;

namespace CountryViewer.Services
{
    public class DialogService
    {
        public void ShowMessage(string message, string title)
        {
            MessageBox.Show(message, title);
        }
    }
}

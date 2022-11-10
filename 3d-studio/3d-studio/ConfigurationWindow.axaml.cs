using Avalonia.Controls;
using Avalonia.Interactivity;
using WifViewer.ViewModels;

namespace WifViewer
{
    public partial class ConfigurationWindow : Window
    {
        public ConfigurationWindow()
        {
            InitializeComponent();

            this.DataContext = new ConfigurationViewModel();
        }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            ((ConfigurationViewModel)DataContext).AcceptChanges();
            Close();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnBrowse(object sender, RoutedEventArgs e)
        {
            //var dlg = new OpenFileDialog()
            //{
            //    Filter = "Executables|*.exe",
            //    CheckFileExists = true
            //};
            //
            //if (dlg.ShowDialog() == true)
            //{
            //    ((ConfigurationViewModel)DataContext).RayTracerPath.Value = dlg.FileName;
            //}
        }
    }
}

using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;
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

        private async void OnBrowse(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog()
            {
                Filters = new List<FileDialogFilter> { new FileDialogFilter { Name = "Executables", Extensions = new List<string> { } } },
                AllowMultiple = false
            };

            var result = await fileDialog.ShowAsync(this);

            if (result != null)
            {
                foreach (var res in result)
                {
                    ((ConfigurationViewModel)DataContext).RayTracerPath.Value = res;
                }
            }
        }

        private async void OnBrowseFfmpeg(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog()
            {
                Filters = new List<FileDialogFilter> { new FileDialogFilter { Name = "Executables", Extensions = new List<string> { } } },
                AllowMultiple = false
            };

            var result = await fileDialog.ShowAsync(this);

            if (result != null)
            {
                foreach (var res in result)
                {
                    ((ConfigurationViewModel)DataContext).FfmpegPath.Value = res;
                }
            }
        }
    }
}

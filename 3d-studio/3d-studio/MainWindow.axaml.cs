using Avalonia.Controls;
using Avalonia.Threading;
using Newtonsoft.Json;
using System;
using System.IO;
using WifViewer.ViewModels;

namespace WifViewer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainViewModel();

            Dispatcher.UIThread.InvokeAsync(CheckForRaytracer, DispatcherPriority.Render);
        }

        private void CheckForRaytracer()
        {
            var file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "wifcfg.json");

            if (File.Exists(file))
            {
                var cfg = JsonConvert.DeserializeObject<WifCfg>(File.ReadAllText(file));
                Configuration.RAYTRACER_PATH = cfg.raytracer;
                Configuration.AUTO_SAVE = cfg.auto_save;
            }

            if (!File.Exists(Configuration.RAYTRACER_PATH))
            {
                var window = new ConfigurationWindow();
                window.ShowDialog(this);
            }
        }
    }
}

using Cells;
using Commands;
using System.IO;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace WifViewer.ViewModels
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            this.Documents = new ObservableCollection<DocumentViewModel>();
            this.CurrentDocument = Cell.Create<DocumentViewModel>(null);

            this.NewScriptCommand = EnabledCommand.FromDelegate(OnNewScript);
            this.LoadScriptCommand = EnabledCommand.FromDelegate(OnLoadScript);
            this.LoadWifCommand = EnabledCommand.FromDelegate(OnLoadWif);
            this.LoadCommand = EnabledCommand.FromDelegate(OnLoad);
            this.ConfigureCommand = EnabledCommand.FromDelegate(OnConfigure);
        }

        public ObservableCollection<DocumentViewModel> Documents { get; }

        public Cell<DocumentViewModel> CurrentDocument { get; }

        public ICommand LoadCommand { get; }

        public ICommand LoadScriptCommand { get; }

        public ICommand NewScriptCommand { get; }

        public ICommand LoadWifCommand { get; }

        public ICommand ConfigureCommand { get; }

        private async void OnLoadWif()
        {
            var fileDialog = new OpenFileDialog()
            {
                Filters = new List<FileDialogFilter> { new FileDialogFilter { Name = "Wif Files", Extensions = new List<string> { "wif" } } },
                AllowMultiple = true
            };

            var window = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow;
            var result = await fileDialog.ShowAsync(window);

            if (result != null)
            {
                foreach (var res in result)
                {
                    LoadWif(res);
                }
            }
        }

        private void LoadWif(string path)
        {
            var animationVM = new AnimationViewModel();
            WifLoader.LoadInSeparateThread(path, animationVM.CreateReceiver());

            var viewer = new AnimationWindow(animationVM);
            viewer.Show();
        }

        private void OnNewScript()
        {
            var document = new DocumentViewModel();

            AddDocument(document);
        }

        private async void OnLoadScript()
        {
            var fileDialog = new OpenFileDialog()
            {
                Filters = new List<FileDialogFilter> { new FileDialogFilter { Name = "Scripts", Extensions = new List<string> { "chai" } } },
                AllowMultiple = true
            };

            var window = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow;
            var result = await fileDialog.ShowAsync(window);

            if (result != null)
            {
                foreach (var res in result)
                {
                    LoadScript(res);
                }
            }
        }

        private void LoadScript(string path)
        {
            var source = File.ReadAllText(path);
            var document = new DocumentViewModel(source, path);

            AddDocument(document);
        }

        private async void OnLoad()
        {
            var fileDialog = new OpenFileDialog()
            {
                Filters = new List<FileDialogFilter> {
                    new FileDialogFilter { Name = "Supported Files", Extensions = new List<string> { "chai", "wif" } },
                    new FileDialogFilter { Name = "Scripts", Extensions = new List<string> { "chai" } },
                    new FileDialogFilter { Name = "Renderings", Extensions = new List<string> { "wif" } }
                },
                AllowMultiple = true
            };

            var window = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow;
            var result = await fileDialog.ShowAsync(window);

            if (result != null)
            {
                foreach (var res in result)
                {
                    if (res.ToLower().EndsWith(".wif"))
                    {
                        LoadWif(res);
                    }
                    else
                    {
                        LoadScript(res);
                    }
                }
            }
        }

        private void AddDocument(DocumentViewModel document)
        {
            this.Documents.Add(document);
            this.CurrentDocument.Value = document;
        }

        private void OnConfigure()
        {
            var window = new ConfigurationWindow();

            var w = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow;
            window.ShowDialog(w);
        }
    }
}

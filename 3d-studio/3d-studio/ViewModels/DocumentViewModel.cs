using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using AvaloniaEdit.Document;
using Cells;
using Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Input;
using WifViewer.Rendering;

namespace WifViewer.ViewModels
{
    public class DocumentViewModel
    {
        public DocumentViewModel() : this("", "untitled")
        {
            // NOP
        }

        public DocumentViewModel(string contents, string path)
        {
            this.Source = new TextDocument(contents);
            this.Path = Cell.Create(path);
            this.ShortPath = Cell.Derived(this.Path, DeriveShortPath);
            this.SaveScriptCommand = EnabledCommand.FromDelegate(OnSaveScript);
            this.SaveScriptAsCommand = EnabledCommand.FromDelegate(OnSaveScriptAs);
            this.RenderScript = EnabledCommand.FromDelegate(OnRenderScript);
            this.IsDirty = Cell.Create(false);
            this.Source.Changed += (s, e) => OnSourceChanged();
        }

        public TextDocument Source { get; }

        public Cell<string> Path { get; }

        public Cell<string> ShortPath { get; }

        public Cell<bool> IsDirty { get; }

        public string SourceString
        {
            get
            {
                return Source.Text;
            }
            set
            {
                this.Source.Text = value;
            }
        }

        public ICommand SaveScriptCommand { get; }

        public ICommand SaveScriptAsCommand { get; }

        public ICommand RenderScript { get; }

        private string DeriveShortPath(string path)
        {
            return System.IO.Path.GetFileName(path);
        }

        private void OnRenderScript()
        {
            if ( HasFilename() && Configuration.AUTO_SAVE )
            {
                Save();
            }

            var animMatch = Regex.Match(Source.Text, @"Pipeline\.animation\((.*?)\)");
            var animationVM = new AnimationViewModel(animMatch.Success ? int.Parse(animMatch.Groups[1].Value) : 30);
            var raytracer = new Renderer();
            var receiver = animationVM.CreateReceiver();

            try
            {
                raytracer.Render(this.Source.Text, receiver);

                var viewer = new AnimationWindow(animationVM);
                viewer.Show();
            }
            catch ( Exception e )
            {
                var box = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("Render", e.Message);
                box.Show();
            }
        }

        private bool HasFilename()
        {
            return this.Path.Value != "untitled";
        }

        private void Save()
        {
            File.WriteAllText(this.Path.Value, this.SourceString);
            this.IsDirty.Value = false;
        }

        private void OnSaveScript()
        {
            if (!HasFilename())
            {
                OnSaveScriptAs();
            }
            else
            {
                Save();
            }
        }

        private async void OnSaveScriptAs()
        {
            var fileDialog = new SaveFileDialog()
            {
                DefaultExtension = ".chai",
                Filters = new List<FileDialogFilter> { new FileDialogFilter { Name = "Scripts", Extensions = new List<string> { "chai" } } },
            };

            var window = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow;
            var result = await fileDialog.ShowAsync(window);
            
            if (result != null)
            {
                this.Path.Value = result;
                OnSaveScript();
            }
        }

        private void OnSourceChanged()
        {
            this.IsDirty.Value = true;
        }
    }
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using AvaloniaEdit.Document;
using Avalonia.Media.Imaging;
using Avalonia.Controls.ApplicationLifetimes;
using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Cells;
using Commands;
using WifViewer.Rendering;
using System.ComponentModel;

namespace WifViewer.ViewModels
{
    public class AnimationViewModel : INotifyPropertyChanged
    {
        private PropertyChangedEventHandler PropertyChanged = (obj, args) => { };

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                lock (PropertyChanged)
                {
                    PropertyChanged += value;
                }
            }
            remove
            {
                lock (PropertyChanged)
                {
                    PropertyChanged -= value;
                }
            }
        }

        public AnimationViewModel(int fps)
        {
            this.Timer = new DispatcherTimer(TimeSpan.FromMilliseconds(fps), DispatcherPriority.Background, (o, e) => OnTimerTick())
            {
                IsEnabled = false
            };

            this.AnimationSpeed = fps;
            this.TimerEnabled = Cell.Create(false);
            this.Frames = new ObservableCollection<WriteableBitmap>();
            this.CurrentFrameIndex = Cell.Create(0);
            this.CurrentFrame = Cell.Derived(this.CurrentFrameIndex, DeriveCurrentFrame);
            this.IsDoneRendering = Cell.Create(false);
            this.MaximumFrameIndex = Cell.Derived(() => this.Frames.Count - 1);
            this.Messages = new TextDocument();
            this.Frames.CollectionChanged += (sender, e) => OnFrameCollectionChanged();
            this.ToggleAnimation = EnabledCommand.FromDelegate(OnToggleAnimation);
            this.ScaleToFill = Cell.Create(true);
            this.ToggleScale = EnabledCommand.CreateTogglingCommand(ScaleToFill);
            this.FullScreen = Cell.Create(false);
            this.ToggleFullScreen = EnabledCommand.CreateTogglingCommand(FullScreen);
            this.ExportFrame = EnabledCommand.FromDelegate(OnExportFrame);
            this.ExportMovie = CellCommand.FromDelegate(this.IsDoneRendering, OnExportMovie);
            this.CopyFrame = EnabledCommand.FromDelegate(OnCopyFrame);
            this.PreviousFrame = EnabledCommand.FromDelegate(OnPreviousFrame);
            this.NextFrame = EnabledCommand.FromDelegate(OnNextFrame);

            this.TimerEnabled.ValueChanged += () => this.Timer.IsEnabled = this.TimerEnabled.Value;
        }

        private void OnToggleAnimation()
        {
            this.Timer.IsEnabled = !this.Timer.IsEnabled;
        }

        public ICommand PreviousFrame { get; }

        public ICommand NextFrame { get; }

        public ICommand ToggleAnimation { get; }

        public DispatcherTimer Timer { get; }

        public Cell<bool> TimerEnabled { get; }

        public int AnimationSpeed
        {
            get
            {
                return (int)(1000 / this.Timer.Interval.TotalMilliseconds);
            }
            set
            {
                this.Timer.Interval = TimeSpan.FromMilliseconds(1000.0 / value);
                PropertyChanged(this, new PropertyChangedEventArgs("AnimationSpeed"));
            }
        }

        private void OnTimerTick()
        {
            if (this.Frames.Count > 0)
            {
                this.CurrentFrameIndex.Value = (this.CurrentFrameIndex.Value + 1) % this.Frames.Count;
            }
        }

        private void OnFrameCollectionChanged()
        {
            this.CurrentFrame.Refresh();
            this.MaximumFrameIndex.Refresh();
        }

        private WriteableBitmap DeriveCurrentFrame(int index)
        {
            if (index >= this.Frames.Count)
            {
                return null;
            }
            else
            {
                return this.Frames[index];
            }
        }

        public ObservableCollection<WriteableBitmap> Frames { get; }

        private void FrameRendered(WriteableBitmap frame)
        {
            this.Frames.Add(frame);
        }

        private void LastFrameRendered()
        {
            this.IsDoneRendering.Value = true;
        }

        private void Message(string message)
        {
            this.Messages.Text += message + "\n";
        }

        public Cell<int> CurrentFrameIndex { get; }

        public Cell<WriteableBitmap> CurrentFrame { get; }

        public Cell<int> MaximumFrameIndex { get; }

        public TextDocument Messages { get; }

        public Cell<bool> IsAnimating { get; }

        public Cell<bool> ScaleToFill { get; }

        public ICommand ToggleScale { get; }

        public Cell<bool> FullScreen { get; }

        public ICommand ToggleFullScreen { get; }

        public Cell<bool> IsDoneRendering { get; }

        public ICommand ExportFrame { get; }

        public ICommand ExportMovie { get; }

        public ICommand CopyFrame { get; }

        public IRenderReceiver CreateReceiver()
        {
            return new RendererReceiver(this);
        }

        private void OnCopyFrame()
        {
            //Clipboard.SetImage(this.CurrentFrame.Value);
        }

        private async void OnExportFrame()
        {
            var fileDialog = new SaveFileDialog()
            {
                DefaultExtension = ".png",
                Filters = new List<FileDialogFilter> { 
                    new FileDialogFilter { Name = "Png Files", Extensions = new List<string> { "png" } },
                    new FileDialogFilter { Name = "Jpeg Files", Extensions = new List<string> { "jpeg" } },
                    new FileDialogFilter { Name = "Gif Files", Extensions = new List<string> { "gif" } },
                },
            };

            var window = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow;
            var result = await fileDialog.ShowAsync(window);

            if (result != null)
            {
                this.CurrentFrame.Value.Save(result);
            }
        }

        private void OnExportMovie()
        {
            //var saveDialog = new SaveFileDialog()
            //{
            //    Filter = "Gif Files|*.gif",
            //    AddExtension = true,
            //    OverwritePrompt = true,
            //    ValidateNames = true
            //};
            //
            //if (saveDialog.ShowDialog() == true)
            //{
            //    var frames = new List<WriteableBitmap>(this.Frames);
            //    var path = saveDialog.FileName;
            //
            //    ThreadPool.QueueUserWorkItem(_ =>
            //    {
            //        var encoder = new GifBitmapEncoder();
            //
            //        foreach (var frame in frames)
            //        {
            //            var bitmapFrame = BitmapFrame.Create(frame);
            //            encoder.Frames.Add(bitmapFrame);
            //        }
            //
            //        using (var file = File.OpenWrite(path))
            //        {
            //            encoder.Save(file);
            //        }
            //    });
            //}
        }

        private void OnPreviousFrame()
        {
            this.CurrentFrameIndex.Value = (this.CurrentFrameIndex.Value - 1 + this.Frames.Count) % this.Frames.Count;
        }

        private void OnNextFrame()
        {
            this.CurrentFrameIndex.Value = (this.CurrentFrameIndex.Value + 1) % this.Frames.Count;
        }

        private class RendererReceiver : IRenderReceiver
        {
            private readonly AnimationViewModel parent;

            public RendererReceiver(AnimationViewModel parent)
            {
                this.parent = parent;
            }

            public void FrameRendered(WriteableBitmap frame)
            {
                Action action = () => parent.FrameRendered(frame);
                Dispatcher.UIThread.InvokeAsync(action, DispatcherPriority.Render);
            }

            public void RenderingDone()
            {
                Action action = () => parent.LastFrameRendered();
                Dispatcher.UIThread.InvokeAsync(action, DispatcherPriority.Render);
            }

            public void Message(string message)
            {
                Action action = () => parent.Message(message);
                Dispatcher.UIThread.InvokeAsync(action, DispatcherPriority.Render);
            }
        }
    }
}

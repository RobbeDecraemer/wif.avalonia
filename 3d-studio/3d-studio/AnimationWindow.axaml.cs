using Avalonia.Controls;
using System;
using WifViewer.ViewModels;

namespace WifViewer
{
    public partial class AnimationWindow : Window
    {
        public AnimationWindow()
        {
            InitializeComponent();
        }

        public AnimationWindow(AnimationViewModel vm)
        {
            InitializeComponent();

            this.DataContext = vm;

            vm.FullScreen.ValueChanged += () => 
            {
                WindowState = vm.FullScreen.Value ? WindowState.FullScreen : WindowState.Normal;
            };

            vm.MaximumFrameIndex.ValueChanged += () =>
            {
                if (vm.MaximumFrameIndex.Value == 0)
                {
                    this.FindControl<TabItem>("resultTab").IsVisible = true;
                    this.FindControl<TabControl>("tabControl").SelectedIndex = 0;
                }
            };
        }

        protected override void OnClosed(EventArgs e)
        {
            this.DataContext = null;

            base.OnClosed(e);
        }
    }
}
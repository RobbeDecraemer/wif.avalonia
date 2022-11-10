using Avalonia.Media.Imaging;

namespace WifViewer.Rendering
{
    public interface IRenderReceiver
    {
        void FrameRendered(WriteableBitmap frame);

        void RenderingDone();

        void Message(string message);
    }
}

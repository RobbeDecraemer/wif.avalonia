using Avalonia.Media.Imaging;

namespace WifViewer.Rendering
{
    public class BitmapDecoder : IConsumer<byte[]>
    {
        private IConsumer<WriteableBitmap> next;

        public BitmapDecoder(IConsumer<WriteableBitmap> next)
        {
            this.next = next;
        }

        public void Consume(byte[] bytes)
        {
            var bitmap = WifLoader.DecodeFrame(bytes);

            if (bitmap != null)
            {
                next.Consume(bitmap);
            }
        }
    }
}

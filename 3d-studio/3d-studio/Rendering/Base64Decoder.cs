namespace WifViewer.Rendering
{
    public class Base64Decoder : IConsumer<string>
    {
        private IConsumer<byte[]> next;

        public Base64Decoder(IConsumer<byte[]> next)
        {
            this.next = next;
        }

        public void Consume(string str)
        {
            var bytes = System.Convert.FromBase64String(str);

            next.Consume(bytes);
        }
    }
}

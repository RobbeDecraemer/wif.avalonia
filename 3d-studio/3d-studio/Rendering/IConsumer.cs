namespace WifViewer.Rendering
{
    public interface IConsumer<T>
    {
        void Consume(T t);
    }
}

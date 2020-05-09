namespace KSGFK
{
    public interface IAsyncHandleWrapper
    {
        bool IsDone { get; }

        void OnComplete();
    }
}
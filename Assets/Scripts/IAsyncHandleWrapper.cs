namespace KSGFK
{
    /// <summary>
    /// 异步行为包装
    /// </summary>
    public interface IAsyncHandleWrapper
    {
        /// <summary>
        /// 是否完成
        /// </summary>
        bool IsDone { get; }

        /// <summary>
        /// 完成后回调
        /// </summary>
        void OnComplete();
    }
}
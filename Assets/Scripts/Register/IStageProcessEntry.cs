namespace KSGFK
{
    
    public interface IStageProcessEntry : IRegisterEntry
    {
        /// <summary>
        /// 预处理，一般用于发出加载资源请求
        /// </summary>
        void PerProcess();

        /// <summary>
        /// 注册前处理资源
        /// </summary>
        void Process();

        /// <summary>
        /// 检查是否可以注册
        /// </summary>
        /// <param name="info">若不可注册则返回原因</param>
        bool Check(out string info);
    }
}
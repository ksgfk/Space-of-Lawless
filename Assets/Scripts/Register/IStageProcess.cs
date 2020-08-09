using System.Threading.Tasks;

namespace KSGFK
{
    /// <summary>
    /// 可阶段性处理的注册项
    /// </summary>
    public interface IStageProcess
    {
        /// <summary>
        /// 预处理，一般用于发出加载资源请求,如果没有异步操作返回null
        /// </summary>
        Task PreProcess();

        /// <summary>
        /// 注册前处理资源
        /// </summary>
        void Process();
    }
}
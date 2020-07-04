namespace KSGFK
{
    public interface IRegisterEntry
    {
        /// <summary>
        /// 注册项唯一运行时Id,同一注册项的运行时Id相同
        /// </summary>
        int RuntimeId { get; set; }

        /// <summary>
        /// 注册项唯一名字
        /// </summary>
        string RegisterName { get; }
    }
}
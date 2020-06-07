namespace KSGFK
{
    public interface IJobCallback<T> where T : IJobWrapper
    {
        int DataId { get; set; }

        T Job { get; set; }
    }
}
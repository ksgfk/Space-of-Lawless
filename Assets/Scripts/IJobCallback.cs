namespace KSGFK
{
    public interface IJobCallback<T> where T : unmanaged
    {
        int DataId { get; set; }

        void JobUpdate(ref T data);

        JobTemplate<T> Job { get; set; }
    }
}
namespace KSGFK
{
    public interface IJobCallback<T> where T : unmanaged
    {
        int DataId { get; set; }

        void OnUpdate(ref T data);
    }
}
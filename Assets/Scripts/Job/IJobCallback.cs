namespace KSGFK
{
    public interface IJobCallback<T> where T : unmanaged
    {
        int DataId { get; set; }

        void JobUpdate(ref T data, ref ActionBuffer buffer);

        IJobWrapper Job { get; set; }
    }
}
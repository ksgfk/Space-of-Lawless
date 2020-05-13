namespace KSGFK
{
    public interface IJobCallback
    {
        int DataId { get; set; }
        
        IJobWrapper JobWrapper { get; set; }
        
        void OnUpdate();
    }
}
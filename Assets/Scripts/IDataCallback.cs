namespace KSGFK
{
    public interface IDataCallback
    {
        int DataId { get; set; }
        
        IJobWrapper JobWrapper { get; set; }
        
        void OnUpdate();
    }
}
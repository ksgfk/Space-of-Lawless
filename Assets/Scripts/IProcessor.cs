namespace KSGFK
{
    public interface IProcessor
    {
        void ProProcess(object target);

        void Process(object target);

        bool Check(object target, out string info);
    }
}
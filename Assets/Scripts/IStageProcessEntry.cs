namespace KSGFK
{
    public interface IStageProcessEntry : IRegisterEntry
    {
        void PerProcess();

        void Process();

        bool Check(out string info);
    }
}
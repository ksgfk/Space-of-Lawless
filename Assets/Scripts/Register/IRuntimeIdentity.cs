namespace KSGFK
{
    public interface IRuntimeIdentity
    {
        int RuntimeId { get; }

        void SetupId(int id);
    }
}
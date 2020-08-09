namespace KSGFK
{
    public interface IRuntimeIdentity
    {
        int RuntimeId { get; }

        void SetId(int id);
    }
}
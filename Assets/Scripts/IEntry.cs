namespace KSGFK
{
    public interface IEntry<T>
    {
        int Id { get; set; }

        string RegisterName { get; }
    }
}
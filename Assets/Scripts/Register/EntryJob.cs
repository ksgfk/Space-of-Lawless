namespace KSGFK
{
    public class EntryJob : IRegisterEntry
    {
        private int _runtimeId = int.MinValue;
        private string name = null;
        private string full_type_name = null;

        public int RuntimeId
        {
            get => _runtimeId;
            set => _runtimeId = Helper.SingleAssign(value, _runtimeId != int.MinValue);
        }

        public string RegisterName => name;
        public string FullTypeName => full_type_name;
    }
}
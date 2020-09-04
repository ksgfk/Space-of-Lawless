using System.IO;

namespace KSGFK
{
    public class StringReader : TextReader
    {
        private readonly string _str;
        private int _p;

        public StringReader(string str)
        {
            _str = str;
            _p = 0;
        }

        public override int Peek() { return _p >= _str.Length ? base.Peek() : _str[_p]; }

        public override int Read() { return _p >= _str.Length ? base.Read() : _str[_p++]; }
    }
}
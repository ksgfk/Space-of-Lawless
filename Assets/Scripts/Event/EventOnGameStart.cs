using System;

namespace KSGFK
{
    public class EventOnGameStart : EventArgs
    {
        public GameManager GM { get; }

        public EventOnGameStart(GameManager gm) { GM = gm; }
    }
}
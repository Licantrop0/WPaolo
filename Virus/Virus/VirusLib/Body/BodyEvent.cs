using System;

namespace VirusLib
{
    public class BodyEvent
    {
        public BodyEventCode Code { get; set; }
        public Object[] Params { get; set; }

        public BodyEvent(BodyEventCode code)
        {
            Code = code;
        }

        public BodyEvent(BodyEventCode code, Object[] p)
        {
            Code = code;
            Params = p;
        }
    }
}
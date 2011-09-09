using System;

namespace Harden
{
    public class HardenException : Exception
    {
        public HardenException(string message)
            : base(message)
        {
        }
    }
}
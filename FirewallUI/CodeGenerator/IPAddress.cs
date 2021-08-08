using System;

namespace CodeManager
{
    public class IPAddress
    {
        public IPAddress(byte p0, byte p1, byte p2, byte p3)
        {
            Part0 = p0;
            Part1 = p1;
            Part2 = p2;
            Part3 = p3;
        }
        public byte Part0 { get; private set; }

        public byte Part1 { get; private set; }

        public byte Part2 { get;private set; }

        public byte Part3 { get;private set; }

        public override string ToString()
        {
            return $"({Part0},{Part1},{Part2},{Part3});";
        }

        public string ToIPString()
        {
            return $"{Part0}.{Part1}.{Part2}.{Part3}";
        }
    }
}

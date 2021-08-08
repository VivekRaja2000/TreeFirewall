using System;

namespace CodeManager
{
    static internal class VariableGenerator
    {
        public static string GenerateVariable(Type type)
        {
            if(type == typeof(PacketRule))
            {
                Guid guid = Guid.NewGuid();
                string Guidstring = "";
                foreach (byte b in guid.ToByteArray())
                    Guidstring += b.ToString();
                return $"PRule{Guidstring}";
            }
            if(type == typeof(IPAddress))
            {
                Guid guid = Guid.NewGuid();
                string Guidstring = "";
                foreach(byte b in guid.ToByteArray())
                    Guidstring+=b.ToString();
                return $"Addr{Guidstring}";
            }
            return null;
        }
    }
}

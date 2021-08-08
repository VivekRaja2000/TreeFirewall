using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeManager
{
    internal class CodeGenerator
    {
        private static readonly string SetUpCode = "void setup(){IPAddressHere;PacketRulesHere;ArrayIPAddress;ArrayPacketRule;FirewallCreate;Serial.begin(9600);while(!Serial);WiFi.begin(\"WIFIID\", \"WIFIPASSWORD\");while(WiFi.status() != WL_CONNECTED){delay(100);}}";

        public static string FireWallCode(List<IPAddress> addresses, List<PacketRule> rules, string wifiName, string password)
        {
            StringBuilder stringBuilder= new StringBuilder();
            try
            {
                stringBuilder.Append(File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CodeTemplate.txt")));
                stringBuilder.Append(SetUpCode);
                List<string> addressVariables = GenerateVariables(addresses.Count, typeof(IPAddress));
                stringBuilder.Replace("IPAddressHere;", GenerateIPAddresses(addresses,addressVariables));
                List<string> rulesVariables = GenerateVariables(rules.Count, typeof(PacketRule));
                stringBuilder.Replace("PacketRulesHere;", Generaterules(rules, rulesVariables));
                stringBuilder.Replace("ArrayIPAddress;", GenerateArray(addressVariables, typeof(IPAddress)));
                stringBuilder.Replace("ArrayPacketRule;", GenerateArray(rulesVariables, typeof(PacketRule)));
                stringBuilder.Replace("FirewallCreate;", $"firewall = Firewall::CreateFirewall(addresses, rules, {addressVariables.Count});");
                stringBuilder.Replace("WIFIID", wifiName);
                stringBuilder.Replace("WIFIPASSWORD", password);

                return stringBuilder.ToString();
            }
            catch (Exception e)
            {
                return "NOCODE" + "\n" + e.ToString();
            }
        }

        private static string GenerateArray(List<string> variables, Type type)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if(type == typeof(IPAddress))
            {
                stringBuilder.Append("InternalIPAddress addresses[] = {");
            }
            else if(type == typeof(PacketRule))
            {
                stringBuilder.Append("PacketRule* rules[] = {");
            }
            for (int i = 0; i < variables.Count; i++)
            {
                if (i == variables.Count - 1)
                    stringBuilder.Append(variables[i]);
                else
                    stringBuilder.Append(variables[i] + ",");
            }
            stringBuilder.Append("};");
            return stringBuilder.ToString();
        }

        private static List<string> GenerateVariables(int count, Type type)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < count; i++)
                list.Add(VariableGenerator.GenerateVariable(type));
            return list;
        }

        private static string GenerateIPAddresses(List<IPAddress> addresses, List<string> variables)
        {
            StringBuilder builder = new StringBuilder();
            for(int i=0;i<variables.Count;i++)
            {
                builder.AppendLine($"InternalIPAddress {variables[i]} = InternalIPAddress{addresses[i].ToString()}");
            }
            return builder.ToString();  
        }

        private static string Generaterules(List<PacketRule> addresses, List<string> variables)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < variables.Count; i++)
            {
                builder.AppendLine($"PacketRule* {variables[i]} = new PacketRule{addresses[i].ToString()};");
            }
            return builder.ToString();
        }
    }
}

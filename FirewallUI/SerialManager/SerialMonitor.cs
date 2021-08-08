using System;
using System.IO.Ports;

namespace SerialManager
{
    public class SerialMonitor
    {
        public static string[] GetAllPorts()
        {
            return SerialPort.GetPortNames();
        }

        private string FindPort()
        {
            string[] ports = GetAllPorts();
            SerialPort port = new SerialPort()
            {
                BaudRate = 9600
            };
            foreach(string portName in ports)
            {
                port.PortName = portName;
                try
                {
                    port.Open();
                    port.WriteLine("0");
                    string read = port.ReadLine();
                    if (read.StartsWith("HELLO"))
                        return portName;
                }
                catch(Exception ex)
                {

                }
            }
            return "NOPORT";
        }

        public string Write
            

    }
}

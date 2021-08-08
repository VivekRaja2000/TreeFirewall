using System;
using System.IO.Ports;
using CodeManager;

namespace SerialManager
{
    public class SerialMonitor
    {
        public static string[] GetAllPorts()
        {
            return SerialPort.GetPortNames();
        }

        public SerialMonitor()
        {
            this.boardport = FindPort();
        }

        public string boardPort { get => FindPort(); }

        private string boardport;

        public string FindPort()
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
                    port.WriteTimeout = 2000;
                    port.ReadTimeout = 2000;
                    port.WriteLine("0");
                    string read = port.ReadLine();
                    if (read.StartsWith("HELLO"))
                    {
                        port.Close();
                        boardport = portName;
                        return portName;
                    }
                }
                catch(Exception)
                {
                    boardport = "NOPORT";
                    return "NOPORT";
                }
                finally
                {
                    port.Close();   
                }
            }
            boardport = "NOPORT";
            return "NOPORT";
        }

        public string CheckIncoming(IPAddress address)
        {
            string board = boardport;
            if (board == "NOPORT")
                return "NO BOARD";
            SerialPort port = new SerialPort(board);
            port.BaudRate = 9600;
            port.DiscardNull = true;
            try
            {
                port.Open();
                if (port.IsOpen)
                {
                    port.WriteLine("1");
                    port.WriteLine(address.ToIPString());
                    string result =  port.ReadLine();
                    port.Close();
                    while (port.IsOpen) ;
                    return result;
                }
                return "PORT ERROR";
            }
            catch(Exception e)
            {
                if (port.IsOpen)
                    port.Close();
                while (port.IsOpen) ;
                return "INTERNAL ERROR";
            }
            finally
            {
                if (port.IsOpen)
                    port.Close();
                while (port.IsOpen) ;
            }
        }

        public string CheckOutgoing(IPAddress address)
        {
            string board = boardport;
            if (board == "NOPORT")
                return "NO BOARD";
            SerialPort port = new SerialPort(board);
            try
            {
                port.Open();
                if (port.IsOpen)
                {
                    port.WriteLine("2");
                    port.WriteLine(address.ToIPString());
                    string result = port.ReadLine();
                    port.Close();
                    while (port.IsOpen) ;
                    return result;
                }
                return "PORT ERROR";
            }
            catch (Exception e)
            {
                if(port.IsOpen)
                    port.Close();
                while (port.IsOpen) ;
                return "INTERNAL ERROR";
            }
            finally
            {
                if (port.IsOpen)
                    port.Close();
                while (port.IsOpen) ;
            }
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.IO;
using System.Net;


namespace AgDroneCmd
{
    class Program
    {
        static void Main(string[] args)
        {
            bool quit = false;
            string command = "";

            System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
            clientSocket.Connect("192.168.2.6", 2003);
            //clientSocket.Connect("192.168.42.1", 2003);

            NetworkStream serverStream = clientSocket.GetStream();
            
            while (command != "quit")
            {
                command = Console.ReadLine();
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes(command + "\n");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
            }
        }
    }
}

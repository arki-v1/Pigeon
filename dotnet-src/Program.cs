using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Pigeon
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread dltracker = new Thread(new ThreadStart(dlproc));
            string address;
            int port;
            string faddress;
            string[] settings;
            byte[] fdata;
            if(args.Length == 0)
            {
                Console.WriteLine("NO ARGUMENTS GIVEN - USE help FOR HELP");
            }
            else if(args[0] == "help")
            {
                Console.WriteLine("PIGEON FILE TRANSFER VERSION 0");
                Console.WriteLine("------------------------------");
                Console.WriteLine("help =  show help box");
                Console.WriteLine("usage: pigeon help");
                Console.WriteLine("send = sends a file from your computer to another ip address");
                Console.WriteLine("usage: pigeon send [FILE ADDRESS] [PORT (overriding settings file)]");
                Console.WriteLine("get = gets a file from remote ip address");
                Console.WriteLine("usage: pigeon get [IP ADDRESS] [PORT] [NEW FILE ADDRESS]");
                Console.WriteLine("------------------------------");
            }
            else if(args[0] == "send")
            {
                if(args.Length < 4)
                {
                    try
                    {
                        settings = File.ReadAllLines("settings");
                        Console.WriteLine("SETTINGS FILE FOUND, APPLYING SETTINGS");
                        address = settings[0];
                        if(args.Length < 3)
                        {
                            port = int.Parse(settings[1]);
                        }
                        else
                        {
                            port = int.Parse(args[2]);
                        }
                    }
                    catch
                    {
                        settings = new string[2];
                        Console.WriteLine("FIRST TIME SETUP");
                        Console.WriteLine("Please enter the address ip:");
                        address = Console.ReadLine();
                        settings[0] = address;
                        if(args.Length < 3)
                        {
                            Console.WriteLine("Please enter the port ip:");
                            port = int.Parse(Console.ReadLine());
                            settings[1] = port.ToString();
                        }
                        else
                        {
                            port = int.Parse(args[2]);
                        }
                    }
                    faddress = args[1];
                    fdata = File.ReadAllBytes(@faddress);
                    send(IPAddress.Parse(address), port, fdata, dltracker);
                }
            }
            else if(args[0] == "get")
            {
                if(args.Length == 4)
                {
                    address = args[1];
                    port = int.Parse(args[2]);
                    faddress = args[3];
                    get(address, port, faddress, dltracker);
                }
                else
                {
                    Console.WriteLine("Argument Length not correct. Exiting.");
                    return;
                }
            }
        }
        static void send(IPAddress address, int port, byte[] fdata, Thread dltracker)
        {
            byte[] buffer;
            TcpClient client;
            TcpListener listener = new TcpListener(address, port);
            client = listener.AcceptTcpClient();
            NetworkStream nstream = client.GetStream();
            buffer = new byte[sizeof(long)];
            long fsize = fdata.Length;
            buffer = BitConverter.GetBytes(fsize);
            nstream.Write(buffer, 0, buffer.Length);
            buffer = new byte[client.SendBufferSize];
            for(int i = 0; i < fsize % client.ReceiveBufferSize; i++)
            {
                long startpoint = 0;
                if((fsize % client.ReceiveBufferSize) - i > 1)
                {
                    buffer = new byte[client.ReceiveBufferSize];
                    nstream.Write(buffer, 0, buffer.Length);
                    startpoint = i * client.ReceiveBufferSize;
                    for(long j = startpoint, x = 0; j < startpoint + client.ReceiveBufferSize; j++, x++ )
                    {
                        fdata[j] = buffer[x];
                    }
                }
                else
                {
                    buffer = new byte[fsize - (client.ReceiveBufferSize * i)];
                    nstream.Write(buffer, 0, buffer.Length);
                    startpoint = i * client.ReceiveBufferSize;
                    for(long j = startpoint, x = 0; j < fsize; j++, x++ )
                    {
                        fdata[j] = buffer[x];
                    }
                }
            }
            fdata = null;
            nstream.Close();
            client.Close();
            listener.Stop();
        }
        static void get(string address, int port, string faddress, Thread dltracker)
        {
            byte[] fdata;
            byte[] buffer;
            TcpClient client = new TcpClient(address, port);
            NetworkStream nstream = client.GetStream();
            buffer = new byte[sizeof(long)];
            long fsize;
            nstream.Read(buffer, 0, buffer.Length);
            fsize = BitConverter.ToInt64(buffer);
            fdata = new byte[fsize];
            buffer = null;
            for(int i = 0; i < fsize % client.ReceiveBufferSize; i++)
            {
                long startpoint = 0;
                if((fsize % client.ReceiveBufferSize) - i > 1)
                {
                    buffer = new byte[client.ReceiveBufferSize];
                    nstream.Read(buffer, 0, buffer.Length);
                    startpoint = i * client.ReceiveBufferSize;
                    for(long j = startpoint, x = 0; j < startpoint + client.ReceiveBufferSize; j++, x++ )
                    {
                        fdata[j] = buffer[x];
                    }
                }
                else
                {
                    buffer = new byte[fsize - (client.ReceiveBufferSize * i)];
                    nstream.Read(buffer, 0, buffer.Length);
                    startpoint = i * client.ReceiveBufferSize;
                    for(long j = startpoint, x = 0; j < fsize; j++, x++ )
                    {
                        fdata[j] = buffer[x];
                    }
                }
                buffer = null;
            }
            File.WriteAllBytes(@faddress, fdata);
            fdata = null;
            nstream.Close();
            client.Close();
        }
        static void dlproc()
        {

        }
    }
}

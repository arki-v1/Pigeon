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
            string address;
            int port;
            string faddress;
            byte[] fdata;
            string[] settings;
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
                        File.WriteAllLines("settings", settings);
                    }
                    faddress = args[1];
                    fdata = File.ReadAllBytes(@faddress);
                    send(IPAddress.Parse(address), port, fdata);
                }
            }
            else if(args[0] == "get")
            {
                if(args.Length == 4)
                {
                    address = args[1];
                    port = int.Parse(args[2]);
                    faddress = args[3];
                    get(address, port, faddress);
                }
                else
                {
                    Console.WriteLine("Argument Length not correct. Exiting.");
                    return;
                }
            }
        }
        static void send(IPAddress address, int port, byte[] fdata)
        {
            byte[] buffer;
            TcpClient client;
            TcpListener listener = new TcpListener(address, port);
            listener.Start();
            Console.WriteLine("Started listening for connections...");
            client = listener.AcceptTcpClient();
            NetworkStream nstream = client.GetStream();
            Console.WriteLine("Connected");
            buffer = new byte[sizeof(long)];
            long fsize = fdata.Length;
            buffer = BitConverter.GetBytes(fsize);
            nstream.Write(buffer, 0, buffer.Length);
            buffer = null;
            long data = 0;
            Console.WriteLine("Starting upload");
            int packets = 0;
            while(fsize > data)
            {
                Console.WriteLine("Uploading...");
                if(((fsize / client.ReceiveBufferSize) - packets) > 1)
                {
                    buffer = new byte[client.SendBufferSize];
                    for(int x = 0; x < buffer.Length; data++, x++ )
                    {
                        try
                        {
                            buffer[x] = fdata[data];
                        }
                        catch(IndexOutOfRangeException)
                        {
                            Console.WriteLine("Problem at {0}, {1}. Error 3. Buffer.length = {2}, fdata.Length = {3}", data, x, buffer.Length, fdata.Length);
                            break;
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e);
                            break;
                        }
                    }
                    nstream.Write(buffer, 0, buffer.Length);
                    Console.WriteLine("Data uploaded");
                }
                else
                {
                    buffer = new byte[fsize - data];
                    for(int x = 0; x < buffer.Length; data++, x++ )
                    {
                        try
                        {
                            buffer[x] = fdata[data];
                        }
                        catch(IndexOutOfRangeException)
                        {
                            Console.WriteLine("Problem at {0}, {1}. Error 4. Buffer.length = {2}, fdata.Length = {3}", data, x, buffer.Length, fdata.Length);
                            break;
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e);
                            break;
                        }
                    }
                    nstream.Write(buffer, 0, buffer.Length);
                }
                Console.WriteLine("{0}%, {1}/{2}", Math.Round(100 * (double)(data / fdata.Length)), data, fdata.Length);
                packets++;
            }
            fdata = null;
            nstream.Close();
            client.Close();
            listener.Stop();
        }
        static void get(string address, int port, string faddress)
        {
            byte[] buffer;
            Console.WriteLine("Waiting for connection...");
            TcpClient client = new TcpClient(address, port);
            NetworkStream nstream = client.GetStream();
            Console.WriteLine("Connected");
            buffer = new byte[sizeof(long)];
            long fsize;
            nstream.Read(buffer, 0, buffer.Length);
            fsize = BitConverter.ToInt64(buffer);
            byte[] fdata = new byte[fsize];
            Console.WriteLine(fdata.Length);
            buffer = null;
            long data = 0;
            Console.WriteLine("Starting download");
            int packets = 0;
            while(fsize > data)
            {
                Console.WriteLine("Downloading...");
                if(((fsize / client.ReceiveBufferSize) - packets) > 1)
                {
                    buffer = new byte[client.ReceiveBufferSize];
                    nstream.Read(buffer, 0, buffer.Length);
                    for(int x = 0; x < buffer.Length; data++, x++ )
                    {
                        try
                        {
                            fdata[data] = buffer[x];
                        }
                        catch(IndexOutOfRangeException)
                        {
                            Console.WriteLine("Problem at {0}, {1}. Error 1. Buffer.length = {2}, fdata.Length = {3}", data, x, buffer.Length, fdata.Length);
                            break;
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e);
                            break;
                        }
                    }
                }
                else
                {
                    buffer = new byte[fsize - data];
                    nstream.Read(buffer, 0, buffer.Length);
                    for(int x = 0; x < buffer.Length; data++, x++ )
                    {
                        try
                        {
                            fdata[data] = buffer[x];
                        }
                        catch(IndexOutOfRangeException)
                        {
                            Console.WriteLine("Problem at {0}, {1}. Error 2. Buffer.length = {2}, fdata.Length = {3}", data, x, buffer.Length, fdata.Length);
                            break;
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e);
                            break;
                        }
                    }
                }
                Console.WriteLine("{0}%, {1}/{2}", Math.Round(100 * (double)(data / fdata.Length)), data, fdata.Length);
                packets++;
            }
            File.WriteAllBytes(@faddress, fdata);
            fdata = null;
            nstream.Close();
            client.Close();
        }
    }
}

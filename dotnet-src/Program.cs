using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Pigeon
{
    class Program
    {
        static void Main(string[] args)
        {
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
        static void send(IPAddress address, int port, byte[] data)
        {
            byte[] buffer;

        }
        static void get(string address, int port, string faddress)
        {

        }
    }
}

using System;
using System.IO;
using System.Text;

namespace Pigeon
{
    class Program
    {
        static void Main(string[] args)
        {
            string gateway;
            int port;
            string[] settings;
            if(args.Length == 0)
            {
                Console.WriteLine("NO ARGUMENTS GIVEN - USE -h FOR HELP");
            }
            else if(args [0] == "-h")
            {
                Console.WriteLine("PIGEON FILE TRANSFER VERSION 0");
                Console.WriteLine("------------------------------");
                Console.WriteLine("-h = help");
                Console.WriteLine("-s = Send");
                Console.WriteLine("-r = receive");
                Console.WriteLine("------------------------------");
            }
            else if(args[0] == "-s")
            {
                try
                {
                    settings = File.ReadAllLines("settings");
                    Console.WriteLine("SETTINGS FILE FOUND, APPLYING SETTINGS");
                    gateway = settings[0];
                    port = int.Parse(settings[1]);
                }
                catch
                {
                    settings = new string[2];
                    Console.WriteLine("FIRST TIME SETUP");
                    Console.WriteLine("Please enter the gateway ip:");
                    gateway = Console.ReadLine();
                    settings[0] = gateway;
                    Console.WriteLine("Please enter the port ip:");
                    port = int.Parse(Console.ReadLine());
                    settings[1] = port.ToString();
                }
            }
        }
    }
}

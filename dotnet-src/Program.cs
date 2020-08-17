﻿using System;
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
            else if(args [0] == "help")
            {
                Console.WriteLine("PIGEON FILE TRANSFER VERSION 0");
                Console.WriteLine("------------------------------");
                Console.WriteLine("help =  show help box");
                Console.WriteLine("send = sends a file from your computer to another ip address");
                Console.WriteLine("get = gets a file from remote ip address");
                Console.WriteLine("------------------------------");
            }
            else if(args[0] == "send")
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
            else if(args[0] == "get")
            {

            }
        }
    }
}

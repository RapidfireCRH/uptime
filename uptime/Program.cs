using System;
using System.Collections.Generic;
using System.Threading;

namespace uptime
{
    class Program
    {
        
        static void Main(string[] args)
        {
            List<pingtest> addr = new List<pingtest>();
            foreach(string x in args)
            {
                try
                {
                    addr.Add(new pingtest(x));
                }
                catch(System.Net.Sockets.SocketException Ex)
                {
                    Console.WriteLine(x + ": " + Ex.Message);
                    Thread.Sleep(1000);
                }
                catch(Exception e)
                {
                    throw e;
                }
            }
            while (true)
            {
                foreach(pingtest a in addr)
                {
                    a.test();
                    Console.Write(a.toString());
                }
                Thread.Sleep(1000);
                Console.Clear();
            }
        }
    }

}
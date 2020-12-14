using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
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
                    Console.WriteLine(a.toString());
                }
                Thread.Sleep(1000);
                Console.Clear();
            }
        }

        public static bool PingHost(string nameOrAddress)
        {
            using (Ping pinger = new Ping())
            {
                try
                {
                    PingReply reply = pinger.Send(nameOrAddress);
                    return reply.Status == IPStatus.Success;
                }
                catch(Exception e)
                {
                    return false;
                }
            }
        }
    }

    class pingtest
    {
        public IPAddress ip;
        public string hostname;

        public struct pingresult_struct
        {
            public DateTime timestamp;
            public bool result;
        }

        public List<pingresult_struct> results;
        public pingresult_struct lastresult;

        public pingtest(IPAddress ip_in)
        {
            ip = ip_in;
            hostname = ip_in.ToString();
            results = new List<pingresult_struct>();
        }

        public pingtest(string hostname_in)
        {
            ip = Dns.GetHostAddresses(hostname_in)[0];
            hostname = hostname_in;
            results = new List<pingresult_struct>();
        }

        public void test()
        {
            lastresult.result = PingHost(ip.ToString());
            lastresult.timestamp = DateTime.Now;
            results.Add(lastresult);
        }

        public string toString()
        {
            string ret = hostname + "(" + ip.ToString() + ")" + Environment.NewLine;
            if (results.Count < 5)
            {
                ret += "Last " + results.Count + " results: " + Environment.NewLine;
                foreach(pingresult_struct res in results)
                {
                    ret += (res.result == true ? "UP " : "Down ") + res.timestamp.ToLongTimeString() + Environment.NewLine;
                }
            }
            else
            {
                ret += "Last 5 results: " + Environment.NewLine +
                (results[results.Count - 5].result == true ? "UP " : "Down ") + results[results.Count - 5].timestamp.ToLongTimeString() + Environment.NewLine +
                (results[results.Count - 4].result == true ? "UP " : "Down ") + results[results.Count - 4].timestamp.ToLongTimeString() + Environment.NewLine +
                (results[results.Count - 3].result == true ? "UP " : "Down ") + results[results.Count - 3].timestamp.ToLongTimeString() + Environment.NewLine +
                (results[results.Count - 2].result == true ? "UP " : "Down ") + results[results.Count - 2].timestamp.ToLongTimeString() + Environment.NewLine +
                (results[results.Count - 1].result == true ? "UP " : "Down ") + results[results.Count - 1].timestamp.ToLongTimeString() + Environment.NewLine;
            }
            return ret;
        }

        private bool PingHost(string nameOrAddress)
        {
            using (Ping pinger = new Ping())
            {
                try
                {
                    PingReply reply = pinger.Send(nameOrAddress);
                    return reply.Status == IPStatus.Success;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }
    }

}
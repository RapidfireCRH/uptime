using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace uptime
{
    class pingtest
    {

        //Variables
        public IPAddress ip;
        public string hostname;
        public List<pingresult_struct> results;
        public pingresult_struct lastresult;

        //Structs
        public struct pingresult_struct
        {
            public DateTime timestamp;
            public bool result;
        }

        //Constructors
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

        /// <summary>
        /// Test - runs test and puts result into lastresult
        /// </summary>
        public void test()
        {
            lastresult.result = PingHost(ip.ToString());
            lastresult.timestamp = DateTime.Now;
            results.Add(lastresult);
        }

        /// <summary>
        /// toString - prints results of last ping test
        /// </summary>
        /// <param name="detail"> expands detail to include last 5 pings </param>
        /// <returns>console output to dispaly to screen</returns>
        public string toString(bool detail = false)
        {
            string ret = hostname + "(" + ip.ToString() + ") " + (lastresult.result == true ? "UP as of " : "Down as of ") + lastresult.timestamp.ToLongTimeString() + Environment.NewLine;
            if (detail)
            {
                if (results.Count < 5)
                {
                    ret += "Last " + results.Count + " results: " + Environment.NewLine;
                    foreach (pingresult_struct res in results)
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
            }
            return ret;
        }

        /// <summary>
        /// Pings target and returns result
        /// </summary>
        /// <param name="nameOrAddress">ip or hostname to ping</param>
        /// <returns>result of ping</returns>
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

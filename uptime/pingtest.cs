using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using Newtonsoft.Json;

namespace uptime
{
    class pingtest
    {

        //Variables
        public IPAddress ip;
        public string hostname;
        public List<pingresult_struct> log;
        public pingresult_struct lastresult;

        //Structs
        public struct pingresult_struct
        {
            public DateTime timestamp;
            public bool result;
        }

        //used to seralize pingtest into json
        private struct serializable_obj
        {
            public string ip;
            public string hostname;
            public pingresult_struct[] log;
        }

        //Constructors
        public pingtest(IPAddress ip_in)
        {
            ip = ip_in;
            hostname = ip_in.ToString();
            log = new List<pingresult_struct>();
        }

        public pingtest(string hostname_in)
        {
            ip = Dns.GetHostAddresses(hostname_in)[0];
            hostname = hostname_in;
            log = new List<pingresult_struct>();
        }

        /// <summary>
        /// Test - runs test and puts result into lastresult
        /// </summary>
        public void test()
        {
            bool before = lastresult.result;
            lastresult.result = PingHost(ip.ToString());
            lastresult.timestamp = DateTime.Now;

            //if results list is empty, add first entry
            if (log.Count == 0)
                log.Add(lastresult);
            else if (log[log.Count-1].result != lastresult.result && before == lastresult.result)
                log.Add(lastresult);
        }

        /// <summary>
        /// toString - prints results of last ping test
        /// </summary>
        /// <param name="detail"> expands detail to include last 5 pings </param>
        /// <returns>console output to dispaly to screen</returns>
        public string toString(bool detail = false)
        {
            string ret;
            if (hostname == ip.ToString())
                ret = hostname + " " + (lastresult.result == true ? "UP as of " : "Down as of ") + lastresult.timestamp.ToLongTimeString() + Environment.NewLine;
            else
                ret = hostname + "(" + ip.ToString() + ") " + (lastresult.result == true ? "UP as of " : "Down as of ") + lastresult.timestamp.ToLongTimeString() + Environment.NewLine;
            if (detail)
            {
                if (log.Count < 5)
                {
                    ret += "Last " + log.Count + " log entries: " + Environment.NewLine;
                    foreach (pingresult_struct res in log)
                    {
                        ret += (res.result == true ? "UP " : "Down ") + res.timestamp.ToLongTimeString() + Environment.NewLine;
                    }
                }
                else
                {
                    ret += "Last 5 log entries: " + Environment.NewLine +
                    (log[log.Count - 5].result == true ? "UP " : "Down ") + log[log.Count - 5].timestamp.ToLongTimeString() + Environment.NewLine +
                    (log[log.Count - 4].result == true ? "UP " : "Down ") + log[log.Count - 4].timestamp.ToLongTimeString() + Environment.NewLine +
                    (log[log.Count - 3].result == true ? "UP " : "Down ") + log[log.Count - 3].timestamp.ToLongTimeString() + Environment.NewLine +
                    (log[log.Count - 2].result == true ? "UP " : "Down ") + log[log.Count - 2].timestamp.ToLongTimeString() + Environment.NewLine +
                    (log[log.Count - 1].result == true ? "UP " : "Down ") + log[log.Count - 1].timestamp.ToLongTimeString() + Environment.NewLine;
                }
            }
            return ret;
        }

        /// <summary>
        /// toJson - returns data as a json string
        /// </summary>
        /// <returns>data in a json string</returns>
        public string toJson()
        {
            serializable_obj serobj = new serializable_obj();
            serobj.ip = ip.ToString();
            serobj.hostname = hostname;
            serobj.log = new pingresult_struct[log.Count];
            for (int i = 0; i < log.Count; i++)
                serobj.log[i] = log[i];
            string ret = JsonConvert.SerializeObject(serobj);
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
                catch (Exception _)
                {
                    return false;
                }
            }
        }
    }
}

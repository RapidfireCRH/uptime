using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;

namespace uptime
{
    class Program
    {
        
        static void Main(string[] args)
        {
            List<pingtest> addr = new List<pingtest>();
            string doc_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            foreach (string x in args)
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
                if (DateTime.Now.Minute == 0)
                {
                    string[] str = new string[addr.Count];
                    int i = 0;

                    foreach (pingtest a in addr)
                        str[i++] = a.toJson();
                    if (File.Exists(Path.Combine(doc_path, DateTime.Now.ToString("yyyy_MM_dd") + ".txt")))
                        File.Delete(Path.Combine(doc_path, DateTime.Now.ToString("yyyy_MM_dd") + ".txt"));
                    using (StreamWriter file = new StreamWriter(Path.Combine(doc_path, DateTime.Now.ToString("yyyy_MM_dd") + ".txt")))
                    {
                        file.Write("[");
                        for (int j = 0; j < str.Length - 1; j++)
                            file.Write(str[j] + ", ");
                        file.Write(str[str.Length-1] + "]");
                    }
                    if(DateTime.Now.Hour == 0)
                    {
                        foreach (pingtest a in addr)
                            a.clear();
                    }
                }
                foreach(pingtest a in addr)
                {
                    a.test();
                    //Console.WriteLine(a.toString(true));
                }
                Thread.Sleep(1000);
                //Console.Clear();
            }
        }
    }

}
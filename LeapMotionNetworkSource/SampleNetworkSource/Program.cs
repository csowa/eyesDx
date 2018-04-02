using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Configuration;

using Leap;

namespace SampleNetworkSource
{
    class Program
    {
        static void Main(string[] args)
        {

            var MAAPSIP = ConfigurationSettings.AppSettings.Get("MAAPSIP");
            var MAAPSPort = int.Parse(ConfigurationSettings.AppSettings.Get("MAAPSPort"));

            var controller = new Controller();

            // setup UDP client connection.
            var client = new UdpClient();
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(MAAPSIP), MAAPSPort);
            client.Connect(ep);

            int counter = 0;
            // adapter started, set 
            Console.WriteLine("Sending data to network adapter. Press 'ESC' key to quit.");
            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
            {
                var vector = controller.Frame().Translation(controller.Frame(1));

                //  get UTC time as Windows Filetime.
                //  http://www.silisoftware.com/tools/date.php
                //  var timestamp = DateTime.UtcNow.AddSeconds(-15).ToFileTimeUtc();  // Use this to demonstrate Skew in RM
                var timestamp = DateTime.UtcNow.ToFileTimeUtc(); 
                string msg = timestamp.ToString();

                msg += "," + counter;

                // Leap Motion (x,y,z)
                msg += "," + vector.x.ToString();
                msg += "," + vector.y.ToString();
                msg += "," + vector.z.ToString();
                  
                // send the message out to the network adapter
                byte[] toBytes = Encoding.ASCII.GetBytes(msg);
                client.Send(toBytes, toBytes.Length);

                // just a pause to hit a frequency ( 50Hz ->  20 ms,   10Hz -> 100ms )
                //System.Threading.Thread.Sleep(100);
                System.Threading.Thread.Sleep(20);
                counter += 1;
            }
            Console.WriteLine("Quitting.");
        }
    }
}

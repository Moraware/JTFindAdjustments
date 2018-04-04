using Moraware.JobTrackerAPI4;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JTFindAdjustments
{
    class Program
    {
        static void Main(string[] args)
        {
            // Needed Parameters for each run (to avoid saving pwds in code)
            String DB;
            String UID;
            String PWD;

            // try running once with low = 1 and high = 300. If you find 300, then try with 300 and 600, etc.

            int nLowestAdjustment; 
            int nHighestAdjustment;
            
            // if no parameters, then ask for them
            // if 5 parameters, then assume they are DB, UID, PWD, nLowestAdjustment, and nHighestAdjustment

            if (args.Count() == 3)
            {
                DB = args[0];
                UID = args[1];
                PWD = args[2];
                nLowestAdjustment = int.Parse(args[3]);
                nHighestAdjustment = int.Parse(args[4]);
            }
            else if (args.Count() == 0)
            {
                Console.WriteLine("This is JTFindAdjustments, a utility for finding serial number adjustments in JobTracker");
                Console.Write("Database (where http://x.moraware.net, provide x): ");
                DB = Console.ReadLine();
                Console.Write("User name: ");
                UID = Console.ReadLine();
                Console.Write("Password: ");
                PWD = Console.ReadLine();
                Console.Write("Lowest adjustment id to look for (integer only, no commas): ");
                nLowestAdjustment = int.Parse(Console.ReadLine());
                Console.Write("Highest adjustment id to look for (integer only, no commas): ");
                nHighestAdjustment = int.Parse(Console.ReadLine());
            }
            else
            {
                Console.WriteLine("Usage: JTFindAdjustments database user password");
                Console.WriteLine("... or omit parameters to be prompted");
                return;
            }

            var JTURL = "https://" + DB + ".moraware.net/";
            var OUTPUTFILE = DB + "-JTFindAdjustments.html";

            Connection conn = new Connection(JTURL + "api.aspx", UID, PWD);
            conn.Connect();

            StreamWriter w = File.AppendText(OUTPUTFILE);
            w.AutoFlush = true;
            w.WriteLine("*********************<br />");
            w.WriteLine("Start {0}<br />", DateTime.Now);

            int nHighestFound = 0;
            for (int i = nLowestAdjustment; i < nHighestAdjustment; i++)
            {
                var snia = conn.GetSerialNumberInventoryAdjustment(i);
                if (snia != null)
                {
                    nHighestFound = i;
                    Console.WriteLine("Found adjustment for serial number {0}", snia.SerialNumber.SerialNumberName);
                    w.WriteLine("Adjustment found for serial number <a href='{0}d.aspx?wp=37&snId={1}'>{2}</a><br />", JTURL, snia.SerialNumber.SerialNumberId, snia.SerialNumber.SerialNumberName);
                }
                else
                {
                    Console.Write(".");
                }
            }
            w.WriteLine("Highest adjustment found: {0}", nHighestFound);
            conn.Disconnect();
            w.WriteLine("*********************<br />");
            w.WriteLine("Finish {0}<br />", DateTime.Now);
            w.WriteLine("*********************<br />");
            w.Close();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}


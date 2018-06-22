//Using a language of your choice, process the sample webserver log snippet below to:
//download the full sample webserver log from https://s3.amazonaws.com/dev.cmm/NASA_access_log_sample and:
//Determine which URL was most frequently requested on July 1st, 1995
//Determine which URL was most frequently requested on each other day represented in the file
//Format the output to have the date and the URL on a line and order them chronologically
//Jessica Groot
//06/21/2018

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TextProcessing
{
    class Program
    {
        public static List<Request> requests = new List<Request>();
        public static List<Exception> errors = new List<Exception>();
        public const string strContinue = "Press any key to continue";
        static void Main(string[] args)
        {
            try
            {
                ReadFile();
                FindMostFrequentURL();
                FindJuly01995();
                FindOtherDays();               
            }
            catch (Exception e)
            {
                LogException(e);
          
            }
            finally
            {
                if (errors.Count > 0)
                {
                    Console.WriteLine("The below errors were found:");
                    foreach (Exception error in errors)
                        Console.WriteLine(error.ToString());

                }
                Console.WriteLine("Press any key to exit...");
                Console.ReadLine();
            }

        }

        public static void ReadFile()
        {
            try
            {
                using (StreamReader file = new StreamReader("C:\\NASA_access_log_sample"))
                {
                    string line = string.Empty;
                    while ((line = file.ReadLine()) != null)
                        ProcessLine(line);
                }
            }
            catch (FileNotFoundException e) { LogException(e); }
        }

        public static void ProcessLine(string line)
        {
            try
            {
                var lineArray = line.Split(" ");
                int urlIndex = Array.FindIndex(lineArray, r => new Regex(@"(\/\w*)").Match(r).Success);
                int dateIndex = Array.FindIndex(lineArray, r => new Regex(@"(\[\d{4}\-\d{2}\-\d{2}[T]\d{2}\:\d{2}\:\d{2}[Z]\])").Match(r).Success);
                var currentUrl = lineArray[urlIndex];
                //no time zone in file
                var currentDate = Convert.ToDateTime(lineArray[dateIndex].Trim('[').Trim(']').Trim('Z'));
                
                Request request = new Request
                {
                    URL = currentUrl,
                    Date = currentDate
                };
                requests.Add(request);
            }
            catch (Exception e) { LogException(e); }
        }

        public static void FindMostFrequentURL()
        {
            try
            {
                var highest = requests.GroupBy(
                    r => r.URL,
                    r => r.Date,
                    (key, d) => new { URL = key, Dates = d.ToList() })
                    .OrderByDescending(g => g.Dates.Count)
                    .FirstOrDefault();

                Console.WriteLine("The most frequently requested url was {0} for a total of {1} requests.", highest.URL, highest.Dates.Count.ToString());
                Console.WriteLine(strContinue);
                Console.ReadLine();
            }
            catch (Exception e) { LogException(e); }
        }

        public static void FindJuly01995()
        {
            try
            {
                var july = requests.GroupBy(
                    r => r.URL,
                    r => r.Date,
                    (key, d) => new { URL = key, Dates = d.Where(e => e.Date.Equals(DateTime.Parse("07-01-1995"))).ToList() })
                    .OrderByDescending(c => c.Dates.Count)
                    .FirstOrDefault();

                Console.WriteLine("The most frequent request on July 1, 1995 was {0} for a total of {1} requests.", july.URL, july.Dates.Count.ToString());
                Console.WriteLine(strContinue);
                Console.ReadLine();
            }
            catch (Exception e) { LogException(e); }
        }

        public static void FindOtherDays()
        {
            try
            {
                var alldates = requests.Where(d => d.Date.Date != DateTime.Parse("07-01-1995").Date)
                   .Select(d => d.Date.Date)
                   .OrderBy(d => d.Date)
                   .Distinct()
                   .ToList();

                Console.WriteLine("The most frequent requests on all other days are listed as followed:");
                foreach (DateTime day in alldates)
                {

                    var request = requests.GroupBy(
                    r => r.URL,
                    r => r.Date,
                    (key, d) => new { URL = key, Dates = d.Where(e => e.Date.Equals(day)).ToList() })
                    .OrderByDescending(c => c.Dates.Count)
                    .FirstOrDefault();

                    Console.WriteLine("{0} - {1}", day.ToShortDateString(), request.URL);
                }


                Console.WriteLine(strContinue);
                Console.ReadLine();
            }
            catch (Exception e) { LogException(e); }
          
        }

        public static void LogException(Exception e)
        {
            errors.Add(e);
        }
       
        public class Request
        {
            public string URL { get; set; }
            public DateTime Date { get; set; }
        }

    }
}


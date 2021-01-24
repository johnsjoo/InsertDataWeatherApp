using InsertDataWeatherApp.Entities;
using InsertDataWeatherApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace InsertDataWeatherApp
{
    class Program
    {
        static string filePath = @"D:\It-Högskolan\Blazor\Inlämningsuppgift\InsertDataWeatherApp\InsertDataWeatherApp\File\TemperaturData.csv";
        static void Main(string[] args)
        {
            Console.WriteLine("Reading file..");
            //ReadCSVFile(filePath);
            bool exit = true;
            while (exit)
            {
                Console.WriteLine("Press [Q] for Weather and Humiditylist");
                Console.WriteLine("Press [W] for Metrological seasons");
                Console.WriteLine("Press [E] for Metrological seasons");
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.Q:
                        foreach (var item in FromWarmToCold("Ute"))
                        {
                            Console.WriteLine(item);
                        }
                        Console.WriteLine("***************************");
                        foreach (var item in HumidMethod("Ute"))
                        {
                            Console.WriteLine(item);
                        }
                        break;
                    case ConsoleKey.W:
                        //MetrologicalSeasons();
                        break;
                    case ConsoleKey.E:
                        exit = false;
                        break;
                }

            }

        }
        public static List<string> HumidMethod(string location)
        {
            using (var context = new EfContext())
            {
                List<string> avgHumidity = new List<string>();

                var resultSet = context.WeatherDataInfo
                    .Where(x => x.Location == $"{location}")
                    .GroupBy(x => x.DateAndTime.Date)
                    .Select(x => new { DateAndTime = x.Key, Avg = x.Average(x => x.Humidity) })
                    .OrderByDescending(x => x.Avg);

                var top = resultSet
                    .Take(10);
                var bottom = resultSet
                    .OrderBy(x => x.Avg)
                    .Take(10)
                    .OrderByDescending(x => x.Avg);


                foreach (var entry in top)
                {
                    avgHumidity.Add($"{entry.DateAndTime:yyyy/MM/dd} | {Math.Round(entry.Avg, 1)} %");
                }
                avgHumidity.Add("-----------");
                foreach (var entry in bottom)
                {
                    avgHumidity.Add($"{entry.DateAndTime:yyyy/MM/dd} | {Math.Round(entry.Avg, 1)} %");
                }
                return avgHumidity;
            }
        }//top 10 varmaste, och top 10 kallaste dagarna, spannet mellan bortfiltrerat.


        public static List<string> FromWarmToCold(string location)
        {
            using (var context = new EfContext())
            {
                List<string> avgTemperatures = new List<string>();

                var resultSet = context.WeatherDataInfo
                    .Where(x => x.Location == $"{location}")
                    .GroupBy(x => x.DateAndTime.Date)
                    .Select(x => new { DateAndTime = x.Key, Avg = x.Average(x => x.Temp) })
                    .OrderByDescending(x => x.Avg);

                var top = resultSet
                    .Take(10);
                var bottom = resultSet
                    .OrderBy(x => x.Avg)
                    .Take(10)
                    .OrderByDescending(x => x.Avg);



                foreach (var entry in top)
                {
                    avgTemperatures.Add($"{entry.DateAndTime:yyyy/MM/dd} | {Math.Round(entry.Avg, 1)} °C");
                }
                avgTemperatures.Add("-----------");
                foreach (var entry in bottom)
                {
                    avgTemperatures.Add($"{entry.DateAndTime:yyyy/MM/dd} | {Math.Round(entry.Avg, 1)} °C");
                }
                return avgTemperatures;
            }
        }//top 10 varmaste, och top 10 kallaste dagarna, spannet mellan bortfiltrerat.


       
       
        public static void ReadCSVFile(string filePath)
        { 
            string[] resultSet = File.ReadAllLines(filePath);
            using (var context = new EfContext())
            {
                foreach (var data in resultSet)
                {
                    WeatherData tmpInfo = new WeatherData();
                    string[] tmpString = data.Split(",");
                    tmpInfo.DateAndTime = DateTime.Parse(tmpString[0]);
                    tmpInfo.Location = tmpString[1];
                    tmpInfo.Temp = decimal.Parse(tmpString[2], CultureInfo.InvariantCulture);
                    tmpInfo.Humidity = int.Parse(tmpString[3], CultureInfo.InvariantCulture);

                    context.Add(tmpInfo);
                }
                context.SaveChanges();
            }
        }
    }
}

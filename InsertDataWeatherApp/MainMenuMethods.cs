using InsertDataWeatherApp.Entities;
using InsertDataWeatherApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace InsertDataWeatherApp
{
    public class MainMenuMethods
    {
        static bool ok = false;
        static string filePath = @"D:\It-Högskolan\Blazor\Inlämningsuppgift\InsertDataWeatherApp\InsertDataWeatherApp\File\TemperaturData.csv";
        //ReadCSVFile(filePath);
        public static void Run()
        {
           
            bool exit = true;
            while (exit)
            {   
                Console.WriteLine("Tryck [S] för att söka efter datum");
                Console.WriteLine("Tryck [T] för Temperaturelist");
                Console.WriteLine("Tryck [H] för Humiditylist");
                Console.WriteLine("Tryck [W] för Metrologisk årstid (vinter)");
                Console.WriteLine("Tryck [F] för Metrologisk årstid (Höst)");
                Console.WriteLine("Tryck [M] för mögelindex");
                Console.WriteLine("Tryck [E] för att avsluta program");

                ConsoleKeyInfo key = Console.ReadKey(true);            
                switch (key.Key)
                {
                    case ConsoleKey.S:
                        Console.Clear();
                        Console.WriteLine("Skriv in datum: yyyy/MM/dd");
                        DateTime date = Convert.ToDateTime(Console.ReadLine());
                        while (ok)
                        {
                            
                            try
                            {
                                foreach (var item in SearchDateMethod(date))
                                {
                                    Console.WriteLine(item);
                                }
                                Console.WriteLine("Tryck på valfri knapp för att fortsätta...");
                                Console.ReadLine();
                                Console.Clear();
                                ok = false;
                            }
                            catch
                            {
                                Console.Clear();
                                Console.WriteLine("Skriv in ett giltigt datum");
                            }


                        }
                        break;
                    case ConsoleKey.T:
                        Console.Clear();
                        Console.Write("skriv in 'Ute' eller 'Inne': ");
                        string weatherInput = Console.ReadLine();
                        foreach (var item in FromWarmToCold(weatherInput))
                        {
                            Console.WriteLine(item);
                        }
                        Console.WriteLine("Tryck på valfri knapp för att fortsätta...");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                    case ConsoleKey.H:
                        Console.Clear();
                        Console.Write("skriv in 'Ute' eller 'Inne': ");
                        string humidInput = Console.ReadLine();
                        Console.Clear();
                        foreach (var item in HumidMethod(humidInput))
                        {
                            Console.WriteLine(item);
                        }
                        Console.WriteLine("Tryck på valfri knapp för att fortsätta...");
                        Console.ReadLine();
                        Console.Clear();
                        break;

                    case ConsoleKey.W:
                        Console.Clear();
                        Console.WriteLine("Datum för metrologisk vinter: " + MetrologicalSeasonWinter());
                        Console.WriteLine("Tryck på valfri knapp för att fortsätta...");
                        Console.ReadLine();
                        Console.Clear();
                        break;

                    case ConsoleKey.F:
                        Console.Clear();
                        for (int i = 0; i < MetrologicalSeasonFall().Count; i++)
                        {
                            if (i == 1)
                            {
                                Console.WriteLine($"Den metrologiska hösten började: {MetrologicalSeasonFall().First()} °C");
                                break;
                            }
                        }
                        Console.WriteLine("Tryck på valfri knapp för att fortsätta...");
                        Console.ReadLine();
                        Console.Clear();
                        break;

                    case ConsoleKey.M:
                        Console.Clear();
                        Console.Write("skriv in 'Ute' eller 'Inne': ");
                        string input = Console.ReadLine();
                        foreach (var item in Moldcheck(input))
                        {
                            Console.WriteLine(item);
                        }
                        Console.WriteLine("Tryck på valfri knapp för att fortsätta...");
                        Console.ReadLine();
                        Console.Clear();
                        break;

                    case ConsoleKey.E:
                        exit = false;
                        break;
                }
            }
        }

        private static List<string> SearchDateMethod(DateTime date)
        {
            using (var context = new EfContext())
            {
                List<string> avgTemperatureSpecDate = new List<string>();

               
                var outSide = context.WeatherDataInfo
                    .Where(x => x.Location == "Ute" && x.DateAndTime.Date == date)
                    .GroupBy(x => x.DateAndTime.Date)
                    .Select(x => new { DateAndTime = x.Key, Avg = x.Average(x => x.Temp) });
                var inSide = context.WeatherDataInfo
                    .Where(x => x.Location == "Inne" && x.DateAndTime.Date == date)
                    .GroupBy(x => x.DateAndTime.Date)
                    .Select(x => new { DateAndTime = x.Key, Avg = x.Average(x => x.Temp) });

                foreach (var item in outSide)
                {
                    avgTemperatureSpecDate.Add($"MedelTemperatur Ute: {item.Avg}");
                }
                avgTemperatureSpecDate.Add("--------");
                foreach (var item in inSide)
                {
                    avgTemperatureSpecDate.Add($"MedelTemperatur Inne: {item.Avg}");
                }
                return avgTemperatureSpecDate;
            }
        }

        private static List<string> MetrologicalSeasonFall()
        {
            List<string> dates = new List<string>();
            using (var context = new EfContext())
            {

                var q1 = context.WeatherDataInfo
                    .Where(x => x.Location == "Ute" && x.DateAndTime > DateTime.Parse("2016-08-01"))
                    .GroupBy(x => x.DateAndTime.Date)
                    .Select(x => new { day = x.Key, tempAVG = x.Average(x => x.Temp) });

                var q2 = q1
                    .OrderBy(x => x.day);

                int turnCounter = 0;

                foreach (var wd in q2)
                {
                    if (wd.tempAVG < 10)
                    {
                        turnCounter++;
                        //Console.WriteLine(wd.day + " " + wd.tempAVG);
                        dates.Add($"{wd.day:yyyy/MM/dd} | {Math.Round(wd.tempAVG, 1)}");
                        if (turnCounter == 5)
                        {
                            return dates;
                        }
                    }
                    else
                    {
                        dates.Clear();
                        turnCounter = 0; //The count starts over.
                    }
                }
                return dates;
            }
        }

        private static List<string> Moldcheck(string input)
        {
            List<string> moldList = new List<string>();
            using (var context = new EfContext())
            {
                var q1 = context.WeatherDataInfo
                    .Where(x => x.Location == $"{input}")
                    .GroupBy(x => x.DateAndTime.Date)
                    .Select(x => new { DateAndTime = x.Key, HumAvg = x.Average(x => x.Humidity), TempAvg = x.Average(x => x.Temp) })
                    .OrderBy(x => ((x.HumAvg - 78)) * (double)(x.TempAvg / 15) / 0.22);

                var topTen = q1
                    .Take(10);

                var bottomTen = q1
                    .OrderByDescending(x => ((x.HumAvg - 78)) * (double)(x.TempAvg / 15) / 0.22)
                    .Take(10)
                    .OrderBy(x => ((x.HumAvg - 78)) * (double)(x.TempAvg / 15) / 0.22);


                foreach (var mold in topTen)
                {
                    if (Math.Round((mold.HumAvg - 78) * (double)(mold.TempAvg / 15) / 0.22) <= 0)
                    {
                        moldList.Add($"{mold.DateAndTime: yyyy/MM/dd} \t {Math.Round((mold.HumAvg - 78) * (double)(mold.TempAvg / 15) / 0.22)}% Noll risk för mögel ");
                    }
                    else
                    {
                        moldList.Add($"{mold.DateAndTime: yyyy/MM/dd} \t {Math.Round((mold.HumAvg - 78) * (double)(mold.TempAvg / 15) / 0.22)}% Noll risk för mögel ");
                        //moldList.Add($"{mold.DateAndTime} {mold.HumAvg} Noll risk för mögel");
                    }
                }

                moldList.Add("------------------------------------------");
                foreach (var mold in bottomTen)
                {
                    if (Math.Round((mold.HumAvg - 78) * (double)(mold.TempAvg / 15) / 0.22) > 0)
                    {
                        moldList.Add($"{mold.DateAndTime: yyyy/MM/dd} \t {Math.Round((mold.HumAvg - 78) * (double)(mold.TempAvg / 15) / 0.22)}%");
                    }
                    else
                    {
                        moldList.Add($"{mold.DateAndTime: yyyy/MM/dd} \t {Math.Round((mold.HumAvg - 78) * (double)(mold.TempAvg / 15) / 0.22)}%");
                    }
                    
                }
                return moldList;

            }

        }


        private static DateTime MetrologicalSeasonWinter()
        {
            using (var context = new EfContext())
            {
                var q1 = context.WeatherDataInfo
                    .Where(x => x.Location == "Ute" && x.DateAndTime > DateTime.Parse("2016-08-01"))
                    .GroupBy(x => x.DateAndTime.Date)
                    .Select(x => new { day = x.Key, tempAVG = x.Average(x => x.Temp) });

                var q2 = q1
                    .OrderBy(x => x.day);



                int turnCounter = 0;

                foreach (var wd in q2)
                {
                    if (wd.tempAVG <= 0)
                    {
                        turnCounter++;

                        if (turnCounter == 5)
                        {
                            Console.WriteLine(wd.day + " " + wd.tempAVG);
                            return wd.day;
                        }
                    }
                    else
                    {
                        turnCounter = 0; //The count starts over.
                    }
                }
                return DateTime.Parse("2099-12-12"); //Data for winter season not found.
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
                    .Select(x => new { DateAndTime = x.Key, AVGHumid = x.Average(x => x.Humidity) })
                    .OrderByDescending(x => x.AVGHumid);

                var top = resultSet
                    .Take(10);
                var bottom = resultSet
                    .OrderBy(x => x.AVGHumid)
                    .Take(10)
                    .OrderByDescending(x => x.AVGHumid);


                foreach (var entry in top)
                {
                    avgHumidity.Add($"{entry.DateAndTime:yyyy/MM/dd} \t {Math.Round(entry.AVGHumid, 1)} %");
                }
                avgHumidity.Add("--------------");
                foreach (var entry in bottom)
                {
                    avgHumidity.Add($"{entry.DateAndTime:yyyy/MM/dd} \t {Math.Round(entry.AVGHumid, 1)} %");
                }
                return avgHumidity;
            }

        }


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
                    avgTemperatures.Add($"{entry.DateAndTime:yyyy/MM/dd} \t {Math.Round(entry.Avg, 1)} °C");
                }
                avgTemperatures.Add("-----------");

                foreach (var entry in bottom)
                {
                    avgTemperatures.Add($"{entry.DateAndTime:yyyy/MM/dd} \t {Math.Round(entry.Avg, 1)} °C");
                }
                return avgTemperatures;
            }
        }


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


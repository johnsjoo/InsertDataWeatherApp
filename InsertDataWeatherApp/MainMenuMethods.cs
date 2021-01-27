using InsertDataWeatherApp.Entities;
using InsertDataWeatherApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace InsertDataWeatherApp
{
    public class MainMenuMethods
    {
        static string filePath = @"D:\It-Högskolan\Blazor\Inlämningsuppgift\InsertDataWeatherApp\InsertDataWeatherApp\File\TemperaturData.csv";
        static bool exit = true;

        //ReadCSVFile(filePath);
        public static void Run()
        {
            while (exit)
            {
                MainMenu();
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.S:
                        Console.Clear();
                        bool dateLoop = true;
                        while (dateLoop)
                        {
                            Console.WriteLine("Skriv in datum: yyyy-MM-dd");
                            try
                            {
                                DateTime date = Convert.ToDateTime(Console.ReadLine());
                                Console.Clear();
                                foreach (var item in SearchDateMethod(date))
                                {
                                    Console.WriteLine(item);
                                }

                                ContinueMethod();
                                dateLoop = false;
                            }
                            catch
                            {
                                Console.Clear();
                                Console.WriteLine("Skriv in ett giltigt datum");
                            }
                        } 
                        break;

                    case ConsoleKey.T:
                        bool tempLoop = true;
                        Console.Clear();
                        while (tempLoop)
                        {
                            Console.WriteLine("Tryck [1] För utomhustemperatur");
                            Console.WriteLine("Tryck [2] För Innomhustemperatur");
                            try
                            {
                                string weatherInput = "";
                                int numberInput = int.Parse(Console.ReadLine());
                                Console.Clear();
                                if (numberInput == 1)
                                    weatherInput = "Ute";
                                else if (numberInput == 2)
                                    weatherInput = "Inne";

                                foreach (var item in FromWarmToCold(weatherInput))
                                {
                                    Console.WriteLine(item);
                                }
                                ContinueMethod();
                                tempLoop = false;
                            }
                            catch
                            {
                                Console.Clear();
                                Console.WriteLine("Skriv in ett giltigt värde");
                            }
                        }
                        break;

                    case ConsoleKey.H:
                        bool humidLoop = true;
                        Console.Clear();
                        while (humidLoop)
                        {
                            Console.WriteLine("Tryck [1] för fuktighet utomhus");
                            Console.WriteLine("Tryck [2] för fuktighet inomhus");
                            try
                            {
                                string humidInput = "";
                                int numberInput = int.Parse(Console.ReadLine());

                                if (numberInput == 1)
                                    humidInput = "Ute";

                                else if (numberInput == 2)
                                    humidInput = "Inne";

                                Console.Clear();
                                foreach (var item in HumidMethod(humidInput))
                                {
                                    Console.WriteLine(item);
                                }
                                ContinueMethod();
                                humidLoop = false;
                            }
                            catch
                            {
                                Console.Clear();
                                Console.WriteLine("Skriv in ett giltigt värde");

                            }
                        }
                        break;

                    case ConsoleKey.W:
                        Console.Clear();
                        MetrologicalSeasonWinter();
                        ContinueMethod();
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
                        ContinueMethod();
                        break;

                    case ConsoleKey.M:
                        bool moldLoop = true;
                        Console.Clear();
                        while (moldLoop)
                        {
                            Console.WriteLine("Tryck [1] För mögelrisk utomhus");
                            Console.WriteLine("Tryck [2] För mögelrisk inomhus");
                            try
                            {
                                string moldInput = "";
                                int numberInput = int.Parse(Console.ReadLine());
                                if (numberInput == 1)
                                    moldInput = "Ute";
                                else if (numberInput == 2)
                                    moldInput = "Inne";

                                Console.Clear();
                                foreach (var item in Moldcheck(moldInput))
                                {
                                    Console.WriteLine(item);
                                }
                                ContinueMethod();
                                moldLoop = false;
                            }
                            catch
                            {
                                Console.Clear();
                                Console.WriteLine("Skriv in ett giltigt värde");
                            }
                        }
                        break;

                    case ConsoleKey.E:
                        exit = false;
                        break; 
                }
                Console.Clear();
            }
        }

        private static List<string> SearchDateMethod(DateTime date)
        {
            using (var context = new EfContext())
            {
                List<string> dates = new List<string>();

                //Sortera på de poster som stämmer med både stränginmatningen och datuminmatningen. 
                var outSide = context.WeatherDataInfo
                    .Where(x => x.Location == "Ute" && x.DateAndTime.Date == date)
                    .GroupBy(x => x.DateAndTime.Date)
                    .Select(x => new { DateAndTime = x.Key, TempAvg = x.Average(x => x.Temp) });

                var inSide = context.WeatherDataInfo
                    .Where(x => x.Location == "Inne" && x.DateAndTime.Date == date)
                    .GroupBy(x => x.DateAndTime.Date)
                    .Select(x => new { DateAndTime = x.Key, TempAvg = x.Average(x => x.Temp) });

                foreach (var item in outSide)
                {
                    dates.Add($"MedelTemperatur Ute var: {Math.Round(item.TempAvg, 1)}°C den {item.DateAndTime.Date}");
                }
                foreach (var item in inSide)
                { 
                    dates.Add($"MedelTemperatur Inne var: {Math.Round(item.TempAvg, 1)}°C den {item.DateAndTime.Date}");
                }
                //Felhantering där data från valt datum saknas.
                if (dates.Count()==0)
                {
                    dates.Add("Ingen data för detta datum");
                }
                return dates;

            }
        }
        private static List<string> MetrologicalSeasonFall()
        {
            List<string> dates = new List<string>();
            using (var context = new EfContext())
            {
                //
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
                        dates.Add($"{wd.day:yyyy/MM/dd} \t {Math.Round(wd.tempAVG, 1)}");
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
        private static List<string> Moldcheck(string location)
        {
            List<string> moldList = new List<string>();
            using (var context = new EfContext())
            {
                var q1 = context.WeatherDataInfo
                    .Where(x => x.Location == $"{location}")
                    .GroupBy(x => x.DateAndTime.Date)
                    .Select(x => new { DateAndTime = x.Key, HumAvg = x.Average(x => x.Humidity), TempAvg = x.Average(x => x.Temp) })//Plocka ut medeltemp och medelfuktighet.
                    .OrderBy(x => ((x.HumAvg - 78)) * (double)(x.TempAvg / 15) / 0.22);//Sortera i ordningen minst till störst efter formels resultat.

                var topTen = q1
                    .Take(10); // Ta endast ut de tio först tio. 

                var bottomTen = q1
                    .OrderByDescending(x => ((x.HumAvg - 78)) * (double)(x.TempAvg / 15) / 0.22)//Sortera efter procentsatsen du får ut
                    .Take(10)
                    .OrderBy(x => ((x.HumAvg - 78)) * (double)(x.TempAvg / 15) / 0.22);//sortera i fallande årning efter resultatet från formeln.

                InsideorOutSide(location);
                moldList.Add("Lägst risk för mögel");
                moldList.Add("******************************************");
                foreach (var mold in topTen)
                {
                    //lägg till rätt typ av sträng i listan. 
                    if (Math.Round((mold.HumAvg - 78) * (double)(mold.TempAvg / 15) / 0.22) <= 0)
                    {
                        moldList.Add($"{mold.DateAndTime: yyyy/MM/dd} \t {Math.Round((mold.HumAvg - 78) * (double)(mold.TempAvg / 15) / 0.22)}% Noll risk för mögel");
                    }
                }
                moldList.Add("\n");
                moldList.Add("Störst risk för mögel(om ej negativt)");
                moldList.Add("******************************************");
                foreach (var mold in bottomTen)
                {
                    //kollar även där det ska vara högst risk för mögel om det finns minusvärden även där.
                    if (Math.Round((mold.HumAvg - 78) * (double)(mold.TempAvg / 15) / 0.22) <= 0)
                    {
                        moldList.Add($"{mold.DateAndTime: yyyy/MM/dd} \t {Math.Round((mold.HumAvg - 78) * (double)(mold.TempAvg / 15) / 0.22)}% Noll risk för mögel");
                    }
                    //Om risken för mögel är över 0%
                    else if (Math.Round((mold.HumAvg - 78) * (double)(mold.TempAvg / 15) / 0.22) > 0)
                    {
                        moldList.Add($"{mold.DateAndTime: yyyy/MM/dd} \t {Math.Round((mold.HumAvg - 78) * (double)(mold.TempAvg / 15) / 0.22)}% Risk för mögel");
                    }
                }
                return moldList;
            }
        }
        private static bool MetrologicalSeasonWinter()
        {
            using (var context = new EfContext())
            {
                var q1 = context.WeatherDataInfo
                    .Where(x => x.Location == "Ute" && x.DateAndTime > DateTime.Parse("2016-08-01"))
                    .GroupBy(x => x.DateAndTime.Date)
                    .Select(x => new { day = x.Key, TempAvg = x.Average(x => x.Temp) })
                    .OrderBy(x => x.day);

                int turnCounter = 0;
                foreach (var wd in q1)
                {
                    if (wd.TempAvg <= 0)
                    {
                        turnCounter++;
                        //lika med true så returnerar vi true och skriver ut en sträng med aktuellt datum.
                        if (turnCounter == 5)
                        {
                            Console.WriteLine(wd.day + " " + wd.TempAvg);
                            return true;
                        }   
                    }
                    else
                    {
                        //Räkningen startar om.
                        turnCounter = 0;
                    }    
                }
                //går vi ur loopen och turnCounter inte gått upp till 5 fanns det inga datum som uppfyllde vilkåren.
                //returneras false hittades inget datum.
                if (turnCounter==0)
                {
                    Console.WriteLine("No data found");
                    
                }
                return false;
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
                    .Select(x => new { DateAndTime = x.Key, AVGHumid = x.Average(x => x.Humidity) });
                    

                var topResult = resultSet
                    .OrderByDescending(x => x.AVGHumid)
                    .Take(10);
                var bottomResult = resultSet
                    .OrderBy(x => x.AVGHumid)//För att få de nedersta raderna.
                    .Take(10)
                    .OrderByDescending(x => x.AVGHumid);//För att få de nedersta raderna i fallande ordning.

                InsideorOutSide(location);
                avgHumidity.Add("Tio dagarna med högst fuktighet");
                avgHumidity.Add("*******************************");
                foreach (var result in topResult)
                {
                    avgHumidity.Add($"{result.DateAndTime:yyyy/MM/dd} \t {Math.Round(result.AVGHumid, 1)} %");
                }
                avgHumidity.Add("\n");
                avgHumidity.Add("Tio dagarna med lägst fuktighet");
                avgHumidity.Add("*******************************");

                foreach (var result in bottomResult)
                {
                    avgHumidity.Add($"{result.DateAndTime:yyyy/MM/dd} \t {Math.Round(result.AVGHumid,1)} %");
                }
                return avgHumidity;
            }

        }
        public static List<string> FromWarmToCold(string location)
        {
            using (var context = new EfContext())
            {
                List<string> avgTemperatures = new List<string>();

                //väljer ut de poster beroende av vilken sträng input vi får (inne eller ute)
                //Sen grupperar vi det på datum-fältet. 
                var resultSet = context.WeatherDataInfo
                    .Where(x => x.Location == $"{location}")
                    .GroupBy(x => x.DateAndTime.Date)
                    .Select(x => new { DateAndTime = x.Key, AvgTemp = x.Average(x => x.Temp) }) //Hämtar ut medeltemp för de grupperande datumen.
                    .OrderByDescending(x => x.AvgTemp);

                var topResult = resultSet
                    .Take(10);
                var bottomResult = resultSet
                    .OrderBy(x => x.AvgTemp)
                    .Take(10)
                    .OrderByDescending(x => x.AvgTemp);

                InsideorOutSide(location);
                avgTemperatures.Add("Tio dagarna med högst temp");
                avgTemperatures.Add("**************************");
                foreach (var result in topResult)
                {
                    avgTemperatures.Add($"{result.DateAndTime:yyyy/MM/dd} \t {Math.Round(result.AvgTemp, 1)} °C");//skriver in värdena till strängar i listan.
                }
                avgTemperatures.Add("\n");
                avgTemperatures.Add("Tio dagarna med lägst temp");
                avgTemperatures.Add("**************************");
                foreach (var result in bottomResult)
                {
                    avgTemperatures.Add($"{result.DateAndTime:yyyy/MM/dd} \t {Math.Round(result.AvgTemp, 1)} °C");
                }
                return avgTemperatures;
            }
        }


        public static void ReadCSVFile(string filePath)
        {
            //Samlar alla rader i filen till en sträng-array.
            string[] resultSet = File.ReadAllLines(filePath);
            using (var context = new EfContext())
            {
                //loopar alla poster i tabellen, tolkar datatyperna och sätter värdet på alla fällt för alla poster.
                foreach (var data in resultSet)
                {
                    WeatherData wdInfo = new WeatherData();
                    string[] wdString = data.Split(",");
                    wdInfo.DateAndTime = DateTime.Parse(wdString[0]);
                    wdInfo.Location = wdString[1];
                    wdInfo.Temp = decimal.Parse(wdString[2], CultureInfo.InvariantCulture);
                    wdInfo.Humidity = int.Parse(wdString[3], CultureInfo.InvariantCulture);

                    //lägg till posterna till tabelln i databasen.
                    context.Add(wdInfo);
                }
                //sparar ändringar i databasen.
                context.SaveChanges();
            }
        }
        private static void ContinueMethod()
        {
            Console.WriteLine("Tryck på valfri knapp för att fortsätta...");
            Console.ReadLine();
            Console.Clear();
        }
        private static void InsideorOutSide(string location)
        {
            if (location == "Ute")
            {
                Console.WriteLine("[Utomhus]");
            }
            else
            {
                Console.WriteLine("[Inomhus]");
            }
        }
        private static void MainMenu()
        {
            Console.WriteLine($"John Sjöö .NET2020 {DateTime.Now}");
            Console.Write("\n");
            Console.WriteLine("********************************");
            Console.WriteLine("*       Väderapplikation       *");
            Console.WriteLine("********************************");
            Console.Write("\n");
            Console.WriteLine("------------MainMenu-------------");
            Console.WriteLine("Tryck [S] för att söka efter datum");
            Console.WriteLine("Tryck [T] för Temperaturlista");
            Console.WriteLine("Tryck [H] för Luftfuktighetslista");
            Console.WriteLine("Tryck [W] för Metrologisk årstid (vinter)");
            Console.WriteLine("Tryck [F] för Metrologisk årstid (Höst)");
            Console.WriteLine("Tryck [M] för Mögelindex");
            Console.WriteLine("Tryck [E] för att avsluta program");
        }
    }
}


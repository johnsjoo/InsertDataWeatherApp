using System;
using System.Collections.Generic;
using System.Text;

namespace InsertDataWeatherApp.Models.Entities
{
    public class WeatherData
    {
        public int Id { get; set; }
        public DateTime DateAndTime { get; set; }
        public string Location { get; set; }
        public decimal Temp { get; set; }
        public int Humidity { get; set; }

        //private static DateTime MetrologicalSeasonFall()
        //{

        //    using (var context = new EfContext())
        //    {

        //        var q1 = context.WeatherDataInfo
        //            .Where(x => x.Location == "Ute" && x.DateAndTime > DateTime.Parse("2016-08-01"))
        //            .GroupBy(x => x.DateAndTime.Date)
        //            .Select(x => new { day = x.Key, tempAVG = x.Average(x => x.Temp) });

        //        var q2 = q1
        //            .OrderBy(x => x.day);

        //        int turnCounter = 0;

        //        foreach (var wd in q2)
        //        {
        //            if (wd.tempAVG < 10)
        //            {
        //                turnCounter++;
        //                //Console.WriteLine(wd.day + " " + wd.tempAVG);
        //                if (turnCounter == 5) 
        //                {
        //                    return wd.day;
        //                }
        //            }
        //            else
        //            {
        //                turnCounter = 0; //The count starts over.
        //            }
        //        }
        //        return DateTime.Parse("2089 12 12"); //Data for fall season not found.
        //    }
        //}

    }
}

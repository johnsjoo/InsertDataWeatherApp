using InsertDataWeatherApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace InsertDataWeatherApp.Entities
{
    class EfContext : DbContext
    {
        string connectionString = string.Empty;

        public EfContext() : base()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false);
            var configuration = builder.Build();
            connectionString = configuration.GetConnectionString("MyConnectionString");
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
        
        public DbSet<WeatherData> WeatherDataInfo { get; set; }
    }
}

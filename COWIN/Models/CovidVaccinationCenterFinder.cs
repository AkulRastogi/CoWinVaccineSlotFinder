﻿using CoWiN.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace CoWin.Models
{
    public class CovidVaccinationCenterFinder
    {
        private IConfiguration _configuration;
        private List<string> districtsToSearch = new List<string>();
        private string currentDate;
        public CovidVaccinationCenterFinder()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();
        }

        public void FindSlot()
        {
            ConfigureSearchCriteria();
            SearchForAvailableSlots();
        }

        private void SearchForAvailableSlots()
        {
            for (int i = 1; i < Convert.ToInt32(_configuration["CoWinAPI:TotalIterations"]); i++)
            {
                Console.WriteLine($"Fetching Resources, Try #{i}");
                foreach (var district in districtsToSearch)
                {
                    new CovidVaccinationCenter(_configuration).GetSlotsByDistrictId(district, currentDate);
                }

                Thread.Sleep(Convert.ToInt32(_configuration["CoWinAPI:SleepIntervalInMilliseconds"]));
            }
        }

        private void ConfigureSearchCriteria()
        {
            foreach(var item in _configuration.GetSection("Districts").GetChildren())
            {
                districtsToSearch.Add(item.Value);
            }
            currentDate = DateTime.Now.ToString("dd-MM-yyyy");
        }

    }
}

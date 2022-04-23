using InsightsLibrary.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp.Infrastructure
{
    public class Settings
    {
        public ApiCredentials ApiCredentials { get; private set; }

        public Settings(IConfigurationRoot configuration)
        {
            ApiCredentials = new ApiCredentials
            {
                key = configuration["api_key"],
                secret = configuration["api_secret"]
            };
        }
    }
}

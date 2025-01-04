using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using database;

namespace Backup
{
    public class Program
    {
        protected Program()
        {

        }

        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();

            //sets the path
            var jsonpath = "C:\\database\\database";

            //reads from the appsetting.json
            builder.SetBasePath(jsonpath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfiguration configuration = builder.Build();

            if (args.Length > 0)
            {
                Redirector direct = new Redirector(configuration);
                direct.Command(args);
            }

            else
            {
                Init i = new Init(configuration);
                i.begin();
            }


        }

    }
}
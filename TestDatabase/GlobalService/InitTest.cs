using database;
using database.Databases.MSSQL;
using Microsoft.Extensions.Configuration;

namespace TestBackup
{
    public class InitTest
    {
        private IConfiguration? configuration;

        private void setConfiguration()
        {
            var builder = new ConfigurationBuilder();

            //sets the path
            var jsonpath = "C:\\database\\database";

            //reads from the appsetting.json
            builder.SetBasePath(jsonpath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            configuration = builder.Build();

        }

        [Fact]
        public void testPopulateDatabaseList()
        {
            try
            {
                setConfiguration();

                if (configuration != null)
                {
                    Init start = new(configuration);
                    start.PopulateDatabaseList();
                    Assert.True(true);
                }

                else
                {
                    Assert.True(false);
                }

            }

            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [Fact]
        public void testRedirector()
        {
            try
            {
                setConfiguration();

                if (configuration != null)
                {
                    Init start = new(configuration);
                    start.Redirector("life", "Backup");
                    Assert.True(true);
                }

                else
                {
                    Assert.Fail();
                }

            }

            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}

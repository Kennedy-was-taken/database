
using database;
using Microsoft.Extensions.Configuration;

namespace TestDatabase
{
    [TestCaseOrderer(
    ordererTypeName: "TestDatabase.PriorityOrderer",
    ordererAssemblyName: "TestDatabase")]
    public class RedirecterTest
    {
        private IConfiguration? configuration;

        Redirector? redirector;

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

        [Fact, TestPriority(1)]
        public void TestCommandHelp()
        {
            try
            {
                setConfiguration();

                if (configuration != null)
                {
                    string[] args = { "--help" };

                    redirector = new(configuration);

                    redirector.Command(args);

                    Assert.True(true);
                }
            }

            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

        [Fact, TestPriority(3)]
        public void TestCommandRandom()
        {
            if(configuration is null)
            { 
                setConfiguration();
            }

            try
            {

                if (configuration != null)
                {
                    string[] args = { "-h" };

                    redirector = new(configuration);

                    redirector.Command(args);

                    Assert.True(true);
                }
            }

            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

        [Fact, TestPriority(4)]
        public void TestCommand2Help()
        {
            if (configuration is null)
            {
                setConfiguration();
            }

            try
            {

                if (configuration != null)
                {
                    string[] args = { "--sql", "--help" };

                    redirector = new(configuration);

                    redirector.Command(args, "sqlserver");

                    Assert.True(true);
                }
            }

            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [Fact, TestPriority(5)]
        public void TestCommand2Random()
        {
            if (configuration is null)
            {
                setConfiguration();
            }

            try
            {

                if (configuration != null)
                {
                    string[] args = { "--sql", "--h" };

                    redirector = new(configuration);

                    redirector.Command(args, "sqlserver");

                    Assert.True(true);
                }
            }

            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact, TestPriority(6)]
        public void TestCommand3()
        {
            if (configuration is null)
            {
                setConfiguration();
            }

            try
            {

                if (configuration != null)
                {
                    string[] args = { "--sql", "--backup", "backup all" };

                    redirector = new(configuration);

                    redirector.Command(args, "sqlserver", "backup");

                    Assert.True(true);
                }
            }

            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact, TestPriority(7)]
        public void TestCommand3Restore()
        {
            if (configuration is null)
            {
                setConfiguration();
            }

            try
            {

                if (configuration != null)
                {
                    string[] args = { "--sql", "--restore", "restore all" };

                    redirector = new(configuration);

                    redirector.Command(args, "sqlserver", "restore");

                    Assert.True(true);
                }
            }

            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact, TestPriority(8)]
        public void TestCommand3Help()
        {
            if (configuration is null)
            {
                setConfiguration();
            }

            try
            {

                if (configuration != null)
                {
                    string[] args = { "--sql", "--backup", "--help" };

                    redirector = new(configuration);

                    redirector.Command(args, "sqlserver", "backup");

                    Assert.True(true);
                }
            }

            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact, TestPriority(9)]
        public void TestCommand3Random()
        {
            if (configuration is null)
            {
                setConfiguration();
            }

            try
            {

                if (configuration != null)
                {
                    string[] args = { "--sql", "--backup", "-something" };

                    redirector = new(configuration);

                    redirector.Command(args, "sqlserver", "backup");

                    Assert.True(true);
                }
            }

            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact, TestPriority(10)]
        public void TestCommand3RandomSpace()
        {
            if (configuration is null)
            {
                setConfiguration();
            }

            try
            {
                if (configuration != null)
                {
                    string[] args = { "--sql", "--backup", " " };

                    redirector = new(configuration);

                    redirector.Command(args, "sqlserver", "backup");

                    Assert.True(true);
                }
            }

            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact, TestPriority(11)]
        public void TestCommand3BackupSearch()
        {
            if (configuration is null)
            {
                setConfiguration();
            }

            try
            {

                if (configuration != null)
                {
                    string[] args = { "--sql", "--backup", "sonar" };

                    redirector = new(configuration);

                    redirector.Command(args, "--sql", "backup");

                    Assert.True(true);
                }
            }

            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact, TestPriority(12)]
        public void TestCommand3RestoreSearch()
        {
            if (configuration is null)
            {
                setConfiguration();
            }

            try
            {

                if (configuration != null)
                {
                    string[] args = { "--sql", "--restore", "sonar" };

                    redirector = new(configuration);

                    redirector.Command(args, "--sql", "restore");

                    Assert.True(true);
                }
            }

            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact, TestPriority(12)]
        public void TestCommand3SearchNotFoundBackup()
        {
            if (configuration is null)
            {
                setConfiguration();
            }

            try
            {

                if (configuration != null)
                {
                    string[] args = { "--sql", "--backup", "sffsrf" };

                    redirector = new(configuration);

                    redirector.Command(args, "--sql", "backup");

                    Assert.False(false);
                }
            }

            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact, TestPriority(12)]
        public void TestCommand3SearchNotFoundRestore()
        {
            if (configuration is null)
            {
                setConfiguration();
            }

            try
            {

                if (configuration != null)
                {
                    string[] args = { "--sql", "--restore", "sffsrf" };

                    redirector = new(configuration);

                    redirector.Command(args, "--sql", "restore");

                    Assert.False(false);
                }
            }

            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

    }
}

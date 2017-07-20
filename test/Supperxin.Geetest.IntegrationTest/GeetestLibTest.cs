using System;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Supperxin.Geetest.IntegrationTest
{
    public class GeetestLibTest
    {
        [Fact]
        public void TestGetCaptcha()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("Configuration.json")
                .Build();

            GeetestLib geetest = new GeetestLib(config["GeetestId"], config["GeetestKey"]);
            String userID = config["GeetestUserId"];
            Byte gtServerStatus = geetest.preProcess(userID);

            Assert.Equal(gtServerStatus, (Byte)1);
            Console.WriteLine(geetest.getResponseStr());
        }
    }
}

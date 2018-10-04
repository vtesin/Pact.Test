using System.Collections.Generic;
using System.IO;
using System.Reflection;
using PactNet;
using PactNet.Infrastructure.Outputters;
using FluentAssertions;

namespace Provider.Test
{
    public class Program
    {
        const string ProviderName = "Cardservice";
        const string ClientName = "Cardclient";

        public static void Main(string[] args)
        {
            var currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var pactDir = Path.Combine(currentDirectory, @"..\..\..\Docs");
            var logDir = Path.Combine(currentDirectory, @"..\..\..\Docs");

            var outputted = new CustomOutputter();
            var config = new PactVerifierConfig
            {
                Outputters = new List<IOutput>() {outputted},
                ProviderVersion = "1.0.0",
                PublishVerificationResults = true,
            };

            IPactVerifier pactVerifier = new PactVerifier(config);
            pactVerifier.PactUri(pactDir);


            //Act
            try
            {
                var pactUriOptions =
                    new PactUriOptions("Vm6YWrQURJ1T7mDIRiKwfexCAc4HbU", "aLerJwBhpEcN0Wm88Wgvs45AR9dXpc");

                pactVerifier
                    .ServiceProvider(ProviderName, "http://mtgdatabase.azurewebsites.net/")
                    .HonoursPactWith(ClientName)
                    //.PactUri(Path.Combine(pactDir, $"{ClientName}-{ProviderName}.json"))
                    .PactUri(@"https://adesso.pact.dius.com.au/pacts/provider/Cardservice/consumer/Cardclient/latest",
                        pactUriOptions)
                    .Verify();


                // Assert
                outputted.Should().NotBeNull();
                var outputtedOutput = outputted.Output.ToLowerInvariant();
                outputtedOutput.Should().NotBeNullOrWhiteSpace();
                outputtedOutput.Should()
                    .Contain($"Verifying a Pact between {ClientName} and {ProviderName}".ToLowerInvariant());
                outputtedOutput.Should().Contain("status code 200");
            }
            finally
            {
                System.Console.ReadLine();
            }
        }

        private class CustomOutputter : IOutput
        {
            public string Output { get; private set; }

            public void WriteLine(string line)
            {
                Output += line;
                System.Console.WriteLine(line);
            }
        }
    }
}
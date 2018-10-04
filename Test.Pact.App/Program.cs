using System;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using System.Collections.Generic;
using Test.Pact.App.Client;
using Test.Pact.App.MockService;
using FluentAssertions;

namespace Test.Pact.App
{
    class Program
    {
        private static IMockProviderService _mockProviderService;
        private static string _mockProviderServiceBaseUri;

        static void Main(string[] args)
        {
            var requestId = "145d5d8b-2056-4dab-8786-a0f9979c6a32";
            var consumerPact = new ConsumerMtgApiPact();
            _mockProviderService = consumerPact.MockProviderService;
            _mockProviderServiceBaseUri = consumerPact.MockProviderServiceBaseUri;
            consumerPact.MockProviderService.ClearInteractions();
            //NOTE: Clears any previously registered interactions before the test is run


            _mockProviderService
                .Given("Card with id '0d906a20abf1428a1d30a694f3cc1cec1692652d' exists")
                .UponReceiving("A GET request to retrieve a card")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/api/cards/0d906a20abf1428a1d30a694f3cc1cec1692652d",
                    Headers = new Dictionary<string, object>
                    {
                        {"Accept", "application/json"},
                        {"RequestId", requestId}
                    }
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 200,
                    Headers = new Dictionary<string, object>
                    {
                        {"Content-Type", "application/json; charset=utf-8"}
                    },
                    Body =
                        new //NOTE: Note the case sensitivity here, the body will be serialised as per the casing defined
                        {
                            card = new
                            {
                                id = 12636,
                                name = "Glorybringer",
                                scryfallImageUrl = "https://img.scryfall.com/cards/large/en/akh/134.jpg"
                            }
                        }
                }); //NOTE: WillRespondWith call must come last as it will register the interaction

            var consumer = new CardApiClient(_mockProviderServiceBaseUri);

            //Act

            var result = consumer.GetCard("0d906a20abf1428a1d30a694f3cc1cec1692652d", requestId);

            //Assert
            result.Should().NotBeNull();
            _mockProviderService.VerifyInteractions();
            //NOTE: Verifies that interactions registered on the mock provider are called once and only once

            consumerPact.Dispose();
        }
    }
}
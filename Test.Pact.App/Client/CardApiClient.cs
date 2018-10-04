using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Test.Pact.App.Contract;

namespace Test.Pact.App.Client
{
    public class CardApiClient
    {
        public string BaseUri { get; set; }

        public CardApiClient(string baseUri = null)
        {
            BaseUri = baseUri ?? "http://mtgdatabase.azurewebsites.net/";
        }

        /// <summary>
        /// get a card based on the hash
        /// </summary>
        /// <param name="id"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public Card GetCard(string id, string requestId)
        {
            string reasonPhrase;

            using (var client = new HttpClient {BaseAddress = new Uri(BaseUri)})
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/api/cards/" + id);
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("RequestId", requestId);
                //request.Headers.Add("Date", DateTime.UtcNow.ToString());

                var response = client.SendAsync(request);

                var content = response.Result.Content.ReadAsStringAsync().Result;
                var status = response.Result.StatusCode;

                reasonPhrase = response.Result.ReasonPhrase;
                //NOTE: any Pact mock provider errors will be returned here and in the response body

                request.Dispose();
                response.Dispose();

                if (status == HttpStatusCode.OK)
                {
                    return !string.IsNullOrEmpty(content)
                        ? JsonConvert.DeserializeObject<Card>(content)
                        : null;
                }
            }

            throw new Exception(reasonPhrase);
        }
    }
}
using Newtonsoft.Json;

namespace Test.Pact.App.Contract
{
    public class Card
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonProperty(PropertyName = "scryfallImageUrl")]
        public string Url { get; set; }
    }
}
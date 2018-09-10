using Newtonsoft.Json.Converters;

namespace Surveillance.ElasticSearchDtos.JsonConverters
{
    public class ElasticSearchDateTimeConverter : IsoDateTimeConverter
    {
        public ElasticSearchDateTimeConverter()
        {
            DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        }
    }
}

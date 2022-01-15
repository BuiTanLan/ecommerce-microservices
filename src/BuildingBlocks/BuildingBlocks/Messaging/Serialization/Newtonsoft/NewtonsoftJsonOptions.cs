using BuildingBlocks.Core.Objects.Collections;
using Newtonsoft.Json;

namespace BuildingBlocks.Core.Messaging.Serialization.Newtonsoft
{
    public class NewtonsoftJsonOptions
    {
        public IList<JsonConverter> Converters { get; set; }
        public ITypeList UnSupportedTypes { get; }

        public NewtonsoftJsonOptions()
        {
            Converters = new List<JsonConverter>();
            UnSupportedTypes = new TypeList();
        }
    }
}

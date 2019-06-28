using Newtonsoft.Json;
using Popcron.Sheets;

namespace RDEManager
{
    public class JSONSerializer : SheetsSerializer
    {
        public override T DeserializeObject<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public override string SerializeObject(object data)
        {
            return JsonConvert.SerializeObject(data);
        }
    }
}

using Microsoft.AspNetCore.Http;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

namespace B3cBonsai.Utility.Extentions
{
    public static class SessionExtentions
    {
        public static T GetComplexData<T>(this ISession session, string key)
        {
            var data = session.GetString(key);
            if (data == null)
            {
                return default;
            }
            return JsonConvert.DeserializeObject<T>(data);
        }

        public static void SetComplexData(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
    }
}

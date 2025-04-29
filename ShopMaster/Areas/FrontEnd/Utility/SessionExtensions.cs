using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;


namespace ShopMaster.Areas.FrontEnd.Utility
{
    //public static class SessionExtensions
    //{
    //    // 儲存物件至 Session，使用 JSON 格式
    //    public static void Set(this ISession session, string key, object value)
    //    {
    //        session.SetString(key, JsonConvert.SerializeObject(value));
    //    }

    //    // 從 Session 中取得物件，並將 JSON 格式轉換回物件
    //    public static T Get<T>(this ISession session, string key)
    //    {
    //        var value = session.GetString(key);
    //        return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
    //    }






    //}

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            session.SetString(key, JsonConvert.SerializeObject(value, settings));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }
}

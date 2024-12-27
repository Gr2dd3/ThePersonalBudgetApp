namespace ThePersonalBudgetApp.DAL.Helpers;

public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        var json = JsonConvert.SerializeObject(value, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
        session.SetString(key, json);
    }

    public static T? Get<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default : JsonConvert.DeserializeObject<T>(value);
    }
}

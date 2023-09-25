using System.Text.Json.Serialization;
using System.Text.Json;
using TrackableEntities.Client.Core;

namespace TrackableEntities.Tests.Acceptance.Helpers;

internal static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new() { ReferenceHandler = ReferenceHandler.Preserve, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault, TypeInfoResolver = new OverrideJsonIgnoreContractResolver() };

    public static TEntity? GetEntity<TEntity, TKey>(this HttpClient client, TKey id)
    {
        var response = client.GetAsync($"api/{typeof(TEntity).Name}/{id}").Result;
        response.EnsureSuccessStatusCode();
        var result = response.Content.ReadFromJsonAsync<TEntity>(DefaultJsonSerializerOptions).Result;
        return result;
    }

    public static IEnumerable<TEntity>? GetEntities<TEntity>(this HttpClient client)
    {
        var response = client.GetAsync($"api/{typeof(TEntity).Name}").Result;
        response.EnsureSuccessStatusCode();
        var result = response.Content.ReadFromJsonAsync<IEnumerable<TEntity>>(DefaultJsonSerializerOptions).Result;
        return result;
    }
    public static IEnumerable<TEntity>? GetEntitiesByKey<TEntity, TKey>(this HttpClient client, string keyName, TKey id)
    {
        var response = client.GetAsync($"api/{typeof(TEntity).Name}/?{keyName}={id}").Result;
        response.EnsureSuccessStatusCode();
        var result = response.Content.ReadFromJsonAsync<IEnumerable<TEntity>>(DefaultJsonSerializerOptions).Result;
        return result;
    }
    public static TEntity? CreateEntity<TEntity>(this HttpClient client, TEntity entity)
    {
        var response = client.PostAsJsonAsync($"api/{typeof(TEntity).Name}", entity).Result;
        response.EnsureSuccessStatusCode();
        var result = response.Content.ReadFromJsonAsync<TEntity>(DefaultJsonSerializerOptions).Result;
        return result;
    }

    public static TEntity? UpdateEntity<TEntity, TKey>(this HttpClient client, TEntity entity, TKey id)
    {
        var response = HttpClientJsonExtensions.PutAsJsonAsync(client, $"api/{typeof(TEntity).Name}/{id}", entity, DefaultJsonSerializerOptions).Result;
        response.EnsureSuccessStatusCode();
        var result = response.Content.ReadFromJsonAsync<TEntity>(DefaultJsonSerializerOptions).Result;
        return result;
    }

    public static void DeleteEntity<TEntity, TKey>(this HttpClient client, TKey id)
    {
        var response = client.DeleteAsync($"api/{typeof(TEntity).Name}/{id}");
        response.Result.EnsureSuccessStatusCode();
    }

    public static bool VerifyEntityDeleted<TEntity, TKey>(this HttpClient client, TKey id)
    {
        var response = client.GetAsync($"api/{typeof(TEntity).Name}/{id}").Result;
        if (response.IsSuccessStatusCode) return false;
        return true;
    }
}

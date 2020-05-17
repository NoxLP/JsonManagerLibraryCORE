using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;
using System.Threading;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

namespace JsonLibraryCORE
{
    public class JsonManager
    {
        public JsonManager()
        {
            //JsonConvert.DefaultSettings = () => DefaultSettings;
        }
        public JsonManager(JsonSerializerOptions settings)
        {
            DefaultSettings = settings;
            //JsonConvert.DefaultSettings = () => DefaultSettings;
        }

        public JsonSerializerOptions DefaultSettings { get; set; }

        public T DeserializeJson<T>(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<T> DeserializeJsonAsync<T>(string json, CancellationToken token = default(CancellationToken))
        {
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(json);
                using (var stream = new MemoryStream(byteArray))
                {
                    return await JsonSerializer.DeserializeAsync<T>(stream, DefaultSettings, token);
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<T> DeserializeJsonAsync<T>(string json, string propertyName, string newValue, CancellationToken token = default(CancellationToken))
        {
            T obj = await Task.Run(() =>
            {
                try
                {
                    using (var jsonDocument = JsonDocument.Parse(json))
                    {
                        if (token != default(CancellationToken) && token.IsCancellationRequested)
                            return default(T);

                        return jsonDocument.RootElement.GetProperty(propertyName).ToObject<T>(DefaultSettings);
                    }
                }
                catch (Exception e)
                {
                    throw;
                }
            });

            return obj;
        }
        public async Task<T> DeserializeJsonFileAsync<T>(string fullPath, CancellationToken token = default(CancellationToken))
        {
            try
            {
                using (var file = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.None, 4096, true))
                {
                    return await JsonSerializer.DeserializeAsync<T>(file, default, token);
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public string SerializeToJsonInMemory<T>(T obj)
        {
            try
            {
                return JsonSerializer.Serialize<T>(obj, DefaultSettings);
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<string> SerializeToJsonInMemoryAsync<T>(T obj, CancellationToken token = default(CancellationToken))
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    await JsonSerializer.SerializeAsync<T>(stream, obj, DefaultSettings, token);
                    using (var reader = new StreamReader(stream))
                    {
                        return await reader.ReadToEndAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task SerializeToJsonInFile<T>(T obj, string fullPath, CancellationToken token = default(CancellationToken))
        {
            using (var file = File.Open(fullPath, FileMode.Create))
            {
                await JsonSerializer.SerializeAsync<T>(file, obj, DefaultSettings, token);
            }
        }
    }
}

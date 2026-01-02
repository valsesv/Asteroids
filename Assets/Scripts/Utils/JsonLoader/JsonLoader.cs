using System.IO;
using UnityEngine;
using ModestTree;
using Newtonsoft.Json;

namespace Utils.JsonLoader
{
    public class JsonLoader
    {
        public T LoadFromStreamingAssets<T>(string fileName) where T : class
        {
            string path = Path.Combine(Application.streamingAssetsPath, fileName);
            return LoadFromPath<T>(path);
        }

        public T LoadFromPath<T>(string path) where T : class
        {
            if (!File.Exists(path))
            {
                Debug.LogError($"JSON file not found at path: {path}");
                return null;
            }

            try
            {
                string jsonContent = File.ReadAllText(path);
                T result = JsonConvert.DeserializeObject<T>(jsonContent);

                Assert.IsNotNull(result, $"Failed to deserialize JSON from {path}");

                return result;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error loading JSON from {path}: {e.Message}");
                return null;
            }
        }
    }
}


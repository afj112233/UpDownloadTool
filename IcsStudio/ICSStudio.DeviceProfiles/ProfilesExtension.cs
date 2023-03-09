using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace ICSStudio.DeviceProfiles
{
    public static class ProfilesExtension
    {
        public static T DeserializeFromFile<T>(string fileName)
        {
            if (File.Exists(fileName))
                using (var file = File.OpenText(fileName))
                {
                    var serializer = new JsonSerializer();
                    var motionDrive =
                        (T) serializer.Deserialize(file, typeof(T));
                    return motionDrive;
                }

            Debug.WriteLine("Don't found file " + fileName);
            return default(T);
        }
    }
}

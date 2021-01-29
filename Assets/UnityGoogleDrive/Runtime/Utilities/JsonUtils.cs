using UnityGoogleDrive.Newtonsoft.Json;
using UnityGoogleDrive.Newtonsoft.Json.Serialization;
using System.Reflection;

namespace UnityGoogleDrive
{
    public static class JsonUtils
    {
        /// <summary>
        /// Allows resolving properties with private setters.
        /// </summary>
        private class PrivateCamelResolver : CamelCasePropertyNamesContractResolver
        {
            protected override JsonProperty CreateProperty (MemberInfo member, MemberSerialization memberSerialization)
            {
                var prop = base.CreateProperty(member, memberSerialization);

                if (!prop.Writable)
                {
                    var property = member as PropertyInfo;
                    if (property != null)
                    {
                        var hasPrivateSetter = property.GetSetMethod(true) != null;
                        prop.Writable = hasPrivateSetter;
                    }
                }

                return prop;
            }
        }

        /// <summary>
        /// Invokes <see cref="JsonConvert.DeserializeObject(string)"/> configured with <see cref="PrivateCamelResolver"/>.
        /// </summary>
        public static T FromJsonPrivateCamel<T> (string json)
        {
            var serializerSettings = new JsonSerializerSettings { ContractResolver = new PrivateCamelResolver() };
            return JsonConvert.DeserializeObject<T>(json, serializerSettings);
        }

        /// <summary>
        /// Invokes <see cref="JsonConvert.SerializeObject(object)"/> configured with <see cref="PrivateCamelResolver"/>.
        /// </summary>
        public static string ToJsonPrivateCamel (object obj)
        {
            var serializerSettings = new JsonSerializerSettings { ContractResolver = new PrivateCamelResolver() };
            return JsonConvert.SerializeObject(obj, serializerSettings);
        }
    }
}

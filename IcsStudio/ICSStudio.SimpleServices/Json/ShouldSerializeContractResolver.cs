using System.Reflection;
using ICSStudio.SimpleServices.DataWrapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ICSStudio.SimpleServices.Json
{
    public class ShouldSerializeContractResolver : DefaultContractResolver
    {
        public static readonly ShouldSerializeContractResolver Instance = new ShouldSerializeContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property.DeclaringType == typeof(AxisCIPDriveParameters))
            {
                if (!property.Ignored)
                {
                    property.ShouldSerialize =
                        instance =>
                        {
                            var axis = (AxisCIPDriveParameters) instance;
                            return axis.SupportAttribute(property.PropertyName);
                        };
                }
            }

            return property;
        }
    }
}

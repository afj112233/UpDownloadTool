using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ICSStudio.Interfaces.Common
{
    public enum Type
    {
        Analog,
        Digital,
        Full_Width
    }

    public enum Style
    {
        [EnumMember(Value = "---------------")]
        Style1,
        [EnumMember(Value = "--- --- --- ---")]
        Style2,
        [EnumMember(Value = "- - - - - - - -")]
        Style3,
        [EnumMember(Value = "--- - --- - --- -")]
        Style4,
        [EnumMember(Value = "--- - - --- - -")]
        Style5,
    }
    //TODO(zyl):add more marker
    public enum MarkerType
    {
        None,
        Boxed,
        [EnumMember(Value = "Up Triangle")]
        UpTriangle,
        [EnumMember(Value = "Down Triangle")]
        DownTriangle,
        [EnumMember(Value = "!")]
        ExclamationMark,
        [EnumMember(Value = "@")]
        At,
        [EnumMember(Value = "#")]
        Marker1,

    }

    public interface IPen
    {
        JObject ToJson();
        string Name { get; }
        string Description { get; }
        string Color { get; set; }
        bool Visible { get; }
        Style Style { get; }
        Type Type { get; }
        int Width { get; }
        int Marker { get; }
        float Min { get; }
        float Max { get; }
        string Units { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.DataType
{
    public static class JTokenExtend
    {
        public static bool FixJToken(this JToken correct,JToken broken)
        {
            var value = correct as JValue;
            if (value != null)
            {
                if (broken is JValue)
                {
                    var v = ((JValue) broken).Value;
                    value.Value = v;
                }
                else
                {
                    return false;
                }

                return true;
            }

            var array = correct as JArray;
            if (array != null)
            {
                if (broken is JArray)
                {
                    var arrayBroken = (JArray) broken;
                    int index = 0;
                    foreach (var token in array)
                    {
                        if (arrayBroken.Count > index)
                        {
                            var result=FixJToken(token, arrayBroken[index]);
                            if (!result) return false;
                        }
                        else
                        {
                            return false;
                        }
                        index++;
                    }
                }
                else
                {
                    return false;
                }

                return true;
            }
            
            return true;
        }
    }
}

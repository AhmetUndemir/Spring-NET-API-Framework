using System;
using System.ComponentModel;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SimpleSprint.Common {
    public static class Extensions {
        public static bool IsValidJson (this string strInput) {
            strInput = strInput.Trim ();
            if ((strInput.StartsWith ("{") && strInput.EndsWith ("}")) || //For object
                (strInput.StartsWith ("[") && strInput.EndsWith ("]"))) //For array
            {
                try {
                    var obj = JToken.Parse (strInput);
                    return true;
                } catch (JsonReaderException jex) {
                    //Exception in parsing json
                    Console.WriteLine (jex.Message);
                    return false;
                } catch (Exception ex) //some other exception
                {
                    Console.WriteLine (ex.ToString ());
                    return false;
                }
            } else {
                return false;
            }
        }


        public static string DescriptionAttr<T> (this T source) {
            FieldInfo fi = source.GetType ().GetField (source.ToString ());

            DescriptionAttribute[] attributes = (DescriptionAttribute[]) fi.GetCustomAttributes (
                typeof (DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return source.ToString ();
        }
    }
}
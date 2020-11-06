using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTMcu
{
    class ConfigRead
    {
        public static void Write(object obj, string local)
        {
            try
            {
                File.WriteAllText(local, JsonConvert.SerializeObject(obj));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static T Read<T>(string local, T obj)
        {
            try
            {
                if (!File.Exists(local))
                {
                    File.WriteAllText(local, JsonConvert.SerializeObject(obj));
                    return obj;
                }
                else
                {
                    return JsonConvert.DeserializeObject<T>(File.ReadAllText(local));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return obj;
        }
    }
}

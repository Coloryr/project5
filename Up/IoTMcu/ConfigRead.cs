﻿using System;
using System.IO;
using System.Text.Json;

namespace IoTMcu
{
    class ConfigRead
    {
        public static void Write(object obj, string local)
        {
            try
            {
                File.WriteAllText(local, JsonSerializer.Serialize(obj));
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
                    File.WriteAllText(local, JsonSerializer.Serialize(obj));
                    return obj;
                }
                else
                {
                    return JsonSerializer.Deserialize<T>(File.ReadAllText(local));
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

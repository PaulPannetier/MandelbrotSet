using System.IO;
using System;
using System.Text.Json;

namespace SME.JSON
{
    public static class JSONUtility
    {
        /// <summary>
        /// Convert a serializable object to an JSON string who represent the object
        /// </summary>
        /// <param name="obj"> The object to convert</param>
        public static string ToJSON<T>(T obj, in bool writeIndented = true)
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = writeIndented };
                return JsonSerializer.Serialize(obj, options);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Convert a JSON string to the object which represent
        /// </summary>
        public static T FromJSONstring<T>(string JSONstring)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(JSONstring);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

    public static class Save
    {
        #region write/load file

        /// <summary>
        /// Save an object in the path. !objsct's attribute must to have properties!
        /// </summary>
        /// <typeparam name="T">The type of the object to save</typeparam>
        /// <param name="objectToWrite">The object to save</param>
        /// <param name="fileName">The path where the object will be save, ex : @"\MyFolder\file.game</param>
        public static void WriteObject<T>(T objectToWrite, string fileName)
        {
            try
            {
                string s = JSONUtility.ToJSON(objectToWrite);
                string path = (Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + fileName);
                File.WriteAllText(path, s);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="fileName"> The filename where the object is saved, ex : @"\MyFolder\file.game</param>
        /// <returns> The object save in the file</returns>
        public static T ReadObject<T>(string fileName)
        {
            try
            {
                return JSONUtility.FromJSONstring<T>(File.ReadAllText(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + fileName));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion
    }
}

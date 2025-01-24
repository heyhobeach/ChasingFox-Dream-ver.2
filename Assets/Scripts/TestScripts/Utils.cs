using System.IO;
using UnityEngine;

namespace JsonUtils
{
    public class Utils
    {
        public static T LoadJson<T>(string fileName)
        {
            string path = Path.Combine(Application.persistentDataPath, fileName);

            // // 테스트용 코드
            // string jsonData = null;
            // try {
            //     jsonData = File.ReadAllText(path);
            // } catch (System.Exception e) {
            //     Debug.Log(e);
            //     SaveJson(fileName, typeof(T));
            //     jsonData = File.ReadAllText(path);
            // }
            // // 테스트용 코드
            string jsonData = File.ReadAllText(path);
			// byte[] bytes = System.Convert.FromBase64String(jsonData);
            // ByteConvert(ref bytes);
			// string decodedJson = System.Text.Encoding.UTF8.GetString(bytes);
            // return JsonUtility.FromJson<T>(decodedJson);
            return JsonUtility.FromJson<T>(jsonData);
        }
        public static void LoadJson<T>(string fileName, ref T data)
        {
            string path = Path.Combine(Application.persistentDataPath, fileName);
            // string jsonData = null;
            // try {
            //     jsonData = File.ReadAllText(path);
            // } catch (System.Exception e) {
            //     Debug.Log(e);
            //     SaveJson(fileName, typeof(T));
            //     jsonData = File.ReadAllText(path);
            // }
			// byte[] bytes = System.Convert.FromBase64String(jsonData);
            // ByteConvert(ref bytes);
			// string decodedJson = System.Text.Encoding.UTF8.GetString(bytes);
            // data = JsonUtility.FromJson<T>(decodedJson);
            string jsonData = File.ReadAllText(path);
            data = JsonUtility.FromJson<T>(jsonData);
        }

        public static void SaveJson<T>(string fileName, T data)
        {
            string path = Path.Combine(Application.persistentDataPath, fileName);
            string jsonData = JsonUtility.ToJson(data, true);
            // byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
            // ByteConvert(ref bytes);
            // string encodedJson = System.Convert.ToBase64String(bytes);
            // File.WriteAllText(path, encodedJson);
            File.WriteAllText(path, jsonData);
        }

        public static void DeleteJson(string fileName)
        {
            string path = Path.Combine(Application.persistentDataPath, fileName);
            File.Delete(path);
        }

        public static FileInfo GetFileInfo(string fileName)
        {
            string path = Path.Combine(Application.persistentDataPath, fileName);
            return new FileInfo(path);
        }

        private static void ByteConvert(ref byte[] bytes)
        {
            for(int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)~bytes[i];
            }
        }
    }
}


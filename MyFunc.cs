using System;
using System.Collections;
using System.Drawing;
using System.Security.Cryptography;
using System.Web.Configuration;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Web.Security;
using System.Configuration;
using System.Web.SessionState;
using System.Web;

namespace ShareLib17
{


    public static partial class MyFunc
    {

        public static string GetObjectValue(object Obj, string Default)
        {
            if (Obj == null)
                return Default;
            if (string.IsNullOrEmpty(Obj.ToString()))
                return Default;
            return Obj.ToString();
        }

        public static int GetObjectValue(object Obj, int Default)
        {
            int result = Default;
            if (int.TryParse(GetObjectValue(Obj, ""), out result))
                return result;
            return Default;
        }

        public static double GetObjectValue(object Obj, double Default)
        {
            double result = Default;
            if (double.TryParse(GetObjectValue(Obj, ""), out result))
                return result;
            return Default;
        }

        public static bool GetObjectValue(object Obj, bool Default)
        {
            bool result = Default;
            if (bool.TryParse(GetObjectValue(Obj, ""), out result))
                return result;
            return Default;
        }

        public static DateTime GetObjectValue(object Obj, DateTime Default)
        {
            if (Obj == null)
                return Default;
            try { return Convert.ToDateTime(Obj); }
            catch { }
            return Default;
        }

        public static Decimal GetObjectValue(object Obj, Decimal Default)
        {
            Decimal result = Default;
            if (Decimal.TryParse(GetObjectValue(Obj, ""), out result))
                return result;
            return Default;
        }

        public static object GetWebconfigValue(string Key)
        {
            return WebConfigurationManager.AppSettings[Key];
        }

        public static string GetWebconfigValue(string Key, string Default)
        {
            return GetObjectValue(WebConfigurationManager.AppSettings[Key], Default);
        }

        public static int GetWebconfigValue(string Key, int Default)
        {
            return GetObjectValue(WebConfigurationManager.AppSettings[Key], Default);
        }

        public static bool GetWebconfigValue(string Key, bool Default)
        {
            return GetObjectValue(WebConfigurationManager.AppSettings[Key], Default);
        }

        public static double GetWebconfigValue(string Key, double Default)
        {
            return GetObjectValue(WebConfigurationManager.AppSettings[Key], Default);
        }

        public static string RemoveCtrlChars(string Text)
        {
            Regex rgx = new Regex("\\s");
            return rgx.Replace(Text, "");
        }

        public static string ExtractHost(string Url)
        {
            if (string.IsNullOrWhiteSpace(Url))
                return null;
            string result = Regex.Replace(Url, "http(\\\\s)?://", "", RegexOptions.IgnoreCase);
            int i = result.IndexOf('/');
            if (i == -1)
                return result;
            return result.Remove(i);
        }

        public static HttpSessionState CurrentSession()
        {
            return HttpContext.Current.Session;
        }

        public static void SaveSessionData(string DataId, object Data)
        {
            if (HttpContext.Current == null)
                return;
            if (HttpContext.Current.Session == null)
                return;
            HttpContext.Current.Session[DataId] = Data;
        }

        public static object GetSessionData(string DataId)
        {
            if (HttpContext.Current == null)
                return null;
            if (HttpContext.Current.Session == null)
                return null;
            return HttpContext.Current.Session[DataId];
        }

        public static string GetSessionData(string DataId, string Default)
        {
            return MyFunc.GetObjectValue(GetSessionData(DataId), Default);
        }

        public static bool GetSessionData(string DataId, bool Default)
        {
            return MyFunc.GetObjectValue(GetSessionData(DataId), Default);
        }

        public static int GetSessionData(string DataId, int Default)
        {
            return MyFunc.GetObjectValue(GetSessionData(DataId), Default);
        }

        public static DateTime GetSessionData(string DataId, DateTime Default)
        {
            return MyFunc.GetObjectValue(GetSessionData(DataId), Default);
        }

        public static byte[] Serialize(object Data)
        {
            Stream stream = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, Data);
            byte[] array = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(array, 0, (int)stream.Length);
            stream.Close();
            return array;
        }

        public static object Deserialize(byte[] Data)
        {
            if (Data.Length == 0)
                return null;
            MemoryStream stream = new MemoryStream(Data);
            BinaryFormatter bf = new BinaryFormatter();
            object result = bf.Deserialize(stream);
            stream.Close();
            return result;
        }

        public static string SerializeToString(object Data)
        {
            byte[] bytes = Serialize(Data);
            return Convert.ToBase64String(bytes);
        }

        public static object DeserializeFromString(string Data)
        {
            byte[] bytes = Convert.FromBase64String(Data);
            return Deserialize(bytes);
        }

    }

}
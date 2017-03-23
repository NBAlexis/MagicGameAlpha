using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class CLuaNetworkInterface
{
    public static string GetDeviceId()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }

    public static int GetIntValue(string sKey, int defaultRet)
    {
        return PlayerPrefs.GetInt("Client." + sKey, defaultRet);
    }

    public static string GetStringValue(string sKey, string defaultRet)
    {
        return PlayerPrefs.GetString("Client." + sKey, defaultRet);
    }

    public static float GetFloatValue(string sKey, float defaultRet)
    {
        return PlayerPrefs.GetFloat("Client." + sKey, defaultRet);
    }

    public static string StringMD5(string sIn)
    {
        if (string.IsNullOrEmpty(sIn))
        {
            sIn = "EmptyString";
        }
        MD5 md5 = new MD5CryptoServiceProvider();
        return md5.ComputeHash(Encoding.Unicode.GetBytes(sIn)).Aggregate("", (s, e) => s + string.Format("{0:x2}", e), s => s);
    }
}

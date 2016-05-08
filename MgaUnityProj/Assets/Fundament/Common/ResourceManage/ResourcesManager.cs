using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

public class ResourcesManager
{
    private static Dictionary<string, Object> m_pCache;

    public static T Load<T>(string sResourcePath) where T : Object
    {
        if (null == m_pCache)
        {
            m_pCache = new Dictionary<string, Object>();
        }
        string sKey = sResourcePath + "_" + typeof (T);
        if (m_pCache.ContainsKey(sKey))
        {
            return (T)m_pCache[sKey];
        }
        T found = LoadFromResourcesFolder<T>(sResourcePath);
        if (null != found)
        {
            m_pCache.Add(sKey, found);    
        }
        return found;
    }

    public static T LoadFromResourcesFolder<T>(string sResourcePath) where T : Object
    {
        return Resources.Load<T>(sResourcePath);
    }

    public static void Clear()
    {
        if (null != m_pCache)
        {
            m_pCache.Clear();    
        }
        m_pCache = null;
        Resources.UnloadUnusedAssets();
    }

    public static void Remove<T>(string sResourcesName)
    {
        string sKey = sResourcesName + "_" + typeof (T);
        if (null != m_pCache && m_pCache.ContainsKey(sKey))
        {
            Resources.UnloadAsset(m_pCache[sKey]);
            m_pCache.Remove(sKey);
        }
    }

    #region Possible AssetBundle

    public static T LoadFromAssetBundle<T>(string sResourcePath) where T : Object
    {
        return null;
    }

    #endregion
}

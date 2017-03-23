using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

//This class is for Psedu-Lua Simplicity
public enum ELuaExpectedType
{
    ELET_Unknown,
    ELET_Int,
    ELET_Float,
    ELET_Bool,
    ELET_Long,
    ELET_String,
    ELET_List,
}

public class CPseduLuaValue
{
    protected ELuaExpectedType m_eType = ELuaExpectedType.ELET_Unknown;

    //private CPseduLuaValue() { /*hide*/ }

    public virtual ELuaExpectedType GetExpectedType()
    {
        return m_eType;
    }

    public virtual object GetExpected()
    {
        return null;
    }

    public new virtual string ToString()
    {
        return "nil";
    }

    public virtual string tostring()
    {
        return ToString();
    }

    public virtual CPseduLuaValue this[CPseduLuaNumber luanumber]
    {
        get { return this;  }
        set { CRuntimeLogger.LogWarning("set v " + value + " to CPseduLuaValue"); }
    }

    public virtual CPseduLuaValue this[int index]
    {
        get { return this; }
        set { CRuntimeLogger.LogWarning("set v " + value + " to CPseduLuaValue"); }
    }

    public virtual CPseduLuaValue this[string sKey]
    {
        get { return this; }
        set { CRuntimeLogger.LogWarning("set v " + value + " to CPseduLuaValue"); }
    }

    public virtual int getn()
    {
        return 0;
    }

    #region Static Casts

    #region Number

    #region int

    public static implicit operator CPseduLuaValue(int fValue)
    {
        CPseduLuaNumber ret = new CPseduLuaNumber();
        ret.SetValueI(fValue);
        return ret;
    }

    public static implicit operator CPseduLuaValue(sint fValue)
    {
        CPseduLuaNumber ret = new CPseduLuaNumber();
        ret.SetValueI(fValue);
        return ret;
    }

    public static implicit operator CPseduLuaValue(byte fValue)
    {
        CPseduLuaNumber ret = new CPseduLuaNumber();
        ret.SetValueI(fValue);
        return ret;
    }

    public static implicit operator CPseduLuaValue(short fValue)
    {
        CPseduLuaNumber ret = new CPseduLuaNumber();
        ret.SetValueI(fValue);
        return ret;
    }

    public static implicit operator CPseduLuaValue(ushort fValue)
    {
        CPseduLuaNumber ret = new CPseduLuaNumber();
        ret.SetValueI(fValue);
        return ret;
    }

    public static implicit operator CPseduLuaValue(long fValue)
    {
        CPseduLuaNumber ret = new CPseduLuaNumber();
        ret.SetValueL(fValue);
        return ret;
    }

    #endregion

    #region float

    public static implicit operator CPseduLuaValue(float fValue)
    {
        CPseduLuaNumber ret = new CPseduLuaNumber();
        ret.SetValueF(fValue);
        return ret;
    }

    public static implicit operator CPseduLuaValue(sfloat fValue)
    {
        CPseduLuaNumber ret = new CPseduLuaNumber();
        ret.SetValueF(fValue);
        return ret;
    }

    #endregion

    #region bool

    public static implicit operator CPseduLuaValue(bool fValue)
    {
        CPseduLuaNumber ret = new CPseduLuaNumber();
        ret.SetValueB(fValue);
        return ret;
    }

    public static implicit operator CPseduLuaValue(sbool fValue)
    {
        CPseduLuaNumber ret = new CPseduLuaNumber();
        ret.SetValueB(fValue);
        return ret;
    }

    #endregion

    #endregion

    #region String

    public static implicit operator CPseduLuaValue(string fValue)
    {
        CPseduLuaString ret = new CPseduLuaString();
        ret.SetValue(fValue);
        ret.m_eType = ELuaExpectedType.ELET_String;
        return ret;
    }

    #endregion

    #region List

    public static implicit operator CPseduLuaValue(object[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        ret.SetValue(fValue);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(List<object> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        ret.SetValue(fValue);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    #region Special Array

    public static implicit operator CPseduLuaValue(int[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(sint[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(uint[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(short[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(ushort[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(long[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(byte[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(float[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(sfloat[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(bool[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(sbool[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(string[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    #endregion

    #region Special List

    public static implicit operator CPseduLuaValue(List<int> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(List<sint> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(List<uint> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(List<short> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(List<ushort> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(List<long> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(List<byte> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(List<float> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(List<sfloat> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(List<bool> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(List<sbool> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(List<string> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    #endregion

    #region PLua

    public static implicit operator CPseduLuaValue(CPseduLuaValue[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(null == fValue[i] ? null : fValue[i].GetExpected()); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(CPseduLuaNumber[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(null == fValue[i] ? null : fValue[i].GetExpected()); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(CPseduLuaString[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(null == fValue[i] ? null : fValue[i].GetExpected()); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(List<CPseduLuaValue> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(null == fValue[i] ? null : fValue[i].GetExpected()); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(List<CPseduLuaNumber> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(null == fValue[i] ? null : fValue[i].GetExpected()); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(List<CPseduLuaString> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(null == fValue[i] ? null : fValue[i].GetExpected()); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    #endregion

    public static implicit operator CPseduLuaValue(Dictionary<string, object> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<string> keys = new List<string>();
        List<object> values = new List<object>();
        foreach (KeyValuePair<string, object> pair in fValue)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
        ret.SetValue(keys.ToArray(), values.ToArray());
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(Dictionary<int, object> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        foreach (KeyValuePair<int, object> pair in fValue)
        {
            CPseduLuaValue v = Create(pair.Value);
            ret[pair.Key] = v;
        }

        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    #region Lua Values List

    public static implicit operator CPseduLuaValue(Dictionary<int, CPseduLuaValue> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        foreach (KeyValuePair<int, CPseduLuaValue> pair in fValue)
        {
            ret[pair.Key] = pair.Value;
        }
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(Dictionary<int, CPseduLuaList> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        foreach (KeyValuePair<int, CPseduLuaList> pair in fValue)
        {
            ret[pair.Key] = pair.Value;
        }
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(Dictionary<int, CPseduLuaNumber> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        foreach (KeyValuePair<int, CPseduLuaNumber> pair in fValue)
        {
            ret[pair.Key] = pair.Value;
        }
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(Dictionary<int, CPseduLuaString> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        foreach (KeyValuePair<int, CPseduLuaString> pair in fValue)
        {
            ret[pair.Key] = pair.Value;
        }
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(Dictionary<string, CPseduLuaValue> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        foreach (KeyValuePair<string, CPseduLuaValue> pair in fValue)
        {
            ret[pair.Key] = pair.Value;
        }
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(Dictionary<string, CPseduLuaList> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        foreach (KeyValuePair<string, CPseduLuaList> pair in fValue)
        {
            ret[pair.Key] = pair.Value;
        }
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(Dictionary<string, CPseduLuaNumber> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        foreach (KeyValuePair<string, CPseduLuaNumber> pair in fValue)
        {
            ret[pair.Key] = pair.Value;
        }
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaValue(Dictionary<string, CPseduLuaString> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        foreach (KeyValuePair<string, CPseduLuaString> pair in fValue)
        {
            ret[pair.Key] = pair.Value;
        }
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    #endregion

    #endregion

    #endregion

    public static CPseduLuaValue Create(object v)
    {
        CPseduLuaValue ret = null;
        if (null == v) { return null;}

        if (v is CPseduLuaValue) { v = ((CPseduLuaValue) v).GetExpected(); }
        if (null == v) { return null; }

        // ReSharper disable CanBeReplacedWithTryCastAndCheckForNull
        if (v is int) { ret = (int)v; }
        if (v is sint) { ret = (sint)v; }
        if (v is uint) { ret = (uint)v; }
        if (v is short) { ret = (short)v; }
        if (v is ushort) { ret = (ushort)v; }
        if (v is long) { ret = (long)v; }
        if (v is byte) { ret = (byte)v; }
        if (v is float) { ret = (float)v; }
        if (v is sfloat) { ret = (sfloat)v; }
        if (v is bool) { ret = (bool)v; }
        if (v is sbool) { ret = (sbool)v; }
        if (v is string) { ret = (string)v; }

        if (v is int[]) { ret = (int[])v; }
        if (v is sint[]) { ret = (sint[])v; }
        if (v is uint[]) { ret = (uint[])v; }
        if (v is short[]) { ret = (short[])v; }
        if (v is ushort[]) { ret = (ushort[])v; }
        if (v is long[]) { ret = (long[])v; }
        if (v is byte[]) { ret = (byte[])v; }
        if (v is float[]) { ret = (float[])v; }
        if (v is sfloat[]) { ret = (sfloat[])v; }
        if (v is bool[]) { ret = (bool[])v; }
        if (v is sbool[]) { ret = (sbool[])v; }
        if (v is string[]) { ret = (string[])v; }
        if (v is object[]) { ret = (object[])v; }

        if (v is List<int>) { ret = (List<int>)v; }
        if (v is List<sint>) { ret = (List<sint>)v; }
        if (v is List<uint>) { ret = (List<uint>)v; }
        if (v is List<short>) { ret = (List<short>)v; }
        if (v is List<ushort>) { ret = (List<ushort>)v; }
        if (v is List<long>) { ret = (List<long>)v; }
        if (v is List<byte>) { ret = (List<byte>)v; }
        if (v is List<float>) { ret = (List<float>)v; }
        if (v is List<sfloat>) { ret = (List<sfloat>)v; }
        if (v is List<bool>) { ret = (List<bool>)v; }
        if (v is List<sbool>) { ret = (List<sbool>)v; }
        if (v is List<string>) { ret = (List<string>)v; }
        if (v is List<object>) { ret = (List<object>)v; }

        if (v is CPseduLuaValue[]) { ret = (CPseduLuaValue[])v; }
        if (v is CPseduLuaNumber[]) { ret = (CPseduLuaNumber[])v; }
        if (v is CPseduLuaString[]) { ret = (CPseduLuaString[])v; }
        if (v is List<CPseduLuaValue>) { ret = (List<CPseduLuaValue>)v; }
        if (v is List<CPseduLuaNumber>) { ret = (List<CPseduLuaNumber>)v; }
        if (v is List<CPseduLuaString>) { ret = (List<CPseduLuaString>)v; }

        if (v is Dictionary<string, object>) { ret = (Dictionary<string, object>)v; }
        if (v is Dictionary<int, object>) { ret = (Dictionary<int, object>)v; }

        if (v is Dictionary<int, CPseduLuaValue>) { ret = (Dictionary<int, CPseduLuaValue>)v; }
        if (v is Dictionary<int, CPseduLuaNumber>) { ret = (Dictionary<int, CPseduLuaNumber>)v; }
        if (v is Dictionary<int, CPseduLuaString>) { ret = (Dictionary<int, CPseduLuaString>)v; }
        if (v is Dictionary<int, CPseduLuaList>) { ret = (Dictionary<int, CPseduLuaList>)v; }

        if (v is Dictionary<string, CPseduLuaValue>) { ret = (Dictionary<string, CPseduLuaValue>)v; }
        if (v is Dictionary<string, CPseduLuaNumber>) { ret = (Dictionary<string, CPseduLuaNumber>)v; }
        if (v is Dictionary<string, CPseduLuaString>) { ret = (Dictionary<string, CPseduLuaString>)v; }
        if (v is Dictionary<string, CPseduLuaList>) { ret = (Dictionary<string, CPseduLuaList>)v; }
        // ReSharper restore CanBeReplacedWithTryCastAndCheckForNull

        return ret;
    }

    // ReSharper disable CanBeReplacedWithTryCastAndCheckForNull
    #region Math Operator

    #region int

    public static CPseduLuaValue operator +(CPseduLuaValue left, int right)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return nleft.GetValueF() + right;
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return nleft.GetValueL() + right;
            }
            return nleft.GetValueI() + right;
        }
        if (left is CPseduLuaString)
        {
            return left.GetExpected() + right.ToString();
        }
        return left;
    }

    public static CPseduLuaValue operator -(CPseduLuaValue left, int right)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return nleft.GetValueF() - right;
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return nleft.GetValueL() - right;
            }
            return nleft.GetValueI() - right;
        }
        return left;
    }

    public static CPseduLuaValue operator *(CPseduLuaValue left, int right)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return nleft.GetValueF() * right;
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return nleft.GetValueL() * right;
            }
            return nleft.GetValueI() * right;
        }
        return left;
    }

    public static CPseduLuaValue operator /(CPseduLuaValue left, int right)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return nleft.GetValueF() / right;
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return nleft.GetValueL() / right;
            }
            return nleft.GetValueI() / right;
        }
        return left;
    }

    public static CPseduLuaValue operator +(int right, CPseduLuaValue left)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return nleft.GetValueF() + right;
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return nleft.GetValueL() + right;
            }
            return nleft.GetValueI() + right;
        }
        if (left is CPseduLuaString)
        {
            return right.ToString() + left.GetExpected();
        }
        return left;
    }

    public static CPseduLuaValue operator -(int right, CPseduLuaValue left)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return right - nleft.GetValueF();
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return right - nleft.GetValueL();
            }
            return right - nleft.GetValueI();
        }
        return left;
    }

    public static CPseduLuaValue operator *(int right, CPseduLuaValue left)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return nleft.GetValueF() * right;
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return right * nleft.GetValueL();
            }
            return nleft.GetValueI() * right;
        }
        return left;
    }

    public static CPseduLuaValue operator /(int right, CPseduLuaValue left)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return right / nleft.GetValueF();
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return right / nleft.GetValueL();
            }
            return right / nleft.GetValueI();
        }
        return left;
    }

    public static CPseduLuaValue operator +(CPseduLuaValue left, uint right)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return nleft.GetValueF() + right;
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return nleft.GetValueL() + right;
            }
            return nleft.GetValueI() + right;
        }
        if (left is CPseduLuaString)
        {
            return left.GetExpected() + right.ToString();
        }
        return left;
    }

    public static CPseduLuaValue operator -(CPseduLuaValue left, uint right)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return nleft.GetValueF() - right;
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return nleft.GetValueL() - right;
            }
            return nleft.GetValueI() - right;
        }
        return left;
    }

    public static CPseduLuaValue operator *(CPseduLuaValue left, uint right)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return nleft.GetValueF() * right;
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return nleft.GetValueL() * right;
            }
            return nleft.GetValueI() * right;
        }
        return left;
    }

    public static CPseduLuaValue operator /(CPseduLuaValue left, uint right)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return nleft.GetValueF() / right;
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return nleft.GetValueL() / right;
            }
            return nleft.GetValueI() / right;
        }
        return left;
    }

    public static CPseduLuaValue operator +(uint right, CPseduLuaValue left)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return nleft.GetValueF() + right;
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return nleft.GetValueL() + right;
            }
            return nleft.GetValueI() + right;
        }
        if (left is CPseduLuaString)
        {
            return right.ToString() + left.GetExpected();
        }
        return left;
    }

    public static CPseduLuaValue operator -(uint right, CPseduLuaValue left)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return right - nleft.GetValueF();
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return right - nleft.GetValueL();
            }
            return right - nleft.GetValueI();
        }
        return left;
    }

    public static CPseduLuaValue operator *(uint right, CPseduLuaValue left)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return nleft.GetValueF() * right;
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return right * nleft.GetValueL();
            }
            return nleft.GetValueI() * right;
        }
        return left;
    }

    public static CPseduLuaValue operator /(uint right, CPseduLuaValue left)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return right / nleft.GetValueF();
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return right / nleft.GetValueL();
            }
            return right / nleft.GetValueI();
        }
        return left;
    }

    public static CPseduLuaValue operator +(CPseduLuaValue left, sint right)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return nleft.GetValueF() + right;
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return nleft.GetValueL() + right;
            }
            return nleft.GetValueI() + right;
        }
        if (left is CPseduLuaString)
        {
            return left.GetExpected() + right.ToString();
        }
        return left;
    }

    public static CPseduLuaValue operator -(CPseduLuaValue left, sint right)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return nleft.GetValueF() - right;
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return nleft.GetValueL() - right;
            }
            return nleft.GetValueI() - right;
        }
        return left;
    }

    public static CPseduLuaValue operator *(CPseduLuaValue left, sint right)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return nleft.GetValueF() * right;
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return nleft.GetValueL() * right;
            }
            return nleft.GetValueI() * right;
        }
        return left;
    }

    public static CPseduLuaValue operator /(CPseduLuaValue left, sint right)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return nleft.GetValueF() / right;
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return nleft.GetValueL() / right;
            }
            return nleft.GetValueI() / right;
        }
        return left;
    }

    public static CPseduLuaValue operator +(sint right, CPseduLuaValue left)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return nleft.GetValueF() + right;
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return nleft.GetValueL() + right;
            }
            return nleft.GetValueI() + right;
        }
        if (left is CPseduLuaString)
        {
            return right.ToString() + left.GetExpected();
        }
        return left;
    }

    public static CPseduLuaValue operator -(sint right, CPseduLuaValue left)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return right - nleft.GetValueF();
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return right - nleft.GetValueL();
            }
            return right - nleft.GetValueI();
        }
        return left;
    }

    public static CPseduLuaValue operator *(sint right, CPseduLuaValue left)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return nleft.GetValueF() * right;
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return right * nleft.GetValueL();
            }
            return nleft.GetValueI() * right;
        }
        return left;
    }

    public static CPseduLuaValue operator /(sint right, CPseduLuaValue left)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return right / nleft.GetValueF();
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return right / nleft.GetValueL();
            }
            return right / nleft.GetValueI();
        }
        return left;
    }

    public static CPseduLuaValue operator +(CPseduLuaValue left, long right)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return nleft.GetValueF() + right;
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return nleft.GetValueL() + right;
            }
            return nleft.GetValueI() + right;
        }
        if (left is CPseduLuaString)
        {
            return left.GetExpected() + right.ToString();
        }
        return left;
    }

    public static CPseduLuaValue operator -(CPseduLuaValue left, long right)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return nleft.GetValueF() - right;
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return nleft.GetValueL() - right;
            }
            return nleft.GetValueI() - right;
        }
        return left;
    }

    public static CPseduLuaValue operator *(CPseduLuaValue left, long right)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return nleft.GetValueF() * right;
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return nleft.GetValueL() * right;
            }
            return nleft.GetValueI() * right;
        }
        return left;
    }

    public static CPseduLuaValue operator /(CPseduLuaValue left, long right)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return nleft.GetValueF() / right;
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return nleft.GetValueL() / right;
            }
            return nleft.GetValueI() / right;
        }
        return left;
    }

    public static CPseduLuaValue operator +(long right, CPseduLuaValue left)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return nleft.GetValueF() + right;
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return nleft.GetValueL() + right;
            }
            return nleft.GetValueI() + right;
        }
        if (left is CPseduLuaString)
        {
            return right.ToString() + left.GetExpected();
        }
        return left;
    }

    public static CPseduLuaValue operator -(long right, CPseduLuaValue left)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return right - nleft.GetValueF();
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return right - nleft.GetValueL();
            }
            return right - nleft.GetValueI();
        }
        return left;
    }

    public static CPseduLuaValue operator *(long right, CPseduLuaValue left)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return nleft.GetValueF() * right;
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return right * nleft.GetValueL();
            }
            return nleft.GetValueI() * right;
        }
        return left;
    }

    public static CPseduLuaValue operator /(long right, CPseduLuaValue left)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Float)
            {
                return right / nleft.GetValueF();
            }
            if (nleft.GetExpectedType() == ELuaExpectedType.ELET_Long)
            {
                return right / nleft.GetValueL();
            }
            return right / nleft.GetValueI();
        }
        return left;
    }
    
    #endregion

    #region float

    public static CPseduLuaValue operator +(CPseduLuaValue left, float right)
    {
        
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            return nleft.GetValueF() + right;
        }
        if (left is CPseduLuaString)
        {
            return left.GetExpected() + right.ToString(CultureInfo.InvariantCulture);
        }
        return left;
    }

    public static CPseduLuaValue operator -(CPseduLuaValue left, float right)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            return nleft.GetValueF() - right;
        }
        return left;
    }

    public static CPseduLuaValue operator *(CPseduLuaValue left, float right)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            return nleft.GetValueF() * right;
        }
        return left;
    }

    public static CPseduLuaValue operator /(CPseduLuaValue left, float right)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            return nleft.GetValueF() / right;
        }
        return left;
    }

    public static CPseduLuaValue operator +(float right, CPseduLuaValue left)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            return nleft.GetValueF() + right;
        }
        if (left is CPseduLuaString)
        {
            return right.ToString(CultureInfo.InvariantCulture) + left.GetExpected();
        }
        return left;
    }

    public static CPseduLuaValue operator -(float right, CPseduLuaValue left)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            return right - nleft.GetValueF();
        }
        return left;
    }

    public static CPseduLuaValue operator *(float right, CPseduLuaValue left)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            return nleft.GetValueF() * right;
        }
        return left;
    }

    public static CPseduLuaValue operator /(float right, CPseduLuaValue left)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            return right / nleft.GetValueF();
        }
        return left;
    }

    public static CPseduLuaValue operator +(CPseduLuaValue left, sfloat right)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            return nleft.GetValueF() * right;
        }
        if (left is CPseduLuaString)
        {
            return left.GetExpected() + right.ToString();
        }
        return left;
    }

    public static CPseduLuaValue operator -(CPseduLuaValue left, sfloat right)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            return nleft.GetValueF() - right;
        }
        return left;
    }

    public static CPseduLuaValue operator *(CPseduLuaValue left, sfloat right)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            return nleft.GetValueF() * right;
        }
        return left;
    }

    public static CPseduLuaValue operator /(CPseduLuaValue left, sfloat right)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            return nleft.GetValueF() / right;
        }
        return left;
    }

    public static CPseduLuaValue operator +(sfloat right, CPseduLuaValue left)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            return nleft.GetValueF() * right;
        }
        if (left is CPseduLuaString)
        {
            return right.ToString() + left.GetExpected();
        }
        return left;
    }

    public static CPseduLuaValue operator -(sfloat right, CPseduLuaValue left)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            return right - nleft.GetValueF();
        }
        return left;
    }

    public static CPseduLuaValue operator *(sfloat right, CPseduLuaValue left)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            return nleft.GetValueF() * right;
        }
        return left;
    }

    public static CPseduLuaValue operator /(sfloat right, CPseduLuaValue left)
    {
        if (left is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            return right / nleft.GetValueF();
        }
        return left;
    }

    #endregion

    #region string

    public static CPseduLuaValue operator +(CPseduLuaValue left, string right)
    {
        return left.ToString() + right;
    }

    public static CPseduLuaValue operator +(string left, CPseduLuaValue right)
    {
        return left + right.ToString();
    }

    #endregion

    #region object

    public static CPseduLuaValue operator +(CPseduLuaValue left, object right)
    {
        if (right is int) { return left + (int) right;}
        if (right is uint) { return left + (uint)right; }
        if (right is sint) { return left + (sint)right; }
        if (right is byte) { return left + (int)(byte)right; }
        if (right is short) { return left + (int)(short)right; }
        if (right is ushort) { return left + (int)(ushort)right; }
        if (right is long) { return left + (long)right; }
        if (right is float) { return left + (float)right; }
        if (right is sfloat) { return left + (sfloat)right; }
        if (right is string) { return left + (string)right; }
        if (right is CPseduLuaValue) { return left + (CPseduLuaValue)right; }
        return left;
    }

    public static CPseduLuaValue operator -(CPseduLuaValue left, object right)
    {
        if (right is int) { return left - (int)right; }
        if (right is uint) { return left - (uint)right; }
        if (right is sint) { return left - (sint)right; }
        if (right is byte) { return left - (int)(byte)right; }
        if (right is short) { return left - (int)(short)right; }
        if (right is ushort) { return left - (int)(ushort)right; }
        if (right is long) { return left - (long)right; }
        if (right is float) { return left - (float)right; }
        if (right is sfloat) { return left - (sfloat)right; }
        if (right is string) { return left - (string)right; }
        if (right is CPseduLuaValue) { return left - (CPseduLuaValue)right; }
        return left;
    }

    public static CPseduLuaValue operator *(CPseduLuaValue left, object right)
    {
        if (right is int) { return left * (int)right; }
        if (right is uint) { return left * (uint)right; }
        if (right is sint) { return left * (sint)right; }
        if (right is byte) { return left * (int)(byte)right; }
        if (right is short) { return left * (int)(short)right; }
        if (right is ushort) { return left * (int)(ushort)right; }
        if (right is long) { return left * (long)right; }
        if (right is float) { return left * (float)right; }
        if (right is sfloat) { return left * (sfloat)right; }
        if (right is string) { return left * (string)right; }
        if (right is CPseduLuaValue) { return left * (CPseduLuaValue)right; }
        return left;
    }

    public static CPseduLuaValue operator /(CPseduLuaValue left, object right)
    {
        if (right is int) { return left / (int)right; }
        if (right is uint) { return left / (uint)right; }
        if (right is sint) { return left / (sint)right; }
        if (right is byte) { return left / (int)(byte)right; }
        if (right is short) { return left / (int)(short)right; }
        if (right is ushort) { return left / (int)(ushort)right; }
        if (right is long) { return left / (long)right; }
        if (right is float) { return left / (float)right; }
        if (right is sfloat) { return left / (sfloat)right; }
        if (right is string) { return left / (string)right; }
        if (right is CPseduLuaValue) { return left / (CPseduLuaValue)right; }
        return left;
    }

    public static CPseduLuaValue operator +(object right, CPseduLuaValue left)
    {
        if (right is int) { return (int)right + left; }
        if (right is uint) { return (uint)right + left; }
        if (right is sint) { return (sint)right + left; }
        if (right is byte) { return (int)(byte)right + left; }
        if (right is short) { return (int)(short)right + left; }
        if (right is ushort) { return (int)(ushort)right + left; }
        if (right is long) { return (long)right + left; }
        if (right is float) { return (float)right + left; }
        if (right is sfloat) { return (sfloat)right + left; }
        if (right is string) { return (string)right + left; }
        if (right is CPseduLuaValue) { return (CPseduLuaValue)right + left; }
        return left;
    }

    public static CPseduLuaValue operator -(object right, CPseduLuaValue left)
    {
        if (right is int) { return (int)right - left; }
        if (right is uint) { return (uint)right - left; }
        if (right is sint) { return (sint)right - left; }
        if (right is byte) { return (int)(byte)right - left; }
        if (right is short) { return (int)(short)right - left; }
        if (right is ushort) { return (int)(ushort)right - left; }
        if (right is long) { return (long)right - left; }
        if (right is float) { return (float)right - left; }
        if (right is sfloat) { return (sfloat)right - left; }
        if (right is string) { return (string)right - left; }
        if (right is CPseduLuaValue) { return (CPseduLuaValue)right - left; }
        return left;
    }

    public static CPseduLuaValue operator *(object right, CPseduLuaValue left)
    {
        if (right is int) { return (int)right * left; }
        if (right is uint) { return (uint)right * left; }
        if (right is sint) { return (sint)right * left; }
        if (right is byte) { return (int)(byte)right * left; }
        if (right is short) { return (int)(short)right * left; }
        if (right is ushort) { return (int)(ushort)right * left; }
        if (right is long) { return (long)right * left; }
        if (right is float) { return (float)right * left; }
        if (right is sfloat) { return (sfloat)right * left; }
        if (right is string) { return (string)right * left; }
        if (right is CPseduLuaValue) { return (CPseduLuaValue)right * left; }
        return left;
    }

    public static CPseduLuaValue operator /(object right, CPseduLuaValue left)
    {
        if (right is int) { return (int)right / left; }
        if (right is uint) { return (uint)right / left; }
        if (right is sint) { return (sint)right / left; }
        if (right is byte) { return (int)(byte)right / left; }
        if (right is short) { return (int)(short)right / left; }
        if (right is ushort) { return (int)(ushort)right / left; }
        if (right is long) { return (long)right / left; }
        if (right is float) { return (float)right / left; }
        if (right is sfloat) { return (sfloat)right / left; }
        if (right is string) { return (string)right / left; }
        if (right is CPseduLuaValue) { return (CPseduLuaValue)right / left; }
        return left;
    }

    #endregion

    public static CPseduLuaValue operator +(CPseduLuaValue left, CPseduLuaValue right)
    {
        if (null == left)
        {
            return right;
        }
        if (null == right)
        {
            return left;
        }

        if (left is CPseduLuaNumber && right is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            CPseduLuaNumber nright = (CPseduLuaNumber)right;
            ELuaExpectedType eType1 = nleft.GetExpectedType();
            ELuaExpectedType eType2 = nright.GetExpectedType();
            if ((ELuaExpectedType.ELET_Bool == eType1 || ELuaExpectedType.ELET_Int == eType1)
             && (ELuaExpectedType.ELET_Bool == eType2 || ELuaExpectedType.ELET_Int == eType2))
            {
                return nleft.GetValueI() + nright.GetValueI();
            }
            if ((ELuaExpectedType.ELET_Bool == eType1 || ELuaExpectedType.ELET_Int == eType1 || ELuaExpectedType.ELET_Long == eType1)
             && (ELuaExpectedType.ELET_Bool == eType2 || ELuaExpectedType.ELET_Int == eType2 || ELuaExpectedType.ELET_Long == eType2))
            {
                return nleft.GetValueL() + nright.GetValueL();
            }
            return nleft.GetValueF() + nright.GetValueF(); 
        }

        if (left is CPseduLuaString || right is CPseduLuaString)
        {
            return left.ToString() + right.ToString();
        }
        return left;
    }

    public static CPseduLuaValue operator -(CPseduLuaValue left, CPseduLuaValue right)
    {
        if (null == left)
        {
            return right;
        }
        if (null == right)
        {
            return left;
        }

        if (left is CPseduLuaNumber && right is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber) left;
            CPseduLuaNumber nright = (CPseduLuaNumber) right;
            ELuaExpectedType eType1 = nleft.GetExpectedType();
            ELuaExpectedType eType2 = nright.GetExpectedType();
            if ((ELuaExpectedType.ELET_Bool == eType1 || ELuaExpectedType.ELET_Int == eType1)
             && (ELuaExpectedType.ELET_Bool == eType2 || ELuaExpectedType.ELET_Int == eType2))
            {
                return nleft.GetValueI() - nright.GetValueI();
            }
            if ((ELuaExpectedType.ELET_Bool == eType1 || ELuaExpectedType.ELET_Int == eType1 || ELuaExpectedType.ELET_Long == eType1)
             && (ELuaExpectedType.ELET_Bool == eType2 || ELuaExpectedType.ELET_Int == eType2 || ELuaExpectedType.ELET_Long == eType2))
            {
                return nleft.GetValueL() - nright.GetValueL();
            }
            return nleft.GetValueF() - nright.GetValueF();
        }
        return left;
    }

    public static CPseduLuaValue operator *(CPseduLuaValue left, CPseduLuaValue right)
    {
        if (null == left)
        {
            return right;
        }
        if (null == right)
        {
            return left;
        }

        if (left is CPseduLuaNumber && right is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            CPseduLuaNumber nright = (CPseduLuaNumber)right;
            ELuaExpectedType eType1 = nleft.GetExpectedType();
            ELuaExpectedType eType2 = nright.GetExpectedType();
            if ((ELuaExpectedType.ELET_Bool == eType1 || ELuaExpectedType.ELET_Int == eType1)
             && (ELuaExpectedType.ELET_Bool == eType2 || ELuaExpectedType.ELET_Int == eType2))
            {
                return nleft.GetValueI() * nright.GetValueI();
            }
            if ((ELuaExpectedType.ELET_Bool == eType1 || ELuaExpectedType.ELET_Int == eType1 || ELuaExpectedType.ELET_Long == eType1)
             && (ELuaExpectedType.ELET_Bool == eType2 || ELuaExpectedType.ELET_Int == eType2 || ELuaExpectedType.ELET_Long == eType2))
            {
                return nleft.GetValueL() * nright.GetValueL();
            }
            return nleft.GetValueF() * nright.GetValueF();
        }
        return left;
    }

    public static CPseduLuaValue operator /(CPseduLuaValue left, CPseduLuaValue right)
    {
        if (null == left)
        {
            return right;
        }
        if (null == right)
        {
            return left;
        }

        if (left is CPseduLuaNumber && right is CPseduLuaNumber)
        {
            CPseduLuaNumber nleft = (CPseduLuaNumber)left;
            CPseduLuaNumber nright = (CPseduLuaNumber)right;
            ELuaExpectedType eType1 = nleft.GetExpectedType();
            ELuaExpectedType eType2 = nright.GetExpectedType();
            if ((ELuaExpectedType.ELET_Bool == eType1 || ELuaExpectedType.ELET_Int == eType1)
             && (ELuaExpectedType.ELET_Bool == eType2 || ELuaExpectedType.ELET_Int == eType2))
            {
                return nleft.GetValueI() / nright.GetValueI();
            }
            if ((ELuaExpectedType.ELET_Bool == eType1 || ELuaExpectedType.ELET_Int == eType1 || ELuaExpectedType.ELET_Long == eType1)
             && (ELuaExpectedType.ELET_Bool == eType2 || ELuaExpectedType.ELET_Int == eType2 || ELuaExpectedType.ELET_Long == eType2))
            {
                return nleft.GetValueL() / nright.GetValueL();
            }
            return nleft.GetValueF() / nright.GetValueF();
        }
        return left;
    }

    public static CPseduLuaValue operator ++(CPseduLuaValue number)
    {
        switch (number.GetExpectedType())
        {
            case ELuaExpectedType.ELET_Int:
                int i1 = (int)number.GetExpected();
                return i1 + 1;
            case ELuaExpectedType.ELET_Long:
                long l1 = (long)number.GetExpected();
                return l1 + 1;
            case ELuaExpectedType.ELET_Bool:
                return true;
            case ELuaExpectedType.ELET_Float:
                float f1 = (float)number.GetExpected();
                return f1 + 1.0f;
        }
        return number;
    }

    #endregion

    #region Compara

    public static bool operator <(CPseduLuaValue v1, CPseduLuaValue v2)
    {
        if (v1 is CPseduLuaNumber && v2 is CPseduLuaNumber)
        {
            CPseduLuaNumber number1 = (CPseduLuaNumber)v1;
            CPseduLuaNumber number2 = (CPseduLuaNumber)v2;
            if (ELuaExpectedType.ELET_Float == number1.GetExpectedType() && ELuaExpectedType.ELET_Float == number2.GetExpectedType())
            {
                return Mathf.RoundToInt(number1.GetValueF() * 10000.0f) < Mathf.RoundToInt(number2.GetValueF() * 10000.0f);
            }
            if (ELuaExpectedType.ELET_Int == number1.GetExpectedType() && ELuaExpectedType.ELET_Float == number2.GetExpectedType())
            {
                return (number1.GetValueI() * 10000) < Mathf.RoundToInt(number2.GetValueF() * 10000.0f);
            }
            if (ELuaExpectedType.ELET_Long == number1.GetExpectedType() && ELuaExpectedType.ELET_Float == number2.GetExpectedType())
            {
                return (number1.GetValueL() * 10000) < Mathf.RoundToInt(number2.GetValueF() * 10000.0f);
            }

            if (ELuaExpectedType.ELET_Float == number1.GetExpectedType() && ELuaExpectedType.ELET_Int == number2.GetExpectedType())
            {
                return Mathf.RoundToInt(number1.GetValueF() * 10000.0f) < (number2.GetValueI() * 10000);
            }
            if (ELuaExpectedType.ELET_Float == number1.GetExpectedType() && ELuaExpectedType.ELET_Long == number2.GetExpectedType())
            {
                return Mathf.RoundToInt(number1.GetValueF() * 10000.0f) < (number2.GetValueL() * 10000);
            }

            if (ELuaExpectedType.ELET_Int == number1.GetExpectedType() && ELuaExpectedType.ELET_Int == number2.GetExpectedType())
            {
                return number1.GetValueI() < number2.GetValueI();
            }
            if (ELuaExpectedType.ELET_Long == number1.GetExpectedType() && ELuaExpectedType.ELET_Int == number2.GetExpectedType())
            {
                return number1.GetValueL() < number2.GetValueI();
            }

            if (ELuaExpectedType.ELET_Long == number1.GetExpectedType() && ELuaExpectedType.ELET_Long == number2.GetExpectedType())
            {
                return number1.GetValueL() < number2.GetValueL();
            }

            return number1.GetValueI() < number2.GetValueI();            
        }
        if (v1 is CPseduLuaString && v2 is CPseduLuaString)
        {
            return v1.ToString().Length < v2.ToString().Length;
        }
        if (v1 is CPseduLuaList && v2 is CPseduLuaList)
        {
            return v1.getn() < v2.getn();
        }
        return false;
    }

    public static bool operator >(CPseduLuaValue number1, CPseduLuaValue number2)
    {
        return number2 < number1;
    }

    public static bool operator ==(CPseduLuaValue v1, CPseduLuaValue v2)
    {
        if (ReferenceEquals(null, v1) && ReferenceEquals(null, v2))
        {
            return true;
        }
        if (ReferenceEquals(null, v1))
        {
            return false;
        }
        if (ReferenceEquals(null, v2))
        {
            return false;
        }
        if (ReferenceEquals(v1, v2))
        {
            return true;
        }

        if (v1 is CPseduLuaNumber && v2 is CPseduLuaNumber)
        {
            CPseduLuaNumber number1 = (CPseduLuaNumber)v1;
            CPseduLuaNumber number2 = (CPseduLuaNumber)v2;
            if (ELuaExpectedType.ELET_Float == number1.GetExpectedType() &&
                ELuaExpectedType.ELET_Float == number2.GetExpectedType())
            {
                return Mathf.RoundToInt(number1.GetValueF()*10000.0f) == Mathf.RoundToInt(number2.GetValueF()*10000.0f);
            }
            if (ELuaExpectedType.ELET_Int == number1.GetExpectedType() &&
                ELuaExpectedType.ELET_Float == number2.GetExpectedType())
            {
                return (number1.GetValueI()*10000) == Mathf.RoundToInt(number2.GetValueF()*10000.0f);
            }
            if (ELuaExpectedType.ELET_Long == number1.GetExpectedType() &&
                ELuaExpectedType.ELET_Float == number2.GetExpectedType())
            {
                return (number1.GetValueL()*10000) == Mathf.RoundToInt(number2.GetValueF()*10000.0f);
            }

            if (ELuaExpectedType.ELET_Int == number1.GetExpectedType() &&
                ELuaExpectedType.ELET_Int == number2.GetExpectedType())
            {
                return number1.GetValueI() == number2.GetValueI();
            }
            if (ELuaExpectedType.ELET_Long == number1.GetExpectedType() &&
                ELuaExpectedType.ELET_Int == number2.GetExpectedType())
            {
                return number1.GetValueL() == number2.GetValueI();
            }

            if (ELuaExpectedType.ELET_Long == number1.GetExpectedType() &&
                ELuaExpectedType.ELET_Long == number2.GetExpectedType())
            {
                return number1.GetValueL() == number2.GetValueL();
            }

            return number1.GetValueI() == number2.GetValueI();
        }
        if (v1 is CPseduLuaString && v2 is CPseduLuaString)
        {
            return v1.ToString() == v2.ToString();
        }
        return false;
    }

    public static bool operator !=(CPseduLuaValue number1, CPseduLuaValue number2)
    {
        return !(number2 == number1);
    }

    public static bool operator >=(CPseduLuaValue number1, CPseduLuaValue number2)
    {
        return (number2 == number1) || (number1 > number2);
    }

    public static bool operator <=(CPseduLuaValue number1, CPseduLuaValue number2)
    {
        return (number2 == number1) || (number1 < number2);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ToString().GetHashCode();
        }
    }

    public bool Equals(CPseduLuaValue other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this == other;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        if (obj.GetType() != typeof(CPseduLuaValue))
        {
            return false;
        }
        return Equals((CPseduLuaValue)obj);
    }

    public static bool operator ==(CPseduLuaValue v1, object v2) { return v1 == Create(v2); }
    public static bool operator !=(CPseduLuaValue v1, object v2) { return !(v1 == Create(v2)); }
    public static bool operator ==(object v1, CPseduLuaValue v2) { return v2 == Create(v1); }
    public static bool operator !=(object v1, CPseduLuaValue v2) { return !(v2 == Create(v1)); }

    #endregion

    #region Cast Back

    public static implicit operator string(CPseduLuaValue value)
    {
        if (null == value)
        {
            return "null";
        }
        return value.ToString();
    }

    public static implicit operator float(CPseduLuaValue sfValue)
    {
        if (sfValue is CPseduLuaNumber)
        {
            return ((CPseduLuaNumber) sfValue).GetValueF();
        }
        return 0.0f;
    }

    public static implicit operator sfloat(CPseduLuaValue sfValue)
    {
        if (sfValue is CPseduLuaNumber)
        {
            return ((CPseduLuaNumber)sfValue).GetValueF();
        }
        return 0.0f;
    }

    public static implicit operator int(CPseduLuaValue sfValue)
    {
        if (sfValue is CPseduLuaNumber)
        {
            return ((CPseduLuaNumber)sfValue).GetValueI();
        }
        return 0;
    }

    public static implicit operator sint(CPseduLuaValue sfValue)
    {
        if (sfValue is CPseduLuaNumber)
        {
            return ((CPseduLuaNumber)sfValue).GetValueI();
        }
        return 0;
    }

    public static implicit operator bool(CPseduLuaValue sfValue)
    {
        if (sfValue is CPseduLuaNumber)
        {
            return ((CPseduLuaNumber)sfValue).GetValueB();
        }
        return false;
    }

    public static implicit operator sbool(CPseduLuaValue sfValue)
    {
        if (sfValue is CPseduLuaNumber)
        {
            return ((CPseduLuaNumber)sfValue).GetValueB();
        }
        return false;
    }

    public static implicit operator long(CPseduLuaValue sfValue)
    {
        if (sfValue is CPseduLuaNumber)
        {
            return ((CPseduLuaNumber)sfValue).GetValueL();
        }
        return 0;
    }

    public static implicit operator object[](CPseduLuaValue sfValue)
    {
        if (sfValue is CPseduLuaList)
        {
            return ((CPseduLuaList)sfValue).GetValue();    
        }
        return new []{sfValue.GetExpected()};
    }

    public static implicit operator Dictionary<string, object>(CPseduLuaValue sfValue)
    {
        return sfValue as CPseduLuaList;
    }

    public static implicit operator Dictionary<int, object>(CPseduLuaValue sfValue)
    {
        if (sfValue is CPseduLuaList)
        {
            object[] objlst = ((CPseduLuaList)sfValue).GetValue();
            Dictionary<int, object> ret = new Dictionary<int, object>(new IntEqualityComparer());
            if (null != objlst)
            {
                for (int i = 0; i < objlst.Length; ++i)
                {
                    if (null != objlst[i] && (!(objlst[i] is CPseduLuaValue)))
                    {
                        ret.Add(i, objlst[i]);
                    }
                }                
            }
            return ret;
        }
        return new Dictionary<int, object>(new IntEqualityComparer()) { {0, sfValue.GetExpected()} };
    }

    #endregion

    // ReSharper restore CanBeReplacedWithTryCastAndCheckForNull
}

public class CPseduLuaNumber : CPseduLuaValue
{
    private long _v1;
    private long _v2;

    public CPseduLuaNumber() //: base()
    {
        m_eType = ELuaExpectedType.ELET_Int;
        _v1 = 0;
        _v2 = 0;
    }

    public override object GetExpected()
    {
        switch (GetExpectedType())
        {
            case ELuaExpectedType.ELET_Int:
                int iValue = GetValueI();
                return iValue;
            //break;
            case ELuaExpectedType.ELET_Long:
                long lValue = GetValueL();
                return lValue;
            //break;
            case ELuaExpectedType.ELET_Bool:
                bool bValue = GetValueB();
                return bValue;
            //break;
        }
        float fValue = GetValueF();
        return fValue;
    }

    public CPseduLuaNumber(object value)
    {
        CPseduLuaNumber number = value as CPseduLuaNumber;
        if (null != number)
        {
            switch (number.GetExpectedType())
            {
                case ELuaExpectedType.ELET_Float:
                    SetValueF(number.GetValueF());
                    m_eType = ELuaExpectedType.ELET_Float;
                    break;
                case ELuaExpectedType.ELET_Long:
                    SetValueL(number.GetValueL());
                    m_eType = ELuaExpectedType.ELET_Long;
                    break;
                case ELuaExpectedType.ELET_Int:
                    SetValueI(number.GetValueI());
                    m_eType = ELuaExpectedType.ELET_Int;
                    break;
                case ELuaExpectedType.ELET_Bool:
                    SetValueB(number.GetValueB());
                    m_eType = ELuaExpectedType.ELET_Bool;
                    break;
                default:
                    SetValueF(number.GetValueF());
                    m_eType = ELuaExpectedType.ELET_Float;
                    break;
            }
        }
        else if (value is long)
        {
            SetValueL((long)value);
            m_eType = ELuaExpectedType.ELET_Long;
        }
        else if (value is ulong)
        {
            SetValueL((long)(ulong)value);
            m_eType = ELuaExpectedType.ELET_Long;
        }
        else if (value is uint)
        {
            SetValueI((int)(uint)value);
            m_eType = ELuaExpectedType.ELET_Int;
        }
        else if (value is int)
        {
            SetValueI((int)value);
            m_eType = ELuaExpectedType.ELET_Int;
        }
        else if (value is sint)
        {
            SetValueI((sint)value);
            m_eType = ELuaExpectedType.ELET_Int;
        }
        else if (value is short)
        {
            SetValueI((short)value);
            m_eType = ELuaExpectedType.ELET_Int;
        }
        else if (value is ushort)
        {
            SetValueI((ushort)value);
            m_eType = ELuaExpectedType.ELET_Int;
        }
        else if (value is byte)
        {
            SetValueI((byte)value);
            m_eType = ELuaExpectedType.ELET_Int;
        }
        else if (value is double)
        {
            SetValueF((float)(double)value);
            m_eType = ELuaExpectedType.ELET_Float;
        }
        else if (value is float)
        {
            SetValueF((float)value);
            m_eType = ELuaExpectedType.ELET_Float;
        }
        else if (value is sfloat)
        {
            SetValueF((sfloat)value);
            m_eType = ELuaExpectedType.ELET_Float;
        }
        else if (value is bool)
        {
            SetValueB((bool)value);
            m_eType = ELuaExpectedType.ELET_Bool;
        }
        else if (value is sbool)
        {
            SetValueB((sbool)value);
            m_eType = ELuaExpectedType.ELET_Bool;
        }
        else
        {
            SetValueI(-1);
            m_eType = ELuaExpectedType.ELET_Int;
        }
    }

    #region Get And Set

    public void SetValueF(float fValue)
    {
        m_eType = ELuaExpectedType.ELET_Float;
        _v1 = Mathf.RoundToInt(CRandom.Range(-1.0f, 2.0f) * fValue * 9876.54f);
        _v2 = Mathf.RoundToInt(fValue * 9876.54f - _v1);
    }

    public float GetValueF()
    {
        switch (m_eType)
        {
            case ELuaExpectedType.ELET_Int:
                return GetValueI();
            case ELuaExpectedType.ELET_Bool:
                return GetValueB() ? 1.0f : 0.0f;
            case ELuaExpectedType.ELET_Long:
                return GetValueL();
        }
        return (_v1 + _v2) / 9876.54f;
    }

    public void SetValueI(int fValue)
    {
        m_eType = ELuaExpectedType.ELET_Int;
        _v1 = CRandom.Range(-1000000, 1000000);
        _v2 = fValue - _v1;
    }

    public int GetValueI()
    {
        switch (m_eType)
        {
            case ELuaExpectedType.ELET_Float:
                return Mathf.RoundToInt(GetValueF());
            case ELuaExpectedType.ELET_Bool:
                return GetValueB() ? 1 : 0;
            case ELuaExpectedType.ELET_Long:
                return (int)GetValueL();
        }
        return (int)(_v1 + _v2);
    }

    public void SetValueB(bool bValue)
    {
        SetValueI(bValue ? CRandom.Range(1, 9876) : 0);
        m_eType = ELuaExpectedType.ELET_Bool;
    }

    public bool GetValueB()
    {
        switch (m_eType)
        {
            case ELuaExpectedType.ELET_Float:
                return true;
            case ELuaExpectedType.ELET_Int:
                return GetValueI() != 0;
            case ELuaExpectedType.ELET_Long:
                return GetValueL() != 0;
        }
        return 0 != (int)(_v1 + _v2);
    }

    public void SetValueL(long fValue)
    {
        m_eType = ELuaExpectedType.ELET_Long;
        _v1 = CRandom.Range(-1000000, 1000000);
        _v2 = fValue - _v1;
    }

    public long GetValueL()
    {
        switch (m_eType)
        {
            case ELuaExpectedType.ELET_Float:
                return Mathf.RoundToInt(GetValueF());
            case ELuaExpectedType.ELET_Bool:
                return GetValueB() ? 1 : 0;
            case ELuaExpectedType.ELET_Int:
                return GetValueI();
        }
        return _v1 + _v2;
    }

    #endregion

    #region Cast Operators

    public static implicit operator CPseduLuaNumber(float fValue)
    {
        CPseduLuaNumber ret = new CPseduLuaNumber();
        ret.SetValueF(fValue);
        return ret;
    }

    public static implicit operator float(CPseduLuaNumber sfValue)
    {
        return sfValue.GetValueF();
    }

    public static implicit operator CPseduLuaNumber(sfloat fValue)
    {
        CPseduLuaNumber ret = new CPseduLuaNumber();
        ret.SetValueF(fValue);
        return ret;
    }

    public static implicit operator sfloat(CPseduLuaNumber sfValue)
    {
        return sfValue.GetValueF();
    }

    public static implicit operator CPseduLuaNumber(int fValue)
    {
        CPseduLuaNumber ret = new CPseduLuaNumber();
        ret.SetValueI(fValue);
        return ret;
    }

    public static implicit operator int(CPseduLuaNumber sfValue)
    {
        return sfValue.GetValueI();
    }

    public static implicit operator CPseduLuaNumber(sint fValue)
    {
        CPseduLuaNumber ret = new CPseduLuaNumber();
        ret.SetValueI(fValue);
        return ret;
    }

    public static implicit operator sint(CPseduLuaNumber sfValue)
    {
        return sfValue.GetValueI();
    }

    public static implicit operator CPseduLuaNumber(bool fValue)
    {
        CPseduLuaNumber ret = new CPseduLuaNumber();
        ret.SetValueB(fValue);
        return ret;
    }

    public static implicit operator CPseduLuaNumber(sbool fValue)
    {
        CPseduLuaNumber ret = new CPseduLuaNumber();
        ret.SetValueB(fValue);
        ret.m_eType = ELuaExpectedType.ELET_Bool;
        return ret;
    }

    public static implicit operator bool(CPseduLuaNumber sfValue)
    {
        return sfValue.GetValueB();
    }

    public static implicit operator CPseduLuaNumber(long fValue)
    {
        CPseduLuaNumber ret = new CPseduLuaNumber();
        ret.SetValueL(fValue);
        ret.m_eType = ELuaExpectedType.ELET_Long;
        return ret;
    }

    public static implicit operator long(CPseduLuaNumber sfValue)
    {
        return sfValue.GetValueL();
    }

    public static implicit operator CPseduLuaList(CPseduLuaNumber sfValue)
    {
        switch (sfValue.GetExpectedType())
        {
            case ELuaExpectedType.ELET_Int:
                int iValue = sfValue.GetValueI();
                return new object[] {iValue};
            //break;
            case ELuaExpectedType.ELET_Long:
                long lValue = sfValue.GetValueL();
                return new object [] {lValue};
            //break;
            case ELuaExpectedType.ELET_Bool:
                bool bValue = sfValue.GetValueB();
                return new object[] {bValue};
            //break;
        }
        float fValue = sfValue.GetValueF();
        return new object[] { fValue };
    }

    public override string ToString()
    {
        switch (GetExpectedType())
        {
            case ELuaExpectedType.ELET_Int:
                int iValue = GetValueI();
                return iValue.ToString(CultureInfo.InvariantCulture);
                //break;
            case ELuaExpectedType.ELET_Long:
                long lValue = GetValueL();
                return lValue.ToString(CultureInfo.InvariantCulture);
                //break;
            case ELuaExpectedType.ELET_Bool:
                bool bValue = GetValueB();
                return bValue.ToString(CultureInfo.InvariantCulture);
                //break;
        }
        float fValue = GetValueF();
        return fValue.ToString(CultureInfo.InvariantCulture);
    }

    public static implicit operator CPseduLuaString(CPseduLuaNumber value)
    {
        return value.ToString();
    }

    #endregion

    #region List Operators

    public override CPseduLuaValue this[CPseduLuaNumber luanumber]
    {
        get { return this[(int)luanumber]; }
        set { this[(int) luanumber] = value; }
    }

    public override CPseduLuaValue this[int index]
    {
        get 
        {
            if (0 != index)
            {
                CRuntimeLogger.LogWarning("index out of range--Psedu Lua Number");
            }
            switch (GetExpectedType())
            {
                case ELuaExpectedType.ELET_Int:
                    int iValue = GetValueI();
                    return iValue;
                //break;
                case ELuaExpectedType.ELET_Long:
                    long lValue = GetValueL();
                    return lValue;
                //break;
                case ELuaExpectedType.ELET_Bool:
                    bool bValue = GetValueB();
                    return bValue;
                //break;
            }
            float fValue = GetValueF();
            return fValue.ToString(CultureInfo.InvariantCulture);
        }
        set
        {
            if (0 != index)
            {
                CRuntimeLogger.LogWarning("index out of range--Psedu Lua Number");
            }
            if (null == value)
            {
                SetValueI(0);
                m_eType = ELuaExpectedType.ELET_Int;
                return;
            }
            object vvalue = value.GetExpected();
            if (vvalue is long)
            {
                SetValueL((long)vvalue);
                m_eType = ELuaExpectedType.ELET_Long;
            }
            else if (vvalue is ulong)
            {
                SetValueL((long)(ulong)vvalue);
                m_eType = ELuaExpectedType.ELET_Long;
            }
            else if (vvalue is uint)
            {
                SetValueI((int)(uint)vvalue);
                m_eType = ELuaExpectedType.ELET_Int;
            }
            else if (vvalue is int)
            {
                SetValueI((int)vvalue);
                m_eType = ELuaExpectedType.ELET_Int;
            }
            else if (vvalue is sint)
            {
                SetValueI((sint)vvalue);
                m_eType = ELuaExpectedType.ELET_Int;
            }
            else if (vvalue is short)
            {
                SetValueI((short)vvalue);
                m_eType = ELuaExpectedType.ELET_Int;
            }
            else if (vvalue is ushort)
            {
                SetValueI((ushort)vvalue);
                m_eType = ELuaExpectedType.ELET_Int;
            }
            else if (vvalue is byte)
            {
                SetValueI((byte)vvalue);
                m_eType = ELuaExpectedType.ELET_Int;
            }
            else if (vvalue is double)
            {
                SetValueF((float)(double)vvalue);
                m_eType = ELuaExpectedType.ELET_Float;
            }
            else if (vvalue is float)
            {
                SetValueF((float)vvalue);
                m_eType = ELuaExpectedType.ELET_Float;
            }
            else if (vvalue is sfloat)
            {
                SetValueF((sfloat)vvalue);
                m_eType = ELuaExpectedType.ELET_Float;
            }
            else if (vvalue is bool)
            {
                SetValueB((bool)vvalue);
                m_eType = ELuaExpectedType.ELET_Bool;                
            }
            else
            {
                SetValueI(-1);
                m_eType = ELuaExpectedType.ELET_Int;
            }
        }
    }

    public override CPseduLuaValue this[string sKey]
    {
        get
        {
            CRuntimeLogger.LogWarning("index out of range--Psedu Lua Number");
            switch (GetExpectedType())
            {
                case ELuaExpectedType.ELET_Int:
                    int iValue = GetValueI();
                    return iValue.ToString(CultureInfo.InvariantCulture);
                //break;
                case ELuaExpectedType.ELET_Long:
                    long lValue = GetValueL();
                    return lValue.ToString(CultureInfo.InvariantCulture);
                //break;
                case ELuaExpectedType.ELET_Bool:
                    bool bValue = GetValueB();
                    return bValue.ToString(CultureInfo.InvariantCulture);
                //break;
            }
            float fValue = GetValueF();
            return fValue.ToString(CultureInfo.InvariantCulture);
        }
        set
        {
            CRuntimeLogger.LogWarning("index out of range--Psedu Lua Number");
            if (null == value)
            {
                SetValueI(0);
                m_eType = ELuaExpectedType.ELET_Int;
                return;
            }
            object vvalue = value.GetExpected();
            if (vvalue is long)
            {
                SetValueL((long)vvalue);
                m_eType = ELuaExpectedType.ELET_Long;
            }
            else if (vvalue is ulong)
            {
                SetValueL((long)(ulong)vvalue);
                m_eType = ELuaExpectedType.ELET_Long;
            }
            else if (vvalue is uint)
            {
                SetValueI((int)(uint)vvalue);
                m_eType = ELuaExpectedType.ELET_Int;
            }
            else if (vvalue is int)
            {
                SetValueI((int)vvalue);
                m_eType = ELuaExpectedType.ELET_Int;
            }
            else if (vvalue is short)
            {
                SetValueI((short)vvalue);
                m_eType = ELuaExpectedType.ELET_Int;
            }
            else if (vvalue is ushort)
            {
                SetValueI((ushort)vvalue);
                m_eType = ELuaExpectedType.ELET_Int;
            }
            else if (vvalue is byte)
            {
                SetValueI((byte)vvalue);
                m_eType = ELuaExpectedType.ELET_Int;
            }
            else if (vvalue is double)
            {
                SetValueF((float)(double)vvalue);
                m_eType = ELuaExpectedType.ELET_Float;
            }
            else if (vvalue is float)
            {
                SetValueF((float)vvalue);
                m_eType = ELuaExpectedType.ELET_Float;
            }
            else if (vvalue is bool)
            {
                SetValueB((bool)vvalue);
                m_eType = ELuaExpectedType.ELET_Bool;
            }
            else
            {
                SetValueI(-1);
                m_eType = ELuaExpectedType.ELET_Int;
            }
        }
    }

    #endregion


    #region Equals

    private long GetV(bool bV1)
    {
        return bV1 ? _v1 : _v2;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (int)((GetV(true) * 397) ^ GetV(false).GetHashCode());
        }
    }

    #endregion
}

public class CPseduLuaString : CPseduLuaValue
{ 
    private string m_sRealString = "";
    public CPseduLuaString() //: base()
    {
        m_eType = ELuaExpectedType.ELET_String;
        m_sRealString = "";
    }

    public CPseduLuaString(object value)
    {
        m_eType = ELuaExpectedType.ELET_String;
        SetValue(value.ToString());
    }

    public override object GetExpected()
    {
        return ToString();
    }

    #region Get And Set

    public void SetValue(string sValue)
    {
        m_sRealString = sValue;
    }

    private string GetValue()
    {
        return m_sRealString;
    }

    #endregion

    #region Cast

    public static implicit operator CPseduLuaString(string fValue)
    {
        CPseduLuaString ret = new CPseduLuaString();
        ret.SetValue(fValue);
        ret.m_eType = ELuaExpectedType.ELET_String;
        return ret;
    }

    public override string ToString()
    {
        return GetValue();
    }

    public static implicit operator CPseduLuaList(CPseduLuaString sfValue)
    {
        return new object[] { sfValue.GetValue() };
    }

    #endregion

    #region List Operators

    public override CPseduLuaValue this[CPseduLuaNumber luanumber]
    {
        get { return this[(int)luanumber]; }
        set { this[(int)luanumber] = value; }
    }

    public override CPseduLuaValue this[int index]
    {
        get
        {
            if (0 != index)
            {
                CRuntimeLogger.LogWarning("index out of range--Psedu Lua String");
            }
            return GetValue();
        }
        set
        {
            if (null == value)
            {
                SetValue("nil");
                m_eType = ELuaExpectedType.ELET_String;
                return;
            }
            object vvalue = value.GetExpected();
            if (0 != index)
            {
                CRuntimeLogger.LogWarning("index out of range--Psedu Lua String");
            }
            // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
            if (vvalue is string)
            {
                SetValue((string)vvalue);
                m_eType = ELuaExpectedType.ELET_String;
            }
            else
            {
                SetValue(value.ToString());
                m_eType = ELuaExpectedType.ELET_String;                
            }
        }
    }

    public override CPseduLuaValue this[string sKey]
    {
        get
        {
            CRuntimeLogger.LogWarning("index out of range--Psedu Lua String");
            return GetValue();
        }
        set
        {
            if (null == value)
            {
                SetValue("nil");
                m_eType = ELuaExpectedType.ELET_String;
                return;
            }
            CRuntimeLogger.LogWarning("index out of range--Psedu Lua String");
            object vvalue = value.GetExpected();
            string s = vvalue as string;
            if (s != null)
            {
                SetValue(s);
                m_eType = ELuaExpectedType.ELET_String;
            }
            else
            {
                SetValue(value.ToString());
                m_eType = ELuaExpectedType.ELET_String;
            }
        }
    }

    #endregion
}

public class CPseduLuaList : CPseduLuaValue
{
    private List<CPseduLuaValue> m_pRealList = null;
    private Dictionary<string, int> m_pRealDic = null;
    public CPseduLuaList()// : base()
    {
        m_eType = ELuaExpectedType.ELET_List;
        m_pRealList = null;
    }

    public CPseduLuaList(IEnumerable<object> content)
    {
        SetValue(content);
    }

    public override object GetExpected()
    {
#if DebugLQ
        List<object> ret = new List<object>();
        foreach (CPseduLuaValue item in m_pRealList)
        {
            ret.Add(item.GetExpected());
        }
        return ret.ToArray();
#endif
        return m_pRealList.Select(item => item.GetExpected()).ToArray();
    }

    public void SetValue(IEnumerable<object> sValue)
    {
        m_pRealList = new List<CPseduLuaValue>();

        foreach (object t in sValue)
        {
            if (null == t)
            {
                m_pRealList.Add(new CPseduLuaValue());
            }
            else if (t is int)
            {
                CPseduLuaNumber number = (int)t;
                m_pRealList.Add(number);
            }
            else if (t is long)
            {
                CPseduLuaNumber number = (long)t;
                m_pRealList.Add(number);
            }
            else if (t is float)
            {
                CPseduLuaNumber number = (float)t;
                m_pRealList.Add(number);
            }
            else if (t is bool)
            {
                CPseduLuaNumber number = (bool)t;
                m_pRealList.Add(number);
            }
            // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
            else if (t is string)
            {
                CPseduLuaString number = (string)t;
                m_pRealList.Add(number);
            }
            // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
            else if (t is CPseduLuaString)
            {
                CPseduLuaString number = (string)(CPseduLuaString)t;
                m_pRealList.Add(number);
            }
            // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
            else if (t is CPseduLuaNumber)
            {
                m_pRealList.Add((CPseduLuaNumber)t);
            }
            else if (t is Array)
            {
                CPseduLuaList number = (object[])t;
                m_pRealList.Add(number);
            }
            else
            {
                m_pRealList.Add(new CPseduLuaValue());
            }
        }
        m_pRealDic = null;
    }

    private readonly List<string> _tmpkeys = new List<string>();
    public void SetValue(IList<string> sKeys, IList<object> sValue)
    {
        m_pRealList = new List<CPseduLuaValue>();
        _tmpkeys.Clear();
        for (int i = 0; i < sValue.Count; ++i)
        {
            if (string.IsNullOrEmpty(sKeys[i]) || _tmpkeys.Contains(sKeys[i]))
            {
                CRuntimeLogger.LogError("Duplicate Key or Null Key --- Psedu Lua List");
                continue;
            }

            if (null == sValue[i])
            {
                m_pRealList.Add(new CPseduLuaValue());
            }
            else if (sValue[i] is int)
            {
                CPseduLuaNumber number = (int)sValue[i];
                m_pRealList.Add(number);
            }
            else if (sValue[i] is sint)
            {
                CPseduLuaNumber number = (int)(sint)sValue[i];
                m_pRealList.Add(number);
            }
            else if (sValue[i] is uint)
            {
                CPseduLuaNumber number = (int)(uint)sValue[i];
                m_pRealList.Add(number);
            }
            else if (sValue[i] is short)
            {
                CPseduLuaNumber number = (int)(short)sValue[i];
                m_pRealList.Add(number);
            }
            else if (sValue[i] is ushort)
            {
                CPseduLuaNumber number = (int)(ushort)sValue[i];
                m_pRealList.Add(number);
            }
            else if (sValue[i] is byte)
            {
                CPseduLuaNumber number = (int)(byte)sValue[i];
                m_pRealList.Add(number);
            }
            else if (sValue[i] is long)
            {
                CPseduLuaNumber number = (long)sValue[i];
                m_pRealList.Add(number);
            }
            else if (sValue[i] is float)
            {
                CPseduLuaNumber number = (float)sValue[i];
                m_pRealList.Add(number);
            }
            else if (sValue[i] is double)
            {
                CPseduLuaNumber number = (float)(double)sValue[i];
                m_pRealList.Add(number);
            }
            else if (sValue[i] is sfloat)
            {
                CPseduLuaNumber number = (float)(sfloat)sValue[i];
                m_pRealList.Add(number);
            }
            else if (sValue[i] is bool)
            {
                CPseduLuaNumber number = (bool)sValue[i];
                m_pRealList.Add(number);
            }
            // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
            else if (sValue[i] is string)
            {
                CPseduLuaString number = (string)sValue[i];
                m_pRealList.Add(number);
            }
            else if (sValue[i] is Array)
            {
                CPseduLuaList number = (object[])sValue[i];
                m_pRealList.Add(number);
            }
            // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
            else if (sValue[i] is CPseduLuaString)
            {
                CPseduLuaString number = (string)(CPseduLuaString)sValue[i];
                m_pRealList.Add(number);
            }
            // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
            else if (sValue[i] is CPseduLuaNumber)
            {
                m_pRealList.Add((CPseduLuaNumber)sValue[i]);
            }
            else
            {
                m_pRealList.Add(new CPseduLuaValue());
            }
            _tmpkeys.Add(sKeys[i]);
        }
        //m_pRealList = values;
        m_pRealDic = new Dictionary<string, int>();
        for (int i = 0; i < _tmpkeys.Count; ++i)
        {
            m_pRealDic.Add(_tmpkeys[i], i);
        }
    }

    public object[] GetValue()
    {
        List<object> ret = new List<object>();
        if (null != m_pRealList)
        {
            foreach (CPseduLuaValue onevalue in m_pRealList)
            {
                if (null == onevalue)
                {
                    ret.Add(null);
                    continue;
                }

                switch (onevalue.GetExpectedType())
                {
                    case ELuaExpectedType.ELET_Unknown:
                        ret.Add(null);
                        break;
                    case ELuaExpectedType.ELET_Int:
                        int n1 = onevalue as CPseduLuaNumber;
                        ret.Add(n1);
                        break;
                    case ELuaExpectedType.ELET_Float:
                        float n2 = onevalue as CPseduLuaNumber;
                        ret.Add(n2);
                        break;
                    case ELuaExpectedType.ELET_Long:
                        long n3 = onevalue as CPseduLuaNumber;
                        ret.Add(n3);
                        break;
                    case ELuaExpectedType.ELET_Bool:
                        bool n4 = onevalue as CPseduLuaNumber;
                        ret.Add(n4);
                        break;
                    case ELuaExpectedType.ELET_String:
                        string s1 = onevalue as CPseduLuaString;
                        ret.Add(s1);
                        break;
                    case ELuaExpectedType.ELET_List:
                        object[] l1 = onevalue as CPseduLuaList;
                        Array ll1 = l1;
                        object lll1 = ll1;// as object;
                        ret.Add(lll1);
                        break;
                }
            }
        }

        return ret.ToArray();
    }

    public string[] GetAllKeys()
    {
        return m_pRealDic.Keys.ToArray();
    }

    public bool ContainsKey(string key)
    {
        return m_pRealDic.ContainsKey(key);
    }

    public void RemoveValue(string key)
    {
        if (!m_pRealDic.ContainsKey(key))
            return;

        m_pRealDic.Remove(key);
    }

    public void insert(object t)
    {
        if (null == t)
        {
            m_pRealList.Add(new CPseduLuaValue());
        }
        else if (t is int)
        {
            CPseduLuaNumber number = (int)t;
            m_pRealList.Add(number);
        }
        else if (t is long)
        {
            CPseduLuaNumber number = (long)t;
            m_pRealList.Add(number);
        }
        else if (t is float)
        {
            CPseduLuaNumber number = (float)t;
            m_pRealList.Add(number);
        }
        else if (t is bool)
        {
            CPseduLuaNumber number = (bool)t;
            m_pRealList.Add(number);
        }
        // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
        else if (t is string)
        {
            CPseduLuaString number = (string)t;
            m_pRealList.Add(number);
        }
        // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
        else if (t is CPseduLuaString)
        {
            CPseduLuaString number = (string)(CPseduLuaString)t;
            m_pRealList.Add(number);
        }
        // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
        else if (t is CPseduLuaNumber)
        {
            m_pRealList.Add((CPseduLuaNumber)t);
        }
        else if (t is Array)
        {
            CPseduLuaList number = (object[])t;
            m_pRealList.Add(number);
        }
        else
        {
            m_pRealList.Add(new CPseduLuaValue());
        }
        m_pRealDic = null;
    }

    public override string ToString()
    {
        if (null == m_pRealList || 0 == m_pRealList.Count)
        {
            return "Null List";
        }

        string sOut = "";
        for (int i = 0; i < m_pRealList.Count; ++i)
        {
            if (null == m_pRealList[i])
            {
                sOut += string.Format("index:{0} {1};", i, "NULL");
            }
            else if (m_pRealList[i] is CPseduLuaNumber)
            {
                CPseduLuaNumber number = m_pRealList[i] as CPseduLuaNumber;
                sOut += string.Format("index:{0} {1};", i, (string)number);
            }
            else if (m_pRealList[i] is CPseduLuaString)
            {
                CPseduLuaString number = m_pRealList[i] as CPseduLuaString;
                sOut += string.Format("index:{0} {1};", i, (string)number);
            }
            else if (m_pRealList[i] is CPseduLuaList)
            {
                CPseduLuaList number = m_pRealList[i] as CPseduLuaList;
                sOut += string.Format("index:{0} {1};", i, (string)number);
            }
            else
            {
                sOut += string.Format("index:{0} {1};", i, "Unkown");
            }
        }
        return sOut;
    }

    public static implicit operator CPseduLuaList(object[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        ret.SetValue(fValue);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(List<object> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        ret.SetValue(fValue);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    #region Special Array

    public static implicit operator CPseduLuaList(int[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(fValue[i]);}
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(sint[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(uint[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(short[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(ushort[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(long[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(byte[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(float[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(sfloat[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(bool[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(sbool[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(string[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    #endregion

    #region Special List

    public static implicit operator CPseduLuaList(List<int> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(List<sint> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(List<uint> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(List<short> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(List<ushort> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(List<long> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(List<byte> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(List<float> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(List<sfloat> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(List<bool> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(List<sbool> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(List<string> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(fValue[i]); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    #endregion

    #region PLua

    public static implicit operator CPseduLuaList(CPseduLuaValue[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(null == fValue[i] ? null : fValue[i].GetExpected()); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(CPseduLuaNumber[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(null == fValue[i] ? null : fValue[i].GetExpected()); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(CPseduLuaString[] fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Length; ++i) { lst.Add(null == fValue[i] ? null : fValue[i].GetExpected()); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(List<CPseduLuaValue> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(null == fValue[i] ? null : fValue[i].GetExpected()); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(List<CPseduLuaNumber> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(null == fValue[i] ? null : fValue[i].GetExpected()); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(List<CPseduLuaString> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<object> lst = new List<object>();
        for (int i = 0; i < fValue.Count; ++i) { lst.Add(null == fValue[i] ? null : fValue[i].GetExpected()); }
        ret.SetValue(lst);
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    #endregion

    public static implicit operator CPseduLuaList(Dictionary<string, object> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        List<string> keys = new List<string>();
        List<object> values = new List<object>();
        foreach (KeyValuePair<string, object> pair in fValue)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
        ret.SetValue(keys.ToArray(), values.ToArray());
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(Dictionary<int, object> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        foreach (KeyValuePair<int, object> pair in fValue)
        {
            ret[pair.Key] = Create(pair.Value);
        }
        
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    #region Lua Values

    public static implicit operator CPseduLuaList(Dictionary<int, CPseduLuaValue> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        foreach (KeyValuePair<int, CPseduLuaValue> pair in fValue)
        {
            ret[pair.Key] = pair.Value;
        }
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(Dictionary<int, CPseduLuaList> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        foreach (KeyValuePair<int, CPseduLuaList> pair in fValue)
        {
            ret[pair.Key] = pair.Value;
        }
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(Dictionary<int, CPseduLuaNumber> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        foreach (KeyValuePair<int, CPseduLuaNumber> pair in fValue)
        {
            ret[pair.Key] = pair.Value;
        }
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(Dictionary<int, CPseduLuaString> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        foreach (KeyValuePair<int, CPseduLuaString> pair in fValue)
        {
            ret[pair.Key] = pair.Value;
        }
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(Dictionary<string, CPseduLuaValue> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        foreach (KeyValuePair<string, CPseduLuaValue> pair in fValue)
        {
            ret[pair.Key] = pair.Value;
        }
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(Dictionary<string, CPseduLuaList> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        foreach (KeyValuePair<string, CPseduLuaList> pair in fValue)
        {
            ret[pair.Key] = pair.Value;
        }
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(Dictionary<string, CPseduLuaNumber> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        foreach (KeyValuePair<string, CPseduLuaNumber> pair in fValue)
        {
            ret[pair.Key] = pair.Value;
        }
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    public static implicit operator CPseduLuaList(Dictionary<string, CPseduLuaString> fValue)
    {
        CPseduLuaList ret = new CPseduLuaList();
        foreach (KeyValuePair<string, CPseduLuaString> pair in fValue)
        {
            ret[pair.Key] = pair.Value;
        }
        ret.m_eType = ELuaExpectedType.ELET_List;
        return ret;
    }

    #endregion

    public static implicit operator object[](CPseduLuaList sfValue)
    {
        return sfValue.GetValue();
    }

    public static implicit operator Dictionary<string, object>(CPseduLuaList sfValue)
    {
        Dictionary<string, object> ret = new Dictionary<string, object>();
        if (null == sfValue.m_pRealDic)
        {
            for (int i = 0; i < sfValue.m_pRealList.Count; ++i)
            {
                ret.Add(i.ToString(CultureInfo.InvariantCulture), sfValue[i]);
            }
        }
        else
        {
            foreach (KeyValuePair<string, int> keyValuePair in sfValue.m_pRealDic)
            {
                ret.Add(keyValuePair.Key, sfValue[keyValuePair.Value]);
            }
        }

        return ret;
    }

    #region List Operators

    public override CPseduLuaValue this[CPseduLuaNumber luanumber]
    {
        get { return this[(int)luanumber]; }
        set { this[(int)luanumber] = value; }
    }

    public override CPseduLuaValue this[int index]
    {
        get
        {
            //CommonCode.Log("i'm in here!!", ELogLevel.Warnning);
            if (null == m_pRealList || index >= m_pRealList.Count)
            {
                CRuntimeLogger.LogError("Index out of range --- Psedu Lua List");
                return null;
            }

            return m_pRealList[index];
        }
        set
        {
            if (null == m_pRealList || index >= m_pRealList.Count)
            {
                if (null == m_pRealList)
                {
                    m_pRealList = new List<CPseduLuaValue>();
                }
                for (int i = m_pRealList.Count; i < index + 1; ++i)
                {
                    m_pRealList.Add(null);
                }
            }

            if (null == value)
            {
                m_pRealList[index] = null;
                return;
            }
            object vvalue = value.GetExpected();
            if (null == vvalue)
            {
                m_pRealList[index] = null;
            }
            else if (vvalue is int)
            {
                CPseduLuaNumber addint = (int)vvalue;
                m_pRealList[index] = addint;
            }
            else if (vvalue is float)
            {
                CPseduLuaNumber addint = (float)vvalue;
                m_pRealList[index] = addint;
            }
            else if (vvalue is bool)
            {
                CPseduLuaNumber addint = (bool)vvalue;
                m_pRealList[index] = addint;
            }
            else if (vvalue is long)
            {
                CPseduLuaNumber addint = (long)vvalue;
                m_pRealList[index] = addint;
            }
            // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
            else if (vvalue is string)
            {
                CPseduLuaString addint = (string)vvalue;
                m_pRealList[index] = addint;
            }
            else if (vvalue is Array)
            {
                CPseduLuaList addint = (object[])vvalue;
                m_pRealList[index] = addint;
            }
            else
            {
                m_pRealList[index] = value;
            }
        }
    }

    public override CPseduLuaValue this[string sKey]
    {
        get
        {
            if (null == m_pRealDic || !m_pRealDic.ContainsKey(sKey))
            {
                CRuntimeLogger.Log("No Key --- Psedu Lua List");
                return null;
            }

            int iIndex = m_pRealDic[sKey];
            return this[iIndex];
        }
        set
        {
            if (null == m_pRealDic)
            {
                if (null == m_pRealList || 0 == m_pRealList.Count)
                {
                    m_pRealDic = new Dictionary<string, int>();
                }
                else
                {
                    CRuntimeLogger.Log("This is a no key list --- Psedu Lua List");
                    return;                    
                }
            }

            if (null == m_pRealList)
            {
                m_pRealList = new List<CPseduLuaValue>();
            }
            
            if (m_pRealDic.ContainsKey(sKey))
            {
                int iIndex = m_pRealDic[sKey];
                this[iIndex] = value;
            }
            else
            {
                int iIndex = m_pRealList.Count;
                m_pRealDic.Add(sKey, iIndex);
                this[iIndex] = value;
            }
        }
    }

    public override int getn()
    {
        if (null == m_pRealList)
        {
            return 0;
        }
        return m_pRealList.Count;
    }

    #endregion

    #region Other Operator

    public void AppendList(CPseduLuaList list)
    {
        int iStart = getn();
        for (int i = 0; i < list.getn(); ++i)
        {
            this[iStart + i] = list[i];
        }
    }

    #endregion
}

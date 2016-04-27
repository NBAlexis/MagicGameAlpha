using System;
using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

public class FDRandom
{
    /// <summary>
    /// Seed
    /// </summary>
    static public uint seed = 0;

    private const uint seed1 = 1234567;
    private const uint seed2 = 1234567 * 2;
    private const uint maxnumber = 654321;

    /// <summary>
    /// Return a fragment between 0 - 1
    /// </summary>
    /// <returns></returns>
    public static float NextF()
    {
        seed = seed1 * seed + seed2;
        seed = seed % maxnumber;
        return seed / (float)maxnumber;
    }

    /// <summary>
    /// return an integer between 0 and maxnumber
    /// </summary>
    /// <returns></returns>
    public static int NextI()
    {
        seed = seed1 * seed + seed2;
        seed = seed % maxnumber;
        return (int)seed;
    }

    /// <summary>
    /// Same as Random.Range()
    /// </summary>
    /// <param name="f1">Lower</param>
    /// <param name="f2">Upper</param>
    /// <param name="iseed">Seed</param>
    /// <returns></returns>
    static public float Range(float f1, float f2, uint iseed)
    {
        seed = iseed;
        return NextF() * (f2 - f1) + f1;
    }

    /// <summary>
    /// Same as Random.Range()
    /// </summary>
    /// <param name="f1">Lower</param>
    /// <param name="f2">Upper + 1</param>
    /// <param name="iseed">Seed</param>
    /// <returns></returns>
    static public int Range(int f1, int f2, uint iseed)
    {
        seed = iseed;
        return NextI() % (f2 - f1) + f1;
    }

    /// <summary>
    /// Same as Random.Range()
    /// </summary>
    /// <param name="f1">Lower</param>
    /// <param name="f2">Upper</param>
    /// <returns></returns>
    static public float Range(float f1, float f2)
    {
        return NextF() * (f2 - f1) + f1;
    }

    /// <summary>
    /// Same as Random.Range()
    /// </summary>
    /// <param name="f1">Lower</param>
    /// <param name="f2">Upper</param>
    /// <returns></returns>
    static public int Range(int f1, int f2)
    {
        return NextI() % (f2 - f1) + f1;
    }

    private class SOrder
    {
        public int theOld;
        public int theNew;
        public int orderValue;
    }

    static private int Sort1(SOrder o1, SOrder o2)
    {
        return o1.orderValue >= o2.orderValue ? 1 : -1;
    }

    public static byte[] DisOrder(byte[] bytes, int iSeed)
    {
        seed = (uint)(iSeed * iSeed * iSeed);

        SOrder[] tmpOrder = new SOrder[bytes.Length];

        for (int i = 0; i < bytes.Length; ++i)
        {
            tmpOrder[i] = new SOrder {theOld = i, orderValue = NextI()};
        }
        List<SOrder> tmpList = tmpOrder.ToList();
        tmpList.Sort(Sort1);

        for (int i = 0; i < bytes.Length; ++i)
        {
            tmpList[i].theNew = i;
        }

        byte[] ret = new byte[bytes.Length];
        for (int i = 0; i < bytes.Length; ++i)
        {
            ret[tmpList[i].theNew] = bytes[tmpList[i].theOld];
        }

        return ret;
    }

    public static byte[] ReOrder(byte[] bytes, int iSeed)
    {
        seed = (uint)(iSeed * iSeed * iSeed);

        SOrder[] tmpOrder = new SOrder[bytes.Length];

        for (int i = 0; i < bytes.Length; ++i)
        {
            tmpOrder[i] = new SOrder {theOld = i, orderValue = NextI()};
        }
        List<SOrder> tmpList = tmpOrder.ToList();
        tmpList.Sort(Sort1);

        for (int i = 0; i < bytes.Length; ++i)
        {
            tmpList[i].theNew = i;
        }

        byte[] ret = new byte[bytes.Length];
        for (int i = 0; i < bytes.Length; ++i)
        {
            ret[tmpList[i].theOld] = bytes[tmpList[i].theNew];
        }

        return ret;
    }

}

public class FDEncode
{
    protected const uint EncodeKey1 = 0xABADCA12; //a bad car
    protected const uint EncodeKey2 = 0x2012DEAD; //2012 dead
    protected const uint EncodeKey3 = 0x31415926; //pi

    public static byte[] BakeByte(byte byTag)
    {
        byte[] retByte = new byte[12];
        byte[] b1 = BitConverter.GetBytes(EncodeKey1);
        byte[] b2 = BitConverter.GetBytes(EncodeKey2);
        byte[] b3 = BitConverter.GetBytes(EncodeKey3);
        switch (byTag % 6)
        {
            case 0:
                b1.CopyTo(retByte, 0);
                b2.CopyTo(retByte, 4);
                b3.CopyTo(retByte, 8);
                break;
            case 1:
                b1.CopyTo(retByte, 0);
                b3.CopyTo(retByte, 4);
                b2.CopyTo(retByte, 8);
                break;
            case 2:
                b2.CopyTo(retByte, 0);
                b1.CopyTo(retByte, 4);
                b3.CopyTo(retByte, 8);
                break;
            case 3:
                b2.CopyTo(retByte, 0);
                b3.CopyTo(retByte, 4);
                b1.CopyTo(retByte, 8);
                break;
            case 4:
                b3.CopyTo(retByte, 0);
                b1.CopyTo(retByte, 4);
                b2.CopyTo(retByte, 8);
                break;
            case 5:
                b3.CopyTo(retByte, 0);
                b2.CopyTo(retByte, 4);
                b1.CopyTo(retByte, 8);
                break;
        }

        return retByte;
    }

    public static byte[] NativeEncodeToBytes(byte[] data)
    {
        byte tag = (byte)FDRandom.Range(0, 256);
        byte orderseed = (byte)FDRandom.Range(0, 256);

        byte[] encodeKey = BakeByte(tag);

        for (int i = 0; i < (data.Length + 0.1f) / 12.0f; ++i)
        {
            for (int j = 0; j < 12 && i * 12 + j < data.Length; ++j)
            {
                data[i * 12 + j] = (byte)(data[i * 12 + j] ^ encodeKey[j]);
            }
        }
        byte[] prebase64_1 = new byte[data.Length + 2];
        byte[] prebase64_2 = FDRandom.DisOrder(data, orderseed);
        prebase64_1[0] = tag;
        prebase64_1[1] = orderseed;
        prebase64_2.CopyTo(prebase64_1, 2);
        return prebase64_1;
    }

    public static byte[] NativeDecodeToBytes(byte[] data)
    {
        if (null == data || data.Length < 1)
        {
            return new byte[1];
        }
        byte[] encodeKey = BakeByte(data[0]);
        byte[] preOr1 = new byte[data.Length - 2];
        byte orderSeed = data[1];
        Array.Copy(data, 2, preOr1, 0, preOr1.Length);
        byte[] preOr2 = FDRandom.ReOrder(preOr1, orderSeed);
        for (int i = 0; i < (preOr2.Length + 0.1f) / 12.0f; ++i)
        {
            for (int j = 0; j < 12 && i * 12 + j < preOr2.Length; ++j)
            {
                preOr2[i * 12 + j] = (byte)(preOr2[i * 12 + j] ^ encodeKey[j]);
            }
        }
        return preOr2;
    }

    public static string NativeEncodeToString(byte[] data)
    {
        return Convert.ToBase64String(NativeEncodeToBytes(data));
    }

    public static byte[] NativeDecodeString(string sString)
    {
        byte[] data = Convert.FromBase64String(sString);
        return NativeDecodeToBytes(data);
    }

    public static string NativeStringEncodeToString(string sOringeString)
    {
        byte[] dataString = Encoding.Unicode.GetBytes(sOringeString);
        return NativeEncodeToString(dataString);
    }

    public static string NativeStringDecodeString(string sPassword)
    {
        byte[] thedata = NativeDecodeString(sPassword);
        return Encoding.Unicode.GetString(thedata);
    }

}

public struct sfloat
{
    private float m_fCut1;
    private float m_fCut2;

    public sfloat(sfloat sfValue)
        : this()
    {
        SetValue(sfValue.GetValue());
    }

    public sfloat(float fValue)
        : this()
    {
        SetValue(fValue);
    }

    private float GetValue()
    {
        return m_fCut1 + m_fCut2;
    }

    private void SetValue(float fValue)
    {
        m_fCut1 = FDRandom.Range(-1.0f, 2.0f) * fValue;
        m_fCut2 = fValue - m_fCut1;
    }

    public static implicit operator sfloat(float fValue)
    {
        return new sfloat(fValue);
    }

    public static implicit operator float(sfloat sfValue)
    {
        return sfValue.GetValue();
    }

    public override string ToString()
    {
        return (m_fCut1 + m_fCut2).ToString(CultureInfo.InvariantCulture);
    }
}

public struct sint
{
    private int m_iCut1;
    private int m_iCut2;

    public sint(sint sfValue)
        : this()
    {
        SetValue(sfValue.GetValue());
    }

    public sint(int fValue)
        : this()
    {
        SetValue(fValue);
    }

    private int GetValue()
    {
        return m_iCut1 + m_iCut2;
    }

    private void SetValue(int iValue)
    {
        m_iCut1 = FDRandom.Range(iValue > 0 ? -10 * iValue - 10 : 10 * iValue - 10, iValue > 0 ? 10 * iValue + 10 : -10 * iValue + 10);
        m_iCut2 = iValue - m_iCut1;
    }

    public static implicit operator sint(int fValue)
    {
        return new sint(fValue);
    }

    public static implicit operator int(sint sfValue)
    {
        return sfValue.GetValue();
    }

    public static implicit operator sint(uint fValue)
    {
        sint sret = new sint((int)fValue);
        return sret;
    }

    public static implicit operator uint(sint sfValue)
    {
        return (uint)sfValue.GetValue();
    }

    public static implicit operator sint(float fValue)
    {
        return new sint(Mathf.RoundToInt(fValue));
    }

    public static implicit operator float(sint sfValue)
    {
        return sfValue.GetValue();
    }

    public static implicit operator sint(sfloat fValue)
    {
        return new sint(Mathf.RoundToInt(fValue));
    }

    public static implicit operator sfloat(sint sfValue)
    {
        return sfValue.GetValue();
    }

    public override string ToString()
    {
        return (m_iCut1 + m_iCut2).ToString();
    }
}

public struct sbool
{
    private int m_fValue;

    public sbool(sbool sfValue)
        : this()
    {
        SetValue(sfValue.GetValue());
    }

    public sbool(bool fValue)
        : this()
    {
        SetValue(fValue);
    }

    private bool GetValue()
    {
        return m_fValue >= 0;
    }

    private void SetValue(bool iValue)
    {
        m_fValue = iValue ? FDRandom.Range(0, 100) : FDRandom.Range(-100, -1);
    }

    public static implicit operator sbool(bool fValue)
    {
        return new sbool(fValue);
    }

    public static implicit operator bool(sbool sfValue)
    {
        return sfValue.GetValue();
    }

    public override string ToString()
    {
        return (m_fValue >= 0).ToString();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMGADataFields : CMGDataElement
{
    public string m_sFieldName = "FieldName";
    public string m_sFieldType = "string";
    public string m_sFieldDefault = "\"\"";
    public string m_sFieldEdtiorName = "变量";

    override public string GetString(string sParent)
    {
        string sRet = base.GetString(sParent);

        sRet += Write("FieldName", sParent, m_sFieldName);
        sRet += Write("FieldType", sParent, m_sFieldType);
        sRet += Write("FieldDefault", sParent, m_sFieldDefault);
        sRet += Write("FieldEdtiorName", sParent, m_sFieldEdtiorName);

        return sRet;
    }

    override public void LoadData(string sTextToParse, string sParent)
    {
        base.LoadData(sTextToParse, sParent);

        m_sFieldName = (string)GetElementValue(sTextToParse, sParent, "FieldName", m_sFieldName);
        m_sFieldType = (string)GetElementValue(sTextToParse, sParent, "FieldType", m_sFieldType);
        m_sFieldDefault = (string)GetElementValue(sTextToParse, sParent, "FieldDefault", m_sFieldDefault);
        m_sFieldEdtiorName = (string)GetElementValue(sTextToParse, sParent, "FieldEdtiorName", m_sFieldEdtiorName);
    }

    public override CMGDataElement GetDefault()
    {
        return new CMGADataFields();
    }

    public override CMGDataElement[] GetDefaultList(int iLength)
    {
        CMGADataFields[] ret = new CMGADataFields[iLength];
        for (int i = 0; i < iLength; ++i)
        {
            ret[i] = new CMGADataFields();
        }
        return ret;
    }

    public override CMGDataElement Copy()
    {
        return new CMGADataFields
        {
            m_sFieldName = m_sFieldName,
            m_sFieldType = m_sFieldType,
            m_sFieldDefault = m_sFieldDefault,
            m_sFieldEdtiorName = m_sFieldEdtiorName
        };
    }
}

public class CMGADataEnumes : CMGDataElement
{
    public string m_sEnumeName = "EnumeName";
    public string[] m_sEnumeMemebers = { "Memeber1", "Member2" };

    override public string GetString(string sParent)
    {
        string sRet = base.GetString(sParent);

        sRet += Write("EnumeName", sParent, m_sEnumeName);
        sRet += Write("Memeber", sParent, m_sEnumeMemebers);

        return sRet;
    }

    override public void LoadData(string sTextToParse, string sParent)
    {
        base.LoadData(sTextToParse, sParent);

        m_sEnumeName = (string)GetElementValue(sTextToParse, sParent, "EnumeName", m_sEnumeName);
        m_sEnumeMemebers = (string[])GetElementValue(sTextToParse, sParent, "Memeber", m_sEnumeMemebers);
    }

    public override CMGDataElement GetDefault()
    {
        return new CMGADataEnumes();
    }

    public override CMGDataElement[] GetDefaultList(int iLength)
    {
        CMGADataEnumes[] ret = new CMGADataEnumes[iLength];
        for (int i = 0; i < iLength; ++i)
        {
            ret[i] = new CMGADataEnumes();
        }
        return ret;
    }

    public override CMGDataElement Copy()
    {
        CMGADataEnumes ret = new CMGADataEnumes
        {
            m_sEnumeName = m_sEnumeName,
        };
        m_sEnumeMemebers.CopyTo(ret.m_sEnumeMemebers, 0);
        return ret;
    }
}

public class CMGADataSubSet : CMGDataElement
{
    public string m_sName = "Name";
    public CMGADataFields[] m_sFields = { new CMGADataFields() };
    public CMGADataEnumes[] m_sEnums = { new CMGADataEnumes() };

    override public string GetString(string sParent)
    {
        string sRet = base.GetString(sParent);

        sRet += Write("Name", sParent, m_sName);
        sRet += Write("Fields", sParent, m_sFields);
        sRet += Write("Enums", sParent, m_sEnums);

        return sRet;
    }

    override public void LoadData(string sTextToParse, string sParent)
    {
        base.LoadData(sTextToParse, sParent);
        m_sName = (string)GetElementValue(sTextToParse, sParent, "Name", m_sName);
        m_sFields = (CMGADataFields[])GetElementValue(sTextToParse, sParent, "Fields", m_sFields);
        m_sEnums = (CMGADataEnumes[])GetElementValue(sTextToParse, sParent, "Enums", m_sEnums);
    }

    public override CMGDataElement GetDefault()
    {
        return new CMGADataSubSet();
    }

    public override CMGDataElement[] GetDefaultList(int iLength)
    {
        CMGADataSubSet[] ret = new CMGADataSubSet[iLength];
        for (int i = 0; i < iLength; ++i)
        {
            ret[i] = new CMGADataSubSet();
        }
        return ret;
    }

    public override CMGDataElement Copy()
    {
        CMGADataSubSet ret = new CMGADataSubSet
        {
            m_sName = m_sName,
        };
        ret.m_sFields = new CMGADataFields[m_sFields.Length];
        for (int i = 0; i < m_sFields.Length; ++i)
        {
            ret.m_sFields[i] = (CMGADataFields)m_sFields[i].Copy();
        }
        ret.m_sEnums = new CMGADataEnumes[m_sEnums.Length];
        for (int i = 0; i < m_sEnums.Length; ++i)
        {
            ret.m_sEnums[i] = (CMGADataEnumes)m_sEnums[i].Copy();
        }
        return ret;
    }
}

public class CMGADataSet : TMGData<CMGADataSubSet>
{
    public string GenerateCSharpCode(string sGameName)
    {
        return "";
    }
}



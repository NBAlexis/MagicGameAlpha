using System.Collections;
using System.Collections.Generic;

public class CLuaRequestData : ILuaRequestData
{
    //ILuaRequestData
    /*
    继承自ILuaRequestData
    传入一个Message Id，返回一个Table。这个Table是发送给服务器的数据
    */
    public CPseduLuaValue MsgId_Login = 1;
    public CPseduLuaValue MsgId_Regist = 2;
    public CPseduLuaValue MsgId_GetCards = 3;
    public CPseduLuaValue MsgId_GetCoin = 4;
    public CPseduLuaValue MsgId_OpenCardPackage = 5;
    public CPseduLuaValue MsgId_BattleFinish = 6;
    public CPseduLuaValue P1EnmPos4 = new object[] {
    1,2,3,4,5.3f,"6","\"",7
    };
    public CPseduLuaValue P1EnmPos3 = new Dictionary<int, CPseduLuaValue>(new IntEqualityComparer())
    {
    {1, new object[] {86,"{()}[]"}},
    {2, new object[] {94,145}},
    {3, new object[] {90,145}},
    {4, new object[] {92,150}},
    {5, new object[] {88,150}},
    {6, new object[] {107,125}},
    {7, new object[] {109,125}},
    {8, new object[] {111,125}},
    {9, new object[] {113,125}},
    };
    public CPseduLuaValue P1EnmPos5 = new Dictionary<string, CPseduLuaValue>()
    {
    {"1", new object[] {86,""}},
    {"[", new object[] {94,145}},
    };
    public CPseduLuaValue P1EnmPos6 = new Dictionary<string, CPseduLuaValue>()
    {
    {"1", new object[] {86,""}}
    };
    public CPseduLuaValue P1EnmPos7 = new Dictionary<int, CPseduLuaValue>(new IntEqualityComparer())
    {
    {1, "a"
    }};
    public CPseduLuaValue P1EnmPos8 = new object[] {1};
    public CPseduLuaValue P1EnmPos9 = new object[] {};
    public CPseduLuaValue P1EnmPos10 = new object[] {1,};
    public CPseduLuaValue P1EnmPos11 = new object[] {1,2,};
    public CPseduLuaValue test1 = 1.2f;
    public CPseduLuaValue test2 = "";
    public CPseduLuaValue test3 = "a" + "b";
    public CPseduLuaValue test4 = "" + "";
    public override Dictionary<string,System.Object> GetRequestData(int iNetworkMessageId)
    {
        CPseduLuaValue req = null;
        if ( MsgId_Login == iNetworkMessageId ) 
        {
            req = GetLoginData();
        }

        else if ( MsgId_Regist == iNetworkMessageId) 
        {
            req = GetRegistData();
        }

        else if ( MsgId_GetCards == iNetworkMessageId) 
        {
            req = GetCardsData();
        }

        else if ( MsgId_OpenCardPackage == iNetworkMessageId) 
        {
        }

        else if ( MsgId_BattleFinish == iNetworkMessageId) 
        {
        }

        return req;
    }

    protected const int Msg_Login_Account = 1;
    protected const int Msg_Login_Password = 2;
    protected CPseduLuaValue GetLoginData()
    {
        //我就试试看注释OK不;
        CPseduLuaValue req = new Dictionary<int, CPseduLuaValue>(new IntEqualityComparer())
        {
        {Msg_Login_Account, CLuaNetworkInterface.GetDeviceId()},
        {Msg_Login_Password, "password"},
        };
        return req;
    }

    /*
    是不是可以这么注释2？
    */
    protected CPseduLuaValue GetRegistData()
    {
        /*
        是不是可以这么注释1？
        */
        return new Dictionary<int, CPseduLuaValue>(new IntEqualityComparer())
        {
        {1, CLuaNetworkInterface.GetDeviceId()},
        {2, "password"},
        };
    }

    protected CPseduLuaValue GetCardsData()
    {
        //先计算MD5,如果MD5一致，服务器不用返回。;
        CPseduLuaValue cardCount = CLuaNetworkInterface.GetIntValue("CardCount", 0);
        CPseduLuaValue cardLst = "";
        for (int i = 1; i <= cardCount; ++i) 
        {
            cardLst = cardLst + CLuaNetworkInterface.GetIntValue("CardID" + i, 0);
            cardLst = cardLst + CLuaNetworkInterface.GetIntValue("CardTemplate" + i, 0);
            cardLst = cardLst + CLuaNetworkInterface.GetIntValue("CardLevel" + i, 0);
        }

        CPseduLuaValue md5 = CLuaNetworkInterface.StringMD5(cardLst);
        return md5;
    }

    public CPseduLuaValue TestPublicFunc(CPseduLuaValue abc,CPseduLuaValue def,CPseduLuaValue cde,CPseduLuaValue aba,CPseduLuaValue abab,CPseduLuaValue d)
    {
        return CLuaNetworkInterface.GetIntValue(abc, 0);
    }

    public CPseduLuaValue TestPublicFunc1(CPseduLuaValue abc,CPseduLuaValue def,CPseduLuaValue cde,CPseduLuaValue aba,CPseduLuaValue abab,CPseduLuaValue d)
    {
        return CLuaNetworkInterface.GetIntValue(abc, 0);
    }

    public CPseduLuaValue TestPublicFunc2()
    {
        return new object[] {1,2,};
    }

    public CPseduLuaValue TestPublicFunc3()
    {
        return new object[] {1,2,""};
    }

    protected CPseduLuaValue TestPublicFunc4(CPseduLuaValue d)
    {
        return CLuaNetworkInterface.GetIntValue(d, 0);
    }

    protected CPseduLuaValue funcret1()
    {
        return new Dictionary<int, CPseduLuaValue>(new IntEqualityComparer())
        { {1, new object[] {1.2}} };
    }

    protected CPseduLuaValue funcret2()
    {
        return new Dictionary<int, CPseduLuaValue>(new IntEqualityComparer())
        { {1, "a" }};
    }

    protected CPseduLuaValue funcret3()
    {
        return new object[] { 1,2.2, };
    }

}



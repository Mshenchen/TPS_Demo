using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//查询成绩
public class MsgGetAchieve : MsgBase
{
    public MsgGetAchieve()
    {
        protoName = "MsgGetAchieve";
    }
    //服务端回
    public int win = 0;
    public int lost = 0;
}
//房间信息
[System.Serializable]
public class RoomInfo
{
    public int id = 0;   //房间id
    public int count = 0;  //人数
    public int status = 0;
}

public class MsgGetRoomList : MsgBase
{
    public MsgGetRoomList(){protoName = "MsgGetRoomList";}
    public RoomInfo[] rooms;
}

public class MsgCreateRoom : MsgBase
{
    public MsgCreateRoom()
    {
        protoName = "MsgCreateRoom";
    }

    public int result = 0;
}
public class MsgEnterRoom : MsgBase
{
    public MsgEnterRoom()
    {
        protoName = "MsgEnterRoom";
    }
    public int id = 0;
    public int result = 0;
}

[System.Serializable]
public class PlayerInfo
{
    public string id = "lpy";
    public int camp = 0;
    public int win = 0;
    public int lost = 0;
    public int isOwner = 0;
}
public class MsgGetRoomInfo : MsgBase
{
    public MsgGetRoomInfo() { protoName = "MsgGetRoomInfo"; }
    public PlayerInfo[] players;
}
public class MsgLeaveRoom : MsgBase
{
    public MsgLeaveRoom() { protoName = "MsgLeaveRoom"; }
    public int result = 0;
}
public class MsgStartBattle : MsgBase
{
    public MsgStartBattle() { protoName = "MsgStartBattle"; }
    public int result = 0;
}

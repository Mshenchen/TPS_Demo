using System;
using System.Collections.Generic;


public class Room
{
    public int id = 0;
    public int maxPlayer = 6;
    public Dictionary<string, bool> playerIds = new Dictionary<string, bool>();
    public string ownerId = "";
    public enum Status
    {
        PREPARE = 0,
        FIGHT = 1,
    }
    public Status status = Status.PREPARE;
    private long lastJudgeTime = 0;
    public void Update()
    {
        if (status == Status.PREPARE) return;
        if(NetManager.GetTimeStamp() - lastJudgeTime < 10f)
        {
            return;
        }
        lastJudgeTime = NetManager.GetTimeStamp();
        int winCamp = Judgement();
        if (winCamp == 0) return;
        status = Status.PREPARE;
        foreach (string id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            if(player.camp == winCamp) { player.data.win++; }
            else { player.data.lost++; }
        }
        MsgBattleResult msg = new MsgBattleResult();
        msg.winCamp = winCamp;
        Broadcast(msg);
    }
    //出生点配置
    //private static float[,,] birthConfig = new float[2, 3, 6]
    //{
       
    //}
    //初始化位置
    private void SetBirthPos(Player player,int index)
    {
        if (player.camp == 1)
        {
            player.x = 0;
            player.y = 0;
            player.z = 0 - 3 * index;
            player.ex = 0;
            player.ey = 0;
            player.ez = 0;
        }
        else
        {
            player.x = 3;
            player.y = 0;
            player.z = 0 - 3 * index;
            player.ex = 0;
            player.ey = 0;
            player.ez = 0;
        }
        
    }
    private void ResetPlayers()
    {
        int count1 = 0;
        int count2 = 0;
        foreach (string id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            if(player.camp == 1)
            {
                SetBirthPos(player, count1);
                count1++;
            }
            else
            {
                SetBirthPos(player, count2);
                count2++;
            }
            player.hp = 100;
        }
        
    }
    public RoleInfo PlayerToRoleInfo(Player player)
    {
        RoleInfo roleInfo = new RoleInfo();
        roleInfo.camp = player.camp;
        roleInfo.id = player.id;
        roleInfo.hp = player.hp;
        roleInfo.x = player.x;
        roleInfo.y = player.y;
        roleInfo.z = player.z;
        roleInfo.ex = player.ex;
        roleInfo.ey = player.ey;
        roleInfo.ez = player.ez;
        return roleInfo;
    }
    public bool StartBattle()
    {
        if(!CanStartBattle()) return false;
        status = Status.FIGHT;
        ResetPlayers();
        MsgEnterBattle msg = new MsgEnterBattle();
        msg.mapId = 1;
        msg.roles = new RoleInfo[playerIds.Count];
        int i = 0;
        foreach (string id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            msg.roles[i] = PlayerToRoleInfo(player);
            i++;
        }
        Broadcast(msg);
        return true;
    }
    public bool IsDie(Player player)
    {
        return player.hp <= 0;
    }
    public int Judgement()
    {
        int count1 = 0;
        int count2 = 0;
        foreach (string id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            if (!IsDie(player))
            {
                if(player.camp == 1) { count1++;}
                if(player.camp == 2) { count2++;}
            }
        }
        if(count1 <= 0)
        {
            return 2;
        }else if(count2 <= 0)
        {
            return 1;
        }
        return 0;
    }
    public bool AddPlayer(string id)
    {
        Player player = PlayerManager.GetPlayer(id);
        if (player == null)
        {
            Console.WriteLine("Room.AddPlayer fail,player is null");
            return false;
        }
        if(playerIds.Count>=maxPlayer)
        {
            Console.WriteLine("Room.AddPlayer fail,reach maxPlayer");
            return false;
        }
        if(status != Status.PREPARE)
        {
            Console.WriteLine("Room.AddPlayer fail,not PREPARE");
            return false;
        }
        if (playerIds.ContainsKey(id))
        {
            Console.WriteLine("Room.AddPlayer fail,already in this room");
            return false;
        }
        playerIds[id] = true;
        player.camp = switchCamp();
        player.roomId = this.id;
        if (ownerId == "")
        {
            ownerId = player.id;
        }
        Broadcast(ToMsg());
        return true;
    }
    public bool RemovePlayer(string id)
    {
        Player player = PlayerManager.GetPlayer(id);
        if(player == null)
        {
            Console.WriteLine("Room.RemovePlayer fail,player is null");
            return false;
        }
        if (!playerIds.ContainsKey(id))
        {
            Console.WriteLine("Room.RemovePlayer fail,not in this room");
            return false;
        }
        playerIds.Remove(id);
        player.camp = 0;
        player.roomId = -1;
        if (IsOwner(player))
        {
            ownerId = SwitchOwner();
        }
        Broadcast(ToMsg());
        if(status == Status.FIGHT)
        {
            player.data.lost++;
            MsgLeaveBattle msg = new MsgLeaveBattle();
            msg.id = player.id;
            Broadcast(msg);
        }
        return true;
    }
    public bool CanStartBattle()
    {
        if(status != Status.PREPARE)
        {
            return false;
        }
        int count1 = 0;
        int count2 = 0;
        foreach (string id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            if(player.camp == 1) { count1++; }
            else { count2++; }
        }
        if(count1 < 1 || count2 < 1)
        {
            return false;
        }
        return true;
    }
    public MsgBase ToMsg()
    {
        MsgGetRoomInfo msg = new MsgGetRoomInfo();
        int count = playerIds.Count;
        msg.players = new PlayerInfo[count];
        int i = 0;
        foreach (string id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            PlayerInfo playerInfo = new PlayerInfo();
            playerInfo.id = player.id;
            playerInfo.camp = player.camp;
            playerInfo.win = player.data.win;
            playerInfo.lost = player.data.lost;
            playerInfo.isOwner = 0;
            if (IsOwner(player))
            {
                playerInfo.isOwner = 1;
            }
            msg.players[i] = playerInfo;
            i++;
        }
        return msg;

    }
    public void Broadcast(MsgBase msg)
    {
        foreach (string id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            player.Send(msg);
        }
    }
    private string SwitchOwner()
    {
        foreach (string id in playerIds.Keys)
        {
            return id;
        }
        return "";
    }

    public bool IsOwner(Player player)
    {
        return player.id == ownerId;
    }

    private int switchCamp()
    {
        int count1 = 0;
        int count2 = 0;
        foreach (string id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            if(player.camp == 1) { count1++; }
            if (player.camp == 2) { count2++; }
        }
        if (count1 <= count2)
        {
            return 1;
        }
        else
        {
            return 2;
        }

    }
}

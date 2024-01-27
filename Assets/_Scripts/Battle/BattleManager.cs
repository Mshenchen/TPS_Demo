using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager
{
    public static Dictionary<string,BasePlayer> players = new Dictionary<string,BasePlayer>();
    public static void Init()
    {
        NetManager.AddMsgListener("MsgEnterBattle", OnMsgEnterBattle);
        NetManager.AddMsgListener("MsgBattleResult", OnMsgBattleResult);
        NetManager.AddMsgListener("MsgLeaveBattle", OnMsgLeaveBattle);
        NetManager.AddMsgListener("MsgSyncPlayer", OnMsgSyncPlayer);
        NetManager.AddMsgListener("MsgFire", OnMsgFire);
        NetManager.AddMsgListener("MsgHit", OnMsgHit);
    }

    private static void OnMsgHit(MsgBase msgBase)
    {
        MsgHit msg = (MsgHit)msgBase;
        SyncPlayer player = (SyncPlayer)GetPlayer(msg.id);
        if (player == null) return;
        //player.Attacked(msg.damage);
    }

    private static void OnMsgFire(MsgBase msgBase)
    {
        MsgFire msg = (MsgFire)msgBase;
        if (msg.id == GameMain.id)
        {
            return;
        }
        SyncPlayer player = (SyncPlayer)GetPlayer(msg.id);
        if (player == null) return;
        player.SyncFire(msg);
    }

    private static void OnMsgSyncPlayer(MsgBase msgBase)
    {
        MsgSyncPlayer msg = (MsgSyncPlayer)msgBase;
        if(msg.id == GameMain.id)
        {
            return;
        }
        SyncPlayer player = (SyncPlayer)GetPlayer(msg.id);
        if (player == null) return;
        player.SyncPos(msg);
    }

    public static void AddPlayer(string id,BasePlayer player)
    {
        players[id] = player;
    }
    public static void RemovePlayer(string id)
    {
        players.Remove(id);
    }
    public static BasePlayer GetPlayer(string id)
    {
        if(players.ContainsKey(id)) return players[id];
        return null;
    }
    public static void Reset()
    {
        foreach (BasePlayer player in players.Values)
        {
            MonoBehaviour.Destroy(player.gameObject);
        }
        players.Clear();
    }
    public static BasePlayer GetCtrlPlayer()
    {
        return GetPlayer(GameMain.id);
    }
    private static void OnMsgLeaveBattle(MsgBase msgBase)
    {
        MsgLeaveBattle msg = (MsgLeaveBattle)msgBase;
        BasePlayer player = GetPlayer(msg.id);
        if(player == null) return;
        RemovePlayer(msg.id);
        MonoBehaviour.Destroy(player.gameObject);
    }

    private static void OnMsgBattleResult(MsgBase msgBase)
    {
        MsgBattleResult msg = (MsgBattleResult)msgBase;
        bool isWin = false;
        BasePlayer player = GetCtrlPlayer();
        if(player != null&&player.camp == msg.winCamp)
        {
            isWin = true;
        }
        //显示界面
        UIManager.Instance.ShowPanel<ResultPanel>("ResultPanel");
    }

    private static void OnMsgEnterBattle(MsgBase msgBase)
    {
        MsgEnterBattle msg = (MsgEnterBattle)msgBase;
        EnterBattle(msg);
    }

    private static void EnterBattle(MsgEnterBattle msg)
    {
        BattleManager.Reset();
        //TODO:关闭面板
        UIManager.Instance.HidePanel("RoomPanel");
        UIManager.Instance.HidePanel("ResultPanel");
        for (int i = 0;i<msg.roles.Length;i++)
        {
            GeneratePlayer(msg.roles[i]);
        }
    }

    private static void GeneratePlayer(RoleInfo roleInfo)
    {
        string objName = "Player_" + roleInfo.id;
        //GameObject playerObj = new GameObject(objName);
        BasePlayer player = null;
        if(roleInfo.id == GameMain.id)
        {
            GameObject playerObj = ResMgr.Instance.Load<GameObject>("MyPlayer");
            playerObj.gameObject.name = objName;
            if (playerObj != null)
            {
                player = playerObj.GetComponent<CtrlPlayer>();
            }
        }
        else
        {
            GameObject playerObj = ResMgr.Instance.Load<GameObject>("OtherPlayer");
            playerObj.gameObject.name = objName;
            player = playerObj.GetComponent<SyncPlayer>();
        }
        //camera
        player.camp = roleInfo.camp;
        player.id = roleInfo.id;
        player.hp = roleInfo.hp;
        Vector3 pos = new Vector3(roleInfo.x, roleInfo.y, roleInfo.z);
        Vector3 rot = new Vector3(roleInfo.ex, roleInfo.ey, roleInfo.ez);
        player.transform.position = pos;
        player.transform.eulerAngles = rot;
        if(roleInfo.camp == 1)
        {
            //player.Init("Player");
        }
        else
        {
            //player.Init("Player");
        }
        AddPlayer(roleInfo.id, player);
    }
}

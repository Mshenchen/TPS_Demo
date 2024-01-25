using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : BasePanel
{
    public Button StartBtn;
    public Button CloseBtn;
    public Transform Content;
    public GameObject PlayerObj;
    public override void OnShow()
    {
        PlayerObj.SetActive(false);
        StartBtn.onClick.AddListener(OnStartClick);
        CloseBtn.onClick.AddListener(OnCloseClick);
        NetManager.AddMsgListener("MsgGetRoomInfo", OnMsgGetRoomInfo);
        NetManager.AddMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
        NetManager.AddMsgListener("MsgStartBattle", OnMsgStartBattle);
        MsgGetRoomInfo msg = new MsgGetRoomInfo();
        NetManager.Send(msg);
    }
    public override void OnHide()
    {
        NetManager.RemoveMsgListener("MsgGetRoomInfo", OnMsgGetRoomInfo);
        NetManager.RemoveMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
        NetManager.RemoveMsgListener("MsgStartBattle", OnMsgStartBattle);
      
    }
    private void OnMsgLeaveRoom(MsgBase msgBase)
    {
        MsgLeaveRoom msg = (MsgLeaveRoom)msgBase;
        if(msg.result == 0 )
        {
            TipMgr.Instance.ShowOneBtnTip("�˳�����");
            UIManager.Instance.ShowPanel<RoomListPanel>("RoomListPanel");
            Close();
            Debug.Log("OnMsgLeaveRoom---------------");
        }
        else
        {
            TipMgr.Instance.ShowOneBtnTip("�˳�����ʧ��");
        }
    }

    private void OnMsgStartBattle(MsgBase msgBase)
    {
        MsgStartBattle msg = (MsgStartBattle)msgBase;
        if(msg.result == 0 )
        {
            Close();
        }
        else
        {
            TipMgr.Instance.ShowOneBtnTip("��սʧ�ܣ�����������Ҫ��һ����ң�ֻ�з����ſ��Կ�ս");
        }
    }

    private void OnMsgGetRoomInfo(MsgBase msgBase)
    {
        MsgGetRoomInfo msg = (MsgGetRoomInfo)msgBase;
        for (int i = Content.childCount-1; i >= 0; i--)
        {
            GameObject o = Content.GetChild(i).gameObject;
            Destroy(o);
        }
        if(msg.players == null)
        {
            return;
        }
        for (int i = 0;i<msg.players.Length;i++)
        {
            GeneratePlayerInfo(msg.players[i]);
        }
    }

    private void GeneratePlayerInfo(PlayerInfo playerInfo)
    {
        GameObject o = Instantiate(PlayerObj);
        o.transform.SetParent(Content);
        o.SetActive(true);
        o.transform.localScale = Vector3.one;
        PlayerItem player = o.GetComponent<PlayerItem>();
        player.InitPlayerItem(playerInfo);
    }

    private void OnCloseClick()
    {
        MsgLeaveRoom msg = new MsgLeaveRoom();
        NetManager.Send(msg);
    }

    private void OnStartClick()
    {
        MsgStartBattle msg = new MsgStartBattle();
        NetManager.Send(msg);
    }
}

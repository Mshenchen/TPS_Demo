using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListPanel : BasePanel
{
    public Text idText;
    public Text scoreText;
    public Button createBtn;
    public Button reflashBtn;
    public Transform content;
    public GameObject roomObj;
    public override void OnShow()
    {
        createBtn.onClick.AddListener(OnCreateClick);
        reflashBtn.onClick.AddListener(OnReflashClick);
        roomObj.SetActive(false);
        idText.text = GameMain.id;
        //Э�����
        NetManager.AddMsgListener("MsgGetAchieve", OnMsgGetAchieve);
        NetManager.AddMsgListener("MsgGetRoomList", OnMsgGetRoomList);
        NetManager.AddMsgListener("MsgCreateRoom", OnMsgCreateRoom);
        NetManager.AddMsgListener("MsgEnterRoom", OnMsgEnterRoom);
        //���Ͳ�ѯ
        MsgGetAchieve msgGetAchieve = new MsgGetAchieve();
        NetManager.Send(msgGetAchieve);
        MsgGetRoomList msgGetRoomList = new MsgGetRoomList();
        NetManager.Send(msgGetRoomList);

    }
    public override void OnHide()
    {
        NetManager.RemoveMsgListener("MsgGetAchieve", OnMsgGetAchieve);
        NetManager.RemoveMsgListener("MsgGetRoomList", OnMsgGetRoomList);
        NetManager.RemoveMsgListener("MsgCreateRoom", OnMsgCreateRoom);
        NetManager.RemoveMsgListener("MsgEnterRoom", OnMsgEnterRoom);
    }

    private void OnMsgEnterRoom(MsgBase msgBase)
    {
       MsgEnterRoom msg = (MsgEnterRoom)msgBase;
        if(msg.result == 0 )
        {
            UIManager.Instance.ShowPanel<RoomPanel>("RoomPanel");
            UIManager.Instance.HidePanel("RoomListPanel");
        }
        else
        {
            TipMgr.Instance.ShowOneBtnTip("���뷿��ʧ��");
        }
    }

    private void OnMsgCreateRoom(MsgBase msgBase)
    {
      MsgCreateRoom msg = (MsgCreateRoom)msgBase;
        if(msg.result == 0 )
        {
            TipMgr.Instance.ShowOneBtnTip("�����ɹ�");
            UIManager.Instance.ShowPanel<RoomPanel>("RoomPanel");
            Close();
        }
        else
        {
            TipMgr.Instance.ShowOneBtnTip("��������ʧ��");
        }
    }

    private void OnMsgGetRoomList(MsgBase msgBase)
    {
        MsgGetRoomList msg = (MsgGetRoomList)msgBase;
        //��շ����б�
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            GameObject o = content.GetChild(i).gameObject;
            Destroy(o);
        }
        if(msg.rooms == null)
        {
            return;
        }
        for (int i = 0; i < msg.rooms.Length; i++)
        {
            GenerateRoom(msg.rooms[i]);
        }

    }

    private void GenerateRoom(RoomInfo roomInfo)
    {
        //��������
        GameObject o = Instantiate(roomObj);
        o.transform.SetParent(content);
        o.SetActive(true);
        o.transform.localScale = Vector3.one;
        //��ȡ���
        RoomItem item = o.GetComponent<RoomItem>();
        item.InitRoomItem(roomInfo);
    }

    private void OnMsgGetAchieve(MsgBase msgBase)
    {
        MsgGetAchieve msg = (MsgGetAchieve)msgBase;
        scoreText.text = msg.win + "ʤ" + msg.lost + "��";
    }

    private void OnReflashClick()
    {
        MsgGetRoomList msg = new MsgGetRoomList();
        NetManager.Send(msg);
    }

    private void OnCreateClick()
    {
        MsgCreateRoom msg = new MsgCreateRoom();
        NetManager.Send(msg);
    }
}

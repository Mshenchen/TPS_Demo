using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    // Start is called before the first frame update
    public Text IdText;
    public Text CountText;
    public Text StatusText;
    public Button JoinBtn;

    public void InitRoomItem(RoomInfo roomInfo)
    {
        IdText.text = roomInfo.id.ToString();
        CountText.text = roomInfo.count.ToString();
        if (roomInfo.status == 0)
        {
            StatusText.text = "׼����";
        }
        else
        {
            StatusText.text = "ս����";
        }
        JoinBtn.name = IdText.text;
        JoinBtn.onClick.AddListener(delegate ()
        {
            OnJoinClick(JoinBtn.name);
        });
        Debug.Log("InitRoomItem----");
    }

    private void OnJoinClick(string idString)
    {
        MsgEnterRoom msg = new MsgEnterRoom();
        msg.id = int.Parse(idString);
        NetManager.Send(msg);
    }

}

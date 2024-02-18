using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    public static string id = "";
    void Start()
    {
        NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
        NetManager.AddMsgListener("MsgKick", OnMsgKick);
        //初始化
        BattleManager.Init();
        UIManager.Instance.ShowPanel<LoginPanel>("LoginPanel");
    }



    // Update is called once per frame
    void Update()
    {
        NetManager.Update();
    }
    private void OnMsgKick(MsgBase msgbase)
    {
        TipMgr.Instance.ShowOneBtnTip("被踢下线");
    }

    private void OnConnectClose(string err)
    {
        Debug.Log("断开连接");
    }

}

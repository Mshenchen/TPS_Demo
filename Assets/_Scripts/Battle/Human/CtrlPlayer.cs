using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrlPlayer : BasePlayer
{
    private float lastSendSyncTime = 0;
    public static float syncInterval = 0.1f;
    private void Update()
    {
        SyncUpdate();
    }

    private void SyncUpdate()
    {
        if(Time.time - lastSendSyncTime < syncInterval) 
        {
            return;
        }
        lastSendSyncTime = Time.time;
        MsgSyncPlayer msg = new MsgSyncPlayer();
        msg.x = transform.position.x;
        msg.y = transform.position.y;
        msg.z = transform.position.z;
        msg.ex = transform.eulerAngles.x;
        msg.ey = transform.eulerAngles.y;
        msg.ez = transform.eulerAngles.z;
        NetManager.Send(msg);
    }
}

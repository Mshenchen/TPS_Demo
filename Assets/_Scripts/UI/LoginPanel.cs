using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{

    public InputField IdInput;
    public InputField PwInput;
    private Button loginBtn;
    private Button registerBtn;
    private Button connectBtn;
    private Button singlePlayBtn;
    void Start()
    {
        
    }
    
    public override void OnShow()
    {
        Debug.Log("OnShow====LoginPanel");
        loginBtn = this.transform.Find("LoginBtn").GetComponent<Button>();
        registerBtn = this.transform.Find("RegisterBtn").GetComponent<Button>();
        connectBtn = this.transform.Find("ConnectBtn").GetComponent<Button>();
        singlePlayBtn = this.transform.Find("SinglePlayBtn").GetComponent<Button>();
        loginBtn.onClick.AddListener(OnLoginClick);
        registerBtn.onClick.AddListener(OnRegisterClick);
        connectBtn.onClick.AddListener(OnConnectClick);
        singlePlayBtn.onClick.AddListener(OnPlayClick);
        //网络协议监听
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        //网络事件监听
        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
        NetManager.Connect("127.0.0.1", 8888);
    }

    private void OnPlayClick()
    {
        Close();
        GameObject obj = GameObject.Find("MyPlayer");
        if (obj != null)
        {
            return;
        }
        string objName = "Player";
        BasePlayer player = null;
        GameObject playerObj = ResMgr.Instance.Load<GameObject>("MyPlayer");
        playerObj.gameObject.name = objName;
        if (playerObj != null)
        {
            player = playerObj.GetComponent<CtrlPlayer>();
        }
    }

    private void OnConnectClick()
    {
      
        NetManager.Connect("127.0.0.1", 8888);
        Debug.Log("OnShow====OnConnectClick");
    }

    private void OnMsgLogin(MsgBase msgBase)
    {
        MsgLogin msg = (MsgLogin)msgBase;
        if(msg.result == 0)
        {
            //设置id
            GameMain.id = msg.id;
            Debug.Log("登录成功");
            UIManager.Instance.ShowPanel<RoomListPanel>("RoomListPanel");
            //UIManager.Instance.HidePanel("LoginPanel");
            Close();
        }
        else
        {
            TipMgr.Instance.ShowOneBtnTip("登录失败");
        }
    }

    public override void OnHide()
    {
        //网络协议监听
        NetManager.RemoveMsgListener("MsgLogin", OnMsgLogin);
        NetManager.RemoveEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.RemoveEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
    }

    private void OnConnectSucc(string err)
    {
        
    }
    private void OnConnectFail(string err)
    {
        
    }

    private void OnRegisterClick()
    {
        //打开注册面板
        UIManager.Instance.ShowPanel<RegisterPanel>("RegisterPanel");
    }

    private void OnLoginClick()
    {
        if(IdInput.text == "" || PwInput.text == "")
        {
            TipMgr.Instance.ShowOneBtnTip("用户名和密码不能为空");
            return;
        }
        MsgLogin msgLogin = new MsgLogin();
        msgLogin.id = IdInput.text;
        msgLogin.pw = PwInput.text;
        NetManager.Send(msgLogin);
        Debug.Log("NetManager.Send(msgLogin);");
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{

    public InputField IdInput;
    public InputField PwInput;
    private Button loginBtn;
    private Button registerBtn;
    
    void Start()
    {
        
    }
    
    public override void OnShow()
    {
        Debug.Log("OnShow====LoginPanel");
        loginBtn = this.transform.Find("LoginBtn").GetComponent<Button>();
        registerBtn = this.transform.Find("RegisterBtn").GetComponent<Button>();
        loginBtn.onClick.AddListener(OnLoginClick);
        registerBtn.onClick.AddListener(OnRegisterClick);
        //网络协议监听
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        //网络事件监听
        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
        NetManager.Connect("127.0.0.1", 8888);
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

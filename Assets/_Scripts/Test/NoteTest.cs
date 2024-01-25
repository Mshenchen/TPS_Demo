using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteTest : MonoBehaviour
{
    public InputField idInput;
    public InputField pwInput;
    public InputField textInput;
    private void Start()
    {
        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
        NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
        NetManager.AddMsgListener("MsgRegister", OnMsgRegister);
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        NetManager.AddMsgListener("MsgKick", OnMsgKick);
        NetManager.AddMsgListener("MsgGetText", OnMsgGetText);
        NetManager.AddMsgListener("MsgSaveText", OnMsgSaveText);
    }
    //��ҵ�����Ӱ�ť
    public void OnConnectClick()
    {
        NetManager.Connect("127.0.0.1", 8888);
    }
    //�����ر�
    public void OnCloseClick()
    {
        NetManager.Close();
    }
    //���ӳɹ��ص�
    void OnConnectSucc(string err)
    {
        Debug.Log("OnConnectSucc");

    }

    //����ʧ�ܻص�
    void OnConnectFail(string err)
    {
        Debug.Log("OnConnectFail " + err);
    }

    //�ر�����
    void OnConnectClose(string err)
    {
        Debug.Log("OnConnectClose");
    }
    // Update is called once per frame
    void Update()
    {
        NetManager.Update();
    }

    public void OnRegisterClick()
    {
        MsgRegister msg = new MsgRegister();
        msg.id = idInput.text;
        msg.pw = pwInput.text;
        NetManager.Send(msg);
    }
    public void OnLoginClick()
    {
        MsgLogin msg = new MsgLogin();
        msg.id = idInput.text;
        msg.pw = pwInput.text;
        Debug.Log(msg.pw + "msg.pw");
        NetManager.Send(msg);
    }
    public void OnSaveClick()
    {
        MsgSaveText msg = new MsgSaveText();
        msg.text = textInput.text;
        NetManager.Send(msg);
    }
    public void OnMsgSaveText(MsgBase msgBase)
    {
        MsgSaveText msg = (MsgSaveText)msgBase;
        if(msg.result == 0)
        {
            Debug.Log("����ɹ�");
        }
        else
        {
            Debug.Log("����ʧ��");
        }
    }

    public void OnMsgGetText(MsgBase msgBase)
    {
        MsgGetText msg = (MsgGetText)msgBase;
        textInput.text = msg.text;

    }

    public void OnMsgKick(MsgBase msgBase)
    {
        Debug.Log("��������");
    }

    public void OnMsgLogin(MsgBase msgBase)
    {
        MsgLogin msg = (MsgLogin)msgBase;
        if(msg.result == 0)
        {
            Debug.Log("��¼�ɹ�");
            //������±��ļ�
            MsgGetText msgGetText = new MsgGetText();
            NetManager.Send(msgGetText);
        }
        else
        {
            Debug.Log("��¼ʧ��");
        }
    }

    public void OnMsgRegister(MsgBase msgBase)
    {
        MsgRegister msg = (MsgRegister)msgBase;
        if(msg.result == 0)
        {
            Debug.Log("ע��ɹ�");
        }
        else
        {
            Debug.Log("ע��ʧ��");
        }
    }
}

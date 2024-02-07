using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : BasePanel
{
    public InputField idInput;
    public InputField pwInput;
    public InputField repInput;
    public Button regBtn;
    public Button closeBtn;
    public override void OnShow()
    {
        regBtn.onClick.AddListener(OnRegClick);
        closeBtn.onClick.AddListener(OnCloseClick);
        NetManager.AddMsgListener("MsgRegister", OnMsgRegister);
    }

    private void OnMsgRegister(MsgBase msgBase)
    {
        MsgRegister msg = (MsgRegister) msgBase;
        if (msg.result == 0)
        {
            Debug.Log("注册成功");
            TipMgr.Instance.ShowOneBtnTip("注册成功");
            Close();
        }
        else
        {
            TipMgr.Instance.ShowOneBtnTip("注册失败");
        }
    }

    private void OnCloseClick()
    {
        Close();
    }

    private void OnRegClick()
    {
        if (idInput.text == "" || pwInput.text == "")
        {
            TipMgr.Instance.ShowOneBtnTip("用户名和密码不能为空");
            return;
        }
        if(repInput.text != pwInput.text)
        {
            TipMgr.Instance.ShowOneBtnTip("两次输入的密码不同");
            return;
        }

        MsgRegister msgReg = new MsgRegister();
        msgReg.id = idInput.text;
        msgReg.pw = pwInput.text;
        NetManager.Send(msgReg);
    }

    //public void Close()
    //{
    //    UIManager.Instance.HidePanel("RegisterPanel");
    //}
    public override void OnHide()
    {
        NetManager.RemoveMsgListener("MsgRegister", OnMsgRegister);
    }
}

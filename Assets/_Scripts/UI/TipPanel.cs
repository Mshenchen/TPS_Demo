using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel : BasePanel
{
    public Text text;
    public Button CloseBtn;
    public void InitInfo(string info)
    {
        text.text = info;
    }

    public override void OnShow()
    {
        base.OnShow();
        CloseBtn.onClick.AddListener(Close);
    }

    public override void OnHide()
    {
        base.OnHide();
        CloseBtn.onClick.RemoveListener(Close);
    }

    //public void Close()
    //{
    //    UIManager.Instance.HidePanel("TipPanel");
    //}
}

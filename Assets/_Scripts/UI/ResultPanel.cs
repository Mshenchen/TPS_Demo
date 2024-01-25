using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanel : BasePanel
{
    public Image winImage;
    public Image lostImage;
    public Button confirmBtn;
    public override void OnShow()
    {
        confirmBtn.onClick.AddListener(OnConfirmClick);
    }

    private void OnConfirmClick()
    {
        UIManager.Instance.ShowPanel<RoomPanel>("RoomPanel");
        Close();
    }
}

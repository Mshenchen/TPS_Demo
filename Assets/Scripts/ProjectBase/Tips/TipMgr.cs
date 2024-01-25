using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipMgr : BaseManager<TipMgr>
{
    public void ShowOneBtnTip(string info)
    {
        
        UIManager.Instance.ShowPanel<TipPanel>("TipPanel", E_UI_Layer.System, (panel) =>
        {
            panel.InitInfo(info);
        });
    }
    
}

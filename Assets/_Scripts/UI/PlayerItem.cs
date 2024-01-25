using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItem : MonoBehaviour
{
    public Text IdText;
    public Text CampText;
    public Text ScoreText;
    public void InitPlayerItem(PlayerInfo playerInfo)
    {
        IdText.text = playerInfo.id;
        if(playerInfo.camp == 1)
        {
            CampText.text = "��";
        }
        else
        {
            CampText.text = "��";
        }
        if(playerInfo.isOwner == 1)
        {
            CampText.text = CampText.text + "! ";
        }
        ScoreText.text = playerInfo.win + "ʤ" + playerInfo.lost + "��";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayer : MonoBehaviour
{
    public float hp = 100;
    public string id = "";
    public int camp = 0;
    void Start()
    {
        
    }

    public virtual void Init(string playerPath)
    {
        GameObject player = ResMgr.Instance.Load<GameObject>(playerPath);
        //player = (GameObject)Instantiate(player);
        player.transform.parent = transform;
        player.transform.localPosition = Vector3.zero;
        player.transform.localRotation = Quaternion.identity;
    }
}

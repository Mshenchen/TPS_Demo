using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour {

    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;

    private Rigidbody bulletRigidbody;
    public BasePlayer player;
    private void Awake() {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void Start() {
        float speed = 50f;
        bulletRigidbody.velocity = transform.forward * speed;
        //发送同步协议
        MsgFire msg = new MsgFire();
        msg.x = transform.position.x;
        msg.y = transform.position.y;
        msg.z = transform.position.z;
        msg.ex = transform.eulerAngles.x;
        msg.ey = transform.eulerAngles.y;
        msg.ez = transform.eulerAngles.z;
        NetManager.Send(msg);
    }

    private void OnCollisionEnter(Collision other) {

        if (other.gameObject.GetComponent<BulletTarget>() != null)
        {
            Instantiate(vfxHitGreen,transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(vfxHitRed,transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
        //if (other.getcomponent<bullettarget>() != null)
        //{
        //    instantiate(vfxhitgreen, transform.position, quaternion.identity);
        //}
        //else
        //{
        //    instantiate(vfxhitred, transform.position, quaternion.identity);
        //}
        //Destroy(gameobject);
        //GameObject collObj = other.gameObject;
        //BasePlayer hitPlayer = collObj.GetComponent<BasePlayer>();
        ////TODO:不能打自己
        //if (hitPlayer != null) 
        //{
        //    SendMsgHit(player, hitPlayer);
        //}
    }
    void SendMsgHit(BasePlayer player,BasePlayer hitPlayer)
    {
        if (hitPlayer == null||player == null) return;
        if(player.id != GameMain.id)
        {
            return;
        }
        MsgHit msg = new MsgHit();
        msg.targetId = hitPlayer.id;
        msg.id = player.id;
        msg.x = transform.position.x;
        msg.y = transform.position.y;
        msg.z = transform.position.z;
        NetManager.Send(msg);
    }
}
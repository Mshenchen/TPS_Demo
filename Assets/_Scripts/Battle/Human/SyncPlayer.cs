using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPlayer : BasePlayer
{
    private Vector3 lastPos;
    private Vector3 lastRot;
    private Vector3 forecastPos;
    private Vector3 forecastRot;
    private float forecastTime;
    private Animator animator;
    public override void Awake()
    {
        lastPos = transform.position;
        lastRot = transform.eulerAngles;
        forecastPos = transform.position;
        forecastRot = transform.eulerAngles;
        forecastTime = Time.time;
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        ForecastUpdate();
    }
    public void ForecastUpdate()
    {
        float t = (Time.time - forecastTime) / CtrlPlayer.syncInterval;
        t = Mathf.Clamp(t,0f,1f);
        Vector3 pos = transform.position;
        pos = Vector3.Lerp(pos, forecastPos,t);
        transform.position = pos;
        Quaternion quat = transform.rotation;
        Quaternion forcastQuat = Quaternion.Euler(forecastRot);
        quat = Quaternion.Lerp(quat, forcastQuat,t);
        transform.rotation = quat;
      
    }
    public void SyncPos(MsgSyncPlayer msg)
    {
        Vector3 pos = new Vector3(msg.x,msg.y,msg.z);
        Vector3 rot = new Vector3(msg.ex,msg.ey,msg.ez);
        forecastPos = pos + 2 * (pos - lastPos);
        forecastRot = rot + 2 * (rot - lastRot);
        lastPos = pos;
        lastRot = rot;
        forecastTime = Time.time;
        //TODO:炮塔同步 
    }
    public void SyncAnim(MsgSyncAnim msg)
    {
        animator.SetFloat("Speed", msg.speedValue);
        animator.SetBool("Jump", msg.jumpValue);
        animator.SetBool("Grounded", msg.groundedValue);
        animator.SetBool("FreeFall", msg.freefallValue);
        animator.SetFloat("MotionSpeed", msg.motionSpeedValue);

    }
    public void SyncFire(MsgFire msg)
    {
        
    }
}

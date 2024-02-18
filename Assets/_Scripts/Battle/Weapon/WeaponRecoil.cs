using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    public CinemachineFreeLook playerAimCamera;
    [HideInInspector]public CinemachineImpulseSource cameraShake;
    public Vector2[] recoilPattern;
    float verticalRecoil;
    float horizontalRecoil;
    public float duration;
    float time;
    int index;
    private void Awake()
    {
        cameraShake = GetComponent<CinemachineImpulseSource>();
    }
    private void Start()
    {
        playerAimCamera = GameObject.Find("Cameras").transform.Find("PlayerFollowCamera").GetComponent<CinemachineFreeLook>();
    }
    int NextIndex(int index)
    {
        return (index + 1) % recoilPattern.Length;
    }
    public void Reset()
    {
        index = 0;
    }
    public void GenerateRecoil()
    {
        time = duration;
        cameraShake.GenerateImpulse(Camera.main.transform.forward);
        horizontalRecoil = recoilPattern[index].x;
        verticalRecoil = recoilPattern[index].y;
        index = NextIndex(index);
    }
    
    private void Update()
    {
        
        if (time > 0)
        {
            playerAimCamera.m_YAxis.Value -= ((verticalRecoil/1000)*Time.deltaTime)/duration;
            playerAimCamera.m_XAxis.Value -= ((horizontalRecoil / 10) * Time.deltaTime) / duration;
            time -= Time.deltaTime;
        }
    }
}

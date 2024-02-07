using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;
using System.Security.Claims;
using UnityEngine.Animations.Rigging;

public class ThirdPersonShootController : MonoBehaviour
{
    private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private InputReader input;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform pfBulletProjectile;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Vector3 DirOffset;
    private CtrlPlayer ctrlPlayer;
    private Animator animator;
    private bool isAim;
    private bool isShoot;
    private Vector3 mouseWorldPosition = Vector3.zero;
    public Transform RightHandPosition;
    public Transform LeftHandPosition;
    public Transform hitTransform;
    public Transform ViewTransform;
    public RaycastWeapon weapon;
    private bool isRotateBody = false;
    private void Awake()
    {
        ctrlPlayer = GetComponent<CtrlPlayer>();
        animator = GetComponent<Animator>();
        aimVirtualCamera = GameObject.Find("Cameras").transform.Find("PlayerAimCamera").GetComponent<CinemachineVirtualCamera>();
        aimVirtualCamera.Follow = this.transform.GetChild(0).GetChild(2);
    }

    void Start()
    {
        input.AimEvent += HandleAim;
        input.ShootEvent += HandleShoot;
    }



    // Update is called once per frame
    void Update()
    {
        //isAim = true;
        RaycastCheck();
        Aim();
        AdjustShootForward();
    }
    private void OnAnimatorIK(int layerIndex)
    {
       
       
    }
    
    private void Aim()
    {
        if (isAim)
        {
            
            aimVirtualCamera.gameObject.SetActive(true);
            ctrlPlayer.SetSensitivity(aimSensitivity);
            ctrlPlayer.SetRotateOnMove(false);
            RotateBody();
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            ctrlPlayer.SetSensitivity(normalSensitivity);
            ctrlPlayer.SetRotateOnMove(true); 
        }
            
    }
    private void AdjustShootForward()
    {
        if (isShoot)
        {
            
            
            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
            transform.forward =  aimDirection;
            //Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;

        }
        else
        {
            
            //aimRig.weight = 0f;
        }
    }
    private void HandleShoot(bool context)
    {
        isShoot = context;
    }

    private void HandleAim(bool context)
    {
        isAim = context;
        
    }
    public void RaycastCheck()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            mouseWorldPosition = raycastHit.point;
        }
    }
    private void RotateChest()
    {
        Vector3 worldAimTarget = mouseWorldPosition;
        //worldAimTarget.y = ViewTransform.position.y;
        Vector3 aimDirection = (worldAimTarget - ViewTransform.position).normalized;
        Quaternion aimRotation = Quaternion.LookRotation(aimDirection);
        //transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        animator.SetBoneLocalRotation(HumanBodyBones.Chest, aimRotation);
    }
    private void RotateBody()
    {
        Vector3 worldAimTarget = mouseWorldPosition;
        worldAimTarget.y = ViewTransform.position.y;
        Vector3 aimDirection = (worldAimTarget - ViewTransform.position).normalized;
        Quaternion aimRotation = Quaternion.LookRotation(aimDirection);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, aimRotation, Time.deltaTime * 20f);
    }
    public void SetHandIK(float weight)
    {
        animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandPosition.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandPosition.rotation);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandPosition.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandPosition.rotation);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, weight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, weight);
    }
}

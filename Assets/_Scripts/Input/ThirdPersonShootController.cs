using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class ThirdPersonShootController : BasePlayer
{
    private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private InputReader input;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform pfBulletProjectile;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Vector3 DirOffset;
    private PlayerController playerController;
    private Animator animator;
    private bool isAim;
    private bool isShoot;
    private Vector3 mouseWorldPosition = Vector3.zero;
    public Transform RightHandPosition;
    public Transform LeftHandPosition;
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
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
        RaycastCheck();
        Aim();
        Shoot();
    }
    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandPosition.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandPosition.rotation);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandPosition.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandPosition.rotation);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
    }
    public void RaycastCheck()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        //Transform hitTransform = null;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask)) {
            mouseWorldPosition = raycastHit.point;
            // hitTransform = raycastHit.transform;
        }
    }
    private void Aim()
    {
        if (isAim)
        {
            //aimVirtualCamera.gameObject.SetActive(true);
            //playerController.SetSensitivity(aimSensitivity);
            //playerController.SetRotateOnMove(false);
            //animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 13f));
            RotateBody();
        }
        else
        {
            //aimVirtualCamera.gameObject.SetActive(false);
            //playerController.SetSensitivity(normalSensitivity);
            //playerController.SetRotateOnMove(true);
            //animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 13f));
        }
            
    }
    private void Shoot()
    {
        if (isShoot)
        {
            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
            transform.forward =  aimDirection;
            Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
            Transform bulletObj = Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            BulletProjectile bullet = bulletObj.GetComponent<BulletProjectile>();
            bullet.player = this;
            isShoot = false;
        }
    }
    private void HandleShoot(bool context)
    {
        isShoot = context;
    }

    private void HandleAim(bool context)
    {
        isAim = context;
        print("isAim"+isAim);
    }

    private void RotateBody()
    {
        Vector3 worldAimTarget = mouseWorldPosition;
        worldAimTarget.y = transform.position.y;
        Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
    }
    
}

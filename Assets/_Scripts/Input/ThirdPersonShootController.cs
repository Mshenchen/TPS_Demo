using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class ThirdPersonShootController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
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
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
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

    public void RaycastCheck()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        Transform hitTransform = null;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask)) {
            mouseWorldPosition = raycastHit.point;
            // hitTransform = raycastHit.transform;
        }
    }
    private void Aim()
    {
        if (isAim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            playerController.SetSensitivity(aimSensitivity);
            playerController.SetRotateOnMove(false);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 13f));
            RotateBody();
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            playerController.SetSensitivity(normalSensitivity);
            playerController.SetRotateOnMove(true);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 13f));
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
            Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
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

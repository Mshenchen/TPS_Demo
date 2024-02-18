using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CtrlPlayer : BasePlayer
{
    public float Sensitivity = 1f;    //灵敏度
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;
    public float MoveSpeed = 2.0f;
    public float SprintSpeed = 5.335f;
    public float SpeedChangeRate = 10.0f;  //加速或减速速率
    private float speed;  //玩家的速度
    private float targetRotation = 0.0f;
    private float animationBlend;
    private float rotationVelocity;
    private float verticalVelocity;
    private float terminalVelocity = 53.0f;
    
    [SerializeField] private InputReader input;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float rotateSpeed;
    private Vector2 moveDirection;
    private Vector2 lookDirection;
    private Vector2 m_Rotation;
    private bool isJumping;
    private bool isSprintState;  //奔跑状态

    private bool isRotateOnMove = true;
    // 相机
    //private CinemachineVirtualCamera playerFollowCamera;
    private CinemachineFreeLook playerFollowCamera;
    private GameObject mainCamera;
    public Transform CinemachineCameraTarget;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    [Tooltip("为相机锁住时添加的角度")]
    public float CameraAngleOverride = 0.0f;
    [Tooltip("相机抬高最大角度")]
    public float TopClamp = 70.0f;
    [Tooltip("相机低下最大角度")]
    public float BottomClamp = -30.0f;
    private const float _threshold = 0.01f;
    [Tooltip("是否锁住相机位置")]
    public bool LockCameraPosition = false;
    private Animator animator;

   
    public float GroundedOffset = -0.14f;
    public bool Grounded = true;
    [Tooltip("地面检测的范围")]
    public float GroundedRadius = 0.28f;
    [Tooltip("地面检测的层级")]
    public LayerMask GroundLayers;
    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;
    [Tooltip("进入跳跃状态所需时间")]
    public float JumpTimeout = 0.50f;
    [Tooltip("进入下落状态所需时间")]
    public float FallTimeout = 0.15f;
    public float JumpHeight = 1.2f;
    public float Gravity = -15.0f;
    private float lastSendSyncTime = 0;
    public static float syncInterval = 0.033f;
    public override void Awake()
    {
        base.Awake();
        // get a reference to our main camera
        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
        animator = GetComponent<Animator>();
        playerFollowCamera = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineFreeLook>();
        CinemachineCameraTarget = this.transform.GetChild(0).GetChild(2);
        playerFollowCamera.Follow = CinemachineCameraTarget.transform;
    }
    private void Start()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        input.LookEvent += HandleLook;
        input.MoveEvent += HandleMove;
        input.JumpEvent += HandleJump;
        input.SprintEvent += HandleSprint;
        input.JumpCancelEvent += HandleCancelJump;

    }
    private void Update()
    {
        GroundedCheck();
        Move();
        JumpAndGravity();
        SyncUpdate();
    }
    public void LateUpdate()
    {
        CameraRotation();
    }
    private void CameraRotation()
    {
        if (lookDirection.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            _cinemachineTargetYaw += lookDirection.x * Sensitivity * Time.deltaTime;
            _cinemachineTargetPitch += lookDirection.y * Sensitivity * Time.deltaTime;
        }
        // 相机旋转角度限制
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // 相机跟随
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }
    private void Move()
    {
        // 根据当前状态设置速度
        float targetSpeed = isSprintState ? SprintSpeed : MoveSpeed;
        if (moveDirection == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        //加速或减速至目标速度
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed,
                Time.deltaTime * SpeedChangeRate);
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }

        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (animationBlend < 0.01f) animationBlend = 0f;

        //单位化移动方向
        Vector3 inputDirection = new Vector3(moveDirection.x, 0.0f, moveDirection.y).normalized;

        // 如果有一个移动输入，当玩家移动时旋转玩家
        if (moveDirection != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity,
                RotationSmoothTime);

            if (isRotateOnMove)
            {
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

        }


        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        // move the player
        controller.Move(targetDirection.normalized * (speed * Time.deltaTime) +
                         new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);

        // update animator if using character

        animator.SetFloat(animIDSpeed, animationBlend);
        animator.SetFloat(animIDMotionSpeed, 1);

    }
    private void JumpAndGravity()
    {
        if (Grounded)
        {
            fallTimeoutDelta = FallTimeout;
            animator.SetBool(animIDJump, false);
            animator.SetBool(animIDFreeFall, false);
            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -2f;
            }
            if (isJumping && jumpTimeoutDelta <= 0.0f)
            {
                verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                animator.SetBool(animIDJump, true);
            }

            if (jumpTimeoutDelta >= 0.0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            jumpTimeoutDelta = JumpTimeout;
            if (fallTimeoutDelta >= 0.0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                animator.SetBool(animIDFreeFall, true);
            }
            isJumping = false;
        }
        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += Gravity * Time.deltaTime;
        }
    }
    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);
        animator.SetBool(animIDGrounded, Grounded);
    }
    private void HandleCancelJump()
    {
        isJumping = false;
    }

    private void HandleJump()
    {
        isJumping = true;
    }

    private void HandleMove(Vector2 dir)
    {
        moveDirection = dir;
    }
    private void HandleLook(Vector2 dir)
    {
        lookDirection = dir;
    }
    private void HandleSprint(bool isSprint)
    {
        isSprintState = isSprint;
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
            GroundedRadius);
    }
   
    public void SetSensitivity(float newSensitivity)
    {
        Sensitivity = newSensitivity;
    }
    public void SetRotateOnMove(bool newRotateOnMove)
    {
        isRotateOnMove = newRotateOnMove;
    }
    private void SyncUpdate()
    {
        if(Time.time - lastSendSyncTime < syncInterval) 
        {
            return;
        }
        lastSendSyncTime = Time.time;
        MsgSyncPlayer msg = new MsgSyncPlayer();
        msg.x = transform.position.x;
        msg.y = transform.position.y;
        msg.z = transform.position.z;
        msg.ex = transform.eulerAngles.x;
        msg.ey = transform.eulerAngles.y;
        msg.ez = transform.eulerAngles.z;
        NetManager.Send(msg);
        MsgSyncAnim msgAnim = new MsgSyncAnim();
        msgAnim.speedValue = animator.GetFloat(animIDSpeed);
        msgAnim.jumpValue = animator.GetBool(animIDJump);
        msgAnim.groundedValue = animator.GetBool(animIDGrounded);
        msgAnim.freefallValue = animator.GetBool(animIDFreeFall);
        msgAnim.motionSpeedValue = animator.GetFloat(animIDMotionSpeed);
        NetManager.Send(msgAnim);
    }
   
}

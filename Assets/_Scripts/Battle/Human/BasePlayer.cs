using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class BasePlayer : MonoBehaviour
{
    public float hp = 100;
    public string id = "";
    public int camp = 0;
    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    public CharacterController controller;
    // animation IDs
    [HideInInspector]public int animIDSpeed;
    [HideInInspector] public int animIDGrounded;
    [HideInInspector] public int animIDJump;
    [HideInInspector] public int animIDFreeFall;
    [HideInInspector] public int animIDMotionSpeed;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
    public virtual void Awake()
    {
        controller = this.GetComponent<CharacterController>();
        AssignAnimationIDs();
    }
    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }
    public void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(controller.center), FootstepAudioVolume);
            }
        }
    }
    public void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (LandingAudioClip == null) return;
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(controller.center), FootstepAudioVolume);
        }
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

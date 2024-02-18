using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReloadWeapon : MonoBehaviour
{
    public Animator rigController;
    public InputReader input;
    public WeaponAnimationEvents weaponAnimationEvents;
    public ActiveWeapon activeWeapon;
    public Transform leftHand;
    GameObject magazineHand;
    void Start()
    {
        input.ReloadBulletEvent += HandleReload;
        weaponAnimationEvents.WeaponAnimationEvent.AddListener(OnAnimationEvent);
    }

    private void OnAnimationEvent(string eventName)
    {
       switch (eventName)
        {
            case "detach_magazine":
                DetachMagazine();
                break;
            case "drop_magazine":
                DropMagazine();
                break;
            case "refill_magazine":
                RefillMagazine();
                break;
            case "attach_magazine":
                AttachMagazine();
                break;

        }
    }

    private void HandleReload(bool obj)
    {
        RaycastWeapon weapon = activeWeapon.GetActiveWeapon();
        input._gameInput.FindAction("Shoot").Disable();
        if(weapon != null&&weapon.ammoCount <= weapon.clipSize)
        {
            Debug.Log("HandleReload");
            rigController.SetTrigger("reload_weapon");
        }
      
    }

    // ÄÃ×ßµ¯¼Ð
    private void DetachMagazine()
    {
        RaycastWeapon weapon = activeWeapon.GetActiveWeapon();
        magazineHand = Instantiate(weapon.magazine, leftHand, true);
        weapon.magazine.SetActive(false);
    }
    //ÈÓµôµ¯¼Ð
    private void DropMagazine()
    {
        GameObject droppedMagazine = Instantiate(magazineHand, magazineHand.transform.position, magazineHand.transform.rotation);
        droppedMagazine.AddComponent<Rigidbody>();
        droppedMagazine.AddComponent<BoxCollider>();
        magazineHand.SetActive(false);
    }
    //»ñµÃµ¯¼Ð
    private void RefillMagazine()
    {
        magazineHand.SetActive(true);
    }
    //×°ÉÏµ¯¼Ð
    private void AttachMagazine()
    {
        RaycastWeapon weapon = activeWeapon.GetActiveWeapon();
        weapon.magazine.SetActive(true);
        Destroy(magazineHand);
        weapon.ammoCount = weapon.clipSize;
        activeWeapon.UpdateBulletNum(weapon);
        rigController.ResetTrigger("reload_weapon");
        input._gameInput.FindAction("Shoot").Enable();
    }

}

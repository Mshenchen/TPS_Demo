using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] private InputReader input;
    public enum WeaponSlot
    {
        Primary,
        Secondary,
    }
    public Rig handIK;
    public Transform[] weaponSlots;
    public bool isHolster; // «∑Ò‘⁄±≥…œ
    private RaycastWeapon weapon;
    private bool isShoot;
    public Transform weaponLeftGrip;
    public Transform weaponRightGrip;
    public Animator rigAnimator;
    public CinemachineFreeLook playerAimCamera;
    RaycastWeapon[] equippedWeapons = new RaycastWeapon[2];
    int activeWeaponIndex;
    public Text BulletNumText;
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        BulletNumText = GameObject.Find("Canvas").transform.Find("BulletNumText").GetComponent<Text>();
        RaycastWeapon existingWeapon = GetComponentInChildren<RaycastWeapon>();
        if (existingWeapon != null)
        {
            Equip(existingWeapon);
        }
        input.ShootEvent += HandleShoot;
        input.EquipEvent += HandleEquip;
        input.SwitchWeaponEvent += HandleSwitchWeapon;
    }
    public RaycastWeapon GetActiveWeapon()
    {
        return GetWeapon(activeWeaponIndex);
    }
    private void HandleSwitchWeapon(bool context)
    {
        if(isHolster == false && equippedWeapons[activeWeaponIndex] != null)
        {
            if(activeWeaponIndex == 0)
            {
                SetActiveWeapon(WeaponSlot.Secondary);
            }
            if (activeWeaponIndex == 1)
            {
                SetActiveWeapon(WeaponSlot.Primary);
            }
        }
    }

    RaycastWeapon GetWeapon(int index)
    {
        if (index < 0 || index >= equippedWeapons.Length)
        {
            return null;
        }
        return equippedWeapons[index];
    }
    private void HandleEquip(bool context)
    {

        ToggleActiveWeapon();
    }

    private void HandleShoot(bool context)
    {
        isShoot = context;
        if (equippedWeapons[activeWeaponIndex] != null)
        {
            if (isShoot == true)
            {
                equippedWeapons[activeWeaponIndex].StartFiring();
            }
            else
            {
                equippedWeapons[activeWeaponIndex].StopFiring();
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        var weapon = GetWeapon(activeWeaponIndex);
        if(weapon != null&& isHolster==false)
        {
            if(weapon.isFiring)
            {
                weapon.Firing();
                UpdateBulletNum(weapon);
            }
            weapon.UpdateBullets(Time.deltaTime);
        }
        else
        {
            //handIK.weight = 0f;
            //rigAnimator.SetLayerWeight(1, 0f);
        }
    }
    void ToggleActiveWeapon()
    {
        isHolster = rigAnimator.GetBool("isHolster");
        if(isHolster)
        {
            StartCoroutine(ActivateWeapon(activeWeaponIndex));
        }
        else
        {
            StartCoroutine(HolsterWeapon(activeWeaponIndex));
        }
    }
    public void Equip(RaycastWeapon newWeapon)
    {
        int weaponSlotIndex = (int)newWeapon.weaponSlot;
        var weapon = GetWeapon(weaponSlotIndex);
        if (weapon != null)
        {
            Destroy(weapon.gameObject);
        }
       
        weapon = newWeapon;
        weapon.recoil.playerAimCamera = playerAimCamera;
        weapon.transform.SetParent(weaponSlots[weaponSlotIndex],false);
        equippedWeapons[weaponSlotIndex] = weapon;
        SetActiveWeapon(newWeapon.weaponSlot);
        UpdateBulletNum(weapon);
    }
    void SetActiveWeapon(WeaponSlot weaponSlot)
    {
        int holsterIndex = activeWeaponIndex;
        int activateIndex = (int)weaponSlot;
        if(holsterIndex == activateIndex)
        {
            holsterIndex = -1;
        }
        StartCoroutine(SwitchWeapon(holsterIndex,activateIndex));
    }
    IEnumerator SwitchWeapon(int holsterIndex,int activateIndex)
    {
        yield return StartCoroutine(HolsterWeapon(holsterIndex));
        yield return StartCoroutine(ActivateWeapon(activateIndex));
        activeWeaponIndex = activateIndex;
    }
    IEnumerator HolsterWeapon(int index)
    {
        isHolster = true;
        var weapon = GetWeapon(index);
        if (weapon != null)
        {
            rigAnimator.SetBool("isHolster", true);
            do
            {
                yield return new WaitForEndOfFrame();
            } while (rigAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f);
        }
    }
    IEnumerator ActivateWeapon(int index)
    {
        
        var weapon = GetWeapon(index);
        if (weapon != null)
        {
            rigAnimator.Play("equip_" + weapon.weaponName);
            rigAnimator.SetBool("isHolster", false);
            do
            {
                yield return new WaitForEndOfFrame();
            } while (rigAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f);
            isHolster = false;
        }
        
    }
    public void UpdateBulletNum(RaycastWeapon weapon)
    {
        BulletNumText.text = weapon.ammoCount.ToString();
    }
}

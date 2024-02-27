using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncWeapon : MonoBehaviour
{

    public Transform[] weaponSlots;
    RaycastWeapon[] equippedWeapons = new RaycastWeapon[2];
    int activeWeaponIndex;
    public Animator rigAnimator;
    public bool isHolster; // «∑Ò‘⁄±≥…œ
    public ActiveWeapon.WeaponSlot weaponSlot;
    // Start is called before the first frame update
    void Start()
    {
        NetManager.AddMsgListener("MsgSwitchWeapon", OnMsgSwitchWeapon);
    }

    private void OnMsgSwitchWeapon(MsgBase msgBase)
    {
        MsgSwitchWeapon msg = (MsgSwitchWeapon)msgBase;
        if(msg.PlayerId != GameMain.id)
        {
            HandleSwitchWeapon();
        }
    }
    private void HandleSwitchWeapon()
    {
        if (isHolster == false && equippedWeapons[activeWeaponIndex] != null)
        {
            if (activeWeaponIndex == 0)
            {
                SetActiveWeapon(ActiveWeapon.WeaponSlot.Secondary);
            }
            if (activeWeaponIndex == 1)
            {
                SetActiveWeapon(ActiveWeapon.WeaponSlot.Primary);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    RaycastWeapon GetWeapon(int index)
    {
        if (index < 0 || index >= equippedWeapons.Length)
        {
            return null;
        }
        return equippedWeapons[index];
    }
    void SetActiveWeapon(ActiveWeapon.WeaponSlot weaponSlot)
    {
        int holsterIndex = activeWeaponIndex;
        int activateIndex = (int)weaponSlot;
        if (holsterIndex == activateIndex)
        {
            holsterIndex = -1;
        }
        StartCoroutine(SwitchWeapon(holsterIndex, activateIndex));
    }
    IEnumerator SwitchWeapon(int holsterIndex, int activateIndex)
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
    public void Equip(RaycastWeapon newWeapon)
    {
        int weaponSlotIndex = (int)newWeapon.weaponSlot;
        var weapon = GetWeapon(weaponSlotIndex);
        if (weapon != null)
        {
            Destroy(weapon.gameObject);
        }
        weapon = newWeapon;
        weapon.transform.SetParent(weaponSlots[weaponSlotIndex], false);
        equippedWeapons[weaponSlotIndex] = weapon;
        SetActiveWeapon(newWeapon.weaponSlot);
    }
}

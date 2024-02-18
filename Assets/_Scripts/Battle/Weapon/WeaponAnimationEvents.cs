using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEvents:UnityEvent<string>
{

}
public class WeaponAnimationEvents : MonoBehaviour
{
    public AnimationEvents WeaponAnimationEvent = new AnimationEvents();
    public void OnAnimationEvent(string eventName)
    {
        WeaponAnimationEvent.Invoke(eventName);
    }


}

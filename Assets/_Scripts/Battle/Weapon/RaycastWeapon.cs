using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    public class Bullet
    {
        public float time;
        public Vector3 initialPos;
        public Vector3 initialVel;
        public TrailRenderer tracer;
    }
    public WeaponRecoil recoil;
    public ActiveWeapon.WeaponSlot weaponSlot;
    public float bulletSpeed = 1000f;
    public float bulletDrop = 1.0f;
    public float bulletMaxLifeTime = 3.0f;
    public bool isFiring = false;
    public ParticleSystem[] muzzleFlash;
    public ParticleSystem hitEffect;
    public TrailRenderer bulletTrailEffect;
    public string weaponName;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    public Transform bulletSpawnPos;
    public int fireRate = 20;
    private Vector3 hitPoint;
    private Vector3 raycastDestination = Vector3.zero;
    private Ray ray;
    private float accumulatedTime = 0f;
    List<Bullet> bullets = new List<Bullet>();
    private void Awake()
    {
        recoil = GetComponent<WeaponRecoil>();
    }
    private void Update()
    {
        RaycastCheck();
        
        //Firing();
    }
    public void StartFiring()
    {
        isFiring = true;
        accumulatedTime = 0f;
        recoil.Reset();
    }
    public void Firing()
    {
        if (isFiring == false) return;
        float fireInterval = 1.0f / fireRate;
        accumulatedTime += Time.deltaTime;
        while (accumulatedTime >= 0.0f)
        {
            FireBullet();
            accumulatedTime -= fireInterval;
        }
    }
    public void UpdateBullets(float deltaTime)
    {
        bullets.ForEach(bullet =>
        {
            Vector3 p0 = GetPosition(bullet);
            bullet.time += deltaTime;
            Vector3 p1 = GetPosition(bullet);
            RaycastSegment(p0, p1, bullet);
        });
        DestoryBullets();
    }

    private void DestoryBullets()
    {
        bullets.RemoveAll(bullet => bullet.time >= bulletMaxLifeTime);
    }

    private void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
    {
        float distance = (end-start).magnitude;
        ray.origin = start;
        ray.direction = end - start;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, distance, aimColliderLayerMask))
        {
            hitEffect.transform.position = raycastHit.point;
            hitEffect.transform.forward = raycastHit.normal;
            hitEffect.Emit(1);
            bullet.tracer.transform.position = raycastHit.point;
            bullet.time = bulletMaxLifeTime;
            //hitTransform.position = mouseWorldPosition;
        }
        else
        {
            bullet.tracer.transform.position = end;
        }
    }

    public void FireBullet()
    {
        //lastTime = Time.time;
        foreach (ParticleSystem particle in muzzleFlash)
        {
            particle.Emit(1);
        }
        Vector3 vel = (raycastDestination-bulletSpawnPos.position).normalized * bulletSpeed;
        var bullet = CreatBullet(bulletSpawnPos.position, vel);
        bullets.Add(bullet);
        recoil.GenerateRecoil();
    }
    public void StopFiring()
    {
        isFiring = false;
        accumulatedTime = 0f;
    }
    public void RaycastCheck()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            raycastDestination = raycastHit.point;
        }
    }
    private Vector3 GetPosition(Bullet bullet)
    {
        //p + v*t + 0.5*g*t*t;
        Vector3 gravity = Vector3.down * bulletDrop;
        return (bullet.initialPos)+(bullet.initialVel*bullet.time)+(0.5f * gravity * bullet.time*bullet.time);
    }
    private Bullet CreatBullet(Vector3 pos,Vector3 vel)
    {
        Bullet bullet = new Bullet();
        bullet.initialPos = pos;
        bullet.initialVel = vel;
        bullet.time = 0f;
        bullet.tracer = Instantiate(bulletTrailEffect, pos, Quaternion.identity);
        bullet.tracer.AddPosition(pos);
        return bullet;
    }
}


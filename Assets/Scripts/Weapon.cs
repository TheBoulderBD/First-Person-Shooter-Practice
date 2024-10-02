using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    private PlayerInput controls;
    private PlayerInput.OnFootActions onFoot;

    private Camera cam;
    private RaycastHit rayHit;

    [SerializeField] private float bulletRange;
    [SerializeField] private float fireRate, reloadTime;
    [SerializeField] private bool isAutomatic;
    [SerializeField] private int magazineSize;
    private int ammoLeft;

    private bool isShooting, readyToShoot, reloading;

    [SerializeField] private GameObject bulletHolePrefab;
    [SerializeField] private float bulletHoleLifeSpan;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] string EnemyTag;

    private void Awake()
    {
        ammoLeft = magazineSize;
        readyToShoot = true;

        controls = new PlayerInput();
        onFoot = controls.OnFoot;

        cam = Camera.main;

        onFoot.Shoot.started += ctx => StartShot();
        onFoot.Shoot.canceled += ctx => EndShot();

        onFoot.Reload.performed += ctx => Reload();

    }

    private void Update()
    {
        if (isShooting && readyToShoot && !reloading && ammoLeft > 0)
        {
            PerformShot();
        }
    }

    private void StartShot()
    {
        isShooting = true;
    }
     private void EndShot()
    {
        isShooting = false;
    }

    private void PerformShot()
    {
        readyToShoot = false;

        Vector3 direction = cam.transform.forward;

        if(Physics.Raycast(cam.transform.position, direction, out rayHit, bulletRange))
        {
            Debug.Log(rayHit.collider.gameObject.name);
            if(rayHit.collider.gameObject.CompareTag("Enemy"))
            {
                Destroy(rayHit.collider.gameObject);
            }
            else
            {
                GameObject bulletHole = Instantiate(bulletHolePrefab, rayHit.point + rayHit.normal * 0.001f, Quaternion.identity) as GameObject;
                bulletHole.transform.LookAt(rayHit.point + rayHit.normal);
                Destroy(bulletHole, bulletHoleLifeSpan);
            }
        }
        muzzleFlash.Play();

        ammoLeft--;

        if(ammoLeft >= 0)
        {
            Invoke("ResetShot", fireRate);

            if (!isAutomatic)
            {
                EndShot();
            }
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinish", reloadTime);
    }

    private void ReloadFinish()
    {
        ammoLeft = magazineSize;
        reloading = false;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}

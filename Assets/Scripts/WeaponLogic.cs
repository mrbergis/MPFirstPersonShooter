using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLogic : MonoBehaviour
{
    const int MAX_AMMO = 30;
    int _ammoCount = MAX_AMMO;
    
    const float SHOT_COOLDOWN = 0.15f;
    float _cooldown = 0.0f;
    
    Animator _animator;
    Camera _mainCamera;
    PlayerLogic _playerLogic;
    FirstPersonLogic _firstPersonLogic;
    
    bool _isReloading = false;
    
    private void Start()
    {
        _animator = GetComponentInParent<Animator>();
        _mainCamera = Camera.main;
        _playerLogic = GetComponentInParent<PlayerLogic>();
        _firstPersonLogic = GetComponentInParent<FirstPersonLogic>();
    }
    
    private void Update()
    {
        if (_cooldown > 0.0f)
        {
            _cooldown -= Time.deltaTime;
        }else
        {
            if(Input.GetButton("Fire1"))
            {
                if(_ammoCount > 0)
                {
                    Shoot();
                }
                else
                {
                    // Play empty clip sound
                }

                _cooldown = SHOT_COOLDOWN;
            }
        }
        
        if(Input.GetButtonDown("Fire2"))
        {
            Reload();
        }
    }

    private void Shoot()
    {
        --_ammoCount;
        
        if (_animator)
        {
            _animator.SetTrigger("Shoot");
        }
        
        Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);
        RaycastHit rayHit;

        if (Physics.Raycast(ray, out rayHit, 100.0f))
        {
            Debug.Log("Hit object: " + rayHit.collider.name);
            Debug.Log("Hit Pos: " + rayHit.point);
        }
        
        if(_firstPersonLogic)
        {
            _firstPersonLogic.AddRecoil();
        }
         
        if(_playerLogic)
        {
            _playerLogic.AddRecoil();
        }
    }
    
    private void Reload()
    {
        _isReloading = true;
        
        _ammoCount = MAX_AMMO;
    }
}

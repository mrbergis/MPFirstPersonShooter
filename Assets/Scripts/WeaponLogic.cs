using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponLogic : MonoBehaviour
{
    const int MAX_AMMO = 30;
    int _ammoCount = MAX_AMMO;
    
    const float SHOT_COOLDOWN = 0.15f;
    float _cooldown = 0.0f;
    
    PlayerLogic _playerLogic;
    FirstPersonLogic _firstPersonLogic;
    
    ParticleSystem _muzzleFlash;
    Light _muzzleFlashLight;
    const float MAX_LIGHT_TIME = 0.2f;
    float _lightTimer = 0.0f;
    
    [SerializeField]
    GameObject bulletImpactObj;

    [SerializeField] private TMP_Text ammoText;
    
    bool _isReloading = false;
    
    AudioSource _audioSource;

    [SerializeField]
    AudioClip shootSound;

    [SerializeField]
    AudioClip emptyClipSound;

    [SerializeField]
    AudioClip reloadingSound;
    
    Vector3 m_startPosition;
    const float TIME_SCALE = 2.0f;

    private Camera _playerCamera;
    
    private void Start()
    {
        Cursor.visible = false;
        
        _playerCamera = GetComponentInParent<Camera>();
        _playerLogic = GetComponentInParent<PlayerLogic>();
        _firstPersonLogic = GetComponentInParent<FirstPersonLogic>();
        _muzzleFlash = GetComponentInChildren<ParticleSystem>();
        _muzzleFlashLight = GetComponentInChildren<Light>();
        
        _audioSource = GetComponent<AudioSource>();
        
        m_startPosition = transform.localPosition;
        
        SetAmmoText();
    }
    
    private void SetAmmoText()
    {
        if(ammoText)
        {
            ammoText.text = "Ammo: " + _ammoCount;
        }
    }
    
    private void Update()
    {
        if (_playerLogic && !_playerLogic.IsLocalPlayer())
        {
            return;
        }
        
        transform.localPosition = m_startPosition + new Vector3(0.0f, Mathf.Sin(Time.time * TIME_SCALE) / 100.0f, 0.0f);
        
        if (_cooldown > 0.0f)
        {
            _cooldown -= Time.deltaTime;
        }else
        {
            if(Input.GetButton("Fire1") && !_isReloading)
            {
                if(_ammoCount > 0)
                {
                    Shoot();
                }
                else
                {
                    // Play empty clip sound
                    PlaySound(emptyClipSound);
                }

                _cooldown = SHOT_COOLDOWN;
            }
        }
        
        if(Input.GetButtonDown("Fire2"))
        {
            Reload();
        }
        
        if(_lightTimer > 0.0f)
        {
            _lightTimer -= Time.deltaTime;
        }else
        {
            _muzzleFlashLight.enabled = false;
        }
    }

    private void Shoot()
    {
        --_ammoCount;
        
        SetAmmoText();
        
        if(_playerLogic)
        {
            _playerLogic.PlayShootAnimation();
        }
        
        PlaySound(shootSound);
        
        Ray ray = new Ray(_playerCamera.transform.position, _playerCamera.transform.forward);
        RaycastHit rayHit;

        if (Physics.Raycast(ray, out rayHit, 100.0f))
        {
            Debug.Log("Hit object: " + rayHit.collider.name);
            Debug.Log("Hit Pos: " + rayHit.point);
            
            // Spawn Bullet Impact FX
            if(bulletImpactObj)
            {
                GameObject.Instantiate(bulletImpactObj, rayHit.point, 
                    Quaternion.FromToRotation(Vector3.up, rayHit.normal) 
                    * Quaternion.Euler(-90, 0, 0));
            }
        }
        
        if(_firstPersonLogic)
        {
            _firstPersonLogic.AddRecoil();
        }
         
        if(_playerLogic)
        {
            _playerLogic.AddRecoil();
        }
        if(_muzzleFlash)
        {
            _muzzleFlash.Play(true);
        }

        if(_muzzleFlashLight)
        { 
            _muzzleFlashLight.enabled = true;
            _lightTimer = MAX_LIGHT_TIME;
        }
    }
    
    private void Reload()
    {
        _isReloading = true;

        if (_playerLogic)
        {
            _playerLogic.PlayReloadAnimation();
        }
        
        PlaySound(reloadingSound, 0.5f);
    }
    
    void PlaySound(AudioClip sound, float volume = 1.0f)
    {
        if(_audioSource && sound)
        {
            _audioSource.volume = volume;
            _audioSource.PlayOneShot(sound);
        }
    }
    
    public bool IsReloading()
    {
        return _isReloading;
    }
    public void SetReloadingState(bool isReloading)
    {
        _isReloading = isReloading;

        if(!_isReloading)
        {
            _ammoCount = MAX_AMMO;
            SetAmmoText();
        }
    }
}

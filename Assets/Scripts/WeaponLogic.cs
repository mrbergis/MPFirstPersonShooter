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
    
    Vector3 _startPosition;
    const float TIME_SCALE = 2.0f;
    
    WeaponLogicMP _weaponLogicMP;
    
    Rigidbody _rigidBody;
    
    private void Start()
    {
        Cursor.visible = false;
        
        _playerLogic = GetComponentInParent<PlayerLogic>();
        _firstPersonLogic = GetComponentInParent<FirstPersonLogic>();
        _muzzleFlash = GetComponentInChildren<ParticleSystem>();
        _muzzleFlashLight = GetComponentInChildren<Light>();
        
        _audioSource = GetComponent<AudioSource>();
        
        _weaponLogicMP = GetComponentInParent<WeaponLogicMP>();
        
        _startPosition = transform.localPosition;
        
        _rigidBody = GetComponent<Rigidbody>();
        
        SetAmmoText();
    }
    
    public void DropWeapon()
    {
        transform.parent = null;

        if(_rigidBody)
        {
            _rigidBody.isKinematic = false;
            _rigidBody.useGravity = true;
        }
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
        if(_lightTimer > 0.0f)
        {
            _lightTimer -= Time.deltaTime;
        }else
        {
            _muzzleFlashLight.enabled = false;
        }
        
        if (_playerLogic && (!_playerLogic.IsLocalPlayer() || _playerLogic.IsDead()))
        {
            return;
        }
        
        transform.localPosition = _startPosition + new Vector3(0.0f, Mathf.Sin(Time.time * TIME_SCALE) / 100.0f, 0.0f);
        
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
        
        if (_weaponLogicMP)
        {
            _weaponLogicMP.CmdShoot();
        }
    }
    
    public void ShootEffect(Vector3 impactPosition, Quaternion impactRotation, bool spawnBulletImpact)
    {
        // Spawn Bullet Impact FX
        if (bulletImpactObj && spawnBulletImpact)
        {
            GameObject.Instantiate(bulletImpactObj, impactPosition, impactRotation);
        }

        if (_firstPersonLogic)
        {
            _firstPersonLogic.AddRecoil();
        }

        if (_playerLogic)
        {
            _playerLogic.AddRecoil();
        }

        if (_muzzleFlash)
        {
            _muzzleFlash.Play(true);
        }

        if (_muzzleFlashLight)
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

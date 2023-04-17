using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public enum Team
{
    Blue,
    Red
}
public class PlayerLogic : NetworkBehaviour
{
    private float _rotationY;
    private const float ROTATION_SPEED = 2.0f;
    
    private float _horizontalInput;
    private float _verticalInput;
    
    private const float MOVEMENT_SPEED = 5.0f;
    
    private Vector3 _horizontalMovement;
    private Vector3 _verticalMovement;
    private Vector3 _heightMovement;
    
    float _jumpHeight = 0.25f;
    float _gravity = 0.981f;
    bool _jump = false;
    
    private CharacterController _characterController;
    private Animator _animator;
    
    [SerializeField]
    Transform leftHandTarget;

    [SerializeField]
    Transform rightHandTarget;
    
    bool _isCrouching = false;
    
    WeaponLogic _weaponLogic;
    
    AudioSource _audioSource;
    
    [SerializeField]
    List<AudioClip> footstepSounds;
    
    [SerializeField]
    GameObject camera;
    
    [SerializeField]
    SkinnedMeshRenderer headRenderer;

    [SerializeField]
    SkinnedMeshRenderer bodyRenderer;
    
    NetworkAnimator _networkAnimator;
    
    const int MAX_HEALTH = 100;
    int _health = MAX_HEALTH;

    bool _isDead = false;
    
    [SyncVar]
    Team _team = Team.Blue;

    [SerializeField]
    Material blueMaterial;

    [SerializeField]
    Material redMaterial;
    
    private void Start()
    {
        SetupCamera();
        
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _weaponLogic = GetComponentInChildren<WeaponLogic>();
        _audioSource = GetComponent<AudioSource>();
        _networkAnimator = GetComponent<NetworkAnimator>();
        
        SetupHeadRendering();
        
        SetHealthText();
    }
    public override void OnStartClient()
    {
        SetColor(_team);
    }
    private void Update()
    {
        if (!isLocalPlayer || _isDead)
        {
            return;
        }
        
        _rotationY += Input.GetAxis("Mouse X") * ROTATION_SPEED;
        transform.rotation = Quaternion.Euler(0, _rotationY, 0);
        
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
        
        if(Input.GetKeyDown(KeyCode.C))
        {
            _isCrouching = !_isCrouching;
            if(_animator)
            {
                _animator.SetBool("IsCrouching", _isCrouching);
            }
        }
        if(Input.GetButtonDown("Jump") && _characterController.isGrounded)
        {
            _jump = true;
        }
    }
    private void FixedUpdate()
    {
        if (!isLocalPlayer || _isDead)
        {
            return;
        }
        
        if(_jump)
        {
            _heightMovement.y = _jumpHeight;
            _jump = false;
        }

        _heightMovement.y -= _gravity * Time.deltaTime;

        _horizontalMovement = transform.right * _horizontalInput * MOVEMENT_SPEED * Time.deltaTime;
        _verticalMovement = transform.forward * _verticalInput * MOVEMENT_SPEED * Time.deltaTime;

        if (_characterController && !_isCrouching)
        {
            _characterController.Move(_horizontalMovement + _verticalMovement + _heightMovement);
        }
        
        if(_animator)
        {
            _animator.SetFloat("HorizontalInput", _horizontalInput);
            _animator.SetFloat("VerticalInput", _verticalInput);
        }
       
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if(_animator && !_isDead)
        {
            if(_weaponLogic && !_weaponLogic.IsReloading())
            {
                SetHandIK(AvatarIKGoal.LeftHand, leftHandTarget);
            }
            
            SetHandIK(AvatarIKGoal.RightHand, rightHandTarget);
        }
    }
    private void SetColor(Team team)
    {
        if(_team == Team.Blue)
        {
            SetMaterial(blueMaterial);
        }
        else if(_team == Team.Red)
        {
            SetMaterial(redMaterial);
        }
    }
    private void SetMaterial(Material material)
    {
        // Body
        Material[] mats = bodyRenderer.materials;
        mats[0] = material;
        bodyRenderer.materials = mats;

        // Head
        mats = headRenderer.materials;
        mats[0] = material;
        headRenderer.materials = mats;
    }
    private void SetHealthText()
    {
        if (UIManager.Instance && isLocalPlayer)
        {
            UIManager.Instance.SetHealthText(_health);
        }
    }
    private void SetupCamera()
    {
        if (Camera.main)
        {
            Camera.main.enabled = false;
        }

        if (camera && isLocalPlayer)
        {
            camera.SetActive(true);
        }
    }
    private void SetupHeadRendering()
    {
        if (headRenderer)
        {
            if (isLocalPlayer)
            {
                headRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
            else
            {
                headRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
        }
    }
    private void SetHandIK(AvatarIKGoal avatarIKGoal, Transform target)
    {
        if(target)
        {
            _animator.SetIKPosition(avatarIKGoal, target.position);
            _animator.SetIKRotation(avatarIKGoal, target.rotation);
            _animator.SetIKPositionWeight(avatarIKGoal, 1.0f);
            _animator.SetIKRotationWeight(avatarIKGoal, 1.0f);
        }
    }
    
    private void PlaySound(AudioClip sound, float volume = 1.0f)
    {
        if (_audioSource && sound)
        {
            _audioSource.volume = volume;
            _audioSource.PlayOneShot(sound);
        }
    }
    [ClientRpc]
    private void RpcDie()
    {
        if(_animator)
        {
            _animator.SetTrigger("Die");
        }

        if (_networkAnimator)
        {
            _networkAnimator.SetTrigger("Die");
        }
        
        if(_weaponLogic)
        {
            _weaponLogic.DropWeapon();
        }

        _isDead = true;
    }
    [ClientRpc]
    private void RpcSetHealth(int health)
    {
        _health = health;
        
        SetHealthText();
    }
    [ClientRpc]
    void RpcSetTeam(Team team)
    {
        _team = team;
        SetColor(_team);
    }
    
    public void SetTeam(Team team)
    {
        RpcSetTeam(team);
    }
    public float GetRotationY()
    {
        return _rotationY;
    }
    
    public void AddRecoil()
    {
        _rotationY += Random.Range(-1.0f, 1.0f);
    }
    
    public bool IsCrouching()
    {
        return _isCrouching;
    }
    
    public void PlayFootstepSound()
    {
        int soundIndex = Random.Range(0, footstepSounds.Count);
        if (soundIndex > 0)
        {
            PlaySound(footstepSounds[soundIndex]);
        }
    }
     
    public bool IsLocalPlayer()
    {
        return isLocalPlayer;
    }
    
    public void PlayShootAnimation()
    {
        if(_animator)
        {
            _animator.SetTrigger("Shoot");
        }

        if(_networkAnimator)
        {
            _networkAnimator.SetTrigger("Shoot");
        }
    }

    public void PlayReloadAnimation()
    {
        if (_animator)
        {
            _animator.SetTrigger("Reload");
        }

        if (_networkAnimator)
        {
            _networkAnimator.SetTrigger("Reload");
        }
    }
    
    public void TakeDamage(int damage)
    {
        if(!isServer)
        {
            return;
        }

        _health -= damage;
        _health = Mathf.Clamp(_health, 0, MAX_HEALTH);
        RpcSetHealth(_health);

        if (_health == 0)
        {
            RpcDie();
        }
    }
    
    public bool IsDead()
    {
        return _isDead;
    }
}


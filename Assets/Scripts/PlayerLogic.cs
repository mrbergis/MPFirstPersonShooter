using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    private float _rotationY;
    private const float ROTATION_SPEED = 2.0f;
    
    private float _horizontalInput;
    private float _verticalInput;
    
    private const float MOVEMENT_SPEED = 5.0f;
    
    private Vector3 _horizontalMovement;
    private Vector3 _verticalMovement;
    
    private CharacterController _characterController;
    private Animator _animator;
    
    [SerializeField]
    Transform leftHandTarget;

    [SerializeField]
    Transform rightHandTarget;
    
    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        _rotationY += Input.GetAxis("Mouse X") * ROTATION_SPEED;
        transform.rotation = Quaternion.Euler(0, _rotationY, 0);
        
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        _horizontalMovement = transform.right * _horizontalInput * MOVEMENT_SPEED * Time.deltaTime;
        _verticalMovement = transform.forward * _verticalInput * MOVEMENT_SPEED * Time.deltaTime;

        if (_characterController)
        {
            _characterController.Move(_horizontalMovement + _verticalMovement);
        }
        
        if(_animator)
        {
            _animator.SetFloat("HorizontalInput", _horizontalInput);
            _animator.SetFloat("VerticalInput", _verticalInput);
        }
       
    }
    
    private void OnAnimatorIK(int layerIndex)
    {
        if(_animator)
        {
            SetHandIK(AvatarIKGoal.LeftHand, leftHandTarget);
            SetHandIK(AvatarIKGoal.RightHand, rightHandTarget);
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
    
    public float GetRotationY()
    {
        return _rotationY;
    }
    
    public void AddRecoil()
    {
        _rotationY += Random.Range(-1.0f, 1.0f);
    }
}


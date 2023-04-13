using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    private float _rotationY;
    private const float ROTATION_SPEED = 2.0f;
    
    float _horizontalInput;
    float _verticalInput;
    
    const float MOVEMENT_SPEED = 5.0f;
    
    Vector3 _horizontalMovement;
    Vector3 _verticalMovement;
    
    CharacterController _characterController;
    private Animator _animator;
    
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        _rotationY += Input.GetAxis("Mouse X") * ROTATION_SPEED;
        transform.rotation = Quaternion.Euler(0, _rotationY, 0);
        
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
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
    

    public float GetRotationY()
    {
        return _rotationY;
    }
}

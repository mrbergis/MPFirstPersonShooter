using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonLogic : MonoBehaviour
{
    float _rotationX = 0.0f;
    const float MIN_X = -50.0f;
    const float MAX_X = 50.0f;
    const float ROTATION_SPEED = 2.0f;
    
    PlayerLogic _playerLogic;
    
    void Start()
    {
        _playerLogic = GetComponentInParent<PlayerLogic>();
    }
    
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        
        _rotationX -= mouseY;
        _rotationX = Mathf.Clamp(_rotationX, MIN_X, MAX_X);
    }
    
    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(_rotationX, _playerLogic.GetRotationY(), 0);
    }
}

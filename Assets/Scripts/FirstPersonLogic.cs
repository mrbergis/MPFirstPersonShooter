using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonLogic : MonoBehaviour
{
    float _rotationX = 0.0f;
    float _startRotationX = 0.0f;
    float _targetRotationX = 0.0f;
    const float MIN_X = -50.0f;
    const float MAX_X = 50.0f;
    const float ROTATION_SPEED = 2.0f;
    
    PlayerLogic _playerLogic;
    
    bool _recoilAnim = false;
    float _recoilAnimProgress;
    
    void Start()
    {
        _playerLogic = GetComponentInParent<PlayerLogic>();
    }
    
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        
        // If player moves mouse, stop recoil lerp anim
        if(mouseX != 0.0f || mouseY != 0.0f)
        {
            _recoilAnim = false;
        }
        
        _rotationX -= mouseY;
        _rotationX = Mathf.Clamp(_rotationX, MIN_X, MAX_X);

        if (_recoilAnim)
        {
            _recoilAnimProgress += Time.deltaTime;
            _rotationX = Mathf.Lerp(_startRotationX, _targetRotationX, _recoilAnimProgress);

            if (Mathf.Abs(_rotationX - _targetRotationX) < 0.1f)
            {
                _rotationX = _targetRotationX;
                _recoilAnim = false;
                _recoilAnimProgress = 0.0f;
            }
        }
    }
    
    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(_rotationX, _playerLogic.GetRotationY(), 0);
    }
    
    public void AddRecoil()
    {
        if(!_recoilAnim)
        {
            _targetRotationX = _rotationX;
        }

        _recoilAnimProgress = 0.0f;
        _recoilAnim = true;

        _rotationX -= ROTATION_SPEED;
        _startRotationX = _rotationX;
    }
}

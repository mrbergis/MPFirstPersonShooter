using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    private float _rotationY;
    private const float ROTATION_SPEED = 2.0f;
    
    void Start()
    {
        
    }
    
    void Update()
    {
        _rotationY += Input.GetAxis("Mouse X") * ROTATION_SPEED;
        transform.rotation = Quaternion.Euler(0, _rotationY, 0);
    }
    
    public float GetRotationY()
    {
        return _rotationY;
    }
}

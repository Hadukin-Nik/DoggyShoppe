using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _rotateOn;
    [SerializeField] private float _reloadTime;

    private float _rotated = 0;
    private float _standingTime = -1;
    private bool _IsMovingFwd = true;
    

    void Update()
    {
        if(_standingTime >= 0 && _standingTime < _reloadTime)
        {
            _standingTime += Time.deltaTime;
            return;
        } else if (_standingTime > 0)
        {
            _standingTime = -1;
        }

        _rotated += Time.deltaTime * _speed;

        if(_IsMovingFwd)
        {
            transform.Rotate(Vector3.up, _speed * Time.deltaTime);
        } else
        {
            transform.Rotate(Vector3.up, -_speed * Time.deltaTime);
        }

        if(_rotated >= _rotateOn)
        {
            _rotated = 0;
            _IsMovingFwd = !_IsMovingFwd;

            if(_IsMovingFwd )
            {
                _standingTime = 0.01f;
            }
        } 

        
    }
}

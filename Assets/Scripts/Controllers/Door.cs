using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _rotateOn;

    private float _rotated = 0;
    private bool _onAnimation = false;
    private bool _IsMovingFwd = true;
    

    private void Update()
    {
        if (!_onAnimation) return;
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
                _onAnimation = false;
            }
        } 
    }

    public void Open()
    {
        _onAnimation = true;
    }
}

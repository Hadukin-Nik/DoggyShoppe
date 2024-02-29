using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed;

    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody>();   
    }


    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal * _speed * Time.deltaTime, 0, vertical * _speed * Time.deltaTime);

        if(Input.GetKey(KeyCode.Space) && transform.position.y <= 0.501) 
        {
            _rigidbody.AddForce(Vector3.up);
        }

        transform.Translate(direction);
    
    }
}

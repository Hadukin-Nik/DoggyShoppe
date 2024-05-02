using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestsFactory : MonoBehaviour
{
    [SerializeField] private GameObject _guest;
    [SerializeField] private Door _door;
    [SerializeField] private Transform _spawnPlace;

    [SerializeField] private float _reloadTime;

    private float _standingTime = 0.01f;
    
    private void Update()
    {
        
        if(_standingTime < 0)
        {
            _standingTime = _reloadTime;

            GameObject spawnedGuest = Instantiate(_guest);
            spawnedGuest.transform.SetPositionAndRotation(_spawnPlace.position, _spawnPlace.rotation);

            _door.Open();
        } else
        {
            _standingTime -= Time.deltaTime;
        }
       
    }
}

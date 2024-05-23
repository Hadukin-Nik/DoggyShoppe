using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            Guest guest = spawnedGuest.GetComponent<Guest>();
            guest.SetOnCreatedAction(OpenDoor);
        } else
        {
            _standingTime -= Time.deltaTime;
        }
       
    }

    public void OpenDoor()
    {
        if(!_door.IsOpenning())
        {
            _door.Open();
        }
    }
}

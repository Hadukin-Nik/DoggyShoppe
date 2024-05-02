using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guest : MonoBehaviour
{
    [SerializeField] private float _height;
    [SerializeField] private float _timeWait;
    private FloorController _floorController;
    private MoveGuest _mover;
    private float _time;
    private bool _waiting;
    void Start()
    {
        _floorController = FindAnyObjectByType<FloorController>();
       
        _mover = transform.GetComponent<MoveGuest>();
        if (_floorController == null)
        {
            Debug.Log("Cannot find floor controller on scene");
        }
        _waiting = true;
        _mover.SetActionOnEnd(setEnableToMove);
        _mover.SetHeight(_height);
        _time = _timeWait;
    }

    // Update is called once per frame
    void Update()
    {
        if(_time <= 0f && _waiting)
        {
            _waiting = false;
            _mover.Move(_floorController.GetWayToRandom(transform.position));
        }

        _time -= Time.deltaTime;
    }

    private void setEnableToMove()
    {
        if(_timeWait == -1f) { return; }
        _waiting = true;
        _time = _timeWait;
    }
}

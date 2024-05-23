using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveGuest : MonoBehaviour
{
    [SerializeField] private float _timeMove;

    private Vector3 _position;

    private float _waitingMove = 0f;
    private float _height = 0f;

    private List<Vector3> _moving;
    private Queue<int> _indexes;
    private int _point = 0;

    private Action _endOfMove;
    void Start()
    {
        _point = -1;
        _position = transform.position;
        _moving = new List<Vector3>();
        _indexes = new Queue<int>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_indexes.Count > 0 && _indexes.Peek() <= _point)
        {
            _indexes.Dequeue();
            _endOfMove();
        }
        if(_point < 0 || _moving == null)
        {
            return;
        }
        if(_point >= _moving.Count - 1)
        {

            _point = -1;
            transform.position = _position;
        }

        if(_point < _moving.Count - 1 && _waitingMove <= 0f)
        {

            _point++;
            _waitingMove = _timeMove;

            transform.position = _moving[_point] + Vector3.up * _height / 2;
           
        }
        _waitingMove -= Time.deltaTime;
    }

    public void Move(List<Vector3> points, Queue<int> useIndex) {
        _indexes = useIndex;
        transform.position = _position;
        _moving = points;
        _point = 0;
    }

    public Vector3 GetVector3()
    {
        return _position;
    }

    public void SetActionOnEnd(Action action)
    {
        _endOfMove += action;
    }

    public void SetHeight(float height)
    {
        _height = height;
    }
}
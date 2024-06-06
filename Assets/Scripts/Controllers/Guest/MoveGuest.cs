using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveGuest : MonoBehaviour
{
    [SerializeField] private float _speed = 0.1f;
    private Vector3 _position;

    private float _height;
    private float _waiting;

    private List<Vector3> _moving;
    private Queue<int> _indexes;
    private int _point = 0;
    private int _cashIndex;
    
    private Animator _animator;

    private Action _endOfMove;
    private Action _destroy;
    private Action _eachMove;
    void Start()
    {
        _waiting = -1;
        _point = -1;
        
        _moving = new List<Vector3>();
        _indexes = new Queue<int>();

        _animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if(_waiting > 0.01)
        {
            _waiting -= Time.deltaTime;
        } else
        {
            _waiting = -1;
        }
        

        if( _waiting > 0)
        {
            return;
        }

        if(_indexes.Count > 0 && _indexes.Peek() <= _point)
        {
            _animator.SetFloat("Speed", 0f);

            _animator.SetTrigger("Picking");
            _waiting = 3.4f;
            _indexes.Dequeue();
            _eachMove();

            return;
        }
        if(_point < 0 || _moving == null)
        {
            _destroy();
            return;
        }

        if (_point >= _moving.Count - 1)
        {
            _point = -1;
        }

        if(_point < _moving.Count - 1 && (_moving[_point] - transform.position).sqrMagnitude < 0.001f)
        {
            if (_cashIndex == _point)
            {
                _endOfMove();
            }

            _point++;
            
        }

        if(_point >= 0)
        {
            Vector3 targetDirection = _moving[_point] - transform.position;
            
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, _speed, 0.0f);

            transform.rotation = Quaternion.LookRotation(newDirection);
            _animator.SetFloat("Speed", 1f);
            Vector3 direction = targetDirection.normalized * _speed * Time.deltaTime;

            transform.position = transform.position + direction;
        }
    }

    public void Move(List<Vector3> points, Queue<int> useIndex, int cashIndex) {
        for (int i = 0; i < points.Count; i++)
        {
            float newY = points[i].y + _height / 2;
            points[i] = new Vector3(points[i].x, newY, points[i].z);
        }

        transform.position = new Vector3(points[0].x, points[0].y, points[0].z);

        _indexes = useIndex;
        _cashIndex = cashIndex;
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

    public void SetActionOnEach(Action action)
    {
        _eachMove += action;
    }

    public void DestroyAction(Action action)
    {
        _destroy += action;
    }

    public void SetHeight(float height)
    {
        _height = height;
    }

    
}

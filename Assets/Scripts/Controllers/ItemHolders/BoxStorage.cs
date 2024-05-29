using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxStorage : MonoBehaviour
{
    [SerializeField] private int capacity;
    [SerializeField] private Vector3 _size;

    [SerializeField] private Transform _pivot;

    bool[,] _spaces;

    private int _used;

    public void Init()
    {
        _spaces = new bool[(int)_size.x, (int)_size.z];
        _used = 0;
    }

    public bool IsPlaceable()
    {
        return capacity - _used > 0;
    }
    public void Displace((int, int) place)
    {
        _used--;

        _spaces[place.Item1, place.Item2] = false;
    }

    public (Vector3, Quaternion, (int, int)) Place()
    {
        if(_spaces == null)
        {
            _spaces = new bool[(int)_size.x, (int)_size.z];
        }

        int deltaZ = -1;
        int deltaX = -1;

        for(int i = 0; i < (int)_size.x && deltaZ < 0; i++)
        {
            for(int j = 0; j < (int)_size.z && deltaZ < 0; j++)
            {
                if (!_spaces[i, j])
                {
                    _spaces[i, j] = true;
                    deltaX = i;
                    deltaZ = j;
                }
            }
        }

        Vector3 nv = (1f / 2 + deltaX) * _pivot.right + (1f / 2 + deltaZ) * _pivot.forward;

        float x = nv.x + _pivot.position.x;
        float z = nv.z + _pivot.position.z;

        _used++;

        return (new Vector3(x, transform.position.y + 1f / 2, z), transform.rotation, (deltaX, deltaZ));
    }
}

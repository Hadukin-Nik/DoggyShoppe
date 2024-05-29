using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;

public class DebugBuildingLayout : MonoBehaviour
{
    private GameObject _greenDebugPlane;
    private GameObject _redDebugPlane;

    private Transform _pointStart;
    private (int, int) _size;

    private Stack<GameObject> _stack;
    //OZ axis (local)
    private Vector3 _correctForwardVector;
    //OX axis (local)
    private Vector3 _correctRightVector;
    
    public DebugBuildingLayout(GameObject greenDebugPlane, GameObject redDebugPlane, Transform pointStart, (int, int) size, Vector3 correctForwardVector, Vector3 correctRightVector)
    {
        _greenDebugPlane = greenDebugPlane;
        _redDebugPlane = redDebugPlane;
        _pointStart = pointStart;
        _size = size;
            
        _correctForwardVector = correctForwardVector;
        _correctRightVector = correctRightVector;
        _stack = new Stack<GameObject>();
    }

    //Debug priority purpose
    public void CreateBuildingMap(bool[,] buildingMatrix)
    {
        while (_stack.Count > 0)
        {
            GameObject gb = _stack.Pop();
            Destroy(gb);
        }
        for (int i = 0; i < _size.Item1; i++)
        {
            for (int j = 0; j < _size.Item2; j++)
            {
                if (!buildingMatrix[i, j])
                {
                    buildObstacle(_greenDebugPlane, i, j, 1);
                }
                else
                {
                    buildObstacle(_redDebugPlane, i, j, 1.1f);
                }
            }
        }

    }

    public void DestroyBuildingMap()
    {
        while (_stack.Count > 0)
        {
            GameObject gb = _stack.Pop();
            Destroy(gb);
        }
    }
    private void buildObstacle(GameObject gameObject, int i, int j, float yd)
    {
        Vector3 x = (j + 0.5f) * _correctForwardVector;
        Vector3 z = (i + 0.5f) * _correctRightVector;
        Vector3 y = _pointStart.up * yd * 0.01f;
        _stack.Push(Instantiate(gameObject, x + z + y + _pointStart.position, _pointStart.rotation));
    }
}

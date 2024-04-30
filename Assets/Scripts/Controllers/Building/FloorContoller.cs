using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    [SerializeField] private Transform _pointStart;

    [SerializeField] private Vector3 _size;

    [SerializeField] private float _delta = 0.5f;

    private float[,] _transformationMatrix;
    private float[,] _transformedGlobalPivot;

    private bool[,] _buildingMatrix;
 

    private Stack<GameObject> _stack;

    [SerializeField] private bool _onCreate;
    [SerializeField] private GameObject _green;
    [SerializeField] private GameObject _red;
    private void Start()
    {
        _stack = new Stack<GameObject>();

        //False means - no building on that point
        Vector3 pointStart = _pointStart.position;
        _buildingMatrix = new bool[Mathf.Abs((int)(_size.x / _delta)),Mathf.Abs((int)(_size.z / _delta))];

        _transformationMatrix = new float[2, 2];
        _transformedGlobalPivot = new float[2, 1];

        Vector3 right = _pointStart.right;
        Vector3 forward = _pointStart.forward;

        float detA = (right.x * forward.z - right.z * forward.x);

        _transformationMatrix[0, 0] = forward.z / detA;
        _transformationMatrix[0, 1] = (-forward.x) / detA;
        _transformationMatrix[1, 0] = (-right.z) / detA;
        _transformationMatrix[1, 1] = (right.x) / detA;

        _transformedGlobalPivot[0, 0] = -(_transformationMatrix[0, 0] * pointStart.x + _transformationMatrix[0, 1] * pointStart.z);
        _transformedGlobalPivot[1, 0] = -(_transformationMatrix[1, 0] * pointStart.x + _transformationMatrix[1, 1] * pointStart.z);
    }

    public void CreateBuildingMap()
    {
        while (_stack.Count > 0)
        {
            GameObject gb = _stack.Pop();
            Destroy(gb);
        }
        for (int i = 0; i < Mathf.Abs((int)(_size.x / _delta)); i++)
        {
            for (int j = 0; j < Mathf.Abs((int)(_size.z / _delta)); j++)
            {
                if (!_buildingMatrix[i, j])
                {
                    buildObstacle(_green, i, j, 1);
                }
                else
                {
                    buildObstacle(_red, i, j, 1.1f);
                }
            }
        }

        _onCreate = false;
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
        Vector3 x = (j + 1) *_delta * _pointStart.forward;
        Vector3 z = (i + 1) *_delta * _pointStart.right;
        Vector3 y = _delta * 2 * _pointStart.up*yd;
        _stack.Push(Instantiate(gameObject, x + z + y + _pointStart.position, _pointStart.rotation));
    }


    private bool isItPossibleToBuild(Transform buildingTransform, Vector3 buildingSize, bool buildImmediately)
    {
        float exp = 0.1f;
        List<Vector3> pointsOnMatrix = new List<Vector3>();

        for(int i = 0; i < buildingSize.x / _delta / exp; i++)
        {
            for(int j = 0; j < buildingSize.z / _delta / exp; j++)
            {
                float deltaX = i * _delta * exp;
                float deltaZ = j * _delta * exp;

                Vector3 nv = (deltaX) * buildingTransform.right + (deltaZ) * buildingTransform.forward - buildingTransform.right * buildingSize.x / 2 - buildingTransform.forward * buildingSize.z/2;

                float xG = (nv.x + buildingTransform.position.x);
                float zG = (nv.z + buildingTransform.position.z);

                float xT = (_transformationMatrix[0, 0] * xG + _transformationMatrix[0, 1] * zG) + _transformedGlobalPivot[0, 0];
                float zT = (_transformationMatrix[1, 0] * xG + _transformationMatrix[1, 1] * zG) + _transformedGlobalPivot[1, 0];

                Vector3 point = new Vector3(xT / _delta, 0, zT / _delta);

                if(xT <= 0 || zT <= 0 || xT >= _size.x || zT >= _size.z || _buildingMatrix[(int)(point.x), (int)(point.z)])
                {
                    return false;
                } else
                {
                    pointsOnMatrix.Add(point);
                }
            }
        }

        if(!buildImmediately)
        {
            return true;
        }

        

        foreach(Vector3 point in pointsOnMatrix)
        {
            _buildingMatrix[(int)(point.x), (int)(point.z)] = true;
        }

        return true;
    }
    

    public bool IsItPossibleToBuild(BuildingContoller buildingContoller)
    {
        return isItPossibleToBuild(buildingContoller.GetTransform(), buildingContoller.GetSize(), false);
    }

    public bool TryToBuild(BuildingContoller buildingContoller)
    {
        return isItPossibleToBuild(buildingContoller.GetTransform(), buildingContoller.GetSize(), true);
    }
}

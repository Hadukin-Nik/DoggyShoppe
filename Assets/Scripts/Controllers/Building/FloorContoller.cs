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

    private void Start()
    {
        //False means - no building on that point
        Vector3 pointStart = _pointStart.position;
        _buildingMatrix = new bool[Mathf.Abs((int)(_size.x / _delta)),Mathf.Abs((int)(_size.z / _delta))];

        _transformationMatrix = new float[2, 2];
        _transformedGlobalPivot = new float[2, 1];

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        float detA = (right.x * forward.z - right.z * forward.z);

        _transformationMatrix[0, 0] = forward.z / detA;
        _transformationMatrix[0, 1] = (-forward.x) / detA;
        _transformationMatrix[1, 0] = (-right.z) / detA;
        _transformationMatrix[1, 1] = (right.x) / detA;

        _transformedGlobalPivot[0, 0] = -(_transformationMatrix[0, 0] * pointStart.x + _transformationMatrix[0, 1] * pointStart.z);
        _transformedGlobalPivot[1, 0] = -(_transformationMatrix[1, 0] * pointStart.x + _transformationMatrix[1, 1] * pointStart.z);
    }

    private bool isItPossibleToBuild(Transform buildingTransform, Vector3 buildingSize, bool buildImmediately)
    {
        List<Vector3> pointsOnMatrix = new List<Vector3>();

        for(int i = 0; i < buildingSize.x / _delta; i++)
        {
            for(int j = 0; j < buildingSize.z / _delta; j++)
            {
                float deltaX = i * _delta;
                float deltaZ = j * _delta;

                Vector3 nv = (deltaX) * buildingTransform.right + (deltaZ) * buildingTransform.forward;

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

using System.Collections.Generic;
using UnityEngine;

public class FloorContoller : MonoBehaviour
{
    [SerializeField] private Vector3 _pointStart;

    [SerializeField] private Vector3 _size;

    [SerializeField] private float _delta = 0.5f;

    private float[,] _transformationMatrix;
    private float[,] _transformedGlobalPivot;

    private bool[,] _buildingMatrix;

    private void Start()
    {
        //False means - no building on that point
        _buildingMatrix = new bool[(int)(pointStart.x / delta),(int)(pointStart.z / delta)];

        _transformationMatrix = new float[4, 4];
        _transformedGlobalPivot = new float[2, 1];

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        float detA = (right.x * forward.z - right.z * forward.y);

        _transformationMatrix[0, 0] = forward.z / detA;
        _transformationMatrix[0, 1] = (-forward.x) / detA;
        _transformationMatrix[1, 0] = (-right.z) / detA;
        _transformationMatrix[1, 1] = (right.x) / detA;

        _transformedGlobalPivot[0, 0] = -(_transformationMatrix[0, 0] * pointStart.x + _transformationMatrix[0, 1] * pointStart.y);
        _transformedGlobalPivot[1, 0] = -(_transformationMatrix[1, 0] * pointStart.x + _transformationMatrix[1, 1] * pointStart.y);
    }

    private bool isItPossibleToBuild(BuildingContoller buildingContoller, bool buildImmediately)
    {
        Transform buildingTransform = buildingContoller.GetTransform();

        Vector3 buildingPoint = buildingContoller.GetPoint();

        List<Vector3> pointsOnMatrix = new List<Vector3>();

        for(int i = 0; i < size.x / delta; i++)
        {
            for(int j = 0; j < size.y / delta; j++)
            {
                float deltaX = i * delta;
                float deltaZ = j * delta;

                Vector3 nv = (deltaX) * buildingTransform.right + (deltaZ) * buildingTransform.forward;

                float xG = (nv.x + buildingPoint.x);
                float zG = (nv.z + buildingPoint.z);

                float xT = (_transformationMatrix[0, 0] * xG + _transformationMatrix[0, 1] * zG) + _transformedGlobalPivot[0, 0];
                float zT = (_transformationMatrix[1, 0] * xG + _transformationMatrix[1, 1] * zG) + _transformedGlobalPivot[1, 0];

                Vector3 point = new Vector3(xT / delta, 0, zT / delta);

                if(xT < 0 || zT < 0 || xT > size.x || zT > size.z || _buildingMatrix[(int)(point.x), (int)(point.z)])
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
        return isItPossibleToBuild(buildingContoller, false);
    }

    public bool TryToBuild(BuildingContoller buildingContoller)
    {
        return isItPossibleToBuild(buildingContoller, true);
    }
}

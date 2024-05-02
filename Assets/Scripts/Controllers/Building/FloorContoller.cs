using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class FloorController : MonoBehaviour
{
    [SerializeField] private Transform _pointStart;

    [SerializeField] private Vector3 _size;

    [SerializeField] private float _delta = 0.5f;

    private float[,] _transformationMatrix;
    private float[,] _transformedGlobalPivot;

    private bool[,] _buildingMatrix;
 

    private Stack<GameObject> _stack;
    private List<List<KeyValuePair<int, int>>> _map;

    [SerializeField] private GameObject _green;
    [SerializeField] private GameObject _red;

    private void Start()
    {
        _map = new List<List<KeyValuePair<int, int>>>();

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
        Vector3 x = (j + 0.5f) *_delta * _pointStart.forward;
        Vector3 z = (i + 0.5f) *_delta * _pointStart.right;
        Vector3 y =  _pointStart.up*yd*0.5f;
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

                Vector3 nv = (deltaX) * buildingTransform.right + (deltaZ) * buildingTransform.forward - buildingTransform.right * buildingSize.x / 2 - buildingTransform.forward * buildingSize.z / 2;

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

        
        List<KeyValuePair<int, int>> arr = new List<KeyValuePair<int, int>> ();
        foreach(Vector3 point in pointsOnMatrix)
        {
            arr.Add(new KeyValuePair<int, int>((int)(point.x), (int)(point.z)));
            _buildingMatrix[(int)(point.x), (int)(point.z)] = true;
        }
        _map.Add(arr);
        return true;
    }

    public List<Vector3> GetWay(Vector3 startPoint, List<KeyValuePair<int, int>> endPoints)
    {
        //point distination
        KeyValuePair<int, int> constPoint = endPoints[endPoints.Count - 1];

        //Heap for search points
        SortedSet<int> distances = new SortedSet<int>();
        Dictionary<int, Stack<KeyValuePair<int, int>>> points = new Dictionary<int, Stack<KeyValuePair<int, int>>>();

        //creating start point
        KeyValuePair<int, int> pointClosest = findClosestPoint(startPoint);
        int distination = (int)(toPoint(pointClosest.Key - constPoint.Key, pointClosest.Value - constPoint.Value)).sqrMagnitude;
        distances.Add(distination);
        points.Add(distination, new Stack<KeyValuePair<int, int>>());
        points.GetValueOrDefault(distination).Push(pointClosest);

        //init parents, checked points
        KeyValuePair<int, int>[,] parents = new KeyValuePair<int, int>[Mathf.Abs((int)(_size.x / _delta)), Mathf.Abs((int)(_size.z / _delta))];
        bool[,] isChecked = new bool[Mathf.Abs((int)(_size.x / _delta)), Mathf.Abs((int)(_size.z / _delta))];
        for (int i = 0; i < Mathf.Abs((int)(_size.x / _delta)); i++)
        {
            for(int j = 0; j < Mathf.Abs((int)(_size.z / _delta)); j++)
            {
                parents[i, j] = new KeyValuePair<int, int>(-1, -1);
                isChecked[i, j] = false;
            }
        }

        //init def for while
        KeyValuePair<int, int> last = new KeyValuePair<int, int>(-1, -1);
        bool isFind = false;
        while (distances.Count > 0 && !isFind)
        {
            Stack<KeyValuePair<int, int>> pointBuf = points.GetValueOrDefault(distances.Min);

            while(pointBuf.Count > 0)
            {
                KeyValuePair<int, int> point = pointBuf.Pop();

                if (isChecked[point.Key, point.Value]) continue;
                isChecked[point.Key, point.Value] = true;

                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        KeyValuePair<int, int> kb = new KeyValuePair<int, int>(i + point.Key, j + point.Value);

                        if (!isInBounds(kb.Key, kb.Value) || isChecked[kb.Key, kb.Value] || _buildingMatrix[kb.Key, kb.Value] && endPoints.IndexOf(kb) == -1) continue;

                        distination = (int)toPoint(kb.Key - constPoint.Key, kb.Value - constPoint.Value).sqrMagnitude;

                        distances.Add(distination);

                        if (!points.ContainsKey(distination))
                        {
                            points.Add(distination, new Stack<KeyValuePair<int, int>>());
                        }

                        points.GetValueOrDefault(distination).Push(kb);

                        if (parents[kb.Key, kb.Value].Key == -1 && parents[kb.Key, kb.Value].Value == -1) parents[kb.Key, kb.Value] = point;

                        if (endPoints.IndexOf(kb) != -1)
                        {
                            last = kb;
                            isFind = true;
                        }
                    }
                }
            }

            distances.Remove(distances.Min);
        }

        if (isInBounds(last.Key, last.Value))
        {
            Stack<KeyValuePair<int, int>> keyValuePairs = new Stack<KeyValuePair<int, int>>();
            while(last.Key != pointClosest.Key || last.Value != pointClosest.Value)
            {
                keyValuePairs.Push(new KeyValuePair<int, int>(last.Key, last.Value));
                last = parents[last.Key, last.Value];
            }

            List<Vector3> ans = new List<Vector3>();
            while(keyValuePairs.Count > 0)
            {
                KeyValuePair<int, int> pair = keyValuePairs.Pop();
                ans.Add(toPoint(pair.Key, pair.Value) + _pointStart.position);
            }

            return ans;
        }
        return null;
    }
    private bool isInBounds(int i, int j)
    {
        return i >= 0 && i < Mathf.Abs((int)(_size.x / _delta)) && j >= 0 && j < Mathf.Abs((int)(_size.z / _delta));
    }
    private KeyValuePair<int, int> findClosestPoint(Vector3 point)
    {
        KeyValuePair<int, int> ans = new KeyValuePair<int, int>(0, 0);
        Vector3 ansP = _pointStart.position;
        for(int i = 0; i < Mathf.Abs((int)(_size.x / _delta)); i++)
        {
            if (i == 0 || i == Mathf.Abs((int)(_size.x / _delta)) - 1)
            {
                for(int j = 0; j < Mathf.Abs((int)(_size.z / _delta)); j++)
                {
                    Vector3 point1 = toPoint(i, j) + _pointStart.position;

                    if ((point - point1).sqrMagnitude < (point - ansP).sqrMagnitude)
                    {
                        ans = new KeyValuePair<int, int>(i, j);
                        ansP = point1;
                    }
                }
            } else
            {

                Vector3 point1 = toPoint(i, 0) + _pointStart.position;
                Vector3 point2 = toPoint(i, Mathf.Abs((int)(_size.z / _delta) - 1)) + _pointStart.position;

                if((point - point1).sqrMagnitude < (point - ansP).sqrMagnitude)
                {
                    ans = new KeyValuePair<int, int>(i, 0);
                    ansP = point1;
                }

                if ((point - point2).sqrMagnitude < (point - ansP).sqrMagnitude)
                {
                    ans = new KeyValuePair<int, int>(i, Mathf.Abs((int)(_size.z / _delta) - 1));
                    ansP = point2;
                }
            }
        }
        return ans;
    }

    public List<Vector3> GetWayToLast(Vector3 startPoint)
    {
        if(_map.Count > 0)
        return GetWay(startPoint, _map[_map.Count - 1]);

        return null;
    }

    public List<Vector3> GetWayToRandom(Vector3 startPoint)
    {
        if (_map.Count > 0)
            return GetWay(startPoint, _map[Random.Range(0, _map.Count)]);

        return null;
    }

    private Vector3 toPoint(float i, float j)
    {
        return i * _delta * _pointStart.right + j * _delta * _pointStart.forward;
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

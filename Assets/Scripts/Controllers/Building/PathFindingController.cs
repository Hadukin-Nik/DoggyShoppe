using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingController
{
    private KeyValuePair<int, int> _size;

    //OZ axis (local)
    private Vector3 _correctForwardVector;
    //OX axis (local)
    private Vector3 _correctRightVector;

    private Vector3 _pointStart;
    public PathFindingController(KeyValuePair<int, int> size, Vector3 correctFwdVector, Vector3 correctRghtVector, Vector3 pointStart)
    {
        _size = size;
        _correctForwardVector = correctFwdVector;
        _correctRightVector = correctRghtVector;
        _pointStart = pointStart;
    }

    private Vector3 toPointFromMatrix(float i, float j)
    {
        return i * _correctRightVector + j * _correctForwardVector;
    }

    public List<Vector3> GetWay(bool[,] buildingMatrix, KeyValuePair<int, int> startPoint, List<KeyValuePair<int, int>> endPoints)
    { 
        //point distination
        KeyValuePair<int, int> constPoint = endPoints[endPoints.Count - 1];

        //Heap for search points
        SortedSet<int> distances = new SortedSet<int>();
        Dictionary<int, Stack<KeyValuePair<int, int>>> points = new Dictionary<int, Stack<KeyValuePair<int, int>>>();

        //creating start point
        int distination = (int)(toPointFromMatrix(startPoint.Key - constPoint.Key, startPoint.Value - constPoint.Value)).sqrMagnitude;
        distances.Add(distination);
        points.Add(distination, new Stack<KeyValuePair<int, int>>());
        points.GetValueOrDefault(distination).Push(startPoint);

        //init parents, checked points
        KeyValuePair<int, int>[,] parents = new KeyValuePair<int, int>[_size.Key, _size.Value];
        bool[,] isChecked = new bool[_size.Key, _size.Value];
        for (int i = 0; i < _size.Key; i++)
        {
            for (int j = 0; j < _size.Value; j++)
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

            while (pointBuf.Count > 0)
            {
                KeyValuePair<int, int> point = pointBuf.Pop();

                if (isChecked[point.Key, point.Value]) continue;
                isChecked[point.Key, point.Value] = true;

                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        KeyValuePair<int, int> kb = new KeyValuePair<int, int>(i + point.Key, j + point.Value);

                        if (!isInBounds(kb.Key, kb.Value) || isChecked[kb.Key, kb.Value] || buildingMatrix[kb.Key, kb.Value] && endPoints.IndexOf(kb) == -1) continue;

                        distination = (int)toPointFromMatrix(kb.Key - constPoint.Key, kb.Value - constPoint.Value).sqrMagnitude;

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
            while (last.Key != startPoint.Key || last.Value != startPoint.Value)
            {
                keyValuePairs.Push(new KeyValuePair<int, int>(last.Key, last.Value));
                last = parents[last.Key, last.Value];
            }

            List<Vector3> ans = new List<Vector3>();
            while (keyValuePairs.Count > 0)
            {
                KeyValuePair<int, int> pair = keyValuePairs.Pop();
                ans.Add(toPointFromMatrix(pair.Key, pair.Value) + _pointStart);
            }

            return ans;
        }
        return null;
    }
    private bool isInBounds(int i, int j)
    {
        return i >= 0 && i < _size.Key && j >= 0 && j < _size.Value;
    }
    


}

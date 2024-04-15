using UnityEngine;

public class BuildingContoller : MonoBehaviour
{
    [SerializeField] private Vector3 _point;
    [SerializeField] private Vector3 _size;
    
    public BuildingContoller(Vector3 point, Vector3 size)
    {
        _point = point;
        _size = size;   
    }

    public Vector3 GetSize()
    {
        return _size;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}

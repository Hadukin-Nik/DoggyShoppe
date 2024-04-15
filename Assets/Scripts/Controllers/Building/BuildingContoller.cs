using UnityEngine;

public class BuildingContoller : MonoBehaviour
{
    [SerializeField] private Vector3 _point;
    [SerializeField] private Vector3 _size;
    
    public Vector3 GetPoint()
    {
        return _point;
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

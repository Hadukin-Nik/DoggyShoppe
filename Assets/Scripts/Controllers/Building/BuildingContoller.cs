using UnityEngine;

public class BuildingContoller : MonoBehaviour
{
    [SerializeField] private Vector3 _size;
    
    public BuildingContoller(Vector3 size)
    {
        _size = size;   
    }

    public Vector3 GetSize()
    {
        return _size;
    }

    public void SetSize(Vector3 size) { _size = size; }

    public Transform GetTransform()
    {
        return transform;
    }
}

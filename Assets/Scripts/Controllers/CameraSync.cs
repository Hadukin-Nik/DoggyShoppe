using UnityEngine;

public class CameraSync : MonoBehaviour
{
    [SerializeField] Camera _cameraFollow;
    private Camera camera;
    
    void Start()
    {
        camera = GetComponent<Camera>();    
    }

    void Update()
    {
        camera.fieldOfView = _cameraFollow.fieldOfView;
        camera.transform.position = _cameraFollow.transform.position;
        camera.transform.rotation = _cameraFollow.transform.rotation;
    }
}

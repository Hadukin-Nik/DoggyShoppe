using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private float _mouseSensetivity;

    [SerializeField] private Transform _playerBody;
    [SerializeField] private Transform _playerFace;

    private float xRotation = -90f;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensetivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensetivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        _playerFace.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        _playerBody.Rotate(Vector3.up * mouseX);
    }
}

using Cinemachine;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private CinemachineVirtualCamera _cam;

    [SerializeField] private float _fovMin = 36f;
    [SerializeField] private float _fovMax = 140f;
    private float _targetFov;

    private void Start()
    {
        _targetFov = _fovMin;
        _cam = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        MovementAndRotation();
        Zoom();
    }

    private void Zoom()
    {
        if (Input.mouseScrollDelta.y < 0)
            _targetFov += 5;
        if (Input.mouseScrollDelta.y > 0)
            _targetFov -= 5;

        _targetFov = Mathf.Clamp(_targetFov, _fovMin, _fovMax);

        var scrollSpeed = 50f;
        _cam.m_Lens.FieldOfView = Mathf.Lerp(_cam.m_Lens.FieldOfView, _targetFov, Time.deltaTime * scrollSpeed); ;
    }

    private void MovementAndRotation()
    {
        var inputDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            inputDir.y = +1f;
        if (Input.GetKey(KeyCode.S))
            inputDir.y = -1f;
        if (Input.GetKey(KeyCode.A))
            inputDir.x = -1f;
        if (Input.GetKey(KeyCode.D))
            inputDir.x = +1f;


        var moveDir = transform.up * inputDir.y + transform.right * inputDir.x;
        var movementSpeed = 50f;
        transform.position += moveDir * movementSpeed * Time.deltaTime;


        var rotateDir = 0f;
        if (Input.GetKey(KeyCode.RightArrow))
            rotateDir = +1f;
        if (Input.GetKey(KeyCode.LeftArrow))
            rotateDir = -1f;

        var rotateSpeed = 300f;
        transform.eulerAngles += new Vector3(0, rotateDir * rotateSpeed * Time.deltaTime, 0);
    }
}

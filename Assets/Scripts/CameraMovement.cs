using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private void Update()
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

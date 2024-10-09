using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of camera movement
    public float lookSpeed = 2f;  // Speed of mouse look
    public float verticalMoveSpeed = 2f; // Speed of moving up and down
    public LayerMask tabletopLayer; // Layer for the tabletop

    private float verticalRotation = 0f; // Vertical rotation angle

    private void Update()
    {
        HandleMovement();
        HandleVerticalMovement();
        PointCameraToMouse();
    }

    private void HandleMovement()
    {
        // Get camera's local forward and right vectors
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Zero out the Y component to restrict movement to X and Z
        forward.y = 0;
        right.y = 0;

        // Normalize the vectors
        forward.Normalize();
        right.Normalize();

        // Camera movement with WASD keys
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += forward * moveSpeed * Time.deltaTime; // Move forward
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= forward * moveSpeed * Time.deltaTime; // Move backward
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= right * moveSpeed * Time.deltaTime; // Move left
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += right * moveSpeed * Time.deltaTime; // Move right
        }

        // Left mouse button for dragging movement
        if (Input.GetMouseButton(0) && IsMouseOverTabletop())
        {
            float moveX = Input.GetAxis("Mouse X") * moveSpeed * Time.deltaTime;
            float moveY = Input.GetAxis("Mouse Y") * moveSpeed * Time.deltaTime;

            transform.Translate(-moveX, 0, -moveY); // Move the camera only in X and Z
        }
    }

    private void HandleVerticalMovement()
    {
        // Use the scroll wheel to move up and down strictly on the Y-axis
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            // Move the camera up or down based on the scroll input
            transform.position += new Vector3(0, scroll * verticalMoveSpeed, 0);
        }
    }

    private void PointCameraToMouse()
    {
            // Rotate the camera based on mouse movement
            float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

            verticalRotation -= mouseY; // Adjust vertical rotation
            verticalRotation = Mathf.Clamp(verticalRotation, -60f, 60f); // Clamp vertical rotation

            // Apply rotation to the camera
            transform.localEulerAngles = new Vector3(verticalRotation, transform.localEulerAngles.y + mouseX, 0);
    }

    private bool IsMouseOverTabletop()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // Check if the raycast hit the tabletop
            return hit.collider != null && (tabletopLayer & (1 << hit.collider.gameObject.layer)) != 0;
        }
        return false;
    }
}

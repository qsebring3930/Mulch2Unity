using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 1f; // Speed of camera movement
    public float lookSpeed = 100f; // Slow down the mouse look for more controlled rotation
    public float verticalMoveSpeed = 2f; // Speed of moving up and down

    public LayerMask environmentLayer; // Layer of the environment objects
    public float collisionBuffer = 0.5f; //Distance between camera and object where it should stop moving

    public float edgeThreshold = 0.05f; // Percentage of the screen width/height where camera starts rotating
    public float screenPadding = 0.1f; // Percentage of screen where the camera should start rotating

    private Vector3 lastMousePos;
    private bool isRotating = false; // Flag to track if the camera should be rotating
    private Vector2 exitDirection = Vector2.zero; // Tracks the direction of exit from bounding box
    private Quaternion targetRotation; // Store the target rotation of the camera
    private Vector3 targetPosition; // Store the target position of the camera

    private void Update()
    {
        HandleMovement();
        HandleVerticalMovement();
        PointCameraToMouse();
    }

    private void HandleMovement()
    {
        // Get camera's current position
        targetPosition = transform.position;

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
            targetPosition += forward * 10f; // Move forward
        }
        if (Input.GetKey(KeyCode.S))
        {
            targetPosition -= forward * 10f; // Move backward
        }
        if (Input.GetKey(KeyCode.A))
        {
            targetPosition -= right * 10f; // Move left
        }
        if (Input.GetKey(KeyCode.D))
        {
            targetPosition += right * 10f; // Move right
        }

        Vector3 direction = targetPosition - transform.position;
        float distance = direction.magnitude;

        if (distance > 0)
        {
            Ray ray = new Ray(transform.position, direction.normalized);
            if (Physics.Raycast(ray, out RaycastHit hit, distance + collisionBuffer, environmentLayer))
            {
                // Adjust the target position to avoid collision
                targetPosition = hit.point - direction.normalized * collisionBuffer;
            }
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);

        // Left mouse button for dragging movement
        //if (Input.GetMouseButton(0) && IsMouseOverTabletop())
        //{
        //float moveX = Input.GetAxis("Mouse X") * moveSpeed * Time.deltaTime;
        //float moveY = Input.GetAxis("Mouse Y") * moveSpeed * Time.deltaTime;

        //transform.Translate(-moveX, 0, -moveY); // Move the camera only in X and Z
        //}
    }

    private void HandleVerticalMovement()
    {
        // Use the scroll wheel to move up and down strictly on the Y-axis
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            // Determine the direction of movement (up or down)
        Vector3 verticalMovement = new Vector3(0, scroll * verticalMoveSpeed, 0);
        Vector3 desiredPosition = transform.position + verticalMovement;

        // Raycast in the direction of movement (up or down)
        Vector3 rayDirection = verticalMovement.normalized;
        float rayDistance = Mathf.Abs(verticalMovement.y) + collisionBuffer;

        Ray ray = new Ray(transform.position, rayDirection);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, environmentLayer))
        {
            // Adjust the desired position to maintain a buffer distance from the collision point
            desiredPosition.y = hit.point.y - rayDirection.y * collisionBuffer;
        }

        // Smoothly move the camera to the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * moveSpeed);
        }
    }

    private void PointCameraToMouse()
    {
        Vector3 currentMousePos = Input.mousePosition;

        // Check if the mouse is inside the bounding box
        if (IsMouseInsideBoundingBox(currentMousePos))
        {
            isRotating = false;// Stop rotation when the mouse is inside the bounding box
            return;
        }

        // If the mouse has moved outside the bounding box, start rotating based on exit direction
        if (!isRotating && !IsMouseInsideBoundingBox(lastMousePos))
        {
            isRotating = true; // Start rotating
            exitDirection = GetMouseExitDirection(currentMousePos); // Track the exit direction
        }

        // Update exit direction even when the mouse is outside the bounding box
        if (isRotating)
        {
            exitDirection = GetMouseExitDirection(currentMousePos);
            RotateCameraBasedOnExitDirection();
        }

        // Update the last mouse position for the next frame
        lastMousePos = currentMousePos;
    }

    private bool IsMouseInsideBoundingBox(Vector3 mousePosition)
    {
        // Check if the mouse is inside the defined bounding box
        return mousePosition.x >= Screen.width * screenPadding &&
               mousePosition.x <= Screen.width * (1 - screenPadding) &&
               mousePosition.y >= Screen.height * screenPadding &&
               mousePosition.y <= Screen.height * (1 - screenPadding);
    }

    private Vector2 GetMouseExitDirection(Vector3 mousePosition)
    {
        // Calculate the direction the mouse is leaving the bounding box
        Vector2 direction = Vector2.zero;

        if (mousePosition.x <= Screen.width * screenPadding) // Left side
            direction.x = -1;
        else if (mousePosition.x >= Screen.width * (1 - screenPadding)) // Right side
            direction.x = 1;

        if (mousePosition.y <= Screen.height * screenPadding) // Bottom side
            direction.y = -1;
        else if (mousePosition.y >= Screen.height * (1 - screenPadding)) // Top side
            direction.y = 1;

        return direction;
    }

    private void RotateCameraBasedOnExitDirection()
    {
        if (Input.GetMouseButton(0)) // Prevent rotation when clicking
        {
            return; // No rotation while dragging
        }


        // Determine horizontal and vertical edge factors
        float edgeFactorX = Mathf.Abs(Input.mousePosition.x - Screen.width / 2) / (Screen.width / 2);
        float edgeFactorY = Mathf.Abs(Input.mousePosition.y - Screen.height / 2) / (Screen.height / 2);

        // Calculate desired rotation angles
        float horizontalRotation = exitDirection.x * edgeFactorX * 100; // Horizontal rotation magnitude
        float verticalRotation = -exitDirection.y * edgeFactorY * 100; // Vertical rotation magnitude (invert Y)

        // Update the target vertical rotation, clamping to avoid flipping
        verticalRotation = Mathf.Clamp(verticalRotation + transform.localEulerAngles.x, 0f, 60f);

        // Calculate the target rotation based on the input
        targetRotation = Quaternion.Euler(verticalRotation, transform.localEulerAngles.y + horizontalRotation, 0);

        // Smoothly interpolate to the target rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * lookSpeed);
        transform.rotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, 0); // stabilize the rotation to always stay parallel to horizon
        Debug.Log("Camera rotation is" + transform.rotation.eulerAngles);
    }

    private bool IsMouseOverTabletop()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // Check if the raycast hit the tabletop
            //return hit.collider != null && (tabletopLayer & (1 << hit.collider.gameObject.layer)) != 0;
        }
        return false;
    }
}

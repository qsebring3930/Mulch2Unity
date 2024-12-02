using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChitManager : MonoBehaviour
{
    public List<GameObject> chitPrefabs; // Array to set chit prefabs in the Inspector
    private GameObject currentChit; // The chit currently being dragged

    private Vector3 mousePosition;
    private Rigidbody rb;
    private Vector3 dragOffset;
    private Plane dragPlane;

    // Bounds of the tabletop surface
    private float minYPosition = 0f;
    private float maxYPosition = 2f;
    private float minXPosition = -12.5f;
    private float maxXPosition = 12.5f;
    private float minZPosition = -30f;
    private float maxZPosition = 32f;

    void OnMouseDown()
    {
        // Create a plane where the card is currently located
        dragPlane = new Plane(Vector3.up, transform.position);
        mousePosition = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);

        // Ray from the camera to the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Calculate the drag offset using the intersection point of the ray and the plane
        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            dragOffset = transform.position - hitPoint;
        }
    }

    void OnMouseDrag()
    {
        if (currentChit == null && chitPrefabs.Count > 0)
        {
            // Instantiate a random chit prefab
            int randomIndex = Random.Range(0, chitPrefabs.Count);
            GameObject chitPrefab = chitPrefabs[randomIndex];
            currentChit = Instantiate(chitPrefab, mousePosition, Quaternion.identity);
            rb = currentChit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // Disable physics while holding
                rb.detectCollisions = false;
            }

            // Remove the prefab from the list
            chitPrefabs.RemoveAt(randomIndex);
        }
        
        if (currentChit != null)
        {
            // Ray from the camera to the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);

            // Get the current mouse world-space height, clamped within tabletop boundaries
            float mouseHeight = Mathf.Clamp(mouseWorldPosition.y, minYPosition, maxYPosition);

            // Update the drag plane to match the clamped mouse height
            dragPlane = new Plane(Vector3.up, new Vector3(0, mouseHeight, 0));

            // Intersect ray with updated drag plane
            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                Vector3 targetPosition = hitPoint + dragOffset;

                // Clamp the position to tabletop boundaries
                targetPosition.x = Mathf.Clamp(targetPosition.x, minXPosition, maxXPosition);
                targetPosition.y = Mathf.Clamp(targetPosition.y, minYPosition, maxYPosition);
                targetPosition.z = Mathf.Clamp(targetPosition.z, minZPosition, maxZPosition);

                // Update the card's position to the dynamically calculated height
                currentChit.transform.position = targetPosition;
            }
        }
    }

    void OnMouseUp()
    {
        if (currentChit != null)
        {
            // Drop the chit and reset
            if (rb != null)
            {
                rb.isKinematic = false; // Disable physics while holding
                rb.detectCollisions = true;
            }
            currentChit = null;
        }
    }
}

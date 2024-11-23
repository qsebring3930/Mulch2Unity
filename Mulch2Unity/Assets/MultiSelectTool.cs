using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiSelectTool : MonoBehaviour
{
    public RectTransform dragBox; // Reference to the UI Image used as the drag box
    public Camera mainCamera; // Reference to the main camera
    public LayerMask selectableLayer; // Layer for the selectable cards
    public LayerMask environmentLayer; // Layer for the environment where the tool can be activated
    public Color highlightColor = Color.yellow; // Color for highlighting selected objects
    private Vector2 startMousePos;
    private List<Renderer> highlightedObjects = new List<Renderer>();
    private bool isDragging = false;

    void Update()
    {
        HandleInput();
    }
    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.layer == 6)
                {
                    // Start dragging only if the mouse is over the environment
                    isDragging = true;
                    startMousePos = Input.mousePosition;
                    dragBox.gameObject.SetActive(true);
                    dragBox.sizeDelta = Vector2.zero; // Reset the size
                }
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            // Update the drag box size and position
            Vector2 currentMousePos = Input.mousePosition;

            Vector2 dragBoxStart = startMousePos; // Start position
            Vector2 dragBoxEnd = currentMousePos; // End position

            // Convert to RectTransform space
            Vector2 dragStartInCanvas, dragEndInCanvas;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                dragBox.parent as RectTransform,
                dragBoxStart,
                mainCamera,
                out dragStartInCanvas
            );
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                dragBox.parent as RectTransform,
                dragBoxEnd,
                mainCamera,
                out dragEndInCanvas
            );

            // Calculate the size and position of the drag box
            Vector2 boxPosition = (dragStartInCanvas + dragEndInCanvas) / 2;
            Vector2 boxSize = new Vector2(
                Mathf.Abs(dragStartInCanvas.x - dragEndInCanvas.x),
                Mathf.Abs(dragStartInCanvas.y - dragEndInCanvas.y)
            );

            dragBox.localPosition = boxPosition;
            dragBox.sizeDelta = boxSize;
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            // End dragging
            isDragging = false;
            dragBox.gameObject.SetActive(false);
        }
    }


    private void HighlightObjectsInBox(Vector2 boxStart, Vector2 boxEnd)
    {
        // Clear previous highlights
        foreach (var renderer in highlightedObjects)
        {
            renderer.material.color = Color.white; // Reset color
        }
        highlightedObjects.Clear();

        // Perform a raycast to check objects inside the box
        Vector3 boxWorldStart = mainCamera.ScreenToWorldPoint(new Vector3(boxStart.x, boxStart.y, 10f));
        Vector3 boxWorldEnd = mainCamera.ScreenToWorldPoint(new Vector3(boxEnd.x, boxEnd.y, 10f));

        Bounds boxBounds = new Bounds();
        boxBounds.SetMinMax(boxWorldStart, boxWorldEnd);

        Collider[] objectsInBox = Physics.OverlapBox(
            boxBounds.center,
            boxBounds.extents,
            Quaternion.identity,
            selectableLayer
        );

        foreach (var obj in objectsInBox)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = highlightColor; // Highlight the object
                highlightedObjects.Add(renderer);
            }
            Debug.Log("Object in box is: " + obj.gameObject.name);
        }
    }

    private void OnDrawGizmos()
    {
        // For debugging the selection box in the scene view
        if (isDragging)
        {
            Vector3 startWorld = mainCamera.ScreenToWorldPoint(new Vector3(startMousePos.x, startMousePos.y, 10f));
            Vector3 endWorld = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));

            Bounds boxBounds = new Bounds();
            boxBounds.SetMinMax(Vector3.Min(startWorld, endWorld), Vector3.Max(startWorld, endWorld));

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(boxBounds.center, boxBounds.size);
        }
    }
}

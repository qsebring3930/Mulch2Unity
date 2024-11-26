using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    [Tooltip("The camera used for highlighting")]
    public Camera selectCam;

    [Tooltip("The drag box object (UI element for selection)")]
    public GameObject dragBoxObject;

    private RectTransform SelectingBoxRect;

    private Rect SelectingRect;
    private Vector3 SelectingStart;

    [Tooltip("Minimum size of the drag box before selecting objects")]
    public float minBoxSizeBeforeSelect = 10f;

    [Tooltip("Delay before checking for objects under the mouse")]
    public float selectUnderMouseTimer = 0.1f;

    private float selectTimer = 0f;

    public List<SelectableCharacter> selectableChars = new List<SelectableCharacter>();
    private List<SelectableCharacter> selectedArmy = new List<SelectableCharacter>();

    private void Awake()
    {
        // Validate the drag box and retrieve its RectTransform
        if (dragBoxObject == null)
        {
            Debug.LogError("Drag Box Object is not assigned! Please set it in the Inspector.");
            return;
        }

        SelectingBoxRect = dragBoxObject.GetComponent<RectTransform>();
        if (SelectingBoxRect == null)
        {
            Debug.LogError("The assigned Drag Box Object does not have a RectTransform component!");
            return;
        }

        // Populate selectable characters
        SelectableCharacter[] chars = FindObjectsOfType<SelectableCharacter>();
        selectableChars.AddRange(chars);
    }

    void OnMouseDown()
    {
        // Update selectable cards
        selectableChars = new List<SelectableCharacter>();
        SelectableCharacter[] chars = FindObjectsOfType<SelectableCharacter>();
        selectableChars.AddRange(chars);

        // Start selection on the tabletop
        ReSelect();

        if (SelectingBoxRect != null)
        {
            SelectingStart = Input.mousePosition;
            SelectingBoxRect.anchoredPosition = SelectingStart;
        }
    }

    void OnMouseDrag()
    {

        // Update the selection box
        SelectingArmy();
        selectTimer += Time.deltaTime;

        if (selectTimer <= selectUnderMouseTimer)
        {
            CheckIfUnderMouse();
        }
    }

    void OnMouseUp()
    {

        if (SelectingBoxRect != null)
        {
            SelectingBoxRect.sizeDelta = Vector2.zero;
        }

        selectTimer = 0f;
    }

    void ReSelect()
    {
        // Clear current selection
        foreach (SelectableCharacter character in selectedArmy)
        {
            character.TurnOffSelector();
        }
        selectedArmy.Clear();
    }

    void SelectingArmy()
    {
        if (SelectingBoxRect == null)
        {
            return;
        }

        Vector2 _pivot = Vector2.zero;
        Vector3 _sizeDelta = Vector3.zero;
        Rect _rect = Rect.zero;

        // Calculate pivot, sizeDelta, and rect based on drag direction
        if (-(SelectingStart.x - Input.mousePosition.x) > 0)
        {
            _sizeDelta.x = -(SelectingStart.x - Input.mousePosition.x);
            _rect.x = SelectingStart.x;
        }
        else
        {
            _pivot.x = 1;
            _sizeDelta.x = SelectingStart.x - Input.mousePosition.x;
            _rect.x = SelectingStart.x - SelectingBoxRect.sizeDelta.x;
        }

        if (SelectingStart.y - Input.mousePosition.y > 0)
        {
            _pivot.y = 1;
            _sizeDelta.y = SelectingStart.y - Input.mousePosition.y;
            _rect.y = SelectingStart.y - SelectingBoxRect.sizeDelta.y;
        }
        else
        {
            _sizeDelta.y = -(SelectingStart.y - Input.mousePosition.y);
            _rect.y = SelectingStart.y;
        }

        // Update pivot and size
        SelectingBoxRect.pivot = _pivot;
        SelectingBoxRect.sizeDelta = _sizeDelta;

        // Update selection rect
        _rect.width = SelectingBoxRect.sizeDelta.x;
        _rect.height = SelectingBoxRect.sizeDelta.y;
        SelectingRect = _rect;

        // Check for objects if the drag box is large enough
        if (_rect.height > minBoxSizeBeforeSelect && _rect.width > minBoxSizeBeforeSelect)
        {
            CheckForSelectedCharacters();
        }
    }

    void CheckForSelectedCharacters()
    {
        foreach (SelectableCharacter character in selectableChars)
        {
            Vector2 screenPos = selectCam.WorldToScreenPoint(character.transform.position);
            if (SelectingRect.Contains(screenPos))
            {
                if (!selectedArmy.Contains(character))
                {
                    selectedArmy.Add(character);
                    character.TurnOnSelector();
                }
            }
            else
            {
                character.TurnOffSelector();
                selectedArmy.Remove(character);
            }
        }
    }

    void CheckIfUnderMouse()
    {
        RaycastHit hit;
        Ray ray = selectCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100f))
        {
            SelectableCharacter character = hit.transform.GetComponent<SelectableCharacter>();
            if (character != null && selectableChars.Contains(character))
            {
                selectedArmy.Add(character);
                character.TurnOnSelector();
            }
        }
    }
}

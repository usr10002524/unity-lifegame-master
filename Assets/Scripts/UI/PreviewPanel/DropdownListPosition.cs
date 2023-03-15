using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class DropdownListPosition : MonoBehaviour
{
    private TMP_Dropdown dropdown;
    private EventTrigger eventTrigger;
    private Vector2 lastPostion;
    private bool lastPositionValid;

    // Start is called before the first frame update
    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.onValueChanged.AddListener(OnChanged);

        eventTrigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener(OnSelect);
        eventTrigger.triggers.Add(entry);

        lastPostion = new Vector2();
        lastPositionValid = false;
    }

    private void OnSelect(BaseEventData data)
    {
        // Debug.Log("Dropdown OnSelect.");

        Transform currentTransform = transform.Find("Dropdown List/Viewport/Content");
        RectTransform currentRect = null;
        if (currentTransform == null)
        {
            // Debug.Log("Dropdown OnSelect. Transform Not Found");
            return;
        }
        currentRect = currentTransform.GetComponent<RectTransform>();
        if (currentRect == null)
        {
            // Debug.Log("Dropdown OnSelect. RectTransform Not Found");
            return;
        }
        if (!lastPositionValid)
        {
            // Debug.Log("Dropdown OnSelect. Data not Ready.");
            return;
        }

        currentRect.anchoredPosition = lastPostion;
        lastPositionValid = false;
        // Debug.Log(string.Format("OnSelect() x:{0}, y:{1}", lastPostion.x, lastPostion.y));
    }

    private void OnChanged(int value)
    {
        Transform currentTransform = transform.Find("Dropdown List/Viewport/Content");
        RectTransform currentRect = null;
        if (currentTransform != null)
        {
            currentRect = currentTransform.GetComponent<RectTransform>();
            lastPostion = currentRect.anchoredPosition;
            lastPositionValid = true;
            // Debug.Log(string.Format("OnSelect() x:{0}, y:{1}", lastPostion.x, lastPostion.y));
        }
    }
}

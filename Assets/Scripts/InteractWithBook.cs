using UnityEngine;
using Oculus.Interaction;

[RequireComponent(typeof(Grabbable))]
public class InteractWithBook : MonoBehaviour
{
    [Header("Références")]
    public GameObject closedBook;
    public GameObject openedBook;

    private Grabbable grabbable;
    private bool isOpen = false;

    void Awake()
    {
        grabbable = GetComponent<Grabbable>();
        grabbable.WhenPointerEventRaised += HandlePointerEvent;
    }

    private void OnDestroy()
    {
        if (grabbable != null)
            grabbable.WhenPointerEventRaised -= HandlePointerEvent;
    }

    private void HandlePointerEvent(PointerEvent evt)
    {
        switch (evt.Type)
        {
            case PointerEventType.Select:
                OpenBook();
                break;
            case PointerEventType.Unselect:
            case PointerEventType.Cancel:
                CloseBook();
                break;
        }
    }

    void Start()
    {
        // États initiaux
        if (closedBook) closedBook.SetActive(true);
        if (openedBook) openedBook.SetActive(false);
    }

    void OpenBook()
    {
        if (isOpen) return;

        isOpen = true;
        if (closedBook) closedBook.SetActive(false);
        if (openedBook) openedBook.SetActive(true);
    }

    void CloseBook()
    {
        if (!isOpen) return;

        isOpen = false;
        if (closedBook) closedBook.SetActive(true);
        if (openedBook) openedBook.SetActive(false);
    }
}
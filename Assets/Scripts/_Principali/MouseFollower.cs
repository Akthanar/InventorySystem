using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class MouseFollower : MonoBehaviour//, IPointerMoveHandler
{
    void Awake () => Instance = this;
    public static MouseFollower Instance;


    public Transform mousePointer;
    
    [SerializeField] private Image grabbedItemImage;
    bool isGrabbingItem = false;


    void Start ()
    {
        // avvisa in caso venisse dimenticato di impostare il mousePointer
        if (mousePointer is null)
        {
            Debug.LogWarning("MousePointer non impostato, cerco di impostarlo in automatico");
            mousePointer = GameObject.Find("Mouse").transform;
        }

        grabbedItemImage = mousePointer.GetChild(1).GetComponent<Image>();
        grabbedItemImage.enabled = false;
    }



    // movimento non abbastanza fluido
    // public void OnPointerMove (PointerEventData eventData) => mousePointer.position = eventData.pointerCurrentRaycast.worldPosition;

    void Update ()
    {
        // prendi e trasforma la posizione del mouse
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        mousePos.z = 0;

        // imposta la posizione del mouse al mousePointer
        mousePointer.position = mousePos;

        if (isGrabbingItem && Input.GetMouseButtonUp(0)) ReleaseItem();
    }


    


    /// <summary> Imposta l'immagine dell'oggetto trascinato, lascia parametri vuoti per nascondere. </summary>
    public void SetGrabbedItem(Item item = null)
    {
        if (item is null) grabbedItemImage.enabled = false;
        else
        {
            grabbedItemImage.sprite = item.icon;
            grabbedItemImage.enabled = true;
            isGrabbingItem = true;
        }
    }

    public void ReleaseItem()
    {
        grabbedItemImage.enabled = false;
        isGrabbingItem = false;
    }
}

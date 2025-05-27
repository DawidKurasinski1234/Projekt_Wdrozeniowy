using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragElements : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image mainImage;

    private Material mainMaterial;
    /// <summary>
    /// Материал, применяемый к объектам на сцене
    /// </summary>
    public Material MainMaterial
    {
        get{ return mainMaterial; }
        set
        {
            if (value != null)
            {
                MainMaterial = value;
                if (mainImage != null)
                    mainImage.color = MainMaterial.color;
            }
        }
    }

    private Transform defaultParentTransform;
    /// <summary>
    /// Трансформ объекта, к которому прикреплена кнопка
    /// </summary>
    public Transform DefaultParentTransform
    {
        get { return defaultParentTransform; }
        set
        {
            if(value != null)
            {
                defaultParentTransform = value;
            }
        }
    }

    private Transform dragParentTransform;
    /// <summary>
    /// Трансформ объекта,к которому прикреплена кнопка во время драга
    /// </summary>
    public Transform DragParentTransfotm
    { 
        get 
        { 
            return dragParentTransform;
        } 
        set 
        {
        if (value != null)
                dragParentTransform = value;
        }
    }

    private int siblingIndex;
    /// <summary>
    /// Номер индекса внутри родительского объекта
    /// </summary>
    public int SiblingIndex
    {
        get { return siblingIndex; }
        set
        {
            if (value > 0)
                siblingIndex = value;
        }
    }


    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(DragParentTransfotm);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
            Input.mousePosition.y, 1));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(DefaultParentTransform);
        transform.SetSiblingIndex(siblingIndex);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            hit.collider.gameObject.GetComponent<Renderer>().material = mainMaterial;
        }
        
    }
}

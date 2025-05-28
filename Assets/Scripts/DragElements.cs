using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragElements : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Tooltip("Obraz UI, którego kolor może być synchronizowany z MainMaterial.")]
    [SerializeField] private Image mainImage;

    private Material mainMaterial;
    /// <summary>
    /// Materiał, stosowany do obiektów na scenie po upuszczeniu.
    /// </summary>
    public Material MainMaterial
    {
        get { return mainMaterial; }
        set
        {
            if (value != null)
            {
                mainMaterial = value; 
                if (mainImage != null && mainMaterial != null) 
                {
                    mainImage.color = mainMaterial.color;
                }
            }
        }
    }

    private Transform defaultParentTransform;
    /// <summary>
    /// Transform obiektu, do którego domyślnie przypisany jest ten element.
    /// </summary>
    public Transform DefaultParentTransform
    {
        get { return defaultParentTransform; }
        set
        {
            if (value != null)
            {
                defaultParentTransform = value;
            }
        }
    }

    private Transform dragParentTransform;
    /// <summary>
    /// Transform obiektu, do którego element jest przypisywany podczas przeciągania.
    /// </summary>
    public Transform DragParentTransform // POPRAWKA: Usunięto literówkę "Transfotm"
    {
        get { return dragParentTransform; }
        set
        {
            if (value != null)
            {
                dragParentTransform = value;
            }
        }
    }

    private int siblingIndex;
    /// <summary>
    /// Indeks elementu wśród rodzeństwa w obiekcie nadrzędnym.
    /// </summary>
    public int SiblingIndex
    {
        get { return siblingIndex; }
        set
        {
            if (value >= 0) 
            {
                siblingIndex = value;
            }
        }
    }

    void Awake()
    {
        
        if (mainImage == null)
        {
            mainImage = GetComponent<Image>();
        }

        // Ustaw domyślny rodzic, jeśli nie został przypisany z zewnątrz
        if (DefaultParentTransform == null && transform.parent != null)
        {
            DefaultParentTransform = transform.parent;
        }
        // Zawsze pobierz aktualny indeks rodzeństwa przy starcie jako domyślny
        SiblingIndex = transform.GetSiblingIndex();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (DragParentTransform != null)
        {
            transform.SetParent(DragParentTransform);
            transform.SetAsLastSibling(); 
        }
        else
        {
            Debug.LogWarning("DragParentTransform nie jest ustawiony. Przeciąganie może nie działać zgodnie z oczekiwaniami.", this);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Ta metoda pozycjonowania działa najlepiej dla UI w przestrzeni świata (World Space) lub obiektów 3D.
        // Dla UI "Screen Space - Overlay", rozważ użycie RectTransformUtility.ScreenPointToLocalPointInRectangle.
        // Wartość Z (tutaj Camera.main.nearClipPlane + 1f) jest kluczowa i zależy od konfiguracji kamery.
        if (Camera.main != null)
        {
            // Użyj eventData.position dla większej spójności z systemem zdarzeń, zamiast Input.mousePosition
            Vector3 screenPoint = new Vector3(eventData.position.x, eventData.position.y, Camera.main.nearClipPlane + 1f);
            transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
        }
        else
        {
            Debug.LogError("Camera.main jest null. Nie można zaktualizować pozycji w OnDrag.", this);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (DefaultParentTransform != null)
        {
            transform.SetParent(DefaultParentTransform);
            transform.SetSiblingIndex(SiblingIndex); // Używa SiblingIndex ustawionego w Awake lub przez setter
        }
        else
        {
            Debug.LogWarning("DefaultParentTransform nie jest ustawiony. Element nie może wrócić do oryginalnego rodzica.", this);
        }

        if (Camera.main != null && mainMaterial != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(eventData.position); 
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Renderer rend = hit.collider.gameObject.GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.material = mainMaterial;
                }
                // else Debug.Log("Trafiony obiekt nie posiada komponentu Renderer.", hit.collider.gameObject);
            }
        }
        else if (Camera.main == null)
        {
            Debug.LogError("Camera.main jest null. Nie można wykonać raycastu w OnEndDrag.", this);
        }
        // else if (mainMaterial == null)
        // {
        //    Debug.LogWarning("MainMaterial nie jest przypisany w DragElements. Nie można zastosować materiału po upuszczeniu.", this);
        // }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NewBehaviourScript : MonoBehaviour
{
    [Tooltip("Prefab elementu, który będzie można przeciągać")]
    [SerializeField] private GameObject dragColorPrefab;

    [Tooltip("Odnośnik do obiektu 'Content' w komponencie ScrollView (rodzic dla tworzonych elementów)")]
    [SerializeField] private Transform scrollViewContent;

    [Tooltip("Lista materiałów, które zostaną przypisane do kolejno tworzonych elementów przeciąganych")]
    [SerializeField] private List<Material> materials;

    private void Start()
    {
        // Podstawowe walidacje
        if (dragColorPrefab == null)
        {
            Debug.LogError("Prefab 'dragColorPrefab' nie jest przypisany w Inspektorze!", this);
            return;
        }
        if (scrollViewContent == null)
        {
            Debug.LogError("Transform 'scrollViewContent' nie jest przypisany w Inspektorze!", this);
            return;
        }
        if (materials == null || materials.Count == 0)
        {
            Debug.LogWarning("Lista materiałów jest pusta lub nieprzypisana.", this);
            return;
        }

        for (int i = 0; i < materials.Count; i++)
        {
            GameObject dragObject = Instantiate(dragColorPrefab, scrollViewContent); 
            DragElements script = dragObject.GetComponent<DragElements>();

            if (script == null)
            {
                Debug.LogError($"Prefab '{dragColorPrefab.name}' nie posiada komponentu DragElements. Pomijanie elementu.", dragObject);
                continue; // Przejdź do następnej iteracji, jeśli brakuje skryptu
            }

            script.MainMaterial = materials[i];
            script.DefaultParentTransform = scrollViewContent;
            script.DragParentTransform = transform; 
            script.SiblingIndex = i; 
        }
    }
}
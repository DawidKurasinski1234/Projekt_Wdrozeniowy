using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [Tooltip("Ссылка на префаб деагуемого элемента")]
    [SerializeField] private GameObject dragColorPrefab;

    [Tooltip("Ссылка на Content ScrollView")]
    [SerializeField] private Transform scrollViewContent;

    [Tooltip("Материалы для перекраски объектов")]
    [SerializeField] private List<Material> materials;

    private void Start()
    {
        for (int i = 0; i < materials.Count; i++) 
        {
            var dradObject = Instantiate(dragColorPrefab, scrollViewContent);
            var script = dradObject.GetComponent<DragElements>();

            script.MainMaterial = materials[i];
            script.DefaultParentTransform = scrollViewContent;
            script.DragParentTransfotm = transform;
            script.SiblingIndex = i;
        }
    }
}

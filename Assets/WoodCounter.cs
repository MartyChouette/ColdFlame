using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WoodCounter : MonoBehaviour
{
    public TMP_Text countText;
    private int woodCount = 0;
    private HashSet<GameObject> woodObjects = new HashSet<GameObject>();

    private void Start()
    {
        UpdateCount();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wood") && !woodObjects.Contains(other.gameObject))
        {
            woodObjects.Add(other.gameObject);
            woodCount++;
            UpdateCount();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wood") && woodObjects.Contains(other.gameObject))
        {
            woodObjects.Remove(other.gameObject);
            woodCount--;
            UpdateCount();
        }
    }

    private void UpdateCount()
    {
        if (countText != null)
        {
            countText.text = $"{woodCount}";
        }
    }
}

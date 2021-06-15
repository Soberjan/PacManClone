using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    TextMeshPro textMesh;

    void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    void Update()
    {
        textMesh.text = "Score: " + GameManager.score + "  Health: " + (GameManager.health+1);
    }
}

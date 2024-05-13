/*using UnityEngine;
using TMPro;

public class MoveEffect : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public float fadeSpeed = 1.0f;

    private TextMeshProUGUI textMesh; // Change to TextMeshProUGUI for UI elements

    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>(); // Get the TextMeshProUGUI component
        if (textMesh == null)
        {
            Debug.LogError("Missing TextMeshProUGUI component on " + gameObject.name);
        }
    }

    void Update()
    {
        if (textMesh != null)
        {
            transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
            var color = textMesh.color;
            color.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = color;
            if (color.a <= 0) Destroy(gameObject);
        }
    }
}*/
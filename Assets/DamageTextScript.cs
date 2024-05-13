using UnityEngine;
using TMPro;
using System.Collections;
public class DamageTextScript : MonoBehaviour
{
    public float fadeSpeed = 1f;
    public float moveSpeed = 1f;
 
private TextMeshProUGUI textMesh;  // For UI text

void Awake()
{
    
    textMesh = GetComponentInChildren<TextMeshProUGUI>();  // For UI text

    if (textMesh == null)
    {
        Debug.LogError("Missing TextMeshPro component on " + gameObject.name);
    }
}

    public void DisplayDamage(int damage)
    {
        if (textMesh == null)
        {
            Debug.LogError("TextMeshPro component not found on " + gameObject.name);
            return;
        }

        textMesh.text = damage.ToString();
        StartCoroutine(FadeOutText());
        Debug.Log("Damage text spawned at: " + textMesh.transform.position);

    }

    IEnumerator FadeOutText()
    {
        Color originalColor = textMesh.color;
        float startTime = Time.time;

        while (Time.time - startTime < 1f)
        {
            float t = (Time.time - startTime) * fadeSpeed;
            textMesh.color = Color.Lerp(originalColor, new Color(originalColor.r, originalColor.g, originalColor.b, 0), t);
            transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
            yield return null;
        }
        
        Destroy(gameObject);
    }
}
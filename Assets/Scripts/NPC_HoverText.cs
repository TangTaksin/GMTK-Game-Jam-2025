using UnityEngine;
using TMPro;
using System.Collections;

public class NPC_HoverTextCool : MonoBehaviour
{
    [Header("Text Settings")]
    public TextMeshProUGUI textToShow;
    public string message = "Pick the odd one out and put it in the hole!";
    public float typeSpeed = 0.04f;
    public float fadeSpeed = 5f;
    public float fadeOutDelay = 0.5f;

    [Header("Effects")]
    public bool glowEffect = true;
    public bool shakeEffect = true;
    public float shakeStrength = 0.05f;

    private bool isMouseOver = false;
    private bool isTyping = false;
    private Color originalColor;
    private Coroutine typeCoroutine;
    private Coroutine fadeCoroutine;
    private Vector3 originalPosition;

    private void Start()
    {
        if (textToShow == null)
        {
            Debug.LogError("TextMeshPro reference not set!");
            return;
        }

        originalColor = textToShow.color;
        textToShow.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        originalPosition = textToShow.transform.localPosition;
    }

    private void OnMouseEnter()
    {
        isMouseOver = true;

        // หยุด fade ถ้าเมาส์กลับมาเร็ว
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        if (typeCoroutine != null)
            StopCoroutine(typeCoroutine);

        typeCoroutine = StartCoroutine(TypeMessage());
    }

    private void OnMouseExit()
    {
        isMouseOver = false;

        if (typeCoroutine != null)
        {
            StopCoroutine(typeCoroutine);
            typeCoroutine = null;
        }

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOutAfterDelay());
    }

    private void Update()
    {
        // Optional glow effect
        if (glowEffect && isMouseOver)
        {
            float glow = Mathf.PingPong(Time.time * 2f, 1f);
            textToShow.outlineWidth = glow * 0.3f;
        }

        // Optional shake effect
        if (shakeEffect && isMouseOver)
        {
            Vector2 randomOffset = Random.insideUnitCircle * shakeStrength;
            textToShow.transform.localPosition = originalPosition + (Vector3)randomOffset;
        }
        else
        {
            textToShow.transform.localPosition = originalPosition;
        }
    }

    private IEnumerator TypeMessage()
    {
        isTyping = true;
        textToShow.text = "";
        Color color = textToShow.color;
        color.a = 1f;
        textToShow.color = color;

        for (int i = 0; i <= message.Length; i++)
        {
            textToShow.text = message.Substring(0, i);
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
    }

    private IEnumerator FadeOutAfterDelay()
    {
        yield return new WaitForSeconds(fadeOutDelay);

        Color currentColor = textToShow.color;
        while (currentColor.a > 0.01f)
        {
            currentColor = textToShow.color;
            float newAlpha = Mathf.Lerp(currentColor.a, 0f, Time.deltaTime * fadeSpeed);
            textToShow.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
            yield return null;

            // ถ้าเมาส์กลับมา ก็หยุด fade
            if (isMouseOver)
                yield break;
        }

        textToShow.text = "";
        textToShow.outlineWidth = 0f;
    }
}

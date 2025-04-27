using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    public float fadeInDuration = 0.5f;
    public float moveInDuration = 0.5f;
    public float scaleInDuration = 0.5f;
    public float delayBetweenElements = 0.1f;
    
    [Header("Animation Curves")]
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Move Settings")]
    public Vector2 moveStartOffset = new Vector2(100, 0);
    
    [Header("Scale Settings")]
    public Vector3 startScale = new Vector3(0.5f, 0.5f, 0.5f);
    
    [Header("Elements")]
    public RectTransform[] animatedElements;
    
    private void Start()
    {
        // Initialize all elements to be invisible
        foreach (RectTransform element in animatedElements)
        {
            if (element == null) continue;
            
            // Get CanvasGroup or add one if it doesn't exist
            CanvasGroup canvasGroup = element.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = element.gameObject.AddComponent<CanvasGroup>();
            
            canvasGroup.alpha = 0f;
        }
        
        // Start animation sequence
        StartCoroutine(AnimateElementsSequentially());
    }
    
    private IEnumerator AnimateElementsSequentially()
    {
        // Wait a frame to ensure everything is initialized
        yield return null;
        
        // Animate each element with a delay
        for (int i = 0; i < animatedElements.Length; i++)
        {
            RectTransform element = animatedElements[i];
            if (element == null) continue;
            
            StartCoroutine(AnimateElement(element, i * delayBetweenElements));
        }
    }
    
    private IEnumerator AnimateElement(RectTransform element, float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);
        
        // Get components
        CanvasGroup canvasGroup = element.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = element.gameObject.AddComponent<CanvasGroup>();
        
        // Store original position and scale
        Vector3 targetPosition = element.anchoredPosition;
        Vector3 targetScale = element.localScale;
        
        // Set starting values
        canvasGroup.alpha = 0f;
        element.anchoredPosition = targetPosition + (Vector3)moveStartOffset;
        element.localScale = startScale;
        
        // Animate
        float elapsedTime = 0f;
        float maxDuration = Mathf.Max(fadeInDuration, moveInDuration, scaleInDuration);
        
        while (elapsedTime < maxDuration)
        {
            elapsedTime += Time.deltaTime;
            
            // Animate fade
            if (elapsedTime <= fadeInDuration)
            {
                float normalizedTime = elapsedTime / fadeInDuration;
                canvasGroup.alpha = fadeCurve.Evaluate(normalizedTime);
            }
            
            // Animate position
            if (elapsedTime <= moveInDuration)
            {
                float normalizedTime = elapsedTime / moveInDuration;
                element.anchoredPosition = Vector3.Lerp(
                    targetPosition + (Vector3)moveStartOffset, 
                    targetPosition, 
                    moveCurve.Evaluate(normalizedTime)
                );
            }
            
            // Animate scale
            if (elapsedTime <= scaleInDuration)
            {
                float normalizedTime = elapsedTime / scaleInDuration;
                element.localScale = Vector3.Lerp(
                    startScale, 
                    targetScale, 
                    scaleCurve.Evaluate(normalizedTime)
                );
            }
            
            yield return null;
        }
        
        // Ensure final values are set exactly
        canvasGroup.alpha = 1f;
        element.anchoredPosition = targetPosition;
        element.localScale = targetScale;
    }
    
    public void AnimateButton(Button button)
    {
        if (button == null) return;
        StartCoroutine(AnimateButtonScale(button.transform as RectTransform));
    }
    
    private IEnumerator AnimateButtonScale(RectTransform buttonRect)
    {
        if (buttonRect == null) yield break;
        
        Vector3 originalScale = buttonRect.localScale;
        Vector3 targetScale = originalScale * 1.1f;
        
        float duration = 0.1f;
        float elapsedTime = 0f;
        
        // Scale up
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / duration;
            buttonRect.localScale = Vector3.Lerp(originalScale, targetScale, normalizedTime);
            yield return null;
        }
        
        elapsedTime = 0f;
        
        // Scale back down
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / duration;
            buttonRect.localScale = Vector3.Lerp(targetScale, originalScale, normalizedTime);
            yield return null;
        }
        
        // Ensure final scale is set exactly
        buttonRect.localScale = originalScale;
    }
}

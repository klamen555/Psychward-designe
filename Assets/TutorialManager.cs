using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep
    {
        [TextArea(2, 4)]
        public string instruction;

        public Sprite characterImage;
    }

    [Header("UI References")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image characterImage;
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Tutorial Steps")]
    [SerializeField] private TutorialStep[] steps;

    [Header("Character Animation")]
    [SerializeField] private float bobHeight = 8f;
    [SerializeField] private float bobSpeed = 2f;

    [Header("Exit Animation")]
    [SerializeField] private float slideDuration = 0.6f;

    private int currentStep;
    private Vector2 panelStartPosition;
    private Vector2 characterStartPosition;
    private bool isActive;

    private void Awake()
    {
        panelStartPosition = panel.anchoredPosition;
        characterStartPosition = characterImage.rectTransform.anchoredPosition;
    }

    private void Start()
    {
        if (steps == null || steps.Length == 0)
        {
            panel.gameObject.SetActive(false);
            return;
        }

        currentStep = 0;
        isActive = true;

        ShowStep();
    }

    private void Update()
    {
        if (!isActive)
            return;

        // Pohyb postavy nahoru a dolù.
        float offset = Mathf.Sin(Time.unscaledTime * bobSpeed) * bobHeight;

        characterImage.rectTransform.anchoredPosition =
            characterStartPosition + Vector2.up * offset;
    }

    private void ShowStep()
    {
        TutorialStep step = steps[currentStep];

        dialogueText.text = step.instruction;

        if (characterName != null)
            characterName.text = "Warden";

        if (characterImage != null)
        {
            characterImage.sprite = step.characterImage;
            characterImage.enabled = step.characterImage != null;
        }
    }

    public void NextStep()
    {
        if (!isActive)
            return;

        currentStep++;

        if (currentStep >= steps.Length)
        {
            StartCoroutine(SlideAway());
            return;
        }

        ShowStep();
    }

    public void SkipTutorial()
    {
        if (!isActive)
            return;

        StartCoroutine(SlideAway());
    }

    private IEnumerator SlideAway()
    {
        isActive = false;

        Vector2 start = panel.anchoredPosition;
        Vector2 target = start + Vector2.down * (panel.rect.height + 100f);

        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / slideDuration);
            float smoothT = t * t * (3f - 2f * t);

            panel.anchoredPosition = Vector2.Lerp(start, target, smoothT);
            canvasGroup.alpha = 1f - smoothT;

            yield return null;
        }

        panel.gameObject.SetActive(false);
    }

    //pak to nekdo odentejte
    private void LateUpdate()
    {
        if (isActive && Input.GetKeyDown(KeyCode.N))
        {
            NextStep();
        }
    }
}
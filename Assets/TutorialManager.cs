using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    public enum ObjectiveType
    {
        None,
        BuildBuildings
    }

    [System.Serializable]
    public class TutorialStep
    {
        [Header("Text")]
        [TextArea(2, 4)]
        public string instruction;

        public Sprite characterImage;

        [Header("Úkol")]
        public ObjectiveType objective = ObjectiveType.None;

        [Tooltip("Prefab budovy, kterou musí hráè postavit.")]
        public GameObject targetBuildingPrefab;

        [Min(1)]
        public int requiredCount = 1;
    }

    [Header("UI References")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image characterImage;
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text progressText;

    [Header("Tutorial Steps")]
    [SerializeField] private TutorialStep[] steps;

    [Header("Animace postavy")]
    [SerializeField] private float bobHeight = 8f;
    [SerializeField] private float bobSpeed = 2f;

    [Header("Animace odchodu")]
    [SerializeField] private float slideDuration = 0.6f;

    private int currentStep;
    private int currentProgress;

    private Vector2 characterStartPosition;
    private bool isActive;
    private bool isTransitioning;

    private void Awake()
    {
        Instance = this;

        if (panel != null)
            canvasGroup = canvasGroup != null
                ? canvasGroup
                : panel.GetComponent<CanvasGroup>();

        if (characterImage != null)
            characterStartPosition =
                characterImage.rectTransform.anchoredPosition;
    }

    private void Start()
    {
        if (steps == null || steps.Length == 0)
        {
            if (panel != null)
                panel.gameObject.SetActive(false);

            return;
        }

        currentStep = 0;
        currentProgress = 0;
        isActive = true;

        ShowStep();
    }

    private void Update()
    {
        if (!isActive || characterImage == null)
            return;

        float offset =
            Mathf.Sin(Time.unscaledTime * bobSpeed) * bobHeight;

        characterImage.rectTransform.anchoredPosition =
            characterStartPosition + Vector2.up * offset;
    }

    private void ShowStep()
    {
        if (currentStep >= steps.Length)
        {
            StartCoroutine(SlideAway());
            return;
        }

        TutorialStep step = steps[currentStep];

        currentProgress = 0;

        if (dialogueText != null)
            dialogueText.text = step.instruction;

        if (characterName != null)
            characterName.text = "Prùvodce";

        if (characterImage != null)
        {
            characterImage.sprite = step.characterImage;
            characterImage.enabled = step.characterImage != null;
        }

        UpdateProgress();

        Debug.Log("Tutorial: " + step.instruction);
    }

    // Zavolá BuildingSystem pøi úspìšném postavení budovy.
    public void NotifyBuildingPlaced(GameObject placedPrefab)
    {
        if (!isActive || isTransitioning)
            return;

        if (currentStep >= steps.Length)
            return;

        TutorialStep step = steps[currentStep];

        // Tento krok nevyžaduje stavìní.
        if (step.objective != ObjectiveType.BuildBuildings)
            return;

        // Poèítáme pouze požadovaný typ budovy.
        if (step.targetBuildingPrefab != placedPrefab)
            return;

        currentProgress++;

        UpdateProgress();

        Debug.Log(
            "Tutorial progress: " +
            currentProgress + "/" + step.requiredCount
        );

        if (currentProgress >= step.requiredCount)
        {
            StartCoroutine(AdvanceToNextStep());
        }
    }

    private void UpdateProgress()
    {
        if (progressText == null)
            return;

        if (currentStep >= steps.Length)
        {
            progressText.text = "";
            return;
        }

        TutorialStep step = steps[currentStep];

        if (step.objective == ObjectiveType.BuildBuildings)
        {
            progressText.text =
                currentProgress + "/" + step.requiredCount;
        }
        else
        {
            progressText.text = "";
        }
    }

    private IEnumerator AdvanceToNextStep()
    {
        if (isTransitioning)
            yield break;

        isTransitioning = true;

        // Krátká prodleva, aby hráè vidìl dokonèené poèítadlo.
        yield return new WaitForSecondsRealtime(0.2f);

        currentStep++;

        if (currentStep >= steps.Length)
        {
            yield return SlideAway();
        }
        else
        {
            ShowStep();
        }

        isTransitioning = false;
    }

    public void NextStep()
    {
        if (!isActive || isTransitioning)
            return;

        StartCoroutine(AdvanceToNextStep());
    }

    public void SkipTutorial()
    {
        if (!isActive || isTransitioning)
            return;

        StartCoroutine(SlideAway());
    }

    private IEnumerator SlideAway()
    {
        isActive = false;

        if (panel == null)
            yield break;

        Vector2 start = panel.anchoredPosition;

        Vector2 target =
            start + Vector2.down * (panel.rect.height + 100f);

        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / slideDuration);
            float smoothT = t * t * (3f - 2f * t);

            panel.anchoredPosition =
                Vector2.Lerp(start, target, smoothT);

            if (canvasGroup != null)
                canvasGroup.alpha = 1f - smoothT;

            yield return null;
        }

        panel.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
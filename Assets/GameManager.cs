using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public VideoPlayer mainVideoPlayer, miniVideoPlayer;
    public VideoClip videoClip0;
    public VideoClip videoClip1;
    public VideoClip videoClip2;
    public VideoClip videoClip3;
    public VideoClip videoClip4;
    public VideoClip videoClip5;
    public GameObject miniVideoPlayerPrefab;
    private bool videoHasSwitchedToFlagVideo = false;
    private bool isBeforeClip3 = false;

    private float videoPlaybackSpeed = 1.0f;
    private float speedDecayRate = 0.1f;
    private float speedIncreaseAmount = 0.5f;
    private float minSpeed = 1.0f;
    private float maxSpeed = 5.0f;

    private GameObject canvas;
    private Transform mouseTransform;

    private ulong grass = 0;

    private int playerStrengthLevel = 1;
    private float playerMultiplicationLevel = 1;
    private float playerExponentLevel = 1;

    private int autoClickNumberLevel = 1;
    private int autoClickStrengthLevel = 1;
    private float autoClickMultiplicationLevel = 1;
    private float autoClickExponentLevel = 1;

    private List<Func<float>> levelGetters = new List<Func<float>>();

    private int playerStrengthPrice = 1;
    private float playerMultiplicationPrice = 5;
    private float playerExponentPrice = 10;

    private int autoClickNumberPrice = 20;
    private int autoClickStrengthPrice = 1;
    private float autoClickMultiplicationPrice = 5;
    private float autoClickExponentPrice = 10;

    private List<Func<double>> realPriceGetters = new List<Func<double>>();

    private bool playing = false;

    private TextMeshProUGUI grassTextMeshPro;

    List<TextMeshProUGUI> LevelText = new List<TextMeshProUGUI>();
    List<TextMeshProUGUI> priceText = new List<TextMeshProUGUI>();
    List<Button> addLevelButton = new List<Button>();

    private void Awake() {
        canvas = GameObject.Find("Canvas");

        levelGetters.Add(() => playerStrengthLevel);
        levelGetters.Add(() => 1 + (playerMultiplicationLevel - 1) * 0.1f);
        levelGetters.Add(() => 1 + (playerExponentLevel - 1) * 0.01f);
        levelGetters.Add(() => autoClickNumberLevel - 1);
        levelGetters.Add(() => autoClickStrengthLevel);
        levelGetters.Add(() => 1 + (autoClickMultiplicationLevel - 1) * 0.1f);
        levelGetters.Add(() => 1 + (autoClickExponentLevel - 1) * 0.01f);

        realPriceGetters.Add(() => Math.Pow(playerStrengthPrice * playerStrengthLevel, 2f));
        realPriceGetters.Add(() => Math.Pow(playerMultiplicationPrice * playerMultiplicationLevel, 2f));
        realPriceGetters.Add(() => Math.Pow(playerExponentPrice * playerExponentLevel, 2f));
        realPriceGetters.Add(() => Math.Pow(autoClickNumberPrice * autoClickNumberLevel, 2f));
        realPriceGetters.Add(() => Math.Pow(autoClickStrengthPrice * autoClickStrengthLevel, 2f));
        realPriceGetters.Add(() => Math.Pow(autoClickMultiplicationPrice * autoClickMultiplicationLevel, 2f));
        realPriceGetters.Add(() => Math.Pow(autoClickExponentPrice * autoClickExponentLevel, 2f));
    }

    void Start()
    {
        mainVideoPlayer.clip = videoClip0;
        mainVideoPlayer.Play();

        StartCoroutine(DecreaseVideoSpeed());
    }

    private IEnumerator DecreaseVideoSpeed() {
        while (true) {
            yield return new WaitForSeconds(0.05f);

            videoPlaybackSpeed = Mathf.Max(videoPlaybackSpeed - speedDecayRate, minSpeed);
        }
    }

    void Update()
    {
        if (!playing) {
            if (grass < 924555555555555)
                return;
            if (mainVideoPlayer.isPlaying)
                return;
            if (videoHasSwitchedToFlagVideo)
                return;

            videoHasSwitchedToFlagVideo = true;
            mainVideoPlayer.clip = videoClip5;
            mainVideoPlayer.Play();

            return;
        }
        grassTextMeshPro.text = grass.ToString();

        for (int i = 0; i < LevelText.Count && i < levelGetters.Count; i++)
            LevelText[i].text = levelGetters[i]().ToString();

        for (int i = 0; i < priceText.Count && i < realPriceGetters.Count; i++)
            priceText[i].text = realPriceGetters[i]().ToString();

        for (int i = 0; i < addLevelButton.Count && i < realPriceGetters.Count; i++) {
            var interactable = addLevelButton[i].interactable;
            if (grass < realPriceGetters[i]())
                interactable = false;
            else
                interactable = true;
            addLevelButton[i].interactable = interactable;
        }

        if (!videoHasSwitchedToFlagVideo) {
            mainVideoPlayer.playbackSpeed = videoPlaybackSpeed;
        }

        if (grass < 924555555555555)
            return;
        if (mainVideoPlayer.isPlaying)
            return;
        if (videoHasSwitchedToFlagVideo)
            return;

        videoHasSwitchedToFlagVideo = true;
        mainVideoPlayer.clip = videoClip4;
        mainVideoPlayer.Play();
    }

    void OnVideoEndClip4(VideoPlayer vp) {
        mainVideoPlayer.clip = videoClip4;
        mainVideoPlayer.Play();
        mainVideoPlayer.loopPointReached -= OnVideoEndClip4;
    }

    void OnVideoEndClip5(VideoPlayer vp) {
        mainVideoPlayer.clip = videoClip5;
        mainVideoPlayer.Play();
        mainVideoPlayer.loopPointReached -= OnVideoEndClip5;
    }

    void RepeatClip2OrClip3(VideoPlayer vp) {
        if(videoPlaybackSpeed > 1) {
            mainVideoPlayer.clip = videoClip2;
            mainVideoPlayer.Play();
        }
        else {
            mainVideoPlayer.clip = videoClip3;
            mainVideoPlayer.Play();
            isBeforeClip3 = false;
            mainVideoPlayer.loopPointReached -= RepeatClip2OrClip3;

            if (grass < 924555555555555)
                return;

            videoHasSwitchedToFlagVideo = true;

            if (playing)
                mainVideoPlayer.loopPointReached += OnVideoEndClip4;
            else
                mainVideoPlayer.loopPointReached += OnVideoEndClip5;
        }
    }


    public void GameStart() {
        foreach (Transform child in canvas.transform) {
            child.gameObject.SetActive(true);
            if (child.gameObject.name.Contains("Level"))
                LevelText.Add(child.gameObject.GetComponent<TextMeshProUGUI>());
            else if(child.gameObject.name.Contains("Price"))
                priceText.Add(child.gameObject.GetComponent<TextMeshProUGUI>());
            else if (child.gameObject.name.Contains("Add"))
                addLevelButton.Add(child.gameObject.GetComponent<Button>());
        }

        canvas.transform.Find("GameStartButton").gameObject.SetActive(false);

        mouseTransform = canvas.transform.Find("Mouse").gameObject.transform;

        grassTextMeshPro = GameObject.Find("TouchedGrass").GetComponent<TextMeshProUGUI>();

        playing = true;

        InvokeRepeating(nameof(AutoClick), 1f, 1f);
        InvokeRepeating(nameof(MoveMouse), 1f, 1f);
    }

    bool mouseUp = true;

    private void MoveMouse() {
        if (mouseUp) {
            mouseTransform.position += new Vector3(-0.1f, 0.1f, 0);
            mouseUp = false;
        }
        else {
            mouseTransform.position -= new Vector3(-0.1f, 0.1f, 0);
            mouseUp = true;
        }
        
    }

    public void TouchGrass() {
        if (!playing) return;

        if (!isBeforeClip3 && !mainVideoPlayer.isPlaying && !videoHasSwitchedToFlagVideo) {
            mainVideoPlayer.clip = videoClip1;
            mainVideoPlayer.Play();
            mainVideoPlayer.loopPointReached += RepeatClip2OrClip3;
            isBeforeClip3 = true;
        }

        videoPlaybackSpeed = Mathf.Min(videoPlaybackSpeed + speedIncreaseAmount, maxSpeed);

        grass += (ulong)Math.Pow(playerStrengthLevel * (1 + (playerMultiplicationLevel - 1) * 0.1), (1 + (playerExponentLevel - 1) * 0.01));
    }

    public void AutoClick() {
        grass += (ulong)Math.Pow((autoClickNumberLevel - 1) * autoClickStrengthLevel * (1 + (autoClickMultiplicationLevel - 1) * 0.1), (1 + (autoClickExponentLevel - 1) * 0.01));
    }

    public void AddPlayerStrength() {
        grass -= (ulong)Math.Pow(playerStrengthPrice * playerStrengthLevel, 2f);
        playerStrengthLevel++;
    }

    public void AddPlayerMultiplication() {
        grass -= (ulong)Math.Pow(playerMultiplicationPrice * playerMultiplicationLevel, 2f);
        playerMultiplicationLevel++;
    }

    public void AddPlayerExponent() {
        grass -= (ulong)Math.Pow(playerExponentPrice * playerExponentLevel, 2f);
        playerExponentLevel++;
    }
    public void AddAutoClickNumber() {
        grass -= (ulong)Math.Pow(autoClickNumberPrice * autoClickNumberLevel, 2f);
        autoClickNumberLevel++;

        GameObject instance = Instantiate(miniVideoPlayerPrefab, canvas.transform);

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        RectTransform instanceRect = instance.GetComponent<RectTransform>();

        if (canvasRect != null && instanceRect != null) {
            float canvasWidth = canvasRect.rect.width;
            float canvasHeight = canvasRect.rect.height;

            float randomX = UnityEngine.Random.Range(-canvasWidth / 2f, canvasWidth / 2f);
            float randomY = UnityEngine.Random.Range(-canvasHeight / 2f, canvasHeight / 2f);

            instanceRect.anchoredPosition = new Vector2(randomX, randomY);

            float randomAngle = UnityEngine.Random.Range(0f, 360f);
            instanceRect.localRotation = Quaternion.Euler(0f, 0f, randomAngle);
        }

        RawImage rawImage = instance.GetComponentInChildren<RawImage>();
        if (rawImage != null && miniVideoPlayer != null) {
            rawImage.texture = miniVideoPlayer.targetTexture;
        }
    }

    public void AddAutoClickStrength() {
        grass -= (ulong)Math.Pow(autoClickStrengthPrice * autoClickStrengthLevel, 2f);
        autoClickStrengthLevel++;
    }

    public void AddAutoClickMultiplication() {
        grass -= (ulong)Math.Pow(autoClickMultiplicationPrice * autoClickMultiplicationLevel, 2f);
        autoClickMultiplicationLevel++;
    }

    public void AddAutoClickExponent() {
        grass -= (ulong)Math.Pow(autoClickExponentPrice * autoClickExponentLevel, 2f);
        autoClickExponentLevel++;
    }
}

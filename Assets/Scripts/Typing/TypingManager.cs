using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using System.Linq;
using UnityEngine.UI;
using System.IO;

public class TypingManager : MonoBehaviour
{
    [Header("Camera")]
    public CinemachineVirtualCamera virtualCamera; 
    public float zoomedInFOV = 30f; 
    public float normalFOV = 60f;   
    public float zoomSpeed = 2f;    

    [Header("Wordlist")]
    public TextAsset jsonFile;
    public WordList wordList;
    public GameObject display;
    public GameObject charPrefab;
    public bool isInteracting = false;

    public bool isZooming = false;
    private List<GameObject> charObjectList = new List<GameObject>();
    private int currentCharIndex = 0;
    private string targetWord;

    InteractionManager interactionManager;
    PauseManager pauseManager;

    [Header("Scoring")]
    public ScoreManager scoreManager;
    private float timeToAdd = 5;

    void Start()
    {
        interactionManager = FindObjectOfType<InteractionManager>();
        pauseManager = FindObjectOfType<PauseManager>();

        string filePath = Application.persistentDataPath + "/wordlist.json";
        if (File.Exists(filePath))
        {
            string savedJson = File.ReadAllText(filePath);
            wordList = JsonUtility.FromJson<WordList>(savedJson);
            Debug.Log("Loaded saved word list from: " + filePath);
        }
        else
        {
            wordList = JsonUtility.FromJson<WordList>(jsonFile.text);
            Debug.Log("Loaded default word list from TextAsset");
        }
    }

    void Update()
    {
        if (interactionManager != null)
        {
            if (targetWord != null && currentCharIndex >= targetWord.Length)
            {
                interactionManager.EndInteraction();
                DestroyChild();
                targetWord = null;
            }
            else if (isInteracting)
            {
                DetectTyping();
            }
        }
    }

    void DisplayPerChar()
    {
        foreach (char c in targetWord)
        {
            GameObject charGO = Instantiate(charPrefab, display.transform);
            TextMeshProUGUI charText = charGO.GetComponentInChildren<TextMeshProUGUI>();
            charText.text = c.ToString();

            charObjectList.Add(charGO);
        }
    }

    void DetectTyping()
    {
        foreach (var charObject in charObjectList)
        {
            charObject.transform.localScale = Vector3.one;
        }

        charObjectList[currentCharIndex].transform.localScale = Vector3.one * 1.35f;

        foreach (char c in Input.inputString)
        {
            Debug.Log(c);
            char targetChar = targetWord[currentCharIndex];
            Image image = charObjectList[currentCharIndex].GetComponent<Image>();

            if (char.ToLower(c) == char.ToLower(targetChar))
            {
                image.color = Color.green;
                currentCharIndex++;
                SfxAudioClip.Instance.PlayAudioAtIndex(SfxAudioClip.AudioCategory.Typing, 0);
            }
            else
            {
                image.color = Color.red;
                SfxAudioClip.Instance.PlayAudioAtIndex(SfxAudioClip.AudioCategory.Typing, 1);
            }
        }
    }

    public void getRandomWord()
    {
        List<Word> filteredWords = GetFilteredWords();
        if (filteredWords.Count > 0 && targetWord == null)
        {
            int randomIndex = Random.Range(0, filteredWords.Count);
            Word selectedWord = filteredWords[randomIndex];
            targetWord = selectedWord.word;

            selectedWord.unlocked = true;

            SaveWordListToJson();

            DisplayPerChar();
        }
    }

    private List<Word> GetFilteredWords()
    {
        if (pauseManager.minutes < 2)
        {
            return wordList.words.Where(word => word.char_count == 5).ToList();
        }
        else if (pauseManager.minutes >= 2 && pauseManager.minutes < 5)
        {
            return wordList.words.Where(word => word.char_count == 6).ToList();
        }
        else
        {
            return wordList.words.Where(word => word.char_count >= 7).ToList();
        }
    }

    public void DestroyChild()
    {
        foreach (Transform child in display.transform)
        {
            Destroy(child.gameObject);
        }
        charObjectList.Clear();
        currentCharIndex = 0;

        scoreManager.AddScore(50);
        if (!(scoreManager.remainingTime > scoreManager.startTime))
        {
            if (scoreManager.remainingTime + timeToAdd > scoreManager.startTime)
            {
                scoreManager.AddTime(scoreManager.startTime - scoreManager.remainingTime);
            }
            else
            {
                scoreManager.AddTime(timeToAdd);
            }
        }
    }

    public void SaveWordListToJson()
    {
        string updatedJson = JsonUtility.ToJson(wordList, true);
        string filePath = Application.persistentDataPath + "/wordlist.json";
        File.WriteAllText(filePath, updatedJson);
        Debug.Log("Word list saved to: " + filePath);
    }

    public IEnumerator ZoomInCamera()
    {
        isZooming = true;

        float initialFOV = virtualCamera.m_Lens.FieldOfView;
        float elapsedTime = 0f;

        while (elapsedTime < zoomSpeed)
        {
            virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(initialFOV, zoomedInFOV, elapsedTime / zoomSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        virtualCamera.m_Lens.FieldOfView = zoomedInFOV;
    }

    public IEnumerator ResetCameraZoom()
    {
        float initialFOV = virtualCamera.m_Lens.FieldOfView;
        float elapsedTime = 0f;

        while (elapsedTime < zoomSpeed)
        {
            virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(initialFOV, normalFOV, elapsedTime / zoomSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        virtualCamera.m_Lens.FieldOfView = normalFOV;
        isZooming = false;
    }
}

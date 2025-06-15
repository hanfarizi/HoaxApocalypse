using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using Unity.Collections.LowLevel.Unsafe;
using System.Linq;


public class Dictionary : MonoBehaviour
{
    [Header("Word")]
    public TextAsset jsonFile;
    public WordList wordList;

    [Header("UI Elements")]
    public TMP_Text wordTextPage1; // Untuk menampilkan kata
    public TMP_Text explanationTextPage1; // Untuk menampilkan penjelasan
    public TMP_Text wordTextPage2; // Untuk menampilkan kata
    public TMP_Text explanationTextPage2; // Untuk menampilkan penjelasan
    public GameObject page1; // Panel untuk halaman 1
    public GameObject page2; // Panel untuk halaman 2
    public GameObject cover; // Panel untuk halaman 2
    public GameObject backCover; // Panel untuk halaman 2

    public Button navBtn;

    int currentPageIndex = 0;

    public EventSystem eventSystem;

    // Start is called before the first frame update

    private void Awake()
    {
        string persistentPath = Application.persistentDataPath;
        Debug.Log("Persistent Data Path: " + persistentPath);

        if (string.IsNullOrWhiteSpace(persistentPath))
        {
            Debug.LogError("Application.persistentDataPath is invalid. Please verify your Company Name and Product Name in Project Settings.");
            return;
        }

        string filePath = Path.Combine(persistentPath, "wordlist.json");

        try
        {
            if (File.Exists(filePath))
            {
                string savedJson = File.ReadAllText(filePath);
                wordList = JsonUtility.FromJson<WordList>(savedJson);
                Debug.Log("Loaded saved word list from: " + filePath);
            }
            else
            {
                // Ensure the directory exists
                string directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                    Debug.Log("Created directory: " + directoryPath);
                }

                wordList = JsonUtility.FromJson<WordList>(jsonFile.text);
                File.WriteAllText(filePath, jsonFile.text);
                Debug.Log("Loaded default word list and saved to: " + filePath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error accessing or creating file: " + e.Message);
        }
    }

    void Start()
    {
        
        //UpdateNavigationButtons();
    }

    public void openBook()
    {
        currentPageIndex = -1;
        UpdatePage();

        int unlockedWords = 0;
        foreach (Word word in wordList.words)
        {
            Debug.Log(word.word);
            if (word.unlocked)
            {
                unlockedWords++;
            }
        }

        Debug.Log($"Unlocked Words: {unlockedWords}");
        SortWordsByUnlockedStatus();
    }
    void Update()
    {
        GameObject currentSelected = eventSystem.currentSelectedGameObject;
        if (currentSelected == navBtn.gameObject)
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            PrevPage();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            NextPage();
        }
    }

    public void resetData()
    {
        foreach (Word word in wordList.words)
        {
            word.unlocked = false;
        }

        SaveWordListToJson();
        Debug.Log("All words have been reset and saved.");
    }
    public void UnlockData()
    {
        foreach (Word word in wordList.words)
        {
            word.unlocked = true;
        }

        SaveWordListToJson();
        Debug.Log("All words have been reset and saved.");
    }

    void UpdatePage()
    {
        if(currentPageIndex < 0)
        {
            cover.SetActive(true);
            backCover.SetActive(false);
            page1.SetActive(false);
            page2.SetActive(false);
        }
        else if (currentPageIndex < wordList.words.Count/2)
        {
            cover.SetActive(false);
            backCover.SetActive(false);
            page1.SetActive(true);
            page2.SetActive(true);

            Word currentWord1 = wordList.words[currentPageIndex*2];
            Word currentWord2 = wordList.words[currentPageIndex*2+1];

            string word1 = currentWord1.unlocked ? currentWord1.word : "???";
            string explanation1 = currentWord1.unlocked ? currentWord1.explanation : "?????????????????????????????????????????????????????";
            string word2 = currentWord2.unlocked ? currentWord2.word : "???";
            string explanation2 = currentWord2.unlocked ? currentWord2.explanation : "?????????????????????????????????????????????????????";

            Debug.Log(word1);
            Debug.Log(explanation1);
            Debug.Log(word2);
            Debug.Log(explanation2);

            wordTextPage2.text = word1;
            explanationTextPage2.text = explanation1;

            wordTextPage1.text = word2;
            explanationTextPage1.text = explanation2;

        }
        else if(currentPageIndex > wordList.words.Count/2)
        {
            cover.SetActive(true);
            backCover.SetActive(true);
            page1.SetActive(false);
            page2.SetActive(false);
        }
    }
    void NextPage()
    {
        if (currentPageIndex < wordList.words.Count / 2)
        {
            currentPageIndex++;
            UpdatePage();
        }
    }

    void PrevPage()
    {
        if (currentPageIndex > -1)
        {
            currentPageIndex--;
            UpdatePage();
            //UpdateNavigationButtons();
        }
    }
    private void SaveWordListToJson()
    {
        try
        {
            string updatedJson = JsonUtility.ToJson(wordList, true);
            string filePath = Path.Combine(Application.persistentDataPath, "wordlist.json");
            File.WriteAllText(filePath, updatedJson);
            Debug.Log("Word list saved to: " + filePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error saving word list: " + e.Message);
        }
    }

    private void SortWordsByUnlockedStatus()
    {
        wordList.words = wordList.words.OrderByDescending(word => word.unlocked).ToList();
        SaveWordListToJson(); // Simpan urutan baru ke file JSON
        Debug.Log("Words sorted by unlocked status.");
    }

}

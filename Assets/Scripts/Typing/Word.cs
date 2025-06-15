using System.Collections.Generic;

[System.Serializable]
public class Word
{
    public int id;
    public string word;
    public string explanation;
    public int char_count;
    public bool unlocked;

}

[System.Serializable]
public class WordList
{
    public List<Word> words;
}

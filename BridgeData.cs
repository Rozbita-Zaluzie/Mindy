using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BridgeData : MonoBehaviour
{
    public int currentLevel;

    public LevelData[] levels;

    public int[] GameData;

    public int difficulty;

    private TextAsset GameDataText;

    public string GameDataPath;

    // 0 - done
    // 1 - to play
    // 2 - locked

    private void Start()
    {
        difficulty = 1;
        string text = "1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2|" + difficulty.ToString();

        // generating GameData.txt
        if (!Application.isMobilePlatform)
        {
            if (File.Exists(Application.dataPath + "/GameData.txt"))
            {
                GameDataPath = Application.dataPath + "/GameData.txt";
            } else
            {
                print("not found");
                File.WriteAllText(Application.dataPath + "/GameData.txt", text);
                GameDataPath = Application.dataPath + "/GameData.txt";
            }
        } else
        {
            if (File.Exists(Application.persistentDataPath + "/GameData.txt"))
            {
                GameDataPath = Application.persistentDataPath + "/GameData.txt";
            } else
            {
                File.WriteAllText(Application.persistentDataPath + "/GameData.txt", text);
                GameDataPath = Application.persistentDataPath + "/GameData.txt";
            }
        }

        // create _GameData_ array from GameData.txt
        string[] txt = File.ReadAllText(GameDataPath).Split('|');
        string[] txt2 = txt[0].Split(',');

        GameData = new int[txt2.Length];

        for (int i = 0; i < txt.Length; i++)
        {
            GameData[i] = int.Parse(txt2[i]);
        }

        SetLevelDataArray();
    }

    public void SetLevelDataArray()
    {
        // create _levels_ array and fill by data from buttons
        GameObject[] levelButtons = GameObject.FindGameObjectsWithTag("Level");

        levels = new LevelData[levelButtons.Length];

        for (int i = 0; i < levelButtons.Length; i++)
        {
            LevelButtonData buttonData = levelButtons[i].GetComponent<LevelButtonData>();
            LevelData newLD = new LevelData(buttonData.level, buttonData.radky, buttonData.sloupce, buttonData.colors);
            levels[i] = newLD;
        }
    }

    // get level data !! starting with 1 !!
    public LevelData GetLevelData(int level)
    {
        return levels[level--];
    }

    public bool IsDefault()
    {
        string text = File.ReadAllText(GameDataPath);
        if (text.Contains("1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ResetData()
    {
        File.WriteAllText(GameDataPath, "1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2|" + difficulty.ToString());
    }

    public void addLevelDone(int playedLevel)
    {
        string[] txt = File.ReadAllText(GameDataPath).Split('|');
        string[] txt2 = txt[0].Split(',');
        int[] numbers = new int[txt2.Length];
        for (int i = 0; i < txt2.Length; i++) {
            numbers[i] = int.Parse(txt2[i]);}
        int playedLevelIndex = playedLevel;

        if (numbers[playedLevelIndex] != 0)
        {
            numbers[playedLevelIndex] = 0;
            if (playedLevel < numbers.Length - 1)
            {
                numbers[playedLevelIndex + 1] = 1;
            }
            if (playedLevel < numbers.Length)
            {
                numbers[playedLevelIndex + 2] = 2;
            }

            if (playedLevelIndex > 0)
            {
                numbers[playedLevelIndex - 1] = 0;
            }

            string rewrite = "";
            for (int i = 1; i < numbers.Length; i++)
            {
                rewrite += numbers[i];
                if (i < numbers.Length - 1) {
                    rewrite += ",";}
            }

            Debug.Log(numbers[0]);
            //string path = Application.persistentDataPath + "/Resources/GameData.txt";
            File.WriteAllText(GameDataPath, rewrite + "|" + difficulty.ToString());
        }        
    }

    public int GetMistakes()
    {
        if (difficulty == 0) {
            return 3; }
        else if (difficulty == 1) {
            return 1; }
        else {
            return 0; }
    }

    public int[] GetGameData()
    {
        string[] txt = File.ReadAllText(GameDataPath).Split('|');
        string[] txt2 = txt[0].Split(',');
        int[] numbers = new int[txt2.Length];

        for (int i = 0; i < txt2.Length; i++)
        {
            numbers[i] = int.Parse(txt2[i]);
        }
        return numbers;
    }

    public void SetDifficulty(int diff)
    {
        difficulty = diff;

        string[] text = File.ReadAllText(GameDataPath).Split('|');

        File.WriteAllText(GameDataPath, text[0] + "|" + diff.ToString());
    }
    public int GetDifficulty()
    {
        string txt = File.ReadAllText(GameDataPath);
        string[] txt2 = txt.Split('|');

        return int.Parse(txt2[1]);
    }
}

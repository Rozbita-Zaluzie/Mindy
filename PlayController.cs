using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayController : MonoBehaviour
{
    public int level;
    public int radky;
    public int sloupce;
    public int colors;

    public int currentDifficulty;
    public int mistakesCount;

    public bool isCounting;

    public int[,] gridInt;
    public GameObject[,] gridObject;

    public GameObject Button;

    public GameObject Bridge;
    public BridgeData BridgeData;

    public GameObject TapOnScreen;
    public Animator TapOnScreenAnim;

    public GameObject failScreen;
    public GameObject winScreen;

    public Text RestTilesText;
    public Text MistakesText;

    public GameObject RemainPanel;
    public GameObject MistakesTimePanel;

    public GameObject PlayUI;

    public Text LevelNum;
    public Text InfoText;
    public Text RemainingTime;

    public GameObject Holder;

    // START / GENERATE / SETDATA
    void Start()
    {
        isCounting = false;

        Bridge = GameObject.FindGameObjectsWithTag("GameController")[0];
        BridgeData = Bridge.GetComponent<BridgeData>();

        TapOnScreen.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);

        this.level = BridgeData.currentLevel;
        SetData(level);

        SetTimeCountDown();

        Generate();
    }

    public void SetData(int level)
    {
        int levelForSearch = level - 1;
        this.radky = BridgeData.levels[levelForSearch].radky;
        this.sloupce = BridgeData.levels[levelForSearch].sloupce;
        this.colors = BridgeData.levels[levelForSearch].colors;

        currentDifficulty = BridgeData.difficulty;
        mistakesCount = 0;
    }

    public void Generate()
    {
        // 0 - blank button
        // 1 - pressed color button
        // 2 - color button

        gridInt = new int[radky, sloupce];
        gridObject = new GameObject[radky, sloupce];
        List<int> colorsArr = new List<int>();

        // create array of random numbers (for colors)
        for (int i = 0; i < colors; i++)
        {
            while (true)
            {
                int rand = Random.Range(0, radky * sloupce);
                if (!colorsArr.Contains(rand))
                {
                    colorsArr.Add(rand);
                    break;
                }
            }
        }

        foreach (int item in colorsArr)
        {
            Debug.Log(item);
        }

        // generate bool array (with colors)
        int x = 0;
        for (int r = 0; r < radky; r++) {
            for (int s = 0; s < sloupce; s++)
            {
                if (colorsArr.Contains(x)) {
                    gridInt[r, s] = 2;
                } else {
                    gridInt[r, s] = 0;
                }
                x++;
            }
        }

        // calculations for size
        float screenWidth = 1080;
        float screenHeight = 1980 * 0.9f;

        float cellWidth = screenWidth / ((radky + 1) * 4 + (radky - 1));
        float cornerSpace = 2 * cellWidth;
        float boxSize = 4 * cellWidth;
        float space = cellWidth;

        float cornerWidthSpace = (screenWidth - (boxSize * radky) - (space * (radky - 1))) / 2;
        float cornerHeightSpace = (screenHeight - (boxSize * sloupce) - (space * (sloupce - 1))) / 2;

        Debug.Log(cellWidth);

        // genenrate GameObject array
        for (int r = 0; r < radky; r++) {
            for (int s = 0; s < sloupce; s++)
            {
                GameObject buttonInstant = Instantiate(Button, Holder.transform);
                RectTransform buttonInstantRectTr = buttonInstant.GetComponent<RectTransform>();

                buttonInstantRectTr.sizeDelta = new Vector2(boxSize, boxSize);

                if (r == 0 && s == 0)
                {
                    // left - bottom corner
                    buttonInstant.transform.localPosition = new Vector3(
                        cornerWidthSpace,
                        cornerHeightSpace, 0);
                }
                else if (r == 0 && s != 0)
                {
                    // left side
                    buttonInstant.transform.localPosition = new Vector3(
                        cornerWidthSpace,
                        cornerHeightSpace + (boxSize * s) + (space * s), 0);
                }
                else if (r != 0 && s == 0)
                {
                    // bottom side
                    buttonInstant.transform.localPosition = new Vector3(
                        cornerWidthSpace + (boxSize * r) + (space * r),
                        cornerHeightSpace, 0);
                }
                else
                {
                    // remaining
                    buttonInstant.transform.localPosition = new Vector3(
                        cornerWidthSpace + (boxSize * r) + (space * r),
                        cornerHeightSpace + (boxSize * s) + (space * s), 0);
                }

                buttonInstant.GetComponent<Button>().onClick.AddListener(OnButtonClick);

                gridObject[r, s] = buttonInstant;
            }
        }
    }

    public void NextLevel()
    {
        level++;
        SetData(level);

        TapOnScreenAnim.speed = 5;
        TapOnScreenAnim.SetTrigger("Start");
        
        TapOnScreen.GetComponent<CanvasGroup>().alpha = 1;

        RectTransform rectTransform = TapOnScreen.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0,0);

        foreach (Transform child in Holder.transform)
        {
            Destroy(child.gameObject);
        }
        Generate();

        PlayUI.SetActive(true);
        winScreen.SetActive(false);
        failScreen.SetActive(false);

        SetTimeCountDown();

        mistakesCount = 0;

        TapOnScreenAnim.speed = 1;
    }

    public void Restart()
    {
        TapOnScreenAnim.SetTrigger("Start");

        foreach (Transform child in Holder.transform)
        {
            Destroy(child.gameObject);
        }
        Generate();

        PlayUI.SetActive(true);
        winScreen.SetActive(false);
        failScreen.SetActive(false);

        RectTransform rectTransform = TapOnScreen.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0,0);

        SetTimeCountDown();

        mistakesCount = 0;
    }

    public void SetTimeCountDown()
    {
        LevelNum.text = level.ToString();
        if (currentDifficulty == 2)
        {
            RemainingTime.text = "5";
        }
        else
        {
            RemainingTime.text = "10";
        }
    }

    public void OnButtonClick()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;

        int x, y;
        GetXY(clickedButton, out x, out y);

        if (x != -1 || y != -1)
        {
            if (isCounting)
            {
                StopAllCoroutines();
                ChangeAllColors(false);
                RemainPanel.SetActive(!RemainPanel.activeInHierarchy);
                MistakesTimePanel.SetActive(!MistakesTimePanel.activeInHierarchy);
                RestTilesText.text = colors.ToString();
                MistakesText.text = "0/" + BridgeData.GetMistakes().ToString();
                isCounting = false;
            }

            if (gridInt[x, y] == 0 && gridInt[x, y] != -1)
            {
                // blank button click
                mistakesCount += 1;
                gridInt[x, y] = -1;
                if (IsFail())
                {
                    // fail
                    clickedButton.GetComponent<Image>().color = Color.green;
                    PlayUI.SetActive(!PlayUI.activeInHierarchy);
                    failScreen.SetActive(!failScreen.activeInHierarchy);
                }

                clickedButton.GetComponent<Image>().color = Color.green;
                MistakesText.text = mistakesCount.ToString() + "/" + BridgeData.GetMistakes().ToString();

            } else if (gridInt[x, y] == 2)
            {
                // color button click --> set 1

                
                clickedButton.GetComponent<Image>().color = Color.red;
                RestTilesText.text = (int.Parse(RestTilesText.text) - 1).ToString();
                gridInt[x, y] = 1;
                if (RestTilesText.text == "0")
                {
                    // win
                    BridgeData.addLevelDone(level);
                    PlayUI.SetActive(!PlayUI.activeInHierarchy);
                    winScreen.SetActive(!winScreen.activeInHierarchy);
                }
            }
        }
    }

    public bool IsFail()
    {
        if (mistakesCount > BridgeData.GetMistakes()) {
            return true;}
        else {
            return false;}
    }

    public void GetXY(GameObject toFind, out int x, out int y)
    {
        x = -1;
        y = -1;
        for (int r = 0; r < radky; r++) {
            for (int s = 0; s < sloupce; s++) {
                if (toFind == gridObject[r, s])
                {
                    x = r;
                    y = s;
                    return;
                }
            }
        }
    }

    public void ChangeAllColors(bool type)
    {
        // true - set colors red
        // false - set colors white

        for (int r = 0; r < radky; r++) {
            for (int s = 0; s < sloupce; s++)
            {

                if (gridInt[r, s] == 1 || gridInt[r, s] == 2)
                {
                    if (type)
                    {
                        gridObject[r, s].GetComponent<Image>().color = Color.red;
                    } else {
                        gridObject[r, s].GetComponent<Image>().color = Color.white;
                    }
                }
            }
        }
    }

    // BUTTONS / ANIMATIONS

    public void OnTapOnScreen()
    {
        ChangeAllColors(true);
        TapOnScreenAnim.SetTrigger("FloatUp");
        StartCoroutine(Counting());
        isCounting = true;
    }

    IEnumerator Counting()
    {
        while (int.Parse(RemainingTime.text) != 0)
        {
            yield return new WaitForSeconds(1f);
            RemainingTime.text = (int.Parse(RemainingTime.text) - 1).ToString();
        }
        if (int.Parse(RemainingTime.text) == 0) {
            ChangeAllColors(false);
            RemainPanel.SetActive(!RemainPanel.activeInHierarchy);
            MistakesTimePanel.SetActive(!MistakesTimePanel.activeInHierarchy);
            RestTilesText.text = colors.ToString();
            MistakesText.text = "0/" + BridgeData.GetMistakes().ToString();
            isCounting = false;
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
}

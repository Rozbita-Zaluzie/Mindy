using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject Menu;
    public GameObject Camping;
    public GameObject Settings;
    public GameObject Bridge;

    public Text GameDataViewer;
    public GameObject DeleteCheck;
    public Text ResetButtonText;

    public int currentDifficulty;

    public GameObject bridgePrefab;
    public GameObject buttonClickParticle;

    public Toggle ButtonEasy;
    public Toggle ButtonNormal;
    public Toggle ButtonHard;

    void Start()
    {
        GameObject[] dontdestroy;
        dontdestroy = GameObject.FindGameObjectsWithTag("GameController");
        if (dontdestroy.Length == 0)
        {
            Bridge = Instantiate(bridgePrefab);
        } else
        {
            Bridge = dontdestroy[0];
        }
        DontDestroyOnLoad(Bridge);
    }

    public void LevelPressed()
    {
        GameObject button = EventSystem.current.currentSelectedGameObject;
        LevelButtonData ButtonScr = button.GetComponent<LevelButtonData>();

        int[] numbers = Bridge.GetComponent<BridgeData>().GetGameData();

        if (numbers[ButtonScr.level - 1] != 2)
        {
            if (GameObject.FindGameObjectsWithTag("Particles").Length == 0)
            {
                Vector3 particlesPosition = new Vector3(button.transform.position.x, button.transform.position.y, 91);

                Instantiate(buttonClickParticle, particlesPosition, Quaternion.identity);

                BridgeData bridgeData = Bridge.GetComponent<BridgeData>();
                bridgeData.currentLevel = ButtonScr.level;
                bridgeData.difficulty = currentDifficulty;

                GameObject sceneLoader = GameObject.Find("SceneLoader");
                sceneLoader.GetComponent<SceneLoader>().PlayLevel(ButtonScr.level);
            }
        }
    }

    public void SetLevelButtons()
    {
        Sprite doneImg = Resources.Load<Sprite>("DoneLevel");
        Sprite lockImg = Resources.Load<Sprite>("LockImage");

        GameObject[] levelButtons = GameObject.FindGameObjectsWithTag("Level");
        int[] numbers = Bridge.GetComponent<BridgeData>().GetGameData();

        for (int i = 0; i < numbers.Length; i++)
        {
            Image buttonImg = levelButtons[i].GetComponent<Image>();
            GameObject buttonTxt = levelButtons[i].transform.GetChild(0).gameObject;
            switch (numbers[i])
            {
                case 0:
                    buttonImg.sprite = doneImg;
                    buttonTxt.SetActive(true);
                    break;

                case 1:
                    buttonImg.sprite = null;
                    buttonTxt.SetActive(true);
                    break;
                case 2:
                    buttonImg.sprite = lockImg;
                    buttonTxt.SetActive(false);
                    break;
            }
        }
    }

    public void ChangeDifficulty(int diff)
    {
        currentDifficulty = diff;
        Bridge.GetComponent<BridgeData>().SetDifficulty(diff);
    }

    public void ResetGameData(bool delete)
    {
        // true - will delete file
        // false - will open chceck window
        if (ResetButtonText.text != "Data are at default")
        {
            if (delete)
            {
                Bridge.GetComponent<BridgeData>().ResetData();
                ResetButtonText.text = "Data are at default";
                DeleteCheck.SetActive(!DeleteCheck.activeInHierarchy);
            }
            else
            {
                DeleteCheck.SetActive(!DeleteCheck.activeInHierarchy);
            }
        }
    }

    // buttons for opening and closing level ui
    public void SwapMenuCamping()
    {
        Camping.SetActive(!Camping.activeInHierarchy);
        Menu.SetActive(!Menu.activeInHierarchy);
        Bridge.GetComponent<BridgeData>().SetLevelDataArray();
        if (Camping.activeInHierarchy)
        {
            SetLevelButtons();
        }
    }

    public void SwapMenuSettings()
    {
        Settings.SetActive(!Settings.activeInHierarchy);
        Menu.SetActive(!Menu.activeInHierarchy);
        if (Settings.activeInHierarchy)
        {
            // set gamepath viewer
            string txt = Bridge.GetComponent<BridgeData>().GameDataPath;
            string[] txt2 = txt.Split('/');
            string finalText = "";
            int num2;

            // gamepath setting to lines
            if (txt2.Length > 4)
            {
                if (txt2.Length % 2 != 0)
                {
                    double num1 = txt2.Length / 2;
                    num1 = Math.Round(num1);
                    num2 = Convert.ToInt32(num1);
                }
                else
                {
                    num2 = txt2.Length / 2;
                }

                
                for (int i = 0; i < txt2.Length; i++)
                {
                    finalText += txt2[i];
                    if (i < txt2.Length)
                    {
                        finalText += "/";
                    }
                    if (i == num2)
                    {
                        finalText += "\n";
                    }
                }
            } else {
                finalText = Bridge.GetComponent<BridgeData>().GameDataPath;}

            GameDataViewer.text = finalText;
            if (Bridge.GetComponent<BridgeData>().IsDefault()) {
                ResetButtonText.text = "Data are at default";}
            else {
                ResetButtonText.text = "Reset Data";}

            int x = Bridge.GetComponent<BridgeData>().GetDifficulty();

            switch (x)
            {
                case 0:
                    ButtonEasy.isOn = true;
                    break;
                case 1:
                    ButtonNormal.isOn = true;
                    break;
                case 2:
                    ButtonHard.isOn = true;
                    break;
            }
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}

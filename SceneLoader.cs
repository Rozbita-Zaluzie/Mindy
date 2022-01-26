using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public Animator transition;

    public void PlayLevel(int level)
    {
        string txt = "Level " + level.ToString();
        gameObject.transform.GetChild(0).transform.Find("Text").GetComponent<Text>().text = txt;

        StartCoroutine(LoadLevel());
    }

    IEnumerator LoadLevel()
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(2.2f);
        SceneManager.LoadScene("Play");
    }

}

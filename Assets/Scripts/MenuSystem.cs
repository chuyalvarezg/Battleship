using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSystem : MonoBehaviour
{
    private int selectedDifficulty = 0;

    [SerializeField]
    private Button[] levels;

    private int lastLevelUnlocked;

    private int easyLevel;
    private int normalLevel;
    private int hardLevel;

    // Start is called before the first frame update
    void Start()
    {
        easyLevel = PlayerPrefs.GetInt("LastEasyLevelCompleted", 0);
        normalLevel = PlayerPrefs.GetInt("LastNormalLevelCompleted", 0);
        
        hardLevel = PlayerPrefs.GetInt("LastHardLevelCompleted", 0);
        
    }

    public void SelectDifficulty(int difficulty)
    {
        selectedDifficulty = difficulty;
        GetUnlockedLevel();
        LoadLevelsMenuPage(0);
    }

    private void GetUnlockedLevel()
    {
        switch (selectedDifficulty)
        {
            case 1:
                lastLevelUnlocked = easyLevel;
                break;
            case 2:
                lastLevelUnlocked = normalLevel;
                break;
            case 3:
                lastLevelUnlocked = hardLevel;
                break;
        }


    }

    public void resetLevelsMenuPage()
    {
        foreach (Button level in levels)
        {
            level.interactable = false;
        }
    }

    public void LoadLevelsMenuPage(int page)
    {
        resetLevelsMenuPage();
        //Debug.Log("diff: " + selectedDifficulty + " level: " + lastLevelUnlocked);
        int i = page*36;
        foreach (Button level in levels)
        {
            i++;
            level.transform.GetChild(0).GetComponent<Text>().text = i.ToString();
            if ((i - 1) <= lastLevelUnlocked) {
                level.interactable = true;
            }
        }
    }

    public void LoadLevel(Button selectedButton)
    {
        string sceneToLoad = "";
        switch (selectedDifficulty)
        {
            case 1:
                sceneToLoad = "EasyLevel";
                break;
            case 2:
                sceneToLoad = "NormalLevel";
                break;
            case 3:
                sceneToLoad = "HardLevel";
                break;
        }
        PlayerPrefs.SetInt("currentLevelSeed", (selectedDifficulty * 1000) + int.Parse(selectedButton.transform.GetChild(0).GetComponent<Text>().text));
        SceneManager.LoadScene(sceneToLoad);
    }


    public void ResetProgress()
    {
        PlayerPrefs.SetInt("LastEasyLevelCompleted", 0);
        PlayerPrefs.SetInt("LastNormalLevelCompleted", 0);
        PlayerPrefs.SetInt("LastHardLevelCompleted", 0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour {

    public GameObject PauseMenu;
    public GameObject GraphicOptionsMenu;
    public GameObject FillMenu;
    public GameObject GameConfigBackUp;
    public Slider SoundSlider;
    public TMPro.TMP_Dropdown GraphicSelectionValor;
    public GameConfig gameConfig;
    //public GameObject hpBar;
    public OnSceneExit die;
    public OnSceneExit win;


    private void Awake()
    {
        GameObject TemporalGameConfig;
        TemporalGameConfig = GameObject.Find("GameConfig");
        if (TemporalGameConfig == null)
        {

            TemporalGameConfig = Instantiate(GameConfigBackUp);
            TemporalGameConfig.name = "GameConfig";

        }
        gameConfig = TemporalGameConfig.GetComponent<GameConfig>();
        UpdateGameConfigAtBeggining();
    }

    private void Start()
    {
        AudioListener.volume = gameConfig.SoundLevel;
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GraphicOptionsMenu.activeInHierarchy)
                Pause();
        }
    }

    public void Pause()
    {
        if (PauseMenu.gameObject.activeInHierarchy == false)
        {
            Time.timeScale = 0;
            PauseMenu.gameObject.SetActive(true);
            //hpBar.SetActive(false);
        }
        else
        {
            Time.timeScale = 1;
            PauseMenu.gameObject.SetActive(false);
            //hpBar.SetActive(true);
        }
    }

    public void ShowGraphicsOptions()
    {
        if (GraphicOptionsMenu.gameObject.activeInHierarchy == false)
        {
            GraphicOptionsMenu.gameObject.SetActive(true);
            UpdateDataAtBeginning();
        }
        else
        {
            GraphicOptionsMenu.gameObject.SetActive(false);
        }
    }

    private void ShowFillMenu ()
    {
        if (FillMenu.gameObject.activeInHierarchy == false)
        {
            FillMenu.gameObject.SetActive(true);
        }
        else
        {
            FillMenu.gameObject.SetActive(false);
        }
    }

    private void ChangeGraphicsLevelTo (int level)
    {  
        QualitySettings.SetQualityLevel(level, true);
    }

    public void ChangeSoundSettings(float SoundLevel)
    {
        AudioListener.volume = SoundLevel;
        gameConfig.SoundLevel = SoundLevel;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("IA 1");
    }

    public void ApplyAndReturnToMenu()
    {
        StartCoroutine(SaveChanges(gameConfig.GraphicsLevel));
    }

    IEnumerator SaveChanges(int GraphicsSettings)
    {
        if (QualitySettings.GetQualityLevel() != GraphicsSettings)
        {
            ShowFillMenu();
            ChangeGraphicsLevelTo(GraphicsSettings);
            yield return new WaitForSecondsRealtime(1f);
            UpdateDataAtBeginning();
            ShowFillMenu();
        }  
    }

    private void UpdateDataAtBeginning()
    {
        GraphicSelectionValor.value = QualitySettings.GetQualityLevel();
        SoundSlider.value = AudioListener.volume;
    }

    public void ChangeTemporalGrapichsLevel(int level)
    {
        gameConfig.GraphicsLevel = level;
    }

    public void UpdateGameConfigAtBeggining()
    {
        gameConfig.GraphicsLevel = QualitySettings.GetQualityLevel();
        gameConfig.SoundLevel = AudioListener.volume;
    }

   
}

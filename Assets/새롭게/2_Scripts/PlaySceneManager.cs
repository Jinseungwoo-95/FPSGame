using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum eGameState
{
    READY = 0,
    START,
    PLAY,
    END,
    RESULT,
    PAUSE,
}

public class PlaySceneManager : MonoBehaviour
{
    public static PlaySceneManager instance;

    // 엔드일 경우 에너미 삭제하기 위한 델리게이트
    public delegate void EndHandler();
    public event EndHandler Endevent;

    [SerializeField] Image helpBG;
    [SerializeField] Image magazineImg;
    [SerializeField] Image hpBar;
    [SerializeField] Text helpText;
    [SerializeField] Text timeText;
    [SerializeField] Text stageText;
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject resultPanel;
    eGameState eState;
    public bool PlayState { get { return eState == eGameState.PLAY; } }

    float timeCheck;    // 시간 관련된거에 사용하는 변수
    EnemySpawner enemySpawner;

    // 스테이지 레벨
    private int stageLevel;
    public int StageLevel { get { return stageLevel; } }

    // 에너미 카운트
    private int enemyCnt;

    private void Awake()
    {
        if(instance == null)
        {
            stageLevel = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        timeCheck = 0;
    }

    void Start()
    {
        helpBG.enabled = false;
        helpText.enabled = false;
        enemySpawner = GameObject.FindObjectOfType<EnemySpawner>();
        PrintTime(0);
        ChangeState(eGameState.READY);
    }

    void Update()
    {
        switch(eState)
        {
            case eGameState.START:
                timeCheck += Time.deltaTime;
                if (timeCheck >= 2f)
                    ChangeState(eGameState.PLAY);
                break;

            case eGameState.PLAY:
                PrintTime(Time.deltaTime);
                if (timeCheck <= 0)
                    ChangeState(eGameState.END);

                if (Input.GetKeyDown(KeyCode.Escape))
                    ChangeState(eGameState.PAUSE);

                break;

            case eGameState.PAUSE:
                if(Input.GetKeyDown(KeyCode.Escape))
                    GameUnPause();
                break;

            case eGameState.END:
                timeCheck += Time.deltaTime;
                if (timeCheck >= 2f)
                    ChangeState(eGameState.RESULT);
                break;
        }
    }

    void PrintTime(float _time)
    {
        timeCheck -= _time;
        int sec = (int)timeCheck;
        int msec = (int)((timeCheck - sec) * 100);
        timeText.text = string.Format("{0}:{1}", sec, msec);
    }

    public void MinusEnemyCnt()
    {
        if(--enemyCnt == 0)
        {
            ++stageLevel;
            enemySpawner.Spawn();
            ChangeState(eGameState.START);
        }
    }

    public void ChangeState(eGameState _state)
    {
        switch(_state)
        {
            case eGameState.READY:
                helpBG.enabled = true;
                helpText.enabled = true;
                helpText.text = "READY!";
                eState = eGameState.READY;
                break;

            case eGameState.START:
                enemyCnt = stageLevel * 10;
                timeCheck = 0;
                helpBG.enabled = true;
                helpText.enabled = true;
                helpText.text = "STAGE" + stageLevel.ToString() + " START!";
                eState = eGameState.START;
                break;

            case eGameState.PLAY:
                timeCheck = stageLevel * 40f;
                helpBG.enabled = false;
                helpText.enabled = false;
                eState = eGameState.PLAY;
                break;

            case eGameState.END:
                timeCheck = 0;
                timeText.text = "0:0";
                Endevent();
                helpBG.enabled = true;
                helpText.enabled = true;
                helpText.text = "GAME OVER!";
                eState = eGameState.END;
                break;

            case eGameState.RESULT:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                helpBG.enabled = false;
                helpText.enabled = false;
                stageText.text = "기록 : " + stageLevel.ToString() + "STAGE";
                resultPanel.SetActive(true);
                eState = eGameState.RESULT;
                break;

            case eGameState.PAUSE:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                eState = eGameState.PAUSE;
                Time.timeScale = 0f;
                pausePanel.SetActive(true);
                break;
        }
    }

    public void GameUnPause()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        eState = eGameState.PLAY;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }

    public void LoadTitle()
    {
        Destroy(gameObject);
        Time.timeScale = 1f;
        stageLevel = 1;
        SceneManager.LoadScene("Title");
    }

    public void GameReStart()
    {
        stageLevel = 1;
        resultPanel.SetActive(false);
        ChangeState(eGameState.READY);
        magazineImg.fillAmount = 1f;
        hpBar.fillAmount = 1;
        SceneManager.LoadScene("Play");
    }

    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

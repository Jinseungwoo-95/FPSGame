using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleUIManager : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private Text bgmTxt;
    [SerializeField] private Text effectTxt;

    public void OnClickStart()
    {
        SceneManager.LoadScene("Play");
    }

    public void OnClickOption()
    {
        nameText.text = "옵 션";
        titlePanel.SetActive(false);
        optionPanel.SetActive(true);
    }

    public void OnClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnClickBack()
    {
        nameText.text = "워킹 데드";
        optionPanel.SetActive(false);
        titlePanel.SetActive(true);
    }

    public void OnClickBGM()
    {
        SoundManager.Instance.bgmOn = !SoundManager.Instance.bgmOn;
        bool bgm = SoundManager.Instance.bgmOn;

        if (bgm)
        {
            bgmTxt.text = "BGM : ON";
            SoundManager.Instance.PlayBGM(eTypeBGM.STAGE1);
        }
        else
        {
            bgmTxt.text = "BGM : OFF";
            SoundManager.Instance.PauseBGM();
        }
    }

    public void OnClickEffect()
    {
        SoundManager.Instance.effectOn = !SoundManager.Instance.effectOn;

        if (SoundManager.Instance.effectOn)
            effectTxt.text = "EFFECT : ON";
        else
            effectTxt.text = "EFFECT : OFF";
    }
}

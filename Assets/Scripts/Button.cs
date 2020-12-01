using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Button : MonoBehaviour
{
    public Controller controllerScript;
    public Image btnImage;
    public Sprite btnUpSprite;
    public Sprite btnDownSprite;
    public TextMeshProUGUI fightTMP;
    private RectTransform childTextRt;
    private bool fingerIsOffButton;


    private void Awake()
    {
        childTextRt = fightTMP.GetComponent<RectTransform>();
    }

    public void TouchDown()
    {
        btnImage.sprite = btnDownSprite;
        childTextRt.localPosition = new Vector3(childTextRt.anchoredPosition.x, -8, 0);
    }
    public void TouchLiftUp()
    {
        btnImage.sprite = btnUpSprite;
        childTextRt.localPosition = new Vector3(childTextRt.anchoredPosition.x, 16, 0);
        if (!fingerIsOffButton)
        {
            if (controllerScript.gameOver)
            {
                controllerScript.StartCoroutine(controllerScript.RematchCo());
            }
            else
            {
                controllerScript.BeginFight();
            }          
        }
    }

    public void FingerIsOffButtonSpace()
    {
        fingerIsOffButton = true;
    }
    public void FingerIsOnButtonSpace()
    {
        fingerIsOffButton = false;
    }
}

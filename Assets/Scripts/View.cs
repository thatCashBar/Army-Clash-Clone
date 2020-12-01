using System.Collections;
using UnityEngine;
using TMPro;

public class View
{
    public static void SetTextToString(TextMeshProUGUI tmp, string s)
    {
        tmp.text = s;
    }

    public static void SetLevelText(TextMeshProUGUI tmp, int level)
    {
        tmp.text = $"Level {level}";
    }

    public static IEnumerator DisplayFightButtonWithDelay(GameObject fightButton, TextMeshProUGUI fightButtonTMP, string s, float delay)
    {
        yield return new WaitForSeconds(delay);
        SetTextToString(fightButtonTMP, s);
        fightButton.gameObject.SetActive(true);
    }

    public static IEnumerator TriggerPopUpAnimWithDelay(Animator anim)
    {
        yield return new WaitForSeconds(.5f);
        anim.gameObject.SetActive(true);
    }
    public static IEnumerator TriggerPopUpAnimOutroCo(Animator anim)
    {
        anim.SetTrigger("exit");
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        anim.gameObject.SetActive(false);
    }
}

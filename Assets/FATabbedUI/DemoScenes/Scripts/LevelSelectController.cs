using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectController : MonoBehaviour
{
    public FAScrollSnapBehaviour scrollSnapBehaviour;
    public Text lvlName;
    public GameObject enterBtn;
    public Animator animator;
    public string animatorBoolParameter = "Focused";
    //public float animDuration = 0.5f;

    private void Start()
    {

        scrollSnapBehaviour.onScrollToPage?.AddListener(FocusLevel);
        scrollSnapBehaviour.onScrollStarted?.AddListener(UnfocusLevel);
    }
    private void OnDisable()
    {

        scrollSnapBehaviour.onScrollToPage?.RemoveListener(FocusLevel);
        scrollSnapBehaviour.onScrollStarted?.RemoveListener(UnfocusLevel);
    }
    public void FocusLevel(int index)
    {
        lvlName.text = "Level " + (index + 1);
        animator.SetBool(animatorBoolParameter, true);
        //If you use Lean Tween you can simply uncomment the bottom lines and comment out the above line (animator.SetBool(animatorBoolParameter, true);)
        //LeanTween.scale(lvlName.gameObject, Vector3.one, animDuration).setEaseOutBounce();
        //LeanTween.scale(enterBtn, Vector3.one, animDuration).setEaseOutBounce();
    }
    public void UnfocusLevel()
    {
        animator.SetBool(animatorBoolParameter, false);
        //If you use Lean Tween you can simply uncomment the bottom lines and comment out the above line (animator.SetBool(animatorBoolParameter, false);)
        //LeanTween.scale(lvlName.gameObject, Vector3.zero, animDuration).setEaseInOutCubic();
        //LeanTween.scale(enterBtn, Vector3.zero, animDuration).setEaseInOutCubic();
    }
}

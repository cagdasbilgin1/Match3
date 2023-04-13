using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(HorizontalLayoutGroup))]
public class FATabBar : MonoBehaviour
{
    /// <summary>
    /// The selector image indicator that slides over the buttons.
    /// </summary>
    [Tooltip("The selector image indicator that slides over the buttons.")]
    public RectTransform selector;
    /// <summary>
    /// Collection of all the buttons this tab bar contains.
    /// </summary>
    [Tooltip("All the buttons this tab bar contains.")]
    public List<RectTransform> buttons;
    /// <summary>
    /// The <see cref="FAScrollSnapBehaviour"/> that this tab bar interact with.
    /// </summary>
    [Tooltip("The scroll view that this tab bar interact with.")]
    public FAScrollSnapBehaviour scrollSnapRect;
    /// <summary>
    /// Gets or sets the starting page.
    /// <para>Gets overriden if used with <see cref="FAScrollSnapBehaviour"/></para>
    /// </summary>
    [Tooltip("Starting Page, gets overriden if used with ScrollSnapBehaviour")]
    public int startingPage = 2;
    /// <summary>
    /// As this whole asset utilises Unity's LayoutGroups components, the sliding animation of the buttons is done by manipulating the values of the <see cref="https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/script-VerticalLayoutGroup.html">VerticalLayoutGroup</see> and <see cref="https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/script-LayoutElement.html">LayoutElement</see> components. 
    /// <para>This controls the FlexibleWidth of the button</para>
    /// </summary>
    [Range(1, 2)]
    public float selectedBtnFlexibleWidth = 1.5f;
    /// <summary>
    /// As this whole asset utilises Unity's LayoutGroups components, the sliding animation of the buttons is done by manipulating the values of the <see cref="https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/script-VerticalLayoutGroup.html">VerticalLayoutGroup</see> and <see cref="https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/script-LayoutElement.html">LayoutElement</see> components. 
    /// <para>This controls the Padding.Top of the button</para>
    /// </summary>
    public int selectedBtnPaddingTop = -30;
    /// <summary>
    /// As this whole asset utilises Unity's LayoutGroups components, the sliding animation of the buttons is done by manipulating the values of the <see cref="https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/script-VerticalLayoutGroup.html">VerticalLayoutGroup</see> and <see cref="https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/script-LayoutElement.html">LayoutElement</see> components. 
    /// <para>This controls the Height of the button's title</para>
    /// </summary>
    public float titlePreferedHeight = 15f;

    HorizontalLayoutGroup horizontalLayoutGroup;

    Vector2 bigCellSize, smallCellSize;

    RectTransform rectTransform;

    Vector2 selectorStartPos;
    readonly List<FATabBarItem> buttonsTabItem = new List<FATabBarItem>();

    FATabBarEffect effectOut;
    FATabBarEffect effectIn;

    private void Awake()
    {

        if (scrollSnapRect != null)
        {
            startingPage = scrollSnapRect.startingPage;
        }
        effectIn = new FATabBarEffect
        {
            FlexibleWidth = selectedBtnFlexibleWidth,
            PaddingTop = selectedBtnPaddingTop,
            TitlePreferedHeight = titlePreferedHeight,
            TitleFlexibleHeight = 1,
            TitleAlpha = 1
        };
        effectOut = new FATabBarEffect
        {
            FlexibleWidth = 1,
            PaddingTop = 0,
            TitlePreferedHeight = 0,
            TitleFlexibleHeight = 0,
            TitleAlpha = 0
        };
    }
    void Start()
    {
        foreach (RectTransform button in buttons)
        {
            buttonsTabItem.Add(button.GetComponent<FATabBarItem>());
        }
        horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();

        rectTransform = transform as RectTransform;
        StartCoroutine(Setup());
    }
    private void OnDisable()
    {

        scrollSnapRect.onScrollEnded.RemoveListener(OnScrollEnded);
        scrollSnapRect.onScrolledPercentage.RemoveListener(OnScrolled);
    }
    IEnumerator Setup()
    {
        //Initial setup for each tab item
        ResetTabItems(startingPage);
        yield return null;
        //calculate selector values
        selectorStartPos = buttons[0].anchoredPosition;
        int random = Random.Range(0, buttons.Count);
        //find random unselected button to get default values
        while (random == startingPage)
        {
            random = Random.Range(0, buttons.Count);
            yield return null;
        }
        yield return null;
        smallCellSize = buttons[random].sizeDelta;
        selector.anchorMin = buttons[startingPage].anchorMin;
        selector.anchorMax = buttons[startingPage].anchorMax;
        selector.pivot = buttons[startingPage].pivot;
        selector.sizeDelta = new Vector2(buttons[startingPage].sizeDelta.x, rectTransform.sizeDelta.y);
        selector.anchoredPosition = new Vector2(SelectorPos(startingPage), selectorStartPos.y);
        scrollSnapRect.onScrolledPercentage.AddListener(OnScrolled);
        scrollSnapRect.onScrollEnded.AddListener(OnScrollEnded);
    }

    void OnScrollEnded(int page)
    {
        ResetTabItems(page);

    }
    //hack for fast movements
    int lastPageA, lastPageB;
    bool hasSetLastPageOnce = false;
    void OnScrolled(float percentageScrolled)
    {
        float percentMltp = percentageScrolled * ((float)buttons.Count - 1) + 1;
        //find previous and next button
        int a, b;
        if (percentMltp >= buttonsTabItem.Count)
        {
            a = buttonsTabItem.Count - 2;
            b = buttonsTabItem.Count - 1;
        }
        else if (percentMltp <= 1)
        {

            a = 0;
            b = 1;
        }
        else
        {
            //Some math for finding left and right page from the current percentage scrolled
            a = Mathf.Approximately(0, percentMltp % 1) ? (int)percentMltp - 2 : (int)Mathf.Floor(percentMltp - 1);
            b = Mathf.Approximately(0, percentMltp % 1) ? (int)percentMltp - 1 : (int)Mathf.Ceil(percentMltp - 1);

        }
        //hack for fast movement
        if (hasSetLastPageOnce)
        {
            if (lastPageA != a)
            {

                ResetTabItem(lastPageA);
            }
            if (lastPageB != b)
            {

                ResetTabItem(lastPageB);
            }
        }
        lastPageA = a;
        lastPageB = b;
        hasSetLastPageOnce = true;
        //lerp values
        float t = Mathf.Approximately(b, percentMltp - 1) ? 1  : percentMltp % 1;
        float selectorAnchor = Mathf.Lerp(SelectorPos(a), SelectorPos(b), t);
        int paddingOut = (int)Mathf.Lerp(selectedBtnPaddingTop, 0, t);
        int paddingIn = (int)Mathf.Lerp(0, selectedBtnPaddingTop, t);
        float titlePreferedOut = Mathf.Lerp(titlePreferedHeight, 0, t);
        float titlePreferedIn = Mathf.Lerp(0, titlePreferedHeight, t);
        float layoutFlexibleWidthOut = Mathf.Lerp(selectedBtnFlexibleWidth, 1, t);
        float layoutFlexibleWidthIn = Mathf.Lerp(1, selectedBtnFlexibleWidth, t);

        //create and apply tab effects
        FATabBarEffect effectOut = new FATabBarEffect
        {
            FlexibleWidth = layoutFlexibleWidthOut,
            PaddingTop = paddingOut,
            TitlePreferedHeight = titlePreferedOut,
            TitleFlexibleHeight = 1 - t,
            TitleAlpha = 1 - t
        };
        FATabBarEffect effectIn = new FATabBarEffect
        {
            FlexibleWidth = layoutFlexibleWidthIn,
            PaddingTop = paddingIn,
            TitlePreferedHeight = titlePreferedIn,
            TitleFlexibleHeight = t,
            TitleAlpha = t
        };
        buttonsTabItem[a].ApplyEffect(effectOut);
        buttonsTabItem[b].ApplyEffect(effectIn);

        //apply selector position
        selector.anchoredPosition = new Vector2(selectorAnchor, selector.anchoredPosition.y);
    }
    /// <summary>
    /// returns the proper position for the tab bar selector image.
    /// </summary>
    /// <param name="page">page index</param>
    /// <returns></returns>
    float SelectorPos(int page)
    {
        return (buttons[page].anchoredPosition.x - (buttons[page].sizeDelta.x * buttons[page].pivot.x)) + (selector.sizeDelta.x * selector.pivot.x);
    }

    /// <summary>
    /// Resets the tab to "unselected state"
    /// </summary>
    /// <param name="tabToReset"></param>
    void ResetTabItem(int tabToReset)
    {
        buttonsTabItem[tabToReset].ApplyEffect(effectOut);
    }

    /// <summary>
    /// Resets all tabs to their correct state (selected or unselected).
    /// </summary>
    /// <param name="selectedPage"></param>
    void ResetTabItems(int selectedPage)
    {
        for (int i = 0; i < buttonsTabItem.Count; i++)
        {
            if (i == selectedPage)
            {
                buttonsTabItem[i].ApplyEffect(effectIn);
            }
            else
            {

                buttonsTabItem[i].ApplyEffect(effectOut);
            }
        }
    }
}

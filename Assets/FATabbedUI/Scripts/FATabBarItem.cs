using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Represents a TabItem inside the <see cref="FATabBar"/>, and is responsible for the scroll animations of the item.
/// </summary>
/// <remarks></remarks>
[RequireComponent(typeof(VerticalLayoutGroup))]
[RequireComponent(typeof(LayoutElement))]
public class FATabBarItem : MonoBehaviour
{
    public Image icon;
    //public Text title;
    public TextMeshProUGUI title2;

    VerticalLayoutGroup layoutGroup;
    LayoutElement layoutElement;

    LayoutElement textLayoutElement;
    private void Awake()
    {
        layoutGroup = GetComponent<VerticalLayoutGroup>();
        textLayoutElement = title2.GetComponent<LayoutElement>();
        layoutElement = GetComponent<LayoutElement>();
        if (textLayoutElement == null)
            textLayoutElement = title2.gameObject.AddComponent<LayoutElement>();
        if (icon.GetComponent<LayoutElement>())
            icon.gameObject.AddComponent<LayoutElement>();
    }
    /// <summary>
    /// Applies the <see cref="FATabBarEffect"/> to this instance.
    /// </summary>
    /// <param name="effect"></param>
    /// <remarks></remarks>
    public void ApplyEffect(FATabBarEffect effect)
    {
        //required
        layoutElement.flexibleWidth = effect.FlexibleWidth;
        //optional
        layoutGroup.padding = new RectOffset(0, 0, effect.PaddingTop, 0);
        textLayoutElement.preferredHeight = effect.TitlePreferedHeight;
        textLayoutElement.flexibleHeight = effect.TitleFlexibleHeight;
        Color c = title2.color;
        c.a = effect.TitleAlpha;
        title2.color = c;

    }
}
/// <summary>
/// Represents a collection of constraints to apply to the <see cref="https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/script-LayoutElement.html">LayoutElement</see> component of this instance.
/// </summary>
public class FATabBarEffect
{
    public float FlexibleWidth { get; set; }
    public int PaddingTop { get; set; }
    public float TitlePreferedHeight { get; set; }
    public float TitleFlexibleHeight { get; set; }
    public float TitleAlpha { get; set; }
}

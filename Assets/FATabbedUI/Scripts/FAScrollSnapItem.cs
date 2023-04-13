using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents animatable item in the <see cref="FAScrollSnapBehaviour"/>. 
/// <para>Uses <see cref="https://docs.unity3d.com/Manual/class-BlendTree.html">Blend Trees</see> for the scrolling animation.</para>
/// <para>Simple blend trees with 2 motions and one float paramater.</para>
/// <para>The motion at value 1 should be when the item is <strong>selected</strong>.</para>
/// <para>The motion at value 0 should be when the item is <strong>not selected</strong>.</para>
/// </summary>
public class FAScrollSnapItem : MonoBehaviour
{
    public float ItemAnchoredPosition {  get { return _anchoredPosition; } }
    /// <summary>
    /// Gets or sets whether this instance has scrolling animations
    /// </summary>
    public bool hasAnimations = true;

    [SerializeField]
    string animatorParam = "State";

    Animator _animator;
    float _anchoredPosition, _difference;
    bool _horizontal = false;
    private void Awake()
    {
        if(GetComponent<Animator>() is Animator anim)
            _animator = anim;
    }

    public void Setup(FAScrollSnapItemSettings settings)
    {
        _horizontal = settings.IsHorisontal;
        _anchoredPosition = settings.Position;
        _difference = settings.Difference;
        if (_animator)
        {
            _animator.SetFloat(animatorParam, 0);
        }
    }
    public void SetSelected()
    {
        if (!hasAnimations)
            return;

        if (_animator)
        {
            _animator.SetFloat(animatorParam, 1);
        }
    }
    /// <summary>
    /// Handler for the <see cref="FAScrollSnapBehaviour.onScrolled"/> event, that is responsible for animating this instance.
    /// </summary>
    /// <param name="pos"></param>
    public void OnScrollHandler(Vector2 pos)
    {
        float scrollPos = (_horizontal) ? pos.x : pos.y;
        if (_animator)
        {
            if (scrollPos == _anchoredPosition)
            {
                _animator.SetFloat(animatorParam, 1);
            }
            else if (scrollPos < _anchoredPosition && scrollPos >= _anchoredPosition - _difference)
            {
                float nextDistance = _anchoredPosition - _difference;
                float t = Mathf.Clamp(Mathf.InverseLerp(nextDistance, _anchoredPosition, scrollPos), 0, 1);
                _animator.SetFloat(animatorParam, t);

            }
            else if (scrollPos > _anchoredPosition && scrollPos <= _anchoredPosition + _difference)
            {
                float nextDistance = _anchoredPosition + _difference;
                float t = Mathf.Clamp(Mathf.InverseLerp(nextDistance, _anchoredPosition, scrollPos), 0, 1);
                _animator.SetFloat(animatorParam, t);
            }
        }
    }
}

/// <summary>
/// Represents a collection of settings used to setup a <see cref="FAScrollSnapItem"/>
/// </summary>
public class FAScrollSnapItemSettings
{
    /// <summary>
    /// Gets or sets the calculated anchoredPosition of the <see cref="FAScrollSnapBehaviour"/>'s content, where the item is considered selected.
    /// </summary>
    public float Position { get; set; }
    /// <summary>
    /// Gets or sets the difference in Screen Space units between two items in the <c>ScrollView</c> collection.
    /// </summary>
    /// <value></value>
    public float Difference { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the <c>ScrollView</c> is horizontal (or vertical) .
    /// </summary>
    /// <value>
    ///   <see langword="true" /> if this instance ; otherwise, <see langword="false" />.</value>
    /// <remarks></remarks>
    public bool IsHorisontal { get; set; }
}
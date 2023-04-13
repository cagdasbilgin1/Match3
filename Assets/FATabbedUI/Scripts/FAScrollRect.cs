using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Represents a Custom ScrollRect that propagades drag events to parent <see cref="FAScrollSnapBehaviour"/>
/// </summary>
public class FAScrollRect : ScrollRect
{
    [HideInInspector]
    public ScrollRect mOutterScroll;
    [HideInInspector]
    public FAScrollSnapBehaviour mOutterScrollRect;
    bool draggingSide = false;

    public override void OnBeginDrag(PointerEventData eventData)
    {
        bool shouldDragSide = (mOutterScrollRect.IsHorizontal) 
            ? Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y) 
            : Mathf.Abs(eventData.delta.y) > Mathf.Abs(eventData.delta.x);
        if (shouldDragSide)
        {
            Vector2 newdelta = (mOutterScrollRect.IsHorizontal) ? new Vector2(eventData.delta.x, 0) : new Vector2(0, eventData.delta.y);
            eventData.delta = newdelta;
            mOutterScroll.OnBeginDrag(eventData);
            mOutterScrollRect?.OnBeginDrag(eventData);

            draggingSide = true;
        }
        else base.OnBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (draggingSide)
        {
            Vector2 newdelta = (mOutterScrollRect.IsHorizontal) ? new Vector2(eventData.delta.x, 0) : new Vector2(0, eventData.delta.y);
            eventData.delta = newdelta;
            mOutterScroll.OnDrag(eventData);
            mOutterScrollRect?.OnDrag(eventData);
        }
        else base.OnDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (draggingSide)
        {
            mOutterScroll.OnEndDrag(eventData);
            mOutterScrollRect?.OnEndDrag(eventData);
            draggingSide = false;
        }
        else base.OnEndDrag(eventData);
    }
}

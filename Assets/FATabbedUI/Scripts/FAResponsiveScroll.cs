using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides functionalities for a scroll which size changes dynamically through the game.
/// (optional)
/// </summary>
/// <remarks></remarks>
[RequireComponent(typeof(FAScrollSnapBehaviour))]
public class FAResponsiveScroll : MonoBehaviour
{
    FAScrollSnapBehaviour ssb;

    private void Awake()
    {
        ssb = GetComponent<FAScrollSnapBehaviour>();

    }
    void OnRectTransformDimensionsChange()
    {
        if(ssb == null)
            ssb = GetComponent<FAScrollSnapBehaviour>();
        ssb.Setup();
    }
}

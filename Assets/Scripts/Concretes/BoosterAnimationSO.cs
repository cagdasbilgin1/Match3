using CollapseBlast.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollapseBlast.ScriptableObjects
{
    [CreateAssetMenu(fileName = "BoosterAnimations", menuName = "CollapseBlast/BoosterAnimationsSO", order = 2)]
    public class BoosterAnimationSO : ScriptableObject
    {
        [SerializeField] List<Animator> boosterAnimators;

        public List<Animator> BoosterAnimators => boosterAnimators;
    }
}


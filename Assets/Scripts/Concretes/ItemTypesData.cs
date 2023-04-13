using System.Collections.Generic;
using UnityEngine;

namespace CollapseBlast.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ItemTypesData", menuName = "CollapseBlast/ItemTypesData", order = 1)]
    public class ItemTypesData : ScriptableObject
    {
        [SerializeField] List<Sprite> redBoxes;
        [SerializeField] List<Sprite> greenBoxes;
        [SerializeField] List<Sprite> blueBoxes;
        [SerializeField] List<Sprite> yellowBoxes;
        [SerializeField] List<Sprite> purpleBoxes;
        [SerializeField] List<Sprite> pinkBoxes;
        [SerializeField] List<Sprite> boosters;

        public List<Sprite> RedBoxes => redBoxes;
        public List<Sprite> GreenBoxes => greenBoxes;
        public List<Sprite> BlueBoxes => blueBoxes;
        public List<Sprite> YellowBoxes => yellowBoxes;
        public List<Sprite> PurpleBoxes => purpleBoxes;
        public List<Sprite> PinkBoxes => pinkBoxes;
        public List<Sprite> Boosters => boosters;
    }
}


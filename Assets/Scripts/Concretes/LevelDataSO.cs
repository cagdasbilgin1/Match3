using CollapseBlast.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollapseBlast.ScriptableObjects
{
    [CreateAssetMenu(fileName = "LevenConfigSO", menuName = "CollapseBlast/LevelConfigSO", order = 0)]
    public class LevelDataSO : ScriptableObject
    {
        [Tooltip("M")][SerializeField] int rows;
        [Tooltip("N")][SerializeField] int columns;
        [Tooltip("K")][SerializeField] List<ItemType> colors;
        [Tooltip("A")][SerializeField] int firstSpecialIconTypeThreshold;
        [Tooltip("B")][SerializeField] int secondSpecialIconTypeThreshold;
        [Tooltip("C")][SerializeField] int thirdSpecialIconTypeThreshold;
        [Tooltip("if empty, colors are chosen randomly")][SerializeField] TextAsset levelJson;
        [SerializeField] int minimumBlastableMatch;
        [SerializeField] ItemType goalItemType;
        [SerializeField] int goalCount;
        [SerializeField] int movesCount;


        public int Rows => rows;
        public int Columns => columns;
        public List<ItemType> ItemTypes => colors;
        public int FirstSpecialIconTypeThreshold => firstSpecialIconTypeThreshold;
        public int SecondSpecialIconTypeThreshold => secondSpecialIconTypeThreshold;
        public int ThirdSpecialIconTypeThreshold => thirdSpecialIconTypeThreshold;
        public TextAsset LevelJson => levelJson;
        public int MinimumBlastableCell => minimumBlastableMatch;
        public ItemType GoalItemType => goalItemType;
        public int GoalCount => goalCount;
        public int MovesCount => movesCount;
    }
}


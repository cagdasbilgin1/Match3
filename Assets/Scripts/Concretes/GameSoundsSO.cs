using UnityEngine;

namespace CollapseBlast.ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameSoundsSO", menuName = "CollapseBlast/GameSoundsSO", order = 4)]
    public class GameSoundsSO : ScriptableObject
    {
        [Header("Musics")]
        [SerializeField] AudioClip gameplayMusic;
        [SerializeField] AudioClip metaMusic;
        [SerializeField] AudioClip levelWin;
        [SerializeField] AudioClip levelLose;

        public AudioClip GameplayMusic => gameplayMusic;
        public AudioClip MetaMusic => metaMusic;
        public AudioClip LevelWin => levelWin;
        public AudioClip LevelLose => levelLose;

        [Header("Sounds")]
        [SerializeField] AudioClip itemBlastSound;
        [SerializeField] AudioClip rocketBoosterSound;
        [SerializeField] AudioClip tntBoosterSound;
        [SerializeField] AudioClip lightBombBoosterSound;

        public AudioClip ItemBlastSound => itemBlastSound;
        public AudioClip RocketBoosterSound => rocketBoosterSound;
        public AudioClip TntBoosterSound => tntBoosterSound;
        public AudioClip LightBombBoosterSound => lightBombBoosterSound;
    }
}


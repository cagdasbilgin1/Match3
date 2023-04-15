using UnityEngine;

namespace CollapseBlast.ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameSoundsSO", menuName = "CollapseBlast/GameSoundsSO", order = 4)]
    public class GameSoundsSO : ScriptableObject
    {
        [Header("Musics")]
        [SerializeField] AudioClip gameplayMusic;
        [SerializeField] AudioClip metaMusic;

        public AudioClip GameplayMusic => gameplayMusic;
        public AudioClip MetaMusic => metaMusic;

        [Header("Sounds")]
        [SerializeField] AudioClip itemBlastSound;

        public AudioClip ItemBlastSound => itemBlastSound;
    }
}


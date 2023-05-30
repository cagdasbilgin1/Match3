using CollapseBlast.Manager;
using CollapseBlast.ScriptableObjects;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] GameSoundsSO _gameSounds;
    [SerializeField] AudioMixerGroup musicMixerGroup;
    [SerializeField] AudioMixerGroup soundMixerGroup;

    AudioSource musicSource;
    AudioSource soundSource;
    AudioSourceManager audioSourceManager;

    const string mixerMusicGroupName = "Master/Music";
    const string mixerSoundGroupName = "Master/Sound";

    public GameSoundsSO GameSounds => _gameSounds;

    void Start()
    {
        GameManager.Instance.gamePlaySceneOpenedEvent += PlayGamePlayMusic;
        GameManager.Instance.metaSceneOpenedEvent += PlayMetaMusic;

        audioSourceManager = Camera.main.GetComponent<AudioSourceManager>();

        musicSource = audioSourceManager.MusicSource;
        musicSource.outputAudioMixerGroup = musicMixerGroup;

        soundSource = audioSourceManager.SoundSource;
        soundSource.outputAudioMixerGroup = soundMixerGroup;

        PlayMetaMusic();
    }

    public void PlayMusic(AudioClip music, float volume = 1, bool isLoop = true)
    {
        musicSource.volume = volume;
        musicSource.loop = isLoop;
        musicSource.clip = music;
        musicSource.outputAudioMixerGroup = GetMixerGroup(mixerMusicGroupName);
        musicSource.Play();
    }

    public void PlaySound(AudioClip sound, float volume = 1f)
    {
        soundSource.volume = volume;
        soundSource.clip = sound;
        soundSource.outputAudioMixerGroup = GetMixerGroup(mixerSoundGroupName);
        soundSource.Play();
    }

    private AudioMixerGroup GetMixerGroup(string mixerGroupName)
    {
        return audioMixer.FindMatchingGroups(mixerGroupName)[0];
    }

    void PlayGamePlayMusic()
    {
        PlayMusic(_gameSounds.GameplayMusic);
    }

    void PlayMetaMusic()
    {
        PlayMusic(_gameSounds.MetaMusic);
    }
}

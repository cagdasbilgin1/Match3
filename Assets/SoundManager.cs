using CollapseBlast.Manager;
using CollapseBlast.ScriptableObjects;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    [SerializeField] GameSoundsSO GameSounds;

    public AudioMixerGroup musicMixerGroup;
    public AudioMixerGroup soundMixerGroup;

    private AudioSource musicSource;
    private AudioSource soundSource;
    AudioSourceManager audioSourceManager;

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

    public void PlayMusic(AudioClip music, string mixerGroupName)
    {
        musicSource.clip = music;
        musicSource.outputAudioMixerGroup = GetMixerGroup(mixerGroupName);
        musicSource.Play();
    }

    public void PlaySound(AudioClip sound, string mixerGroupName)
    {
        soundSource.clip = sound;
        soundSource.outputAudioMixerGroup = GetMixerGroup(mixerGroupName);
        soundSource.Play();
    }

    private AudioMixerGroup GetMixerGroup(string mixerGroupName)
    {
        return audioMixer.FindMatchingGroups(mixerGroupName)[0];
    }

    void PlayGamePlayMusic()
    {
        PlayMusic(GameSounds.GameplayMusic, "Master/Music");
    }

    void PlayMetaMusic()
    {
        PlayMusic(GameSounds.MetaMusic, "Master/Music");
    }
}

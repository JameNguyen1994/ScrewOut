using PS.Utils;
using Storage;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;


public class AudioController : Singleton<AudioController>
{
    [SerializeField] private List<Sounds> sounds;
    [SerializeField] private List<Sounds> musics;
    //[SerializeField] private AudioClip[] testClips;
    //[SerializeField] private List<Sounds> soundsCombo;
    //[SerializeField] private List<Sounds> soundsEndCombo;
    private SoundName curMusicName;
    private Sounds curMusic;


    private void Start()
    {
        foreach (Sounds s in sounds)
        {
            s.audio = gameObject.AddComponent<AudioSource>();
            s.audio.clip = s.clip;
            s.audio.priority = 1;
            s.audio.playOnAwake = false;
            s.audio.volume = s.volume;
        }
        foreach (Sounds s in musics)
        {
            s.audio = gameObject.AddComponent<AudioSource>();
            s.audio.clip = s.clip;
            s.audio.playOnAwake = false;
            s.audio.volume = s.volume;
        }

        //musics[0].audio.Play();
        ChangeMusic(SoundName.Music, true);
        //if (IsMusic()) PlayMusic(SoundName.GameMusic, true);
        //  if (db.) PlayMusic(SoundName.GameMusic, true);
    }

    public void PlaySound(SoundName name, bool isUsePitch = false, bool isLoop = false)
    {
        if (!IsSound())
            return;
        Debug.Log($"PlaySound: {name}");
        Sounds sound = sounds.Find(x => x.name == name);
        if (sound != null)
        {
            if (isUsePitch)
            {
                sound.audio.priority = 1;
                sound.audio.pitch = Random.Range(0.8f, 1.2f);
            }

            sound.audio.loop = isLoop;
        }

        sound?.audio.Play();
    }

    //public void PlaySoundCombo(int id, bool isUsePitch = false)
    //{
    //    if (!IsSound())
    //        return;
    //    if (id > soundsCombo.Count - 1)
    //    {
    //        id = soundsCombo.Count - 1;
    //    }
    //    Sounds sound = soundsCombo[id];
    //    if (isUsePitch && sound != null)
    //    {
    //        sound.audio.priority = 1;
    //        sound.audio.pitch = Random.Range(0.8f, 1.2f);
    //    }
    //    sound?.audio.Play();
    //}
    //public void PlaySoundEndCombo(int id, bool isUsePitch = false)
    //{
    //    if (!IsSound())
    //        return;

    //    Sounds sound = soundsEndCombo[id];
    //    if (isUsePitch && sound != null)
    //    {
    //        sound.audio.priority = 1;
    //        sound.audio.pitch = Random.Range(0.8f, 1.2f);
    //    }
    //    sound?.audio.Play();
    //}

    // public void ChangeMusic(SoundName name, bool loop = false)
    // {
    //     curMusic?.audio.Stop();
    //     PlayMusic(name);
    // }

    public async UniTask ChangeMusic(SoundName name, bool loop = false)
    {

        if (!IsMusic())
            return;

        if (curMusic == null)
        {
            curMusic = musics.Find(x => x.name == name);
        }

        float currentVolume = curMusic.volume;
        await DOVirtual.Float(currentVolume, 0, 0.5f, volume =>
        {
            curMusic.audio.volume = volume;
        });

        curMusic = musics.Find(x => x.name == name);

        curMusic.audio.loop = loop;
        curMusic.audio.clip = curMusic.clip;
        curMusic.audio.Play();

        await DOVirtual.Float(0, currentVolume, 0.3f, volume =>
        {
            curMusic.audio.volume = volume;
        });
    }

    public void PlayMusic(SoundName name, bool loop = false, float volume = 0.25f)
    {
        if (!IsMusic())
            return;
        curMusic = musics.Find(x => x.name == name);

        if (curMusic != null)
        {
            curMusic.volume = volume;
            curMusic.audio.priority = 128;
            curMusic.audio.loop = loop;
            curMusic.audio.Play();
        }
    }

    public void UpdateSound(bool isON)
    {
        for (int i = 0; i < sounds.Count; i++)
        {
            sounds[i].audio.mute = !isON;
        }
    }

    public void UpdateMusic(bool isON)
    {
        for (int i = 0; i < musics.Count; i++)
        {
            musics[i].audio.mute = !isON;
        }
    }

    public bool IsSound() => Db.storage.SETTING_DATAS.sound;

    public bool IsMusic() => Db.storage.SETTING_DATAS.music;

    public void SetSound(bool isOn)
    {
        Db.storage.IS_SOUND = isOn;
        UpdateSound(isOn);
    }

    public void SetMusic(bool isOn)
    {
        //_db.IS_MUSIC = isOn;
        //UpdateMusic(isOn);
    }

    //public void ChangeMusic()
    //{
    //    _db.MUSIC = !_db.MUSIC;
    //    UpdateMusic(_db.MUSIC);
    //}

    //public void ChangeSound()
    //{
    //    _db.SOUND = !_db.SOUND;
    //    UpdateSound(_db.SOUND);
    //    _db.MUSIC = !_db.SOUND;
    //    UpdateMusic(_db.SOUND);
    //}

    public void AdjustAudioSourceVolume(AudioSource source, float value)
    {
        source.volume = Mathf.Clamp01(value);
    }

    public Sounds GetPlaySound(SoundName name, bool isLoop = false)
    {
        Sounds sound = sounds.Find(x => x.name == name);
        sound.audio.loop = isLoop;
        sound.audio.Play();
        return sound;
    }

    public void StopSound(SoundName name)
    {
        Sounds sound = sounds.Find(x => x.name == name);
        sound?.audio.Stop();
    }

    public void StopMusic(SoundName name)
    {
        Sounds music = musics.Find(x => x.name == name);
        music?.audio.Stop();
    }
}

[System.Serializable]
public class Sounds
{
    public SoundName name;
    [Range(0, 1)]
    public float volume = 1;
    public AudioClip clip;
    [HideInInspector] public AudioSource audio;
}

public enum SoundName
{
    Click = 0,
    Win = 1,
    Lose = 2,
    Merge = 3,
    Booster_AddHole = 4,
    Booster_Hammer = 5,
    Booster_Clear = 6,
    Booster_Magnet = 7,
    Effectt_Appear = 8,
    Popup = 9,
    Close_Popup_Booster = 10,
    Coin = 11,
    Star = 12,
    Win_Popup = 13,
    IAP_Complete = 14,
    Buy_Heart = 15,
    Lose_New = 16,
    Revive = 17,
    Music = 18,
    DuckDance = 19,
    ScrewClick = 20,
    ScrewUp = 21,
    ScrewDown = 22,
    ClearBox = 23,
    /////
    CollectCoin = 24,
    CollectHeart = 25,
    CollectBooster = 26,
    Ban_Ads = 27,
    CollectExp = 28,
    StartLevel = 29,

    // Quest
    QuestProcess = 30,
    GiftProcess = 31,
    OpenGift = 32,
    QuestStar = 33,


    Fly = 34,
    LevelStart_ScrewDown = 35,

    Anim_Box_Open = 36,
    Anim_Screw_Down = 37,
    Anim_Box_Move = 38,

    WIN_WITH_TRUMPET = 39,
    WIN_WITH_FIREWORK = 40,
    COIN_LEVEL_BONUS = 41,
    MUSIC_LEVEL_BONUS = 42,
    CLOCK_ROTATE = 43,

    CAP_EXPLOSION = 44,
    CAP_SWIPE = 45,

    COLLECT_SCREW = 46,
    WEEKLY_PROCESS = 47,
    LEVEL_BONUS_COMPLETE = 48,
}

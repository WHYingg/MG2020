using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 音乐模块
/// </summary>
public class MusicManager : ManagerBase<MusicManager>
{
    private AudioSource bgAudioSource = null;
    /// <summary>
    /// 多音乐构成的背景音乐音频合集
    /// </summary>
    private List<AudioClip> bgClips = new List<AudioClip>();
    /// <summary>
    /// 背景音乐音量大小
    /// </summary>
    private float bgValue = 1;

    private int bgClipsIndex = 0;
    private bool isMultipleBgMusic = false;
    private bool isSwitch = false;

    private GameObject sound2DObj = null;
    private List<AudioSource> sound2DList = new List<AudioSource>();
    private float soundValue = 1;

    public MusicManager()
    {
        MonoManager.GetInstance().AddUpdateListener(OnUpdate);
        EventCenter.AddListener(EventDefine.SceneSwitch, () => { MonoManager.GetInstance().RemoveUpdateListener(OnUpdate); });
        //TODO 根据玩家设置初始化音量、音效大小

    }

    private void OnUpdate()
    {
        RemoveDie2DSound();
        //TODO 根据玩家设置音量、音效大小

    }

    #region 背景音乐

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="path">音频路径</param>
    /// <param name="isLoop">是否循环</param>
    public void PlayBgMusic(AudioClip clip, bool isLoop)
    {
        if (bgAudioSource == null)
        {
            GameObject obj = new GameObject("BgMusic");
            bgAudioSource = obj.AddComponent<AudioSource>();
        }
        bgAudioSource.clip = clip;
        bgAudioSource.volume = bgValue;
        bgAudioSource.loop = isLoop;
        bgAudioSource.Play();
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="path">音频路径</param>
    public void PlayBgMusic(string path, bool isLoop)
    {
        if (bgAudioSource == null)
        {
            GameObject obj = new GameObject("BgMusic");
            bgAudioSource = obj.AddComponent<AudioSource>();
        }
        ResourceManager.GetResourceManager().LoadAsync<AudioClip>(path, (clip) =>
         {
             bgAudioSource.clip = clip;
             bgAudioSource.volume = bgValue;
             bgAudioSource.loop = isLoop;
             bgAudioSource.Play();
         });
    }

    /// <summary>
    /// 背景音乐暂停
    /// </summary>
    public void PauseBgMusic()
    {
        if (bgAudioSource == null)
        {
            return;
        }
        bgAudioSource.Pause();
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBgMusic()
    {
        if (bgAudioSource == null)
        {
            return;
        }
        bgAudioSource.Stop();
    }

    /// <summary>
    /// 改变背景音乐 音量大小
    /// </summary>
    /// <param name="v"></param>
    public void ChangeBgValue(float v)
    {
        if (bgAudioSource == null)
        {
            return;
        }
        bgValue = v;
        bgAudioSource.volume = bgValue;
    }

    /// <summary>
    /// 切换背景音乐
    /// </summary>
    /// <param name="clip">音频</param>
    public void SwitchBgMusic(AudioClip clip)
    {
        if (bgAudioSource == null)
        {
            return;
        }
        bgAudioSource.Pause();
        bgAudioSource.clip = clip;
        bgAudioSource.Play();
    }

    /// <summary>
    /// 播放由多个音频组成的背景音乐
    /// </summary>
    /// <param name="clips"></param>
    /// <param name="isLoop"></param>
    public void PlayMultipleBgMusic(List<AudioClip> clips)
    {
        if (bgAudioSource == null)
        {
            GameObject obj = new GameObject("MultipleBgMusic");
            bgAudioSource = obj.AddComponent<AudioSource>();
        }
        bgClips.Clear();
        foreach (var item in clips)
        {
            bgClips.Add(item);
        }
        bgAudioSource.loop = false;
        bgAudioSource.clip = bgClips[bgClipsIndex];
        bgAudioSource.volume = bgValue;
        bgAudioSource.Play();
        isSwitch = false;
        isMultipleBgMusic = true;
    }

    /// <summary>
    /// 切换多音频背景音乐
    /// </summary>
    private void SwitchMultipleBgMusic()
    {
        if (isMultipleBgMusic)
        {
            if (isSwitch)
            {
                bgAudioSource.clip = bgClips[bgClipsIndex];
                bgAudioSource.Play();
                isSwitch = false;
            }

            if (!bgAudioSource.isPlaying)
            {
                bgClipsIndex = (bgClipsIndex + 1) % bgClips.Count;
                isSwitch = true;
            }
        }

    }

    #endregion

    #region 2D音效

    /// <summary>
    /// 播放2D音效
    /// </summary>
    /// <param name="clip">音频</param>
    public void Play2DSound(AudioClip clip, bool isLoop)
    {
        if (sound2DObj == null)
        {
            sound2DObj = new GameObject("2DSound");
        }
        AudioSource source = sound2DObj.AddComponent<AudioSource>();
        source.clip = clip;
        source.Play();
        source.volume = soundValue;
        sound2DList.Add(source);
    }

    /// <summary>
    /// 播放2D音效
    /// </summary>
    /// <param name="path">音频路径</param>
    public void Play2DSound(string path, bool isLoop)
    {
        if (sound2DObj == null)
        {
            sound2DObj = new GameObject("2DSound");
        }
        ResourceManager.GetResourceManager().LoadAsync<AudioClip>(path, (clip) =>
        {
            AudioSource source = sound2DObj.AddComponent<AudioSource>();
            source.clip = clip;
            source.Play();
            source.volume = soundValue;

            sound2DList.Add(source);
        });
    }

    /// <summary>
    /// 播放2D音效
    /// </summary>
    /// <param name="path">音频路径</param>
    public void Play2DSound(string path, bool isLoop, UnityAction<AudioSource> callBack)
    {
        if (sound2DObj == null)
        {
            sound2DObj = new GameObject("2DSound");
        }
        ResourceManager.GetResourceManager().LoadAsync<AudioClip>(path, (clip) =>
        {
            AudioSource source = sound2DObj.AddComponent<AudioSource>();
            source.clip = clip;
            source.Play();
            source.volume = soundValue;
            sound2DList.Add(source);
            source.loop = isLoop;
            callBack(source);
        });
    }

    /// <summary>
    /// 停止音效播放
    /// </summary>
    /// <param name="source"></param>
    public void StopSound(AudioSource source)
    {
        if (sound2DList.Contains(source))
        {
            sound2DList.Remove(source);
            source.Stop();
            GameObject.Destroy(source);
        }
    }

    /// <summary>
    /// 改变音效音量大小
    /// </summary>
    /// <param name="value"></param>
    public void ChangeSoundValue(float value)
    {
        soundValue = value;
        foreach (var item in sound2DList)
        {
            item.volume = soundValue;
        }
    }

    /// <summary>
    /// 移除播放完成了的2D音效
    /// </summary>
    private void RemoveDie2DSound()
    {
        for (int i = sound2DList.Count - 1; i >= 0; --i)
        {
            if (!sound2DList[i].isPlaying)
            {
                GameObject.Destroy(sound2DList[i]);
                sound2DList.RemoveAt(i);
            }
        }

    }

    #endregion

}

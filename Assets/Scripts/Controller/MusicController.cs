using UnityEngine;

/// <summary>
/// 全局背景音乐控制器，继承自单例基类。
/// 由于基类 DRMSingleton 默认执行了 DontDestroyOnLoad，该脚本会持续存在。
/// </summary>
public class MusicController : DRMSingleton<MusicController>
{
    [Header("BGM Settings")]
    public AudioClip bgmClip;
    [Range(0f, 1f)]
    public float volume = 0.5f;
    public bool playOnAwake = true;

    private AudioSource _audioSource;

    protected override void Awake()
    {
        base.Awake();
        
        // 如果是重复实例，base.Awake() 会将其销毁，因此这里需要再次检查确保 _instance 是当前对象
        if (Instance != this) return;

        InitAudioSource();
    }

    private void Start()
    {
        if (playOnAwake && bgmClip != null)
        {
            PlayBGM(bgmClip);
        }
    }

    private void InitAudioSource()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        _audioSource.loop = true;
        _audioSource.playOnAwake = false;
        _audioSource.volume = volume;
    }

    /// <summary>
    /// 播放指定的背景音乐
    /// </summary>
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        
        if (_audioSource.clip == clip && _audioSource.isPlaying) return;

        _audioSource.clip = clip;
        _audioSource.Play();
    }

    /// <summary>
    /// 停止播放背景音乐
    /// </summary>
    public void StopBGM()
    {
        _audioSource.Stop();
    }

    /// <summary>
    /// 动态调节音量
    /// </summary>
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (_audioSource != null)
        {
            _audioSource.volume = volume;
        }
    }
}

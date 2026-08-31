using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField]
    private AudioSource footstepsSource;

    [SerializeField]
    private AudioSource hitSource;

    [SerializeField]
    private AudioSource ambientSource;

    [Header("Footsteps")]
    [SerializeField]
    private AudioClip[] footstepClips;

    [SerializeField]
    [Range(0f, 3f)]
    private float footstepsVolume = 1.5f;

    [Header("Enemy Hit")]
    [SerializeField]
    private AudioClip[] enemyHitClips;

    [SerializeField]
    [Range(0f, 3f)]
    private float enemyHitVolume = 1.5f;

    [Header("Wood Hit")]
    [SerializeField]
    private AudioClip[] woodHitClips;

    [SerializeField]
    [Range(0f, 3f)]
    private float woodHitVolume = 1.8f;

    [Header("Stone Hit")]
    [SerializeField]
    private AudioClip[] stoneHitClips;

    [SerializeField]
    [Range(0f, 3f)]
    private float stoneHitVolume = 1.8f;

    [Header("Ambient")]
    [SerializeField]
    private AudioClip ambientClip;

    [SerializeField]
    [Range(0f, 1f)]
    private float ambientVolume = 0.25f;

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartAmbient();
    }

    // =========================================================
    // FOOTSTEPS
    // =========================================================

    public void PlayFootstep()
    {
        if (footstepsSource == null)
        {
            return;
        }

        if (footstepClips == null ||
            footstepClips.Length == 0)
        {
            return;
        }

        AudioClip clip =
            GetRandomClip(
                footstepClips
            );

        if (clip == null)
        {
            return;
        }

        footstepsSource.PlayOneShot(
            clip,
            footstepsVolume
        );
    }

    // =========================================================
    // ENEMY HIT
    // =========================================================

    public void PlayEnemyHit()
    {
        PlayRandomHit(
            enemyHitClips,
            enemyHitVolume
        );
    }

    // =========================================================
    // WOOD HIT
    // =========================================================

    public void PlayWoodHit()
    {
        PlayRandomHit(
            woodHitClips,
            woodHitVolume
        );
    }

    // =========================================================
    // STONE HIT
    // =========================================================

    public void PlayStoneHit()
    {
        PlayRandomHit(
            stoneHitClips,
            stoneHitVolume
        );
    }

    // =========================================================
    // AMBIENT
    // =========================================================

    private void StartAmbient()
    {
        if (ambientSource == null)
        {
            return;
        }

        if (ambientClip == null)
        {
            return;
        }

        ambientSource.clip =
            ambientClip;

        ambientSource.loop = true;

        ambientSource.volume =
            ambientVolume;

        ambientSource.Play();
    }

    public void StopAmbient()
    {
        if (ambientSource == null)
        {
            return;
        }

        ambientSource.Stop();
    }

    public void StartAmbientAgain()
    {
        StartAmbient();
    }

    // =========================================================
    // INTERNAL
    // =========================================================

    private void PlayRandomHit(
        AudioClip[] clips,
        float volume
    )
    {
        if (hitSource == null)
        {
            return;
        }

        if (clips == null ||
            clips.Length == 0)
        {
            return;
        }

        AudioClip clip =
            GetRandomClip(
                clips
            );

        if (clip == null)
        {
            return;
        }

        hitSource.PlayOneShot(
            clip,
            volume
        );
    }

    private AudioClip GetRandomClip(
        AudioClip[] clips
    )
    {
        if (clips == null ||
            clips.Length == 0)
        {
            return null;
        }

        int index =
            Random.Range(
                0,
                clips.Length
            );

        return clips[index];
    }
}
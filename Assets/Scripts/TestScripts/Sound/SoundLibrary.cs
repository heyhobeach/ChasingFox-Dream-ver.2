using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundLibrary : MonoBehaviour
{
    public SoundClipPackage[] soundClips; // Array to hold sound clips
    private AudioSource audioSource; // AudioSource component
    private SoundClip currentClip; // Current sound clip being

    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (audioSource.isPlaying && audioSource.time >= currentClip.playRange.y) StopSound();
    }

    public void PlaySoundOneShot(Object clip)
    {
        if (clip == null)
        {
            Debug.LogError("Sound clip is not a valid AudioClip!");
            return;
        }
        switch(clip.GetType().Name)
        {
            case nameof(SoundClipPackage):
                if((clip as SoundClipPackage).soundClips.Length == 0)
                {
                    Debug.LogError("Sound clip package is empty!");
                    return;
                }
                if((clip as SoundClipPackage).soundClips.Length > 1)
                {
                    Debug.LogWarning("Sound clip package has more than one sound clip!");
                }
                var temp = (clip as SoundClipPackage).soundClips[0];
                audioSource.time = temp.playRange.x;
                audioSource.PlayOneShot(temp.clip);
                break;
            case nameof(SoundClip):
                var tempClip = clip as SoundClip;
                audioSource.time = tempClip.playRange.x;
                audioSource.PlayOneShot(tempClip.clip);
                break;
            default:
                Debug.LogError("Sound clip is not a valid AudioClip!");
                return;
        }

    }
    public void RandomPlaySoundOneShot(Object clip)
    {
        if (clip == null || clip.GetType() != typeof(SoundClipPackage))
        {
            Debug.LogError("Sound clip is not a valid AudioClip!");
            return;
        }
        var randomIndex = Random.Range(0, (clip as SoundClipPackage).soundClips.Length);
        PlaySoundOneShot((clip as SoundClipPackage).soundClips[randomIndex]);
    }

    // Field clips

    public void PlaySoundOneShot(Vector2Int index)
    {
        if (index.x < 0 || index.x >= soundClips.Length || index.y < 0 || index.y >= soundClips[index.x].soundClips.Length)
        {
            Debug.LogError("Sound index out of range!");
            return;
        }

        audioSource.time = soundClips[index.x].soundClips[index.y].playRange.x;
        audioSource.PlayOneShot(soundClips[index.x].soundClips[index.y].clip);
        currentClip = soundClips[index.x].soundClips[index.y];
    }
    public void RandomPlaySoundOneShot(int index)
    {
        if (index < 0 || index >= soundClips.Length)
        {
            Debug.LogError("Sound index out of range!");
            return;
        }
        var randomIndex = Random.Range(0, soundClips[index].soundClips.Length);
        audioSource.time = soundClips[index].soundClips[randomIndex].playRange.x;
        audioSource.PlayOneShot(soundClips[index].soundClips[randomIndex].clip);
        currentClip = soundClips[index].soundClips[randomIndex];
    }

    public void PlaySound(Vector2Int index)
    {
        if (index.x < 0 || index.x >= soundClips.Length || index.y < 0 || index.y >= soundClips[index.x].soundClips.Length)
        {
            Debug.LogError("Sound index out of range!");
            return;
        }

        audioSource.clip = soundClips[index.x].soundClips[index.y].clip;
        audioSource.time = soundClips[index.x].soundClips[index.y].playRange.x;
        audioSource.Play();
        currentClip = soundClips[index.x].soundClips[index.y];
    }
    public void StopSound()
    {
        audioSource.Stop();
        currentClip = null;
    }
}

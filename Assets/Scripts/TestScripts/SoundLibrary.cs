using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundLibrary : MonoBehaviour
{
    public AudioClip[] soundClips; // Array to hold sound clips
    private AudioSource audioSource; // AudioSource component

    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public void PlaySound(int index)
    {
        if (index < 0 || index >= soundClips.Length)
        {
            Debug.LogError("Sound index out of range!");
            return;
        }

        audioSource.clip = soundClips[index];
        audioSource.Play();
    }
    public void PlaySoundOneShot(int index)
    {
        if (index < 0 || index >= soundClips.Length)
        {
            Debug.LogError("Sound index out of range!");
            return;
        }

        audioSource.PlayOneShot(soundClips[index]);
    }
    public void StopSound(int index)
    {
        if (index < 0 || index >= soundClips.Length)
        {
            Debug.LogError("Sound index out of range!");
            return;
        }

        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource != null && audioSource.clip == soundClips[index])
        {
            audioSource.Stop();
        }
    }
}

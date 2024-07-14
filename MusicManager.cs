using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public AudioClip musicToPlay;
    private AudioSource musicSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicSource = GetComponent<AudioSource>();
        musicSource.clip = musicToPlay;
        musicSource.loop = true;
        musicSource.Play();
    }
}

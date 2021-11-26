using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager current;

    [Header("Setup")]
    [SerializeField] AudioSource audioSource;
    private AudioClip currentClip;

    private void Awake() {
        current = this;
    }

    [Header("UI Sounds")]
    [SerializeField] AudioClip popUpSound;
    [SerializeField] AudioClip nextButtonSound;
    [SerializeField] AudioClip confirmSound;
    [SerializeField] AudioClip cancelSound;
    //[SerializeField] AudioClip selectSound;
    


    private void PlayClip(AudioClip clip)
    {
        if (!audioSource.isPlaying)
        {
            currentClip = clip;
            audioSource.clip = currentClip;
            audioSource.Play();

        }
    }

    public void PlaySound_PopUp()
    {
        PlayClip(popUpSound);
        
    }

    public void PlaySound_NextButton()
    {
        PlayClip(nextButtonSound);

    }

    public void PlaySound_Confirm()
    {
        PlayClip(confirmSound);

    }

    public void PlaySound_Cancel()
    {
        PlayClip(cancelSound);

    }

    // public void PlaySound_Select()
    // {
    //     PlayClip(selectSound);
    // }
    
}

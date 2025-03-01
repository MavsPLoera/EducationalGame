using UnityEngine;

public class Audio_Controller : MonoBehaviour
{
    public AudioClip buttonClickSound;
    private AudioSource audioSource; //thing that will play the audio clip

    public void Start()
    {
        //Dont need to specifiy since component is already on the object.
        audioSource = GetComponent<AudioSource>();
    }

    public void playButtonSound()
    {
        //Play one shot will play even if gameobject is destoryed

        //How to change pitch?
        audioSource.PlayOneShot(buttonClickSound);
    }
}

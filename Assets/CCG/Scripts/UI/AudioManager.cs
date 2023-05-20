using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static AudioClip youWinSound;
    public static AudioClip kartAtmaSound;
    public static AudioClip kartCekmeSound;
    public static AudioClip kartSaldýrýSound;
    public static AudioClip yeniTurSound;
    public static AudioClip youLostSound;
    static AudioSource audioSrc;
    void Start()
    {
        youWinSound = Resources.Load<AudioClip>("youWin");
        youLostSound = Resources.Load<AudioClip>("youLost");
        kartAtmaSound = Resources.Load<AudioClip>("kartAtma");
        kartCekmeSound = Resources.Load<AudioClip>("kartCekme");
        kartSaldýrýSound = Resources.Load<AudioClip>("kartSaldýrý");
        yeniTurSound = Resources.Load<AudioClip>("yeniTur");
        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "kartSaldýrý":
                audioSrc.PlayOneShot(kartSaldýrýSound);
                break;
            case "kartCekme":
                audioSrc.PlayOneShot(kartCekmeSound);
                break;
            case "yeniTur":
                audioSrc.PlayOneShot(yeniTurSound);
                break;
            case "kartAtma":
                audioSrc.PlayOneShot(kartAtmaSound);
                break;
            case "youWin":
                audioSrc.PlayOneShot(youWinSound);
                break;
            case "youLose":
                audioSrc.PlayOneShot(youLostSound);
                break;


        }
    }
}

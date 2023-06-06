using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : Singleton<SFXManager>
{
    private AudioSource m_AudioSource;
    public SFX_SO sfxList;
    // Start is called before the first frame update
    private void OnEnable()
    {
        //EventHandler.ItemInteractEvent += PlayAudioOnInteract;
        EventHandler.SmellEvent+=PlayAudioOnSmell;  
        EventHandler.TasteEvent+=PlayAudioOnTaste;  
        //EventHandler.ItemPickUpEvent+=PlayAudioOnInteract;  

        m_AudioSource = GetComponent<AudioSource>();
    }

    private void OnDisable()
    {
        //EventHandler.ItemInteractEvent -= PlayAudioOnInteract;
        //EventHandler.ItemPickUpEvent -= PlayAudioOnInteract;
    }
    public void PlayAudioOnInteract(string Name, GameObject gameObj)
    {

        m_AudioSource.clip = sfxList.GetAudioClipByString("spoonOut");
        if (m_AudioSource.clip)
        {
            m_AudioSource.Play();
        }

    }

    public void PlayAudioOnSpoonOut(string Name,GameObject gameObj)
    {
        m_AudioSource.clip=sfxList.GetAudioClipByString("spoonOut");
        if (m_AudioSource.clip)
        {
            m_AudioSource.Play();
        }

    }

    public void PlayAudioOnPour(string Name, GameObject gameObj)
    {
        m_AudioSource.clip = sfxList.GetAudioClipByString("drop");
        if (m_AudioSource.clip)
        {
            m_AudioSource.Play();
        }

    }

    public void PlayAudioOnSmell(string Name, uint degree,GameObject obj)
    {
        m_AudioSource.clip = sfxList.GetAudioClipByString("smell");
        if (m_AudioSource.clip)
        {
            m_AudioSource.Play();
        }
    }

    public void PlayAudioOnTaste(string Name, uint degree, GameObject obj)
    {
        m_AudioSource.clip = sfxList.GetAudioClipByString("taste");
        if (m_AudioSource.clip)
        {
            m_AudioSource.Play();
        }
    }
}

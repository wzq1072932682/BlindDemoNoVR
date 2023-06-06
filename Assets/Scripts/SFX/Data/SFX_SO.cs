using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="SFX_SO",menuName ="BlindDemo/SFX/SFX_SO")]
public class SFX_SO : ScriptableObject
{
    public List<SFXData> SFX_Data;

    public AudioClip GetAudioClipByString(string itemName)
    {
        var audio = SFX_Data.Find(i => i.itemName == itemName);
        if (audio != null)
        {
            return audio.audioClip;
        }
        else
        {
            return null;
        }
        //}
        //if (SFX_Data.Find(i => i.itemName == itemName) != null)
        //{
        //    return 
        //}
        //return SFX_Data.Find(i=>i.itemName == itemName).audioClip;
    }
}

[System.Serializable]
public class SFXData
{
    public string itemName;
    public AudioClip audioClip;
}
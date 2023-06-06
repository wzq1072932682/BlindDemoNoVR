using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SaltLogic : MonoBehaviour
{
    [SerializeField]
    private uint MaxSalt;
    [SerializeField]
    private uint MinSalt;
    public uint SaltRange { get =>MaxSalt-MinSalt; }
    public uint CurrentSalt = 0;
    private void OnEnable()
    {
        CurrentSalt = 0;
        EventHandler.TasteEvent += SetSalt;

    }

    private void OnDisable()
    {
        EventHandler.TasteEvent -= SetSalt;
    }

    void SetSalt(string type,uint degree, GameObject obj)
    {
        if (type == eTasteType.salt.ToString())
        {
            CurrentSalt = Math.Clamp(degree,MinSalt,MaxSalt);
        }
    }
}

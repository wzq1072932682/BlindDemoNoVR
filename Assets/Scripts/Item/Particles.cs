using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles : MonoBehaviour
{
    public eTasteType tasteType;
    public uint tasteValue;

    private SphereCollider collider;
    private void OnEnable()
    {
        collider=this.gameObject.GetComponent<SphereCollider>();
        Destroy(this.gameObject, 5.0f);
    }

    Particles(eTasteType tasteType, uint tasteValue, SphereCollider collider)
    {
        this.tasteType = tasteType;
        this.tasteValue = tasteValue;
        this.collider = collider;
    }

    private void OnTriggerEnter(Collider other)
    {
        var itemComp = other.GetComponent<Item>();
        if(itemComp != null)
        {
            if(itemComp.type==eItemType.pot)
            {
                itemComp.tasteValue=tasteValue;
            }
        }
    }
}

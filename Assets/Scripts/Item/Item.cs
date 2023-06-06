using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour
{
    public eHandSide handSide;
    public eItemType type;

    public bool bCanBeTaste;
    public bool bCanBeSmell;

    //public uint currentValue;

    public eTasteType tasteType;
    public uint tasteValue;

    public eSmellType smellType;
    public uint smellValue;

    private Item currentCollideItem;

    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            var item = other.gameObject.GetComponent<Item>();
            if (item != null)
            {
                currentCollideItem = item;
                EventHandler.CallItemHoverOnEvent(currentCollideItem.type.ToString(), currentCollideItem.gameObject);
                if (this == InteractiveManager.Instance.ItemOnHand)
                {
                    InteractiveManager.Instance.SetTargetItem(currentCollideItem.gameObject);
                }
                if (this.tasteValue == 0&&currentCollideItem.type==eItemType.container)//nothing on the spoon
                {
                    EventHandler.CallChangeIndicatorEvent(IndicateState.SpoonOut);
                }
                    //

            }
            //if (currentCollideItem != null)
            //{
            //    Debug.Log("collide with " + currentCollideItem.type.ToString());
            //}
        }
    }

    private void Update()
    {
        // if (type == eItemType.tool)
        // {
        //     Debug.Log(this.gameObject.transform.position);
        // }
    }


    private void OnTriggerExit(Collider other)
    {
        if (currentCollideItem&&currentCollideItem != InteractiveManager.Instance.ItemOnHand)
        {
            EventHandler.CallItemHoverOffEvent(currentCollideItem.type.ToString(), currentCollideItem.gameObject);
            currentCollideItem = null;
            EventHandler.CallChangeIndicatorEvent(IndicateState.TasteAndSmell);
            //EventHandler.CallChangeIndicatorEvent(IndicateState.RNone);
        }
    }
    public void Interact()
    {
        bool bHasInteract = false;
        if(currentCollideItem != null)
        {
            Debug.Log(type.ToString() + " interact with " + currentCollideItem.type.ToString());

            if (type == eItemType.tool && currentCollideItem.type == eItemType.container && tasteValue == 0)
            {
                if (!bHasInteract)
                {
                    tasteValue = 1;
                    EventHandler.CallChangeUIEvent("用勺子舀了盐");
                    SFXManager.Instance.PlayAudioOnSpoonOut("spoonOut", null);
                    //animation here;
                    var SpoonOutIEnumerator = SpoonOut();
                    StartCoroutine(SpoonOutIEnumerator);
                    InteractiveManager.Instance.SpoonOutCoroutine = SpoonOutIEnumerator;
                    //InteractiveManager.Instance.Director.Play();
                    //Spoon and container situation;
                    bHasInteract = true;
                }
                
            }
        }
        if ((type == eItemType.tool||type==eItemType.container) && this.tasteValue > 0)
        //else if(type == eItemType.tool && currentCollideItem.type == eItemType.pot)
        {
            if (!bHasInteract)
            {
                var PourIEnumerator = Pour();
                StartCoroutine(PourIEnumerator);
                //currentCollideItem.tasteValue = tasteValue;
                SFXManager.Instance.PlayAudioOnPour("drop", null);
                EventHandler.CallChangeUIEvent("把盐倒出来");
                EventHandler.CallChangeIndicatorEvent(IndicateState.BothNone);
                tasteValue = 0;
                //Spoon and Pot situation;
                bHasInteract= true;
            }
        }
    }

    private IEnumerator SpoonOut()
    {
        transform.localPosition = new Vector3(0.0f, -0.1f, 0.2f);
        float angle = 0.0f;
        var currentAngle = transform.localEulerAngles;
        float elapsedTime = 0.0f;
        float waitTime = 2.0f;
         while (elapsedTime < waitTime&&InteractiveManager.Instance.ItemOnHand==this)
        {
            transform.localEulerAngles = Vector3.Lerp(currentAngle, new Vector3(0.0f,180.0f,90.0f), elapsedTime);
            elapsedTime+= Time.deltaTime*2.0f;
            yield return null;
        }
        transform.localEulerAngles = new Vector3(0.0f, 180.0f, 90.0f);
        yield return new WaitForSeconds(0.5f);
        currentAngle=transform.localEulerAngles;
        elapsedTime = 0.0f;
        while (elapsedTime < waitTime && InteractiveManager.Instance.ItemOnHand == this)
        {
            transform.localEulerAngles = Vector3.Lerp(currentAngle, new Vector3(0.0f, 180.0f,0.0f), elapsedTime);
            elapsedTime += Time.deltaTime * 2.0f;
            yield return null;
        }
        if(InteractiveManager.Instance.ItemOnHand == this)
        {
            transform.localEulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
            transform.localPosition = Vector3.zero;
        }
        yield return null;
    }

    private IEnumerator Pour()
    {
        float angle = 0.0f;
        var currentAngle = transform.localEulerAngles;
        float elapsedTime = 0.0f;
        float waitTime = 2.0f;
        while (elapsedTime < waitTime && InteractiveManager.Instance.ItemOnHand == this)
        {
            transform.localEulerAngles = Vector3.Lerp(currentAngle, new Vector3(0.0f, 180.0f, 90.0f), elapsedTime);
            elapsedTime += Time.deltaTime * 2.0f;
            yield return null;
        }
        transform.localEulerAngles = new Vector3(0.0f, 180.0f, 90.0f);
        yield return new WaitForSeconds(0.5f);
        Instantiate(InteractiveManager.Instance.SaltParticle,this.gameObject.transform.position,Quaternion.identity);
        currentAngle = transform.localEulerAngles;
        elapsedTime = 0.0f;
        while (elapsedTime < waitTime && InteractiveManager.Instance.ItemOnHand == this)
        {
            transform.localEulerAngles = Vector3.Lerp(currentAngle, new Vector3(0.0f, 180.0f, 0.0f), elapsedTime);
            elapsedTime += Time.deltaTime * 2.0f;
            yield return null;
        }
        if (InteractiveManager.Instance.ItemOnHand == this)
        {
            transform.localEulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
        }
        yield return null;
    }
}

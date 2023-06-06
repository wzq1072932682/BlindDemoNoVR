using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Indicate_UI : MonoBehaviour
{


    private Indicate_Logic indicate_Logic;
    public Image indicateSlot_E;
    public Image indicateSlot_R;
    //public Image image_indicate2;
    public Sprite sprite_SmellAndTaste;
    public Sprite sprite_Pour;
    public Sprite sprite_SpoonOut;

    public bool bHand;
    // Start is called before the first frame update

    private void OnEnable()
    {
        indicate_Logic= GetComponent<Indicate_Logic>();
        HideIndicator("",null);
        EventHandler.ItemHoverOnEvent += ShowIndicatorOnHover;
        EventHandler.ItemHoverOffEvent += HideIndicator;
        EventHandler.InteractiveEvent_Internal += ShowIndicatorOnInteract;
        EventHandler.SmellEvent += HideIndicatorOnTasteOrSmell;
        EventHandler.TasteEvent += HideIndicatorOnTasteOrSmell;

        EventHandler.ItemPickUpEvent += ShowIndicatorTool;
        EventHandler.ItemHoverOffEvent += ShowIndicatorTool;

    bHand = false;
    }

    public void SetIndicateSprite(int idx)
    {
        if (idx == 0)
        {
            indicateSlot_E.enabled = false;
            indicateSlot_E.sprite = null;
        }//E none
        else if(idx==1)
        {
            indicateSlot_R.enabled = true;
            indicateSlot_R.sprite = sprite_SpoonOut;
        }//spoon out
        else if (idx == 2)
        {
            indicateSlot_R.enabled = true;
            indicateSlot_R.sprite = sprite_Pour;
        }//rotate
        else if (idx == 3)
        {
            indicateSlot_E.enabled = true;
            indicateSlot_E.sprite = sprite_SmellAndTaste;
        }//tasteAndSmell
        else if (idx == 4)
        {
            indicateSlot_R.enabled = false;
            indicateSlot_R.sprite = null;
        }//R none
        else if (idx == 5)
        {
            indicateSlot_E.enabled = false;
            indicateSlot_E.sprite = null;
            indicateSlot_R.enabled = false;
            indicateSlot_R.sprite = null;
        }
    }
    private void Start()
    {
        HideIndicator("",null);
    }
    private void Update()
    {
        
    }
    private void OnDisable()
    {
        EventHandler.ItemHoverOnEvent -= ShowIndicatorOnHover;
        EventHandler.ItemHoverOffEvent -= HideIndicator;
        EventHandler.InteractiveEvent_Internal -= ShowIndicatorOnInteract;
        EventHandler.SmellEvent -= HideIndicatorOnTasteOrSmell;
        EventHandler.TasteEvent -= HideIndicatorOnTasteOrSmell;

        EventHandler.ItemPickUpEvent -= ShowIndicatorTool;
        EventHandler.ItemHoverOffEvent -= ShowIndicatorTool;
    }
    void ShowIndicatorOnHover(string str, GameObject item)
    {
        var itemType = (eItemType)System.Enum.Parse(typeof(eItemType), str);
        var itemComp=item.GetComponent<Item>();
        if (!InteractiveManager.Instance.bHandEmpty&&InteractiveManager.Instance.ItemOnHand.type == eItemType.tool)
        {
            if (itemComp!=null&&itemComp.type==eItemType.container)
            {
                //image_indicate1.enabled = true;
                //image_indicate1.sprite = sprite_SpoonOut;
                bHand = false;
                
            }
        }
    }

    void ShowIndicatorOnHavingSalt(string str, GameObject item)
    {
        var itemComp = item.GetComponent<Item>();
        if (itemComp.tasteValue > 0)
        {
            //image_indicate1.enabled = true;
            //image_indicate1.sprite = sprite_Smell;
            
            //image_indicate2.enabled = true;
            //image_indicate2.sprite = sprite_Pour;
        }
    }

    void ShowIndicatorOnInteract(GameObject from, GameObject to, string type, uint value)
    {
        var fromItem = from.GetComponent<Item>();
        var toItem = to.GetComponent<Item>();
        if (toItem.type == eItemType.tool)
        {
            //if (value > 0)
            //{
            //image_indicate1.enabled = true;
            //image_indicate1.sprite = sprite_Smell;
            
            //image_indicate2.enabled = true;
            //image_indicate2.sprite = sprite_Taste;
            bHand = true;
            //}
        }
    }



    void ShowIndicatorTool(string type, GameObject obj)
    {
        var Item = obj.GetComponent<Item>();

        if (Item.type == eItemType.tool)
        {
            //image_indicate1.enabled = true;
            //image_indicate1.sprite = sprite_Smell;

            //image_indicate2.enabled = true;
            //image_indicate2.sprite = sprite_Taste;
            bHand = true;
        }
    }

    void HideIndicator(string str, GameObject item)
    {
        if (!bHand)
        {
            //image_indicate1.enabled = false;
            //image_indicate1.sprite = null;

            //image_indicate2.enabled = false;
            //image_indicate2.sprite = null;
        }
    }
    // Update is called once per frame
    void HideIndicatorOnTasteOrSmell(string type, uint degree, GameObject obj)
    {
        //image_indicate1.enabled = false;
        //image_indicate1.sprite = null;

        //image_indicate2.enabled = false;
        //image_indicate2.sprite = null;
    }
}

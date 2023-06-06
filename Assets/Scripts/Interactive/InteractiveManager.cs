using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class InteractiveManager : Singleton<InteractiveManager>
{
    private Transform handTrans;
    //private Transform leftHandTrans;
    private Transform noseTrans;

    //public bool bLeftHandEmpty;
    public bool bHandEmpty;
    //private bool bInteractiveLeft;
    private bool bInteractive;

    public Item ItemOnHand;
    //public Item ItemOnLeftHand;
    public GameObject TargetItem;
    public PlayableDirector Director;

    private IEnumerator TasteOrSmellCoroutine;
    public IEnumerator SpoonOutCoroutine;
    public IEnumerator PourCoroutine;

    public GameObject SaltParticle;

    private void OnEnable()
    {


        handTrans = GameObject.FindGameObjectWithTag("RightHand").transform;
        //leftHandTrans = GameObject.FindGameObjectWithTag("LeftHand").transform;
        noseTrans = GameObject.FindGameObjectWithTag("Nose").transform;
        bHandEmpty = bInteractive = true;

        ItemOnHand = null;

        EventHandler.ItemPickUpEvent += PickupItem;
        EventHandler.ItemDropEvent+=DropItem;
        EventHandler.TasteEvent += GetItemClose;
        EventHandler.SmellEvent += GetItemClose;

        EventHandler.ItemInteractEvent += ToolContainerInteract;
        EventHandler.InteractiveEvent += SpoonOutFromContainer;

        EventHandler.InteruptEvent += InteruptSmellOrTasteItem;
    }

    private void OnDisable()
    {
        EventHandler.ItemPickUpEvent -= PickupItem;
        EventHandler.ItemDropEvent -= DropItem;
        EventHandler.TasteEvent -= GetItemClose;
        EventHandler.SmellEvent -= GetItemClose;

        EventHandler.ItemInteractEvent -= ToolContainerInteract;
        EventHandler.InteractiveEvent -= SpoonOutFromContainer;

        EventHandler.InteruptEvent -= InteruptSmellOrTasteItem;
    }
    public void PickupItem(string itemName,GameObject itemObj)
    {
        var item=itemObj.GetComponent<Item>();
        if (item.type != eItemType.pot)
        {
            itemObj.GetComponent<Rigidbody>().isKinematic = true;
            UseSphereCollider(itemObj);
            ItemOnHand = item;
            ItemOnHand.transform.parent = handTrans;
            ItemOnHand.transform.localPosition = Vector3.zero; //reset relative position,keey rotation;
            ItemOnHand.transform.localRotation = Quaternion.identity;
            ItemOnHand.transform.localEulerAngles = new Vector3(180.0f, 0.0f, 180.0f);
            bHandEmpty = false;

            EventHandler.CallChangeIndicatorEvent(IndicateState.TasteAndSmell);

        }
        // else if(item.type != eItemType.container) 
        // {
        //     itemObj.GetComponent<Rigidbody>().isKinematic = true;
        //     UseSphereCollider(itemObj);
        //     ItemOnLeftHand = item;
        //     ItemOnLeftHand.transform.parent = handTrans;
        //     ItemOnLeftHand.transform.localPosition = Vector3.zero; //reset relative position,keey rotation;
        //     ItemOnLeftHand.transform.localRotation = Quaternion.identity;
        //     bLeftHandEmpty = false;
        // }
    }

    public void DropItem(string str,GameObject obj)
    {
        if(ItemOnHand != null)
        {
            //var posBeforeDrop = new Vector3(100.0f, 100.0f, 100.0f);
            ItemOnHand.transform.parent = null;
            //ItemOnHand.transform.position = posBeforeDrop;
            ResetCollider(ItemOnHand.gameObject);
            ItemOnHand.GetComponent<Rigidbody>().isKinematic = false;
            //ItemOnHand.transform.position = position;
            ItemOnHand = null;
            bHandEmpty = true;
            EventHandler.CallChangeIndicatorEvent(IndicateState.BothNone);
            if (SpoonOutCoroutine!=null)
            {
                StopCoroutine(SpoonOutCoroutine);
                SpoonOutCoroutine = null;
            }
            if (PourCoroutine != null)
            {
                StopCoroutine(PourCoroutine);
                PourCoroutine= null;
            }
            
        }
        //if (ItemOnLeftHand!=null)
        //{
        //    ItemOnLeftHand.transform.parent = null;
        //    ResetCollider(ItemOnLeftHand.gameObject);
        //    ItemOnLeftHand.GetComponent<Rigidbody>().isKinematic = false;
        //    //ItemOnHand.transform.position = position;
        //    ItemOnLeftHand = null;
        //    bLeftHandEmpty = true;
        //}

    }

    public void GetItemClose(string type, uint degree, GameObject item)
    {
        TasteOrSmellCoroutine = SmellOrTasteItem(item);
        StartCoroutine(TasteOrSmellCoroutine);
    }

    public void InteruptSmellOrTasteItem(GameObject item)
    {
        if (TasteOrSmellCoroutine != null)
        {
            StopCoroutine(TasteOrSmellCoroutine);
            TasteOrSmellCoroutine = null;
        }
        GetItemFar("", 0, item);
    }

    private IEnumerator SmellOrTasteItem(GameObject item)
    {
        EventHandler.CallChangeIndicatorEvent(IndicateState.BothNone);
        item.transform.position = noseTrans.transform.position;
        ItemOnHand.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            //yield return new WaitForSeconds(1.0f);
            //item.transform.position = handTrans.transform.position;
        EventHandler.CallChangeUIEvent("");

        yield return null;
    }

    private void GetItemFar(string type, uint degree, GameObject item)
    {
        item.transform.position = handTrans.transform.position;
        item.transform.localEulerAngles = new Vector3(180.0f, 0.0f, 180.0f);
        EventHandler.CallChangeIndicatorEvent(IndicateState.TasteAndSmell);
    }



    private void ToolContainerInteract(string item, GameObject gameObj)
    {
        var itemComp=gameObj.GetComponent<Item>();
        if (itemComp != null)
        {
            itemComp.Interact();
        }
    }

    private void UseSphereCollider(GameObject itemObj)
    {
        itemObj.GetComponent<Collider>().enabled = false;
        itemObj.AddComponent<SphereCollider>();
        itemObj.GetComponent<SphereCollider>().radius *= 1.2f;
        itemObj.GetComponent<SphereCollider>().isTrigger = true;
    }

    private void ResetCollider(GameObject itemObj)
    {
        SphereCollider sphereCollider;
        if (itemObj.TryGetComponent<SphereCollider>(out sphereCollider))
        {
            Destroy(sphereCollider);
        }
        itemObj.GetComponent<Collider>().enabled=true;
    }

    private void SpoonOutFromContainer(GameObject from,GameObject to)
    {
        //if (from.GetComponent<Item>().type == eItemType.container && to.GetComponent<Item>().type == eItemType.tool)
        //{
        //    EventHandler.CallInteractEvent_Internal(from,to,"salt",1);
        //}

        if (from.GetComponent<Item>().type == eItemType.tool && to.GetComponent<Item>().type == eItemType.container)
        {
            EventHandler.CallInteractEvent_Internal(to, from, "salt", 1);//reverse order
            EventHandler.CallChangeIndicatorEvent(IndicateState.TasteAndSmell);
            //EventHandler.CallChangeIndicatorEvent(IndicateState.SpoonOut);
        }
    }

    public void SetTargetItem(GameObject targetItem)
    {
        TargetItem = targetItem;
    }
}

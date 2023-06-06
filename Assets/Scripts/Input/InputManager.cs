using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class InputManager : MonoBehaviour
{
    public enum HoldState
    {
        None,
        ShortHold,
        LongHold
    }

    private InputAction leftMouseClick;//Pick up object
    private InputAction ePress;
    private InputAction fPress;
    private InputAction rPress;
    private InputAction eHold;
    private float eHoldTime;
    private bool bEHold;
    private HoldState eCurrentEHoldState;

    private InputAction leftMouseHold;//Smell and taste
    //private InputAction leftMouseDoubleTap;//discard object

    //private InputAction rightMouseClick;
    //private InputAction rightMouseHold;
    //private InputAction rightMouseDoubleTap;

    private InputAction leftShiftPress;//interactive with object

    //private Transform leftHandTrans;
    //private Transform rightHandTrans;
    //private Transform noseTrans;

    //private bool bLeftHandEmpty;
    //private bool bRightHandEmpty;
    //private bool bInteractiveLeft;
    //private bool bInteractiveRight;

    //private Item leftHandItem;
    //private Item rightHandItem;

    private bool bLeftMouseHold;
    //private bool bRightMouseHold;
    private Item LastItemHoverOn;
    private GameObject CurrentItemHoverOn;

    public PostProcessController PPControler;

    private RaycastHit raycastHit;
    private Ray ray;


    private void OnEnable()
    { 
        leftMouseClick=new InputAction();
        leftMouseClick.AddBinding("<Mouse>/leftButton").WithInteraction("press");
        leftMouseClick.started += ctx => LeftMouseClick();
        leftMouseClick.Enable();

        leftShiftPress = new InputAction();
        leftShiftPress.AddBinding("<Keyboard>/leftShift").WithInteraction("press");
        leftShiftPress.performed += ctx => LeftShiftPress();
        leftShiftPress.Enable();

        ePress = new InputAction();
        ePress.AddBinding("<Keyboard>/e").WithInteraction("press");
        ePress.performed += ctx => EPress();
        ePress.Enable();

        eHold=new InputAction();
        eHold.AddBinding("<Keyboard>/e").WithInteraction("hold");
        eHold.performed += ctx => EHold();
        eHold.canceled += ctx => ERelease();
        eHold.Enable();

        fPress = new InputAction();
        fPress.AddBinding("<Keyboard>/f").WithInteraction("press");
        fPress.performed += ctx => FPress();
        fPress.Enable();

        rPress = new InputAction();
        rPress.AddBinding("<Keyboard>/r").WithInteraction("press");
        rPress.performed += ctx => RPress();
        rPress.Enable();

        leftMouseHold = new InputAction();
        leftMouseHold.AddBinding("<Mouse>/leftButton").WithInteraction("hold");
        leftMouseHold.performed += ctx => LeftMouseHold();
        leftMouseHold.canceled += ctx => LeftMouseRelease();
        leftMouseHold.Enable();

        //leftMouseDoubleTap = new InputAction();
        //leftMouseDoubleTap.AddBinding("<Mouse>/leftButton").WithInteraction("multitap");
        //leftMouseDoubleTap.performed += ctx => LeftMouseDoubleTap();
        //leftMouseDoubleTap.Enable();

        //rightMouseClick = new InputAction();
        //rightMouseClick.AddBinding("<Mouse>/RightButton").WithInteraction("press");
        //rightMouseClick.started += ctx => RightMouseClick();
        //rightMouseClick.Enable();

        //rightMouseHold = new InputAction();
        //rightMouseHold.AddBinding("<Mouse>/rightButton").WithInteraction("hold");
        //rightMouseHold.performed += ctx => RightMouseHold();
        //rightMouseHold.canceled += ctx => RightMouseRelease();
        //rightMouseHold.Enable();

        //rightMouseDoubleTap = new InputAction();
        //rightMouseDoubleTap.AddBinding("<Mouse>/rightButton").WithInteraction("multitap");
        //rightMouseDoubleTap.performed += ctx => RightMouseDoubleTap();
        //rightMouseDoubleTap.Enable();

        LastItemHoverOn = null;
        CurrentItemHoverOn = null;
        bLeftMouseHold = false;
        eHoldTime = 0.0f;
        bEHold= false;
        eCurrentEHoldState = HoldState.None;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (bEHold)
        {
            eHoldTime += Time.deltaTime;
            DetectHoldTime();
        }
    }

    private void FixedUpdate()
    {
        LeftMouseHover();
    }

    void LeftMouseHover()
    {
        if (Physics.Raycast(ray, out raycastHit, 100f))
        {
            if (raycastHit.transform != null)
            {
                if (raycastHit.transform.GetComponent<Item>() != null)
                {
                    var targetItem = raycastHit.transform.GetComponent<Item>();
                    //InteractiveManager.Instance.SetTargetItem(raycastHit.transform.gameObject);
                    if (targetItem!=LastItemHoverOn)
                    {
                        //if(LastItemHoverOn != null)
                        //{
                        //    EventHandler.CallItemHoverOffEvent(LastItemHoverOn.type.ToString(), LastItemHoverOn.gameObject);
                        //}
                        LastItemHoverOn = targetItem;
                        EventHandler.CallItemHoverOnEvent(targetItem.type.ToString(), targetItem.gameObject);
                    }
                }
                else
                {
                    if (LastItemHoverOn != null)
                    {
                        if(LastItemHoverOn != InteractiveManager.Instance.ItemOnHand)
                        {
                            EventHandler.CallItemHoverOffEvent(LastItemHoverOn.type.ToString(), LastItemHoverOn.gameObject);
                            LastItemHoverOn = null;
                        }
                    }
                }
            }
        }
    }

    void LeftMouseClick()
    {

    }
    void LeftMouseHold()
    {

        if (Physics.Raycast(ray, out raycastHit, 100f))
        {
            if (raycastHit.transform != null)
            {
                if (raycastHit.transform.GetComponent<Item>() != null)
                {
                    var targetItem = raycastHit.transform.GetComponent<Item>();
                    EventHandler.CallItemPickUpEvent(targetItem.type.ToString(), targetItem.gameObject);
                }
                //Debug.Log("Left Mouse Clicked at: " + raycastHit.transform.gameObject.name);
            }
        }
    }
    void LeftMouseRelease()
    {
        if (!InteractiveManager.Instance.bHandEmpty)
        {
            EventHandler.CallInteruptEvent(InteractiveManager.Instance.ItemOnHand.gameObject);
            EventHandler.CallItemDropEvent("", InteractiveManager.Instance.ItemOnHand.gameObject);
        }
    }
    void EPress()
    {
        var item = InteractiveManager.Instance.ItemOnHand;
        if (item != null)
        {
            if (item.bCanBeSmell)
            {
                EventHandler.CallSmellEvent(item.smellType.ToString(), item.smellValue, item.gameObject);
            }
        }
        //Debug.Log("E Press");
    }

    void EHold()
    {
        eHoldTime = 0.0f;
        eCurrentEHoldState = HoldState.None;
        bEHold = true;
    }

    void DetectHoldTime()
    {
        if (eHoldTime > 2.0f)
        {
            if(eCurrentEHoldState!= HoldState.LongHold)
            {
                eCurrentEHoldState = HoldState.LongHold;
                var item = InteractiveManager.Instance.ItemOnHand;
                if (item != null)
                {
                    if (item.bCanBeTaste)
                    {
                        EventHandler.CallTasteEvent(item.tasteType.ToString(), item.tasteValue, item.gameObject);
                    }
                }
                Debug.Log("hold for 2 secs");
            }

        }else if (eHoldTime > 1.0f)
        {
            if (eCurrentEHoldState != HoldState.ShortHold)
            {
                eCurrentEHoldState = HoldState.ShortHold;
                //var item = InteractiveManager.Instance.ItemOnHand;
                //if (item != null)
                //{
                //    if (item.bCanBeTaste)
                //    {
                //        EventHandler.CallTasteEvent(item.tasteType.ToString(), item.tasteValue, item.gameObject);
                //    }
                //}
                Debug.Log("hold for 1 sec");
            }
        }
    }

    void ERelease()
    {
        eHoldTime = 0.0f;
        eCurrentEHoldState = HoldState.None;
        bEHold= false;
        if (!InteractiveManager.Instance.bHandEmpty)
        {
            EventHandler.CallInteruptEvent(InteractiveManager.Instance.ItemOnHand.gameObject);
        }
        
    }

    void FPress()
    {
        var item = InteractiveManager.Instance.ItemOnHand;
        if(item != null)
        {
            if (item.bCanBeSmell)
            {
                EventHandler.CallSmellEvent(item.smellType.ToString(), item.smellValue,item.gameObject);
            }
        }
        //Debug.Log("F Press");
    }

    void RPress()
    {
        if (!InteractiveManager.Instance.bHandEmpty)
        {
            EventHandler.CallItemInteractEvent(InteractiveManager.Instance.ItemOnHand.type.ToString(), InteractiveManager.Instance.ItemOnHand.gameObject);
            if (InteractiveManager.Instance.TargetItem)
            {
                EventHandler.CallInteractEvent(InteractiveManager.Instance.ItemOnHand.gameObject, InteractiveManager.Instance.TargetItem.gameObject);//from hand to target
            }
        }
        //Debug.Log("R Press");
    }
    //void RightMouseClick()
    //{
    //    RaycastHit raycastHit;
    //    Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

    //    if (Physics.Raycast(ray, out raycastHit, 100f))
    //    {
    //        if (raycastHit.transform != null)
    //        {
    //            if (raycastHit.transform.GetComponent<Item>() != null)
    //            {
    //                var targetItem = raycastHit.transform.GetComponent<Item>();
    //                if (targetItem.handSide == eHandSide.rightHand)
    //                {
    //                    //if (bRightHandEmpty)
    //                    //{
    //                    //    rightHandItem = targetItem;
    //                    //    mItems_PickupTrans.Add(rightHandItem, new LocAndRot(targetItem.transform.position, targetItem.transform.rotation));
    //                    //    rightHandItem.transform.parent = rightHandTrans;
    //                    //    rightHandItem.transform.localPosition = Vector3.zero;//reset relative position,keey rotation;
    //                    //    bRightHandEmpty = false;
    //                    //}
    //                    //else
    //                    //{
    //                    //    LocAndRot oldItemLocAndRot;
    //                    //    mItems_PickupTrans.TryGetValue(rightHandItem, out oldItemLocAndRot);
    //                    //    rightHandItem.transform.parent = null;
    //                    //    rightHandItem.transform.position = oldItemLocAndRot.Position;
    //                    //    rightHandItem.transform.rotation = oldItemLocAndRot.Rotation;
    //                    //    mItems_PickupTrans.Remove(rightHandItem);

    //                    //    rightHandItem = targetItem;
    //                    //    mItems_PickupTrans.Add(rightHandItem, new LocAndRot(targetItem.transform.position, targetItem.transform.rotation));
    //                    //    rightHandItem.transform.parent = rightHandTrans;
    //                    //    rightHandItem.transform.localPosition = Vector3.zero;//reset relative position,keey rotation;
    //                    //}

    //                    EventHandler.CallItemPickUpEvent("Spoon", targetItem.gameObject);
    //                }
    //            }
    //            Debug.Log("Right Mouse Clicked at: " + raycastHit.transform.gameObject.name);
    //        }
    //    }
    //}

    void LeftMouseDoubleTap()
    {
        //if (!bLeftHandEmpty)
        //{
        //    LocAndRot oldItemLocAndRot;
        //    mItems_PickupTrans.TryGetValue(leftHandItem, out oldItemLocAndRot);
        //    leftHandItem.transform.parent = null;
        //    leftHandItem.transform.position = oldItemLocAndRot.Position;
        //    leftHandItem.transform.rotation = oldItemLocAndRot.Rotation;
        //    mItems_PickupTrans.Remove(leftHandItem);
            
        //    bLeftHandEmpty = true;
        //}
        //EventHandler.CallItemDropEvent(leftHandItem.GetComponent<Item>().name, leftHandItem.gameObject);
        Debug.Log("Left Mouse Double Tap");
    }

    //void RightMouseDoubleTap()
    //{
    //    if (!bRightHandEmpty)
    //    {
    //        LocAndRot oldItemLocAndRot;
    //        mItems_PickupTrans.TryGetValue(rightHandItem, out oldItemLocAndRot);
    //        rightHandItem.transform.parent = null;
    //        rightHandItem.transform.position = oldItemLocAndRot.Position;
    //        rightHandItem.transform.rotation = oldItemLocAndRot.Rotation;
    //        mItems_PickupTrans.Remove(rightHandItem);
    //        EventHandler.CallItemDropEvent(rightHandItem.GetComponent<Item>().name, rightHandItem.gameObject);
    //        bRightHandEmpty = true;
    //    }

    //    Debug.Log("Right Mouse Double Tap");
    //}



    //void RightMouseRelease()
    //{
    //    if (bRightMouseHold)
    //    {
    //        Debug.Log("right Mouse Release");
    //        bRightMouseHold = false;

    //        if (bInteractiveRight)
    //        {
    //            rightHandItem.transform.parent = rightHandTrans;
    //            rightHandItem.transform.localPosition = Vector3.zero;
    //            bInteractiveRight = false;
    //        }
    //    }
    //}

    //void RightMouseHold()
    //{
    //    bLeftMouseHold = true;
    //    Debug.Log("Right Mouse Hold");

    //    if (!bRightHandEmpty)
    //    {
    //        rightHandItem.transform.parent = noseTrans;
    //        rightHandItem.transform.localPosition = Vector3.zero;
    //        bInteractiveRight = true;
    //    }
    //}


    void LeftShiftPress()
    {
        Debug.Log("test");

    }


}

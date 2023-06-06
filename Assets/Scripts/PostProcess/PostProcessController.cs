using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PostProcessController : MonoBehaviour
{
    private Vector3 leftHandPosition;
    private Vector3 rightHandPosition;
    public float leftHandRadius;
    public float rightHandRadius;
    public float duration;
    public float easeTime;

    private bool bUpdateLeftHand;
    private bool bUpdateRightHand;
    private bool bUpdateEffect;

    private Vector3 lastTimeLeftHandPosition;
    private Vector3 lastTimeRightHandPosition;
    private float lastTimeLeftHandRadius;
    private float lastTimeRightHandRadius;

    private PostProcess_BlindEffect PP_BlindEffect;

    private InputAction leftMouseClick;
    private InputAction rightMouseClick;

    RaycastHit raycastHit;
    Ray ray;

    private float effectCurrentTime;
    private void OnEnable()
    {
        PP_BlindEffect = GetComponent<PostProcess_BlindEffect>();

        //leftMouseClick = new InputAction(binding: "<Mouse>/leftButton");
        //leftMouseClick.performed += ctx => LeftMouseClicked();
        //leftMouseClick.Enable();

        //leftMouseClick = new InputAction(binding: "<Mouse>/rightButton");
        //leftMouseClick.performed += ctx => RightMouseClicked();
        //leftMouseClick.Enable();

        updateBlindEffectEvent += SetBlindEffect;

        EventHandler.ItemHoverOnEvent += ShowObjectOutline;
        EventHandler.ItemHoverOffEvent += HideObjectOutline;
        EventHandler.ItemPickUpEvent += ShowObjectOutline;
        EventHandler.ItemDropEvent += HideObjectOutline;
    }

    private void OnDisable()
    {
        updateBlindEffectEvent -= SetBlindEffect;

        EventHandler.ItemHoverOnEvent -= ShowObjectOutline;
        EventHandler.ItemHoverOffEvent -= HideObjectOutline;
        EventHandler.ItemPickUpEvent -= ShowObjectOutline;
        EventHandler.ItemDropEvent -= HideObjectOutline;
    }
    // Start is called before the first frame update
    void Start()
    {
        bUpdateLeftHand=true;
        bUpdateRightHand = false;

        leftHandPosition=lastTimeLeftHandPosition = new Vector3(1.7f, 0.0f, 2.0f);
        rightHandPosition=lastTimeRightHandPosition = new Vector3(10.65f, -0.5f, 5.4f);
        //for test purpose only
        leftHandRadius = rightHandRadius = 6.5f;
        lastTimeLeftHandRadius = lastTimeRightHandRadius = 2.5f;

        effectCurrentTime = 0.0f;

    }

    // Update is called once per frame
    void Update()
    {
        if (bUpdateEffect)
        {
            if (bUpdateLeftHand)
            {
                float currentLeftHandRadius = Mathf.Lerp(0.0f,leftHandRadius,Mathf.Clamp01(effectCurrentTime/Mathf.Max(easeTime,0.01f)));
                CallUpdateBlindEffectEvent(leftHandPosition,lastTimeRightHandPosition,currentLeftHandRadius,lastTimeRightHandRadius);
            }
            else
            {
                //calc current radius;
                float currentRightHandRadius = Mathf.Lerp(0.0f, rightHandRadius, Mathf.Clamp01(effectCurrentTime / Mathf.Max(easeTime, 0.01f)));
                CallUpdateBlindEffectEvent(lastTimeLeftHandPosition, rightHandPosition, lastTimeLeftHandRadius, currentRightHandRadius);
            }
            
            if(effectCurrentTime >= duration-easeTime)
            {   
                //gradually stop effect
                if (bUpdateLeftHand)
                {
                    Debug.Log("Left Hand Decrease");
                    float currentLeftHandRadius = Mathf.Lerp(0.0f, leftHandRadius, Mathf.Clamp01((duration-effectCurrentTime) / Mathf.Max(easeTime, 0.01f)));
                    CallUpdateBlindEffectEvent(leftHandPosition, lastTimeRightHandPosition, currentLeftHandRadius, lastTimeRightHandRadius);
                    if (currentLeftHandRadius <= 0.0f)
                    {
                        Debug.Log("Effect Stop");
                        bUpdateEffect = false;
                        bUpdateLeftHand = false;
                        effectCurrentTime = 0.0f;
                        lastTimeLeftHandRadius = 0.0f;
                    }
                }
                else
                {
                    Debug.Log("Right Hand Decrease");
                    float currentRightHandRadius = Mathf.Lerp(0.0f, rightHandRadius, Mathf.Clamp01((duration - effectCurrentTime) / Mathf.Max(easeTime, 0.01f)));
                    CallUpdateBlindEffectEvent(lastTimeLeftHandPosition, rightHandPosition, lastTimeLeftHandRadius, currentRightHandRadius);
                    if(currentRightHandRadius <= 0.0f)
                    {
                        Debug.Log("Effect Stop");
                        bUpdateEffect = false;
                        bUpdateRightHand = false;
                        effectCurrentTime = 0.0f;
                        lastTimeRightHandRadius = 0.0f;
                    }
                }
            }
            effectCurrentTime += Time.deltaTime;
        }

        //RaycastHit raycastHit;
        ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out raycastHit, 100f))
        {
            if (raycastHit.transform != null)
            {
                UpdateEffectGradually(raycastHit.point, true);
                //UpdateEffect(raycastHit.point,leftHandRadius,rightHandRadius, true);
                //Debug.Log(raycastHit.point);
                //CurrentClickedGameObject(raycastHit.transform.gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        

    }

    private void LateUpdate()
    {
        lastTimeLeftHandPosition = leftHandPosition;
        lastTimeRightHandPosition = rightHandPosition;
        lastTimeLeftHandRadius=leftHandRadius;
        lastTimeRightHandRadius=rightHandRadius;
        //record last position here
    }
    public static event Action<Vector3,Vector3, float, float> updateBlindEffectEvent;

    public void CallUpdateBlindEffectEvent(Vector3 effectPosition1, Vector3 effectPosition2, float effectRadius1, float effectRadius2)
    {
        updateBlindEffectEvent?.Invoke(effectPosition1, effectPosition2, effectRadius1, effectRadius2);
    }

    private void SetBlindEffect(Vector3 effectPosition1, Vector3 effectPosition2, float effectRadius1,float effectRadius2)
    {
        PP_BlindEffect.SetMaterialParameter(effectPosition1, effectPosition2, effectRadius1, effectRadius2);
    }

    private void SetBlindEffectGradually(Vector3 effectPosition, float effectRadius, float effectDuration, float easeTime, bool bIsLeftHand)
    {


        if (bIsLeftHand)
        {
            SetBlindEffect(effectPosition, lastTimeRightHandPosition, effectRadius, lastTimeRightHandRadius);
            leftHandPosition = effectPosition;
        }
        else
        {
            SetBlindEffect(lastTimeLeftHandPosition, effectPosition, lastTimeLeftHandRadius, effectRadius);
            rightHandPosition = effectPosition;
        }
    }

    public void LeftMouseClicked()
    {
        //Input.mousePosition
        RaycastHit raycastHit;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out raycastHit, 100f))
        {
            if (raycastHit.transform != null)
            {
                UpdateEffectGradually(raycastHit.point, true);
                //UpdateEffect(raycastHit.point,leftHandRadius,rightHandRadius, true);
                //Debug.Log(raycastHit.point);
                //CurrentClickedGameObject(raycastHit.transform.gameObject);
            }
        }
    }

    public void RightMouseClicked()
    {
        RaycastHit raycastHit;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out raycastHit, 100f))
        {
            if (raycastHit.transform != null)
            {
                UpdateEffectGradually(raycastHit.point, false);
                //UpdateEffect(raycastHit.point, leftHandRadius, rightHandRadius, false);
                //Debug.Log(raycastHit.point);
                //CurrentClickedGameObject(raycastHit.transform.gameObject);
            }
        }
    }

    private void UpdateEffect(Vector3 position,float leftHandRadius,float rightHandRadius,bool bUpdateLeftHand)
    {
        if (bUpdateLeftHand)
        {
            SetBlindEffect(position, lastTimeRightHandPosition, leftHandRadius, rightHandRadius);
            leftHandPosition = position;
        }
        else
        {
            SetBlindEffect(lastTimeLeftHandPosition, position, leftHandRadius, rightHandRadius);
            rightHandPosition= position;
        }
    }

    private void UpdateEffectGradually(Vector3 position,bool _bUpdateLeftHand)
    {
        effectCurrentTime = 0.0f;
        bUpdateEffect = true;
        if (_bUpdateLeftHand)
        {
            bUpdateLeftHand = true;
            bUpdateRightHand = false;
            leftHandPosition = position;
        }
        else
        {
            bUpdateLeftHand = false; 
            bUpdateRightHand = true;
            rightHandPosition = position;
        }
    }

    public void ShowObjectOutline(string itemName,GameObject gameObj)
    {
        if (gameObj != null)
        {
            var objectMat = gameObj.GetComponent<MeshRenderer>().material;
            if (objectMat)
            {
                objectMat.SetColor("_Color", Color.black);
            }
        }
    }//change materialColorToBlack to set outline ot current object

    public void HideObjectOutline(string itemName,GameObject gameObj)
    {
        if(gameObj != null)
        {
            var objectMat = gameObj.GetComponent<MeshRenderer>().material;
            if (objectMat)
            {
                objectMat.SetColor("_Color", Color.white);
            }
        }
    }

}

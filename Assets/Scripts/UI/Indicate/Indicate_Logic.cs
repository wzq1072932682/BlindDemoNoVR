using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicate_Logic : MonoBehaviour
{
    public Indicate_UI indicate_UI;


    private IndicateState currentState;
    private bool bHasChanged;
    public void SetIndicateState(IndicateState NextState)
    {
        if (NextState != currentState)
        {
            currentState = NextState;
            bHasChanged = true;
        }
    }
    private void OnEnable()
    {
        EventHandler.ChangeIndicatorEvent += SetIndicateState;
    }
    private void Start()
    {
        currentState = IndicateState.BothNone;
        bHasChanged = false;
    }
    // Start is called before the first frame update
    private void OnDisable()
    {
        EventHandler.ChangeIndicatorEvent -= SetIndicateState;
    }
    // Update is called once per frame
    void Update()
    {
        if (bHasChanged)
        {
            switch (currentState)
            {
                case IndicateState.ENone:
                    indicate_UI.SetIndicateSprite(0);
                    bHasChanged= false;
                    break;
                case IndicateState.RNone:
                    indicate_UI.SetIndicateSprite(4);
                    bHasChanged = false;
                    break;
                case IndicateState.SpoonOut:
                    indicate_UI.SetIndicateSprite(1);
                    bHasChanged = false;
                    break;
                case IndicateState.Rotate:
                    indicate_UI.SetIndicateSprite(2);
                    bHasChanged = false;
                    break;
                case IndicateState.TasteAndSmell:
                    indicate_UI.SetIndicateSprite(2);
                    indicate_UI.SetIndicateSprite(3);
                    bHasChanged = false;
                    break;
                case IndicateState.BothNone:
                    indicate_UI.SetIndicateSprite(5);
                    bHasChanged = false;
                    break;
                default:
                    indicate_UI.SetIndicateSprite(5);
                    bHasChanged = false;
                    break;
            }
        }
    }


}

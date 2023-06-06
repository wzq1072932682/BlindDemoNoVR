using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaltUI : MonoBehaviour
{
    public Image m_Scrollbar;
    public Image m_Image;
    public Image m_Feedback;
    public Image m_ScrollbarBackground;
    public TextMeshProUGUI feedbackText;
    public TasteFeedback_SO saltFeedback;
    public TasteFeedback_SO smellFeedback;

    private SaltLogic m_Logic;

    public Sprite image_NoTaste;
    public Sprite image_Salt;
    private void OnEnable()
    {
        m_Logic = GetComponent<SaltLogic>();

        var saltImage = GameObject.Find("SaltImage");

        if (m_Feedback != null)
        {
            m_Feedback.sprite = null;
            m_Feedback.enabled = false;
        }

        feedbackText.text = "";
        //m_Image = saltImage.GetComponentInChildren<Image>();

        //m_Image.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        if (m_Scrollbar != null)
        {
            m_Scrollbar.enabled = false;
            //var saltScrollImage = GameObject.Find("SaltScrollBar").GetComponent<Image>();
            //m_Scrollbar = saltScrollImage;
            //m_Scrollbar.fillAmount = 0.5f;
            m_Scrollbar.fillAmount = m_Logic.CurrentSalt / m_Logic.SaltRange;
        }
        if (m_ScrollbarBackground != null)
        {
            m_ScrollbarBackground.enabled = false;
        }

        //m_Scrollbar.numberOfSteps = m_Logic.SaltRange;
        //m_Scrollbar.size = 1 / m_Logic.SaltRange;
        //m_Scrollbar.value = m_Logic.CurrentSalt / m_Logic.SaltRange;

        EventHandler.TasteEvent += SetSaltUI;
        EventHandler.SmellEvent += SetSmellUI;
        EventHandler.ItemFinishInteractEvent += HideSaltImage;
        EventHandler.ChangeUIEvent += ChangeUI;
    }

    private void OnDisable()
    {
        EventHandler.TasteEvent -= SetSaltUI;
        EventHandler.SmellEvent -= SetSmellUI;
        EventHandler.ItemFinishInteractEvent -= HideSaltImage;
        EventHandler.ChangeUIEvent -= ChangeUI;
    }

    void SetSaltUI(string type, uint degree, GameObject obj)
    {
        if (type == eTasteType.salt.ToString())
        {
            UpdateScrollBar();
            if (degree > 0)
            {
                StartCoroutine(ShowImage(m_Feedback, image_Salt));
                //m_Feedback.enabled = true;
                //m_Feedback.sprite = image_Salt;
            }

            //ShowSaltImage();
            SetFeedbackText(degree);
        }
    }

    void SetSmellUI(string type, uint degree, GameObject obj)
    {
        if (type == eSmellType.none.ToString())
        {
            SetSmellFeedbackText(degree);
            StartCoroutine(ShowImage(m_Feedback, image_NoTaste));
            //m_Feedback.enabled = true;
            //m_Feedback.sprite = image_NoTaste;
        }
    }

    void UpdateScrollBar()
    {

        m_Scrollbar.fillAmount = (float)m_Logic.CurrentSalt / (float)m_Logic.SaltRange;
        StartCoroutine(ShowScrollBar());
    }

    IEnumerator ShowScrollBar()
    {
        m_Scrollbar.enabled = true;
        m_ScrollbarBackground.enabled = true;
        yield return new WaitForSeconds(2.0f);
        m_Scrollbar.enabled = false;
        m_ScrollbarBackground.enabled = false;
    }

    IEnumerator ShowImage(Image img,Sprite sprite)
    {
        img.enabled = true;
        img.sprite= sprite;
        yield return new WaitForSeconds(2.0f);
        img.enabled = false;
    }

    void ShowSaltImage()
    {
        m_Image.enabled = true;
        m_Image.sprite = image_Salt;
    }

    void HideSaltImage(string str,GameObject obj)
    {
        m_Image.enabled = false;
        m_Image.sprite = null;
    }

    void SetFeedbackText(uint value)
    {
        feedbackText.text = saltFeedback.GetFeedbackByRange(value);
        //Debug.Log(feedbackText.text);
    }

    void SetSmellFeedbackText(uint value)
    {
        feedbackText.text = smellFeedback.GetFeedbackByRange(value);
    }

    void ChangeUI(string str)
    {
        feedbackText.text = str;
    }
    
}

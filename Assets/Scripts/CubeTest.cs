using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class CubeTest : RayEventBase
{
    public GameObject HoverTipGo;
    public Text HoverTipText;

    private RectTransform hoverTipRec;

    public override void Awake()
    {
        base.Awake();
        OnPointerClickCallback = new UnityEvent<PointerEventData>();
        OnPointerClickCallback.AddListener(OnPointerClick_);
        OnPointerEnterCallback = new UnityEvent<PointerEventData>();
        OnPointerEnterCallback.AddListener(OnPointerEnter_);
        OnPointerExitCallback = new UnityEvent<PointerEventData>();
        OnPointerExitCallback.AddListener(OnPointerExit_);
        hoverTipRec = HoverTipGo.GetComponent<RectTransform>();
        BaseLayerMask = ~(1 << 10);
    }

    private void OnDestroy()
    {
        OnPointerClickCallback?.RemoveListener(OnPointerClick_);
        OnPointerEnterCallback?.RemoveListener(OnPointerEnter_);
        OnPointerExitCallback?.RemoveListener(OnPointerExit_);
    }

    private void OnPointerClick_(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} OnPointerClick");
    }

    private void OnPointerEnter_(PointerEventData eventData)
    {
        HoverTipText.text = $"{gameObject.name}";
        HoverTipGo.transform.position = eventData.position + new Vector2(hoverTipRec.sizeDelta.x, hoverTipRec.sizeDelta.y);
        HoverTipGo.SetActive(true);
    }

    private void OnPointerExit_(PointerEventData eventData)
    {
        HoverTipText.text = string.Empty;
        HoverTipGo.SetActive(false);
    }
}
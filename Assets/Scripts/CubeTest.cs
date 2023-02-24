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
        OnPointerClickCallback.AddListener(_OnPointerClick);
        OnPointerEnterCallback = new UnityEvent<PointerEventData>();
        OnPointerEnterCallback.AddListener(_OnPointerEnter);
        OnPointerExitCallback = new UnityEvent<PointerEventData>();
        OnPointerExitCallback.AddListener(_OnPointerExit);
        hoverTipRec = HoverTipGo.GetComponent<RectTransform>();
        BaseLayerMask = ~(1 << 10);
    }

    private void OnDestroy()
    {
        OnPointerClickCallback?.RemoveListener(_OnPointerClick);
        OnPointerEnterCallback?.RemoveListener(_OnPointerEnter);
        OnPointerExitCallback?.RemoveListener(_OnPointerExit);
    }

    private void _OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} OnPointerClick");
    }

    private void _OnPointerEnter(PointerEventData eventData)
    {
        HoverTipText.text = $"{gameObject.name}";
        HoverTipGo.transform.position = eventData.position + new Vector2(hoverTipRec.sizeDelta.x, hoverTipRec.sizeDelta.y);
        HoverTipGo.SetActive(true);
    }

    private void _OnPointerExit(PointerEventData eventData)
    {
        HoverTipText.text = string.Empty;
        HoverTipGo.SetActive(false);
    }
}
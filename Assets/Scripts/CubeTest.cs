//#define NativeInterface
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using HighlightingSystem;

#if NativeInterface
public class CubeTest : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
#else
public class CubeTest : RayEventBase
#endif
{
    public GameObject HoverTipGo;
    public Text HoverTipText;

    private RectTransform hoverTipRec;
    private Highlighter highlighter;
#if NativeInterface
    private void Awake()
    {
        highlighter = this.GetComponent<Highlighter>();
        if (highlighter == null)
        {
            highlighter = this.gameObject.AddComponent<Highlighter>();
        }
        hoverTipRec = HoverTipGo.GetComponent<RectTransform>();
    }
#else
    public override void Awake()
    {
        base.Awake();
        highlighter = this.GetComponent<Highlighter>();
        if (highlighter == null)
        {
            highlighter = this.gameObject.AddComponent<Highlighter>();
        }
        OnPointerClickCallback = new UnityEvent<PointerEventData>();
        OnPointerClickCallback.AddListener(_OnPointerClick);
        OnPointerEnterCallback = new UnityEvent<PointerEventData>();
        OnPointerEnterCallback.AddListener(_OnPointerEnter);
        OnPointerExitCallback = new UnityEvent<PointerEventData>();
        OnPointerExitCallback.AddListener(_OnPointerExit);

        OnBeginDragCallback = new UnityEvent<PointerEventData>();
        OnBeginDragCallback.AddListener(_OnBeginDrag);
        OnDragCallback = new UnityEvent<PointerEventData>();
        OnDragCallback.AddListener(_OnDrag);
        OnEndDragCallback = new UnityEvent<PointerEventData>();
        OnEndDragCallback.AddListener(_OnEndDrag);

        hoverTipRec = HoverTipGo.GetComponent<RectTransform>();
        BaseLayerMask = ~(1 << 10);
    }
#endif
    private void OnDestroy()
    {
#if !NativeInterface
        OnPointerClickCallback?.RemoveListener(_OnPointerClick);
        OnPointerEnterCallback?.RemoveListener(_OnPointerEnter);
        OnPointerExitCallback?.RemoveListener(_OnPointerExit);
        OnBeginDragCallback.RemoveListener(_OnBeginDrag);
        OnDragCallback.RemoveListener(_OnDrag);
        OnEndDragCallback.RemoveListener(_OnEndDrag);
#endif
    }

    private Vector3 m_OrigPos;
    //偏移值
    Vector3 m_Offset;
    //当前物体对应的屏幕坐标
    Vector3 m_TargetScreenVec;

#if !NativeInterface
    private void _OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} OnPointerClick");
    }

    private void _OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} OnPointerEnter++++++++++++++++");
        highlighter?.ConstantOnImmediate(Color.red);
        HoverTipText.text = $"{gameObject.name}";
        HoverTipGo.transform.position = eventData.position + new Vector2(hoverTipRec.sizeDelta.x, hoverTipRec.sizeDelta.y);
        HoverTipGo.SetActive(true);
    }

    private void _OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} OnPointerExit++++++++++++++++");
        highlighter?.ConstantOffImmediate();
        HoverTipText.text = string.Empty;
        HoverTipGo.SetActive(false);
    }


    private void _OnBeginDrag(PointerEventData eventData)
    {
        if (gameObject.name== "Cube 2")
        {
            return;
        }


        Debug.Log($"{gameObject.name} OnBeginDrag");
        //if (gameObject.name.Equals("Cube 2"))
        {
            m_OrigPos = gameObject.transform.position;
            //当前物体对应的屏幕坐标
            m_TargetScreenVec = Camera.main.WorldToScreenPoint(transform.position);
            //偏移值=物体的世界坐标，减去转化之后的鼠标世界坐标（z轴的值为物体屏幕坐标的z值）
            m_Offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3
            (eventData.position.x, eventData.position.y, m_TargetScreenVec.z));
        }
    }

    private void _OnDrag(PointerEventData eventData)
    {
        if (gameObject.name == "Cube 2")
        {
            return;
        }
        Debug.Log($"{gameObject.name} OnDrag");
        //if (gameObject.name.Equals("Cube 2"))
        {
            //gameObject.transform.position = Camera.main.ScreenToWorldPoint(eventData.position);
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, m_TargetScreenVec.z)) + m_Offset;
        }
    }

    private void _OnEndDrag(PointerEventData eventData)
    {
        if (gameObject.name == "Cube 2")
        {
            return;
        }
        Debug.Log($"{gameObject.name} OnEndDrag");
        //if (gameObject.name.Equals("Cube 2"))
        {
            gameObject.transform.position = m_OrigPos;
        }
    }
#endif


#if NativeInterface
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} OnBeginDrag + {eventData.position}");
        if (gameObject.name.Equals("Cube"))
        {
            m_OrigPos = gameObject.transform.position;

            //当前物体对应的屏幕坐标
            m_TargetScreenVec = Camera.main.WorldToScreenPoint(transform.position);
            //偏移值=物体的世界坐标，减去转化之后的鼠标世界坐标（z轴的值为物体屏幕坐标的z值）
            m_Offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3
            (eventData.position.x, eventData.position.y, m_TargetScreenVec.z));
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} OnDrag");
        if (gameObject.name.Equals("Cube"))
        {
            //gameObject.transform.position = Camera.main.ScreenToWorldPoint(eventData.position);
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, m_TargetScreenVec.z)) + m_Offset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} OnEndDrag + {eventData.position}");
        if (gameObject.name.Equals("Cube"))
        {
            gameObject.transform.position = m_OrigPos;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} OnPointerEnter++++++++++++++++");
        HoverTipText.text = $"{gameObject.name}";
        HoverTipGo.transform.position = eventData.position + new Vector2(hoverTipRec.sizeDelta.x, hoverTipRec.sizeDelta.y);
        HoverTipGo.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} OnPointerExit++++++++++++++++");
        HoverTipText.text = string.Empty;
        HoverTipGo.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} OnPointerClick");
    }
#endif
}
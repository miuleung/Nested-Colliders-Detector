using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;

[DisallowMultipleComponent]
public class RayEventBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerMoveHandler
    , IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool m_IsPriorityOnTop;
    public bool IsPriorityOnTop { get { return m_IsPriorityOnTop; } set { m_IsPriorityOnTop = value; } }
    public UnityEvent<PointerEventData> OnPointerClickCallback;
    public UnityEvent<PointerEventData> OnPointerEnterCallback;
    public UnityEvent<PointerEventData> OnPointerExitCallback;

    public UnityEvent<PointerEventData> OnBeginDragCallback;
    public UnityEvent<PointerEventData> OnDragCallback;
    public UnityEvent<PointerEventData> OnEndDragCallback;

    private int arrayLength = 50;

    private Ray ray;

    private RaycastHit[] curHitArray;
    private List<RaycastHit> curCheckHitList;
    private int checkHitLength;

    //OnMove中使用
    private RaycastHit[] moveHitArray;
    private List<RaycastHit> curMoveHitList;
    private RaycastHit[] hitArrayLast;
    private int moveHitLength = 0;
    private int moveHitLengthLast = 0;

    public LayerMask BaseLayerMask { get { return baseLayerMask; } set { baseLayerMask = value; } }
    private LayerMask baseLayerMask = ~0;

    /// <summary>
    /// 是否本体发生交互
    /// 鼠标是否进入物体
    /// </summary>
    public bool IsSelfInteract
    {
        get { return isSelfInteract; }
    }
    private bool isSelfInteract = false;
    /// <summary>
    /// 拖拽纪录物体
    /// </summary>
    public GameObject DragGo { get { return m_DragGo; } }
    private GameObject m_DragGo;
    private RayEventBase m_targetScript;
    /// <summary>
    /// 是否正在拖拽中
    /// </summary>
    public bool IsSelfDrag { get { return m_IsSelfDrag; } }
    private bool m_IsSelfDrag;

    public virtual void Awake()
    {
        curHitArray = new RaycastHit[arrayLength];
        hitArrayLast = new RaycastHit[arrayLength];
        moveHitArray = new RaycastHit[arrayLength];
    }

    #region 接口实现
    public async void OnPointerClick(PointerEventData eventData)
    {
        if (!await MultiColliderCheck(InteractType.Click, eventData))
        {
            return;
        }
        OnPointerClickCallback?.Invoke(eventData);
    }

    public async void OnPointerEnter(PointerEventData eventData)
    {
        if (!await MultiColliderCheck(InteractType.Enter, eventData))
        {
            return;
        }
        OnPointerEnterCallback?.Invoke(eventData);
    }

    public async void OnPointerExit(PointerEventData eventData)
    {
        if (!await MultiColliderCheck(InteractType.Exit, eventData))
        {
            return;
        }
        OnPointerExitCallback?.Invoke(eventData);
        isSelfInteract = false;
    }

    public async void OnBeginDrag(PointerEventData eventData)
    {
        if (!await MultiColliderCheck(InteractType.BeginDrag, eventData))
        {
            return;
        }
        m_DragGo = this.gameObject;
        m_targetScript = this.gameObject.GetComponent<RayEventBase>();
        m_IsSelfDrag = true;
        OnBeginDragCallback?.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (m_IsSelfDrag)
        {
            OnEndDragCallback?.Invoke(eventData);
            m_DragGo = null;
            m_targetScript = null;
            m_IsSelfDrag = false;
            return;
        }
        if (m_DragGo == null) return;
        ExecuteEvents.Execute(m_DragGo, eventData, ExecuteEvents.endDragHandler);
    }
    #endregion

    #region Raycast
    private async UniTask<bool> MultiColliderCheck(InteractType interactType, PointerEventData eventData)
    {
        checkHitLength = 0;
        switch (interactType)
        {
            case InteractType.Enter:
                ray = eventData.enterEventCamera.ScreenPointToRay(eventData.position);
                checkHitLength = Physics.RaycastNonAlloc(ray, curHitArray, float.MaxValue, baseLayerMask.value);
                if (checkHitLength > 0)
                {
                    curCheckHitList = await curHitArray.ToUniTaskAsyncEnumerable().Take(checkHitLength).ToListAsync();
                    curCheckHitList.Sort((x, y) => x.distance.CompareTo(y.distance));
                    if (curCheckHitList[curCheckHitList.Count - 1].collider.gameObject != this.gameObject)
                    {
                        isSelfInteract = false;
                        return isSelfInteract;
                    }
                    isSelfInteract = true;
                    return isSelfInteract;
                }
                isSelfInteract = false;
                return isSelfInteract;
            case InteractType.Exit:
                return isSelfInteract;
            case InteractType.Click:
                ray = eventData.pressEventCamera.ScreenPointToRay(eventData.position);
                checkHitLength = Physics.RaycastNonAlloc(ray, curHitArray, float.MaxValue, baseLayerMask.value);
                if (checkHitLength > 0)
                {
                    curCheckHitList = await curHitArray.ToUniTaskAsyncEnumerable().Take(checkHitLength).ToListAsync();
                    curCheckHitList.Sort((x, y) => x.distance.CompareTo(y.distance));
                    if (checkHitLength == 1 && curCheckHitList[0].collider.gameObject == this.gameObject)
                    {
                        return true;
                    }
                    var script = curCheckHitList[curCheckHitList.Count - 1].collider.gameObject.GetComponent<RayEventBase>();
                    if (script != null && script.gameObject != this.gameObject && script.IsSelfInteract)
                    {
                        ExecuteEvents.Execute(curCheckHitList[curCheckHitList.Count - 1].collider.gameObject, eventData, ExecuteEvents.pointerClickHandler);
                        return false;
                    }
                    return true;
                }
                return false;
            case InteractType.BeginDrag:
                ray = eventData.pressEventCamera.ScreenPointToRay(eventData.position);
                checkHitLength = Physics.RaycastNonAlloc(ray, curHitArray, float.MaxValue, baseLayerMask.value);
                if (checkHitLength > 0)
                {
                    curCheckHitList = await curHitArray.ToUniTaskAsyncEnumerable().Take(checkHitLength).ToListAsync();
                    curCheckHitList.Sort((x, y) => x.distance.CompareTo(y.distance));
                    if (checkHitLength == 1 && curCheckHitList[0].collider.gameObject == this.gameObject)
                    {
                        var script0 = curCheckHitList[0].collider.gameObject.GetComponent<RayEventBase>();
                        if (script0.IsSelfInteract)
                        {
                            return true;
                        }
                    }
                    var script = curCheckHitList[curCheckHitList.Count - 1].collider.gameObject.GetComponent<RayEventBase>();
                    if (script != null && script.gameObject != this.gameObject && script.IsSelfInteract)
                    {
                        m_DragGo = curCheckHitList[curCheckHitList.Count - 1].collider.gameObject;//需要记录实际发生拖拽的实例，以便EndDrag使用
                        m_targetScript = m_DragGo.GetComponent<RayEventBase>();
                        ExecuteEvents.Execute(curCheckHitList[curCheckHitList.Count - 1].collider.gameObject, eventData, ExecuteEvents.beginDragHandler);
                        return false;
                    }
                    return true;
                }
                return false;
        }
        return false;
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (m_DragGo == null) return;

        if (m_targetScript != null && m_targetScript.m_IsSelfDrag)
        {
            m_targetScript.OnDragCallback?.Invoke(eventData);
        }
    }

    public async void OnPointerMove(PointerEventData eventData)
    {
        moveHitLength = 0;
        ray = Camera.main.ScreenPointToRay(eventData.position);
        moveHitLength = Physics.RaycastNonAlloc(ray, moveHitArray, float.MaxValue, baseLayerMask.value);
        if (moveHitLength > 0)
        {
            curMoveHitList = await moveHitArray.ToUniTaskAsyncEnumerable().Take(moveHitLength).ToListAsync();
            curMoveHitList.Sort((x, y) => x.distance.CompareTo(y.distance));
            //旧的响应退出事件
            if (moveHitLengthLast > 0)
            {
                var e = hitArrayLast.ToUniTaskAsyncEnumerable().Take(moveHitLengthLast).GetAsyncEnumerator();
                try
                {
                    while (await e.MoveNextAsync())
                    {
                        if (curMoveHitList[curMoveHitList.Count - 1].collider.gameObject != e.Current.collider.gameObject)
                        {
                            var script = e.Current.collider.gameObject.GetComponent<RayEventBase>();
                            if (script != null && script.IsSelfInteract)
                            {
                                ExecuteEvents.Execute(e.Current.collider.gameObject, eventData, ExecuteEvents.pointerExitHandler);
                            }
                        }
                    }
                }
                finally
                {
                    if (e != null)
                    {
                        await e.DisposeAsync();
                    }
                }
            }
            //处理新的事件
            for (int i = 0; i < curMoveHitList.Count; i++)
            {
                var script = curMoveHitList[i].collider.gameObject.GetComponent<RayEventBase>();
                if (script != null && script.IsSelfInteract && i != curMoveHitList.Count - 1)//外层的响应退出事件
                {
                    ExecuteEvents.Execute(curMoveHitList[i].collider.gameObject, eventData, ExecuteEvents.pointerExitHandler);
                }
                if (script != null && !script.IsSelfInteract && i == curMoveHitList.Count - 1)//最里层响应进入事件
                {
                    ExecuteEvents.Execute(curMoveHitList[i].collider.gameObject, eventData, ExecuteEvents.pointerEnterHandler);
                }
            }
            moveHitLengthLast = moveHitLength;
            curMoveHitList.CopyTo(hitArrayLast);
            return;
        }
        else
        {
            if (moveHitLengthLast > 0)
            {
                var e = hitArrayLast.ToUniTaskAsyncEnumerable().Take(moveHitLengthLast).GetAsyncEnumerator();
                try
                {
                    while (await e.MoveNextAsync())
                    {
                        var script = e.Current.collider.gameObject.GetComponent<RayEventBase>();
                        if (script != null && script.IsSelfInteract)
                        {
                            ExecuteEvents.Execute(e.Current.collider.gameObject, eventData, ExecuteEvents.pointerExitHandler);
                        }
                    }
                }
                finally
                {
                    if (e != null)
                    {
                        await e.DisposeAsync();
                    }
                }
            }
            curMoveHitList?.Clear();
            moveHitLengthLast = moveHitLength;
        }
    }
    #endregion
}
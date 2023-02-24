using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class RayEventBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerMoveHandler
{
    public UnityEvent<PointerEventData> OnPointerClickCallback;
    public UnityEvent<PointerEventData> OnPointerEnterCallback;
    public UnityEvent<PointerEventData> OnPointerExitCallback;

    private int arrayLength = 50;

    private Ray ray;

    private RaycastHit[] hitArray;
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

    //是否本体发生交互
    public bool IsSelfInteract
    {
        get { return isSelfInteract; }
    }
    private bool isSelfInteract = false;

    public virtual void Awake()
    {
        hitArray = new RaycastHit[arrayLength];
        hitArrayLast = new RaycastHit[arrayLength];
        moveHitArray = new RaycastHit[arrayLength];
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!MultiColliderCheck(InteractType.Click, eventData))
        {
            return;
        }
        OnPointerClickCallback?.Invoke(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!MultiColliderCheck(InteractType.Enter, eventData))
        {
            return;
        }
        OnPointerEnterCallback?.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!MultiColliderCheck(InteractType.Exit, eventData))
        {
            return;
        }
        OnPointerExitCallback?.Invoke(eventData);
        isSelfInteract = false;
    }

    #region Raycast
    private bool MultiColliderCheck(InteractType interactType, PointerEventData eventData)
    {
        checkHitLength = 0;
        switch (interactType)
        {
            case InteractType.Enter:
                ray = eventData.enterEventCamera.ScreenPointToRay(eventData.position);
                checkHitLength = Physics.RaycastNonAlloc(ray, hitArray, float.MaxValue, baseLayerMask.value);
                if (checkHitLength > 0)
                {
                    curCheckHitList = hitArray.Take(checkHitLength).ToList();
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
                checkHitLength = Physics.RaycastNonAlloc(ray, hitArray, float.MaxValue, baseLayerMask.value);
                if (checkHitLength > 0)
                {
                    curCheckHitList = hitArray.Take(checkHitLength).ToList();
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
        }
        return false;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        moveHitLength = 0;
        ray = Camera.main.ScreenPointToRay(eventData.position);
        moveHitLength = Physics.RaycastNonAlloc(ray, moveHitArray, float.MaxValue, baseLayerMask.value);
        if (moveHitLength > 0)
        {
            curMoveHitList = moveHitArray.Take(moveHitLength).ToList();
            curMoveHitList.Sort((x, y) => x.distance.CompareTo(y.distance));
            //旧的响应退出事件
            if (moveHitLengthLast > 0)
            {
                foreach (var item in hitArrayLast.Take(moveHitLengthLast))
                {
                    if (curMoveHitList[curMoveHitList.Count - 1].collider.gameObject != item.collider.gameObject)
                    {
                        var script = item.collider.gameObject.GetComponent<RayEventBase>();
                        if (script != null && script.IsSelfInteract)
                        {
                            ExecuteEvents.Execute(item.collider.gameObject, eventData, ExecuteEvents.pointerExitHandler);
                        }
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
                foreach (var item in hitArrayLast.Take(moveHitLengthLast))
                {
                    var script = item.collider.gameObject.GetComponent<RayEventBase>();
                    if (script != null && script.IsSelfInteract)
                    {
                        ExecuteEvents.Execute(item.collider.gameObject, eventData, ExecuteEvents.pointerExitHandler);
                    }
                }
            }
            curMoveHitList?.Clear();
            moveHitLengthLast = moveHitLength;
        }
    }
    #endregion
}
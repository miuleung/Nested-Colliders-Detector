# NestedCollidersDetector

һ���ڶ����ײ�����Ƕ�ס��ڵ�����£����д�͸������Demo��

# Summary

1. PC�¼�⣬Ҳֻ��PC�д�����
2. ����C���������̨Log
3. ֧��LayerMask����ɸѡ�����ײ����

# How to use?

1.�̳�RayEventBase
2.ע���¼�

> ע���¼�

```csharp
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
```

3.�¼��߼�

> �¼��߼�

```csharp
    private void OnPointerClick_(PointerEventData eventData)
    {
        //dosomthing
    }

    private void OnPointerEnter_(PointerEventData eventData)
    {
         //dosomthing
    }

    private void OnPointerExit_(PointerEventData eventData)
    {
         //dosomthing
    }
```

# Demo GIF



# Roadmap

1.�����Ż�
2.���Drag���¼���չ
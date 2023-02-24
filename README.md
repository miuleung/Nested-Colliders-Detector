# NestedCollidersDetector

一个在多个碰撞器多层嵌套、遮挡情况下，进行穿透交互的Demo。

# Summary

1. PC下检测，也只有PC有此需求
2. 按键C可清除控制台Log
3. 支持LayerMask配置筛选检测碰撞器。

# How to use?

1.继承RayEventBase
2.注册事件

> 注册事件

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

3.事件逻辑

> 事件逻辑

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

1.性能优化
2.添加Drag等事件扩展
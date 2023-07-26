# NestedCollidersDetector

һ���ڶ����ײ�����Ƕ�ס��ڵ�����£����д�͸������Demo��

# Summary

1. PC��ʹ�ã�Ҳֻ��PC�д�����
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
        OnPointerClickCallback.AddListener(_OnPointerClick);
        OnPointerEnterCallback = new UnityEvent<PointerEventData>();
        OnPointerEnterCallback.AddListener(_OnPointerEnter);
        OnPointerExitCallback = new UnityEvent<PointerEventData>();
        OnPointerExitCallback.AddListener(_OnPointerExit);
        BaseLayerMask = ~(1 << 10);
    }
```

3.�¼��߼�

> �¼��߼�

```csharp
    private void _OnPointerClick(PointerEventData eventData)
    {
        //dosomthing
    }

    private void _OnPointerEnter(PointerEventData eventData)
    {
         //dosomthing
    }

    private void _OnPointerExit(PointerEventData eventData)
    {
         //dosomthing
    }
```

# Demo GIF

>Ƕ��/�ڵ���ײ���㼶�ṹչʾ��
![](./GIF/gif1.gif)

>����չʾ�����ϽǺ�ɫCubeΪLayerMask���壻LogΪ����������
![](./GIF/gif2.gif)

# Reference

HighlightingSystem����Ŀ�����˸�������������й����룬���û�����Ƴ���ش��뼴��

UniTask

# Roadmap

1.�����ԭ��Raycast�����໥���õ�����

# UIEventListener

[![Version](https://img.shields.io/npm/v/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)
[![Downloads](https://img.shields.io/npm/dm/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)
[![DeepWiki](https://img.shields.io/badge/DeepWiki-Explore-blue)](https://deepwiki.com/eframework-org/U3D.UGUI)

UIEventListener 是一个 UI 事件监听器组件，封装了多种 Unity UI 事件接口，用于处理 UI 交互的各种事件。

## 功能特性

- Unity UI 事件系统接口封装
- 支持事件回调和事件委托绑定

## 使用手册

### 组件获取

1. 获取或添加监听器：
  - 获取GameObject上的监听器，如果不存在则自动添加：
    ```csharp
    UIEventListener listener = UIEventListener.Get(gameObject);
    ```
  - 也可以传入组件来获取其GameObject上的监听器：
    ```csharp
    UIEventListener listener = UIEventListener.Get(transform);
    ```

### 事件绑定

1. 使用事件委托：
  ```csharp
  // 使用事件委托绑定点击事件
  listener.OnPointerClickEvent += (eventData) => {
      Debug.Log("点击了：" + gameObject.name);
  };
  ```

2. 使用回调函数：
  ```csharp
  // 直接指定回调函数
  listener.OnPointerClickFunc = HandleClick;
  
  // 回调函数定义
  private void HandleClick(PointerEventData eventData) {
      Debug.Log("点击了：" + eventData.pointerCurrentRaycast.gameObject.name);
  }
  ```

## 常见问题

### 1. 如何判断一个 UI 元素当前是否被按下？

UIEventListener组件包含一个`IsPressed`属性，可以用来检查UI元素当前的按压状态：

```csharp
UIEventListener listener = UIEventListener.Get(buttonObj);
if (listener.IsPressed) {
    // 按钮当前被按下
    Debug.Log("按钮处于按下状态");
}
```

### 2. UIEventListener 支持哪些类型的事件？

UIEventListener支持以下类型的事件：
  - 按钮选中/取消选中 (OnButtonSelectEvent/OnButtonDeselectEvent)
  - 指针点击 (OnPointerClickEvent)
  - 指针按下/抬起 (OnPointerDownEvent/OnPointerUpEvent)
  - 指针进入/退出 (OnPointerEnterEvent/OnPointerExitEvent)
  - 拖拽相关 (OnBeginDragEvent/OnDragEvent/OnEndDragEvent)

更多问题，请查阅[问题反馈](../CONTRIBUTING.md#问题反馈)。

## 项目信息

- [更新记录](../CHANGELOG.md)
- [贡献指南](../CONTRIBUTING.md)
- [许可证](../LICENSE)

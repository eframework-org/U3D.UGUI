# UIEventListener

`UIEventListener` 是一个UI事件监听器组件，用于处理UI交互的各种事件。它实现了Unity UI事件系统的多种接口，简化事件处理和回调管理。

## 功能特性

- 统一封装Unity UI事件系统的各种交互接口
- 支持事件回调和事件委托两种绑定方式
- 提供静态帮助方法快速获取或添加组件
- 自动跟踪按钮的按下状态

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

### 事件绑定方式

1. 使用事件委托（推荐）：
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

### 常见用例

1. 点击事件处理：
   ```csharp
   var button = UIEventListener.Get(buttonObj);
   button.OnPointerClickEvent += (data) => {
       // 处理点击逻辑
       Debug.Log("按钮被点击");
   };
   ```

2. 拖拽功能实现：
   ```csharp
   var dragObj = UIEventListener.Get(dragableObj);
   dragObj.OnBeginDragEvent += (data) => { 
       // 开始拖拽逻辑
       Debug.Log("开始拖拽");
   };
   dragObj.OnDragEvent += (data) => { 
       // 更新位置
       dragableObj.transform.position = data.position;
   };
   dragObj.OnEndDragEvent += (data) => { 
       // 结束拖拽逻辑
       Debug.Log("结束拖拽");
   };
   ```

3. 悬停效果：
   ```csharp
   var hoverObj = UIEventListener.Get(imageObj);
   hoverObj.OnPointerEnterEvent += (data) => {
       // 鼠标进入效果
       imageObj.GetComponent<Image>().color = Color.grey;
   };
   hoverObj.OnPointerExitEvent += (data) => {
       // 鼠标离开效果
       imageObj.GetComponent<Image>().color = Color.white;
   };
   ```

## 常见问题

### Q: UIEventListener和Button组件有什么区别？

A: Button组件主要处理点击功能，而UIEventListener提供了更全面的事件处理能力，包括点击、拖拽、悬停等多种交互类型。它允许你通过代码更灵活地管理UI事件，而不需要在Inspector中配置事件响应。

### Q: 如何判断一个UI元素当前是否被按下？

A: UIEventListener组件包含一个`IsPressed`属性，可以用来检查UI元素当前的按压状态：

```csharp
UIEventListener listener = UIEventListener.Get(buttonObj);
if (listener.IsPressed) {
    // 按钮当前被按下
    Debug.Log("按钮处于按下状态");
}
```

### Q: 一个GameObject可以有多个UIEventListener组件吗？

A: 不建议在同一个GameObject上添加多个UIEventListener组件。通过`UIEventListener.Get()`方法获取组件时，如果已存在则返回现有组件，如果不存在则添加新组件，确保每个GameObject只有一个UIEventListener实例。

### Q: 如何移除已添加的事件监听？

A: 可以使用C#事件委托的`-=`操作符移除之前添加的事件监听：

```csharp
// 定义事件处理方法
void HandleClick(PointerEventData data) {
    Debug.Log("点击");
}

// 添加事件监听
listener.OnPointerClickEvent += HandleClick;

// 移除事件监听
listener.OnPointerClickEvent -= HandleClick;
```

### Q: UIEventListener支持哪些类型的事件？

A: UIEventListener支持以下类型的事件：
- 按钮选中/取消选中 (OnButtonSelectEvent/OnButtonDeselectEvent)
- 指针点击 (OnPointerClickEvent)
- 指针按下/抬起 (OnPointerDownEvent/OnPointerUpEvent)
- 指针进入/退出 (OnPointerEnterEvent/OnPointerExitEvent)
- 拖拽相关 (OnBeginDragEvent/OnDragEvent/OnEndDragEvent)

更多问题，请查阅[问题反馈](../CONTRIBUTING.md#问题反馈)。

## 技术细节

- 实现了Unity UI事件系统的多个接口：
  - ISelectHandler, IDeselectHandler
  - IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
  - IBeginDragHandler, IDragHandler, IEndDragHandler
- 提供了事件委托和回调函数两种方式进行事件响应
- 使用非序列化的布尔字段`IsPressed`跟踪UI元素的按下状态
- 通过静态方法简化了获取和添加组件的过程

## 项目信息

- [更新记录](../CHANGELOG.md)
- [贡献指南](../CONTRIBUTING.md)
- [许可证](../LICENSE)

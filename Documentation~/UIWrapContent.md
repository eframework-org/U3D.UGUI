# UIWrapContent

`UIWrapContent` 是一个循环列表内容组件，扩展自ScrollRect，提供高效的大数据列表和网格显示能力，通过元素复用降低内存占用。

## 功能特性

- 支持多种布局模式：垂直列表、水平列表、垂直网格、水平网格
- 高效的元素复用机制，只创建可见区域所需的UI元素
- 支持单一原型和多原型模式，适应不同的列表需求
- 提供灵活的适配器接口，便于自定义数据绑定逻辑
- 自动处理元素的创建、回收和位置更新
- 支持动态调整内容尺寸和滚动位置
- 支持元素缓存，减少重复创建开销

## 使用手册

### 基本设置

1. 组件添加：
   ```csharp
   // 在UI对象上添加UIWrapContent组件
   var wrapContent = gameObject.AddComponent<UIWrapContent>();
   
   // 设置viewport和content引用
   wrapContent.viewport = viewportRectTransform;
   wrapContent.content = contentRectTransform;
   ```

2. 布局模式选择：
   ```csharp
   // 选择布局模式
   wrapContent.Layout = UIWrapContent.LayoutMode.VerticalList;  // 垂直列表
   // 或其他选项：HorizontalList、VerticalGrid、HorizontalGrid
   
   // 设置相应的参数（segment表示间距或网格的行/列数，padding表示边距）
   wrapContent.segment = 10; // 间距或行列数
   wrapContent.padding = new Vector2(10, 10); // 左/上和右/下内边距
   ```

### 适配器实现

1. 简单适配器（单一类型项目）：
   ```csharp
   // 创建简单适配器
   wrapContent.Adapter = new UIWrapContent.ISimpleAdapter(
       wrapContent,
       () => dataList.Count, // 获取数据总数
       (cell, index) => {
           // 设置单元格数据
           cell.GetComponent<Text>().text = dataList[index].ToString();
       }
   );
   ```

2. 自定义适配器（多类型项目）：
   ```csharp
   // 创建自定义适配器类
   public class MyAdapter : UIWrapContent.ICustomAdapter 
   {
       private List<ItemData> dataList;
       
       public MyAdapter(List<ItemData> dataList) {
           this.dataList = dataList;
       }
       
       public bool SingleZygote => false; // 多种单元格类型
       
       public int GetCellType(int index) {
           return dataList[index].type; // 根据数据类型返回
       }
       
       public RectTransform GetCellZygote(int type) => Object.FindAnyObjectByType<UIWrapContent>().content.GetChild(0).GetComponent<RectTransform>();
       
       public int GetCellCount() {
           return dataList.Count;
       }
       
       public void SetCellData(RectTransform cell, int index) {
           var data = dataList[index];
           // 根据数据更新UI元素
           if (GetCellType(index) == 0) {
               // 设置类型A单元格的数据
               var nameText = cell.Find("Name").GetComponent<Text>();
               nameText.text = data.name;
           } else {
               // 设置类型B单元格的数据
               var valueText = cell.Find("Value").GetComponent<Text>();
               valueText.text = data.value.ToString();
           }
       }
   }
   
   // 使用自定义适配器
   wrapContent.Adapter = new MyAdapter(dataList);
   ```

### 运行时控制

1. 滚动控制：
   ```csharp
   // 滚动到指定位置（0-1范围）
   wrapContent.ScrollTo(0.5f); // 滚动到中间位置
   wrapContent.ScrollTo(0f);   // 滚动到开头
   wrapContent.ScrollTo(1f);   // 滚动到末尾
   ```

2. 数据更新：
   ```csharp
   // 数据变更后重新加载列表
   dataList.Add(new ItemData());
   wrapContent.Reload();
   ```

3. 遍历可见项：
   ```csharp
   // 遍历当前可见的所有单元格
   wrapContent.EachItem((index, item) => {
       Debug.Log($"可见项: 索引={index}, 名称={item.name}");
       
       // 可以对可见项进行操作，例如高亮显示
       if (index == selectedIndex) {
           item.GetComponent<Image>().color = Color.yellow;
       }
   });
   ```

## 常见问题

### Q: UIWrapContent与传统的ScrollRect有什么区别？

A: 传统ScrollRect会为每个数据项创建一个UI元素，当数据量大时会导致性能问题和内存占用过高。UIWrapContent采用元素复用机制，只创建可见区域所需的UI元素，大大提高了性能，特别适合大数据列表。

### Q: 如何在单元格中添加点击事件？

A: 可以在适配器的SetCellData方法中为单元格添加点击事件：

```csharp
public void SetCellData(RectTransform cell, int index) {
    // 设置数据
    cell.GetComponent<Text>().text = dataList[index].ToString();
    
    // 添加点击事件
    var button = cell.GetComponent<Button>();
    if (button) {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {
            OnItemClicked(index);
        });
    }
}

private void OnItemClicked(int index) {
    Debug.Log($"点击了项目：{index}");
    // 处理点击逻辑
}
```

### Q: 如何处理不同高度/宽度的单元格？

A: UIWrapContent的多原型模式支持不同尺寸的单元格，但在同一布局中每种类型的单元格尺寸应保持一致。实现示例：

```csharp
public RectTransform GetCellZygote(int type) {
    switch (type) {
        case 0: return smallCellTemplate; // 小尺寸单元格
        case 1: return normalCellTemplate; // 普通尺寸单元格
        case 2: return largeCellTemplate; // 大尺寸单元格
        default: return normalCellTemplate;
    }
}
```

### Q: 为什么我的列表在滚动时会出现跳跃或闪烁？

A: 可能的原因和解决方法：

1. 单元格尺寸计算不准确：确保在GetCellZygote方法中返回正确尺寸的单元格
2. 布局参数不合理：调整spacing（间距）和padding（内边距）参数
3. 滚动速度过快：可以在ScrollRect组件上调整减速率(decelerationRate)和弹性(elasticity)参数
4. 内容尺寸不正确：确保适配器的GetCellCount方法返回准确的数据数量

### Q: 如何优化UIWrapContent的性能？

A: 以下是一些优化建议：

1. 减少单元格预制体的复杂度，尽量使用简单的UI结构
2. 适当设置缓冲区大小，避免频繁创建和销毁单元格
3. 在SetCellData方法中避免执行昂贵的操作
4. 尽可能使用对象池技术来缓存单元格实例
5. 考虑实现数据虚拟化，只加载需要显示的数据

更多问题，请查阅[问题反馈](../CONTRIBUTING.md#问题反馈)。

## 技术细节

- 扩展自Unity UI的ScrollRect，保留了所有ScrollRect的基本功能
- 通过四种布局模式支持不同的显示需求：
  - VerticalList：垂直列表，单列滚动布局
  - HorizontalList：水平列表，单行滚动布局
  - VerticalGrid：垂直网格，多列垂直滚动布局
  - HorizontalGrid：水平网格，多行水平滚动布局
- 使用RecyleModule实现元素回收和复用机制：
  - 维护已使用池(UsedPool)和缓存池(CachedPool)管理UI元素
  - 动态创建和回收单元格，减少内存占用
  - 有效避免了大数据量下的性能问题
- 提供ICustomAdapter接口支持自定义适配器：
  - SingleZygote选项支持单一或多种单元格类型
  - GetCellType确定数据项使用哪种单元格类型
  - GetCellZygote获取对应类型的单元格模板
  - GetCellCount获取总数据数量
  - SetCellData设置单元格数据
- 支持单元格缓存，避免频繁的GameObject创建和销毁操作
- 使用ScrollTo方法支持编程控制滚动位置
- EachItem方法便于遍历和操作可见的单元格

## 项目信息

- [更新记录](../CHANGELOG.md)
- [贡献指南](../CONTRIBUTING.md)
- [许可证](../LICENSE)

# UIWrapContent

[![Version](https://img.shields.io/npm/v/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)
[![Downloads](https://img.shields.io/npm/dm/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)

UIWrapContent 是一个循环列表内容组件，扩展自ScrollRect，提供高效的大数据列表和网格显示能力，通过元素复用降低内存占用。

## 功能特性

- 支持多种布局模式：垂直列表、水平列表、垂直网格、水平网格
- 高效的元素复用机制，只创建可见区域所需的UI元素
- 支持单一原型和多原型模式，适应不同的列表需求
- 提供灵活的适配器接口，便于自定义数据绑定逻辑
- 自动处理元素的创建、回收和位置更新
- 支持动态调整内容尺寸和滚动位置
- 支持元素缓存，减少重复创建开销

## 使用手册

### 适配器实现

1. 简单适配器：
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

2. 自定义适配器：
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
   // 重新加载列表
   wrapContent.Reload();
   ```

3. 遍历可见项：
   ```csharp
   // 遍历当前可见的所有单元格
   wrapContent.EachItem((index, item) => {
       Debug.Log($"可见项: 索引={index}, 名称={item.name}");
   });
   ```

## 常见问题

### Q: UIWrapContent与传统的ScrollRect有什么区别？

A: 传统ScrollRect会为每个数据项创建一个UI元素，当数据量大时会导致性能问题和内存占用过高。UIWrapContent采用元素复用机制，只创建可见区域所需的UI元素，大大提高了性能，特别适合大数据列表。

### Q: 如何优化UIWrapContent的性能？

A: 以下是一些优化建议：

1. 减少单元格预制体的复杂度，尽量使用简单的UI结构
2. 适当设置缓冲区大小，避免频繁创建和销毁单元格
3. 在SetCellData方法中避免执行昂贵的操作

更多问题，请查阅[问题反馈](../CONTRIBUTING.md#问题反馈)。

## 项目信息

- [更新记录](../CHANGELOG.md)
- [贡献指南](../CONTRIBUTING.md)
- [许可证](../LICENSE)

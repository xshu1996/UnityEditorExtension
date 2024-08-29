# UnityEditorExtension
Unity编辑器扩展和常用功能的集合

## 已支持的功能

- Editor Window
  - 支持获取Unity Editor Window 内置所有 Icon
  - 查找被静态字段引用的资源, 避免资源无法被GC
  - 将指定目录的 `*.lua` 文件批量迁移到指定目录并修改后缀为 `.lua.txt` 且指定 **AssetBundle 包名** 
  - 资源监控窗口, 可以查看所有图片被引用的次数以及被谁引用、重复的图片资源(不同名或者不同路径base64相同)、图片的硬盘占用和内存占用
- Gizmos
  - 全场景检索开启**Raycast**的物体,并用Gizmos描边突出显示
- MenuItem
  - 实现**锁定/解锁** Inspector面板快捷键
  - 实现清空控制台打印快捷键
  - 实现锁定/解锁选中的GameObject
- Attribute
  - 实现MonoBehavior支持**根据条件在Inspector显隐**的字段
  - 实现获取项目SceneName并生成枚举装饰给组件的字段
  - 实现将**组件的属性显示在Inspector面板**
- Other
  - 代理全局点击事件, 实现在全局的点击事件之后插入想要执行的委托, 例如: 全局添加按钮点击事件



## TODO List

- 引用查找器丰富更多的查找信息与优化窗口交互细节
- 资源监控窗口支持更多类型的资源
- 实现自定义AssetBundle构建流程

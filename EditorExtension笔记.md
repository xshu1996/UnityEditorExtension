# UnityEditorExtension
# MenuItem

### 1. 路径前缀设置

- Asset/
- GameObject/
- Window/
- CONTEXT/${ComponentName}: 选项扩展在组件右上角三个点选项列表, 同路径可重写引擎自带选项
- ${CustomPath}/: 自定义顶部菜单栏



## ScriptedImporter

自定义资源导入类型 ``[ScriptedImporter(int version, string fileExtension)]``

**注意：Scripted Importer 无法处理已由 Unity 本身处理的文件扩展名。**
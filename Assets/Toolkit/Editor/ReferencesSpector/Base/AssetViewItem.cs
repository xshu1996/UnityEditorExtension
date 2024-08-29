
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace Toolkit.Editor.ReferencesSpector.Base
{
    public interface IAssetData
    {
        string assetPath { get; set; }
        long memorySize { get; set; }
        long runtimeMemorySize { get; set; }
        int referenceCount { get; }
        string Guid { get; set; }
    }

    // 带数据的TreeViewItem
    public class AssetViewItem<T> : TreeViewItem where T : IAssetData
    {
        public T data;
    }

    /// <summary>
    /// 资源引用树
    /// </summary>
    public class AssetTreeView<T> : TreeView where T : IAssetData
    {
        /// <summary>
        /// 图标宽度
        /// </summary>
        const float m_IconWidth = 18f;

        /// <summary>
        /// 列表高度>
        /// </summary>
        const float m_RowHeights = 20f;
        public AssetViewItem<T> assetRoot;

        private GUIStyle _stateGUIStyle = new GUIStyle { richText = true, alignment = TextAnchor.MiddleCenter };

        /// <summary>
        /// 列信息枚举
        /// </summary>
        enum MyColumns
        {
            Name, Path, refCount, MemorySize, RuntimeMemorySize,
        }

        public AssetTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
            rowHeight = m_RowHeights;
            columnIndexForTreeFoldouts = 0;
            showAlternatingRowBackgrounds = true;
            showBorder = false;
            // center foldout in the row since we also center content. See RowGUI
            customFoldoutYOffset = (m_RowHeights - EditorGUIUtility.singleLineHeight) * 0.5f;
            extraSpaceBeforeIconAndLabel = m_IconWidth;
        }

        /// <summary>
        /// 响应右击事件
        /// </summary>
        /// <param name="id"></param>
        protected override void ContextClickedItem(int id)
        {
            GenericMenu menu = new GenericMenu();
            var item = (AssetViewItem<T>)FindItem(id, rootItem);
            menu.AddItem(new GUIContent("CopyPath"), false, () =>
            {
                GUIUtility.systemCopyBuffer = item.data.assetPath;
            });
            
            if (item is AssetViewItem<TextureInfo> textInfo)
            {
                menu.AddItem(new GUIContent("PrintReferences"), false, () =>
                {
                    Debug.Log($"========> {textInfo.data.name} References Path: ");
                    foreach (var refPath in textInfo.data.referencePaths)
                    {
                        Debug.Log(refPath);
                    }
                });
            }
            menu.ShowAsContext();
            // SetExpanded(id, !IsExpanded(id));
        }

        /// <summary>
        /// 响应双击事件
        /// </summary>
        /// <param name="id"></param>
        protected override void DoubleClickedItem(int id)
        {
            var item = (AssetViewItem<T>)FindItem(id, rootItem);
            // 在ProjectWindow中高亮双击资源
            if (item != null)
            {
                var assetObject = AssetDatabase.LoadAssetAtPath(item.data.assetPath, typeof(Object));
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = assetObject;
                EditorGUIUtility.PingObject(assetObject);
            }
        }

        /// <summary>
        /// 生成ColumnHeader
        /// </summary>
        /// <param name="treeViewWidth"></param>
        /// <returns></returns>
        public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth)
        {
            var columns = new[]
            {
                // 图标+名称
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Name"),
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = false,
                    width = 200,
                    minWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = false,
                    canSort = true
                },
                // 路径
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("AssetsPath"),
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = false,
                    width = 360,
                    minWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = false,
                    canSort = false
                },
                // 引用次数
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("RefCount"),
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = false,
                    width = 70,
                    minWidth = 70,
                    autoResize = false,
                    allowToggleVisibility = false,
                    canSort = true
                },
                // 硬盘大小
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Memory"),
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = false,
                    width = 70,
                    minWidth = 70,
                    autoResize = false,
                    allowToggleVisibility = true,
                    canSort = true
                },
                // 运行内存大小
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("RuntimeMemory"),
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = false,
                    width = 100,
                    minWidth = 100,
                    autoResize = false,
                    allowToggleVisibility = true,
                    canSort = true
                },
            };

            var state = new MultiColumnHeaderState(columns);
            return state;
        }

        protected override TreeViewItem BuildRoot()
        {
            return assetRoot;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = (AssetViewItem<T>)args.item;
            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                CellGUI(args.GetCellRect(i), item, (MyColumns)args.GetColumn(i), ref args);
            }
        }

        /// <summary>
        /// 绘制列表中的每项内容
        /// </summary>
        /// <param name="cellRect"></param>
        /// <param name="item"></param>
        /// <param name="column"></param>
        /// <param name="args"></param>
        private void CellGUI(Rect cellRect, AssetViewItem<T> item, MyColumns column, ref RowGUIArgs args)
        {
            CenterRectUsingSingleLineHeight(ref cellRect);
            switch (column)
            {
                case MyColumns.Name:
                {
                    var iconRect = cellRect;
                    iconRect.x += GetContentIndent(item);
                    iconRect.width = m_IconWidth;
                    if (iconRect.x < cellRect.xMax)
                    {
                        var icon = GetIcon(item.data.assetPath);
                        if (icon != null)
                            GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
                    }

                    args.rowRect = cellRect;
                    base.RowGUI(args);
                    break;
                }
                case MyColumns.Path:
                {
                    GUI.Label(cellRect, item.data.assetPath);
                    break;
                }
                case MyColumns.refCount:
                {
                    GUI.Label(cellRect, item.data.referenceCount.ToString());
                    break;
                }
                case MyColumns.MemorySize:
                {
                    GUI.Label(cellRect, EditorUtility.FormatBytes(item.data.memorySize));
                    break;
                }
                case MyColumns.RuntimeMemorySize:
                {
                    GUI.Label(cellRect, EditorUtility.FormatBytes(item.data.runtimeMemorySize));
                    break;
                }
            }
        }

        /// <summary>
        /// 根据资源信息获取资源图标
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private Texture2D GetIcon(string path)
        {
            Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            
            if (obj != null)
            {
                Texture2D icon = AssetPreview.GetMiniThumbnail(obj);

                if (icon == null)
                    icon = AssetPreview.GetMiniTypeThumbnail(obj.GetType());
                        
                return icon;
            }

            return null;
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            if (root.children == null) return new List<TreeViewItem>();
            return base.BuildRows(root);
        }

        protected override IList<int> GetAncestors(int id)
        {
            if (rootItem.children == null) return new List<int>();
            return base.GetAncestors(id);
        }

        protected override IList<int> GetDescendantsThatHaveChildren(int id)
        {
            if (rootItem.children == null) return new List<int>();
            return base.GetDescendantsThatHaveChildren(id);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Utility.Attributes
{
#if UNITY_EDITOR
    public class PropertyActiveAttribute : PropertyAttribute
    {
        public enum CompareType
        {
            Equal,
            NonEqual,
            Less,
            LessEqual,
            More,
            MoreEqual,
        }

        public string field;
        public CompareType compareType;
        public object compareValue;

        public PropertyActiveAttribute(string fieldName, CompareType type, object value)
        {
            field = fieldName;
            compareType = type;
            compareValue = value;
        }
    }

    [CustomPropertyDrawer(typeof(PropertyActiveAttribute), false)]
    public class PropertyActiveEditor : PropertyDrawer
    {
        private bool isShow = false;

        /// <summary>
        /// 修改属性占用高度
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attr = attribute as PropertyActiveAttribute;
            if (attr == null) return 0;
            
            var field = attr.field;
            var compareType = attr.compareType;
            var compareValue = attr.compareValue;

            object parent = property.GetActualObjectParent();
            FieldInfo info = parent.GetType().GetField(field);
            object fieldValue = info?.GetValue(parent);

            isShow = IsMeetCondition(fieldValue, compareType, compareValue);
            if (!isShow) return 0;
            return base.GetPropertyHeight(property, label);
        }

        /// <summary>
        /// 符合条件
        /// </summary>
        private bool IsMeetCondition(object fieldValue, PropertyActiveAttribute.CompareType compareType,
            object compareValue)
        {
            if (compareType == PropertyActiveAttribute.CompareType.Equal)
            {
                return fieldValue.Equals(compareValue);
            }
            else if (compareType == PropertyActiveAttribute.CompareType.NonEqual)
            {
                return !fieldValue.Equals(compareValue);
            }
            else if (IsValueType(fieldValue.GetType()) && IsValueType(compareValue.GetType()))
            {
                switch (compareType)
                {
                    case PropertyActiveAttribute.CompareType.Less:
                        return Comparer.DefaultInvariant.Compare(fieldValue, compareValue) < 0;
                    case PropertyActiveAttribute.CompareType.LessEqual:
                        return Comparer.DefaultInvariant.Compare(fieldValue, compareValue) <= 0;
                    case PropertyActiveAttribute.CompareType.More:
                        return Comparer.DefaultInvariant.Compare(fieldValue, compareValue) > 0;
                    case PropertyActiveAttribute.CompareType.MoreEqual:
                        return Comparer.DefaultInvariant.Compare(fieldValue, compareValue) >= 0;
                }
            }

            return false;
        }

        /// <summary>
        /// 是否是值类型
        /// </summary>
        private bool IsValueType(Type type)
        {
            // IsPrimitive就是系统自带的类，IsValueType就是值类型，再排除char剩下的就是int，float这些了
            return (type.IsPrimitive && type.IsValueType && type != typeof(char));
        }

        /// <summary>
        /// 绘制
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (isShow) EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public static class UnityEditorHelper
    {
        public static T GetActualObject<T>(this SerializedProperty property)
        {
            try
            {
                if (property == null)
                    return default(T);
                var serializedObject = property.serializedObject;
                if (serializedObject == null)
                {
                    return default(T);
                }

                var targetObject = serializedObject.targetObject;

                var slicedName = property.propertyPath.Split('.').ToList();
                List<int> arrayCounts = new List<int>();
                for (int index = 0; index < slicedName.Count; index++)
                {
                    arrayCounts.Add(-1);
                    var currName = slicedName[index];
                    if (currName.EndsWith("]"))
                    {
                        var arraySlice = currName.Split('[', ']');
                        if (arraySlice.Length >= 2)
                        {
                            arrayCounts[index - 2] = Convert.ToInt32(arraySlice[1]);
                            slicedName[index] = string.Empty;
                            slicedName[index - 1] = string.Empty;
                        }
                    }
                }

                while (string.IsNullOrEmpty(slicedName.Last()))
                {
                    int i = slicedName.Count - 1;
                    slicedName.RemoveAt(i);
                    arrayCounts.RemoveAt(i);
                }

                return DescendHierarchy<T>(targetObject, slicedName, arrayCounts, 0);
            }
            catch
            {
                return default(T);
            }
        }

        public static object GetActualObjectParent(this SerializedProperty property)
        {
            try
            {
                if (property == null)
                    return default;
                
                // 获取当前序列化的Object
                var serializedObject = property.serializedObject;
                if (serializedObject == null)
                    return default;

                // 获取targetObject，这里的targetObject就是
                // 我不好描述直接举个例子：a.b.c.d.e.f,比如serializedObject就是f，那么targetObject就是a
                var targetObject = serializedObject.targetObject;
                // 还是上面的例子propertyPath其实就是a.b.c.d.e.f
                // 但是如果其中某一个是Array的话假设是b那么就会变成a.b.Array.data[x].c.d.e.f
                // 其中x为index
                var slicedName = property.propertyPath.Split('.').ToList();
                List<int> arrayCounts = new List<int>();
                // 根据"."分好后还需要获取其中的数组及其index保存在一个表中
                for (int index = 0; index < slicedName.Count; index++)
                {
                    arrayCounts.Add(-1);
                    var currName = slicedName[index];
                    if (currName.EndsWith("]"))
                    {
                        var arraySlice = currName.Split('[', ']');
                        if (arraySlice.Length >= 2)
                        {
                            arrayCounts[index - 2] = Convert.ToInt32(arraySlice[1]);
                            slicedName[index] = string.Empty;
                            slicedName[index - 1] = string.Empty;
                        }
                    }
                }

                // 清除数组导致的空
                while (string.IsNullOrEmpty(slicedName.Last()))
                {
                    int i = slicedName.Count - 1;
                    slicedName.RemoveAt(i);
                    arrayCounts.RemoveAt(i);
                }

                // 如果和属性名称相同则清除
                if (slicedName.Last().Equals(property.name))
                {
                    int i = slicedName.Count - 1;
                    slicedName.RemoveAt(i);
                    arrayCounts.RemoveAt(i);
                }

                // 如果空了那么返回targetObject为当前的父对象
                if (slicedName.Count == 0) return targetObject;
                // 继续清除数组，防止父对象也是数组
                while (string.IsNullOrEmpty(slicedName.Last()))
                {
                    int i = slicedName.Count - 1;
                    slicedName.RemoveAt(i);
                    arrayCounts.RemoveAt(i);
                }

                // 如果空了那么返回targetObject为当前的父对象
                if (slicedName.Count == 0) return targetObject;
                // 获取父物体
                return DescendHierarchy<object>(targetObject, slicedName, arrayCounts, 0);
            }
            catch
            {
                return default;
            }
        }
        
        static T DescendHierarchy<T>(object targetObject, List<string> splitName, List<int> splitCounts, int depth)
        {
            if (depth >= splitName.Count)
                return default;

            string currName = splitName[depth];

            if (string.IsNullOrEmpty(currName))
                return DescendHierarchy<T>(targetObject, splitName, splitCounts, depth + 1);

            int arrayIndex = splitCounts[depth];

            var newField = targetObject.GetType().GetField(currName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (newField == null)
            {
                Type baseType = targetObject.GetType().BaseType;
                while (baseType != null && newField == null)
                {
                    newField = baseType.GetField(currName,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    baseType = baseType.BaseType;
                }
            }

            var newObj = newField.GetValue(targetObject);
            if (depth == splitName.Count - 1)
            {
                T actualObject = default(T);
                if (arrayIndex >= 0)
                {
                    if (newObj.GetType().IsArray && ((System.Array)newObj).Length > arrayIndex)
                        actualObject = (T)((System.Array)newObj).GetValue(arrayIndex);

                    var newObjList = newObj as IList;
                    if (newObjList != null && newObjList.Count > arrayIndex)
                    {
                        actualObject = (T)newObjList[arrayIndex];
                    }
                }
                else
                {
                    actualObject = (T)newObj;
                }

                return actualObject;
            }
            else if (arrayIndex >= 0)
            {
                if (newObj is IList)
                {
                    IList list = (IList)newObj;
                    newObj = list[arrayIndex];
                }
                else if (newObj is Array)
                {
                    Array a = (Array)newObj;
                    newObj = a.GetValue(arrayIndex);
                }
            }

            return DescendHierarchy<T>(newObj, splitName, splitCounts, depth + 1);
        }
    }
#endif
}
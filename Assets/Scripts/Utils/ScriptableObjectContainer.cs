using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    public abstract class ScriptableObjectContainer : SerializedScriptableObject
    {
        private readonly List<Object> _childrenObjects = new();

        protected void ClearExtraElements(int size)
        {
            if (size >= _childrenObjects.Count) 
                return;
            RemoveChildren(_childrenObjects.GetRange(size, _childrenObjects.Count - size));
        }

        protected void AddChild(Object child)
        {
            AssetDatabase.AddObjectToAsset(child, this);
            AssetDatabase.SaveAssets();
            _childrenObjects.Add(child);
        }

        protected void AddChildren(List<Object> children)
        {
            foreach (var child in children)
            {
                AssetDatabase.AddObjectToAsset(child, this);
            }

            _childrenObjects.AddRange(children);
            AssetDatabase.SaveAssets();
        }

        protected void RemoveChild(Object child)
        {
            AssetDatabase.RemoveObjectFromAsset(child);
            AssetDatabase.SaveAssets();
            _childrenObjects.Remove(child);
        }

        protected void RemoveChildren(List<Object> children)
        {
            foreach (var child in children)
            {
                AssetDatabase.RemoveObjectFromAsset(child);
                DestroyImmediate(child);
            }

            _childrenObjects.RemoveAll(children.Contains);
            AssetDatabase.SaveAssets();
        }
    }
}
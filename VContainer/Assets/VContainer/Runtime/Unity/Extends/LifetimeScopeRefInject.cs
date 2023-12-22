using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace VContainer.Unity
{
    public class LifetimeScopeRefInject : LifetimeScope
    {
        [SerializeField] private List<MonoBehaviour> autoInjectMonoBehaviours;

        protected override void Awake()
        {
            base.Awake();
            AutoInjectMonoBehaviour();
        }

        void AutoInjectMonoBehaviour()
        {
            if (!autoRun) return;
            foreach (var item in autoInjectMonoBehaviours)
            {
                if (item == null) continue;
                Container.Inject(item);
            }
        }

        public void ReplaceMonoBehavioursInject(List<MonoBehaviour> target)
        {
            autoInjectMonoBehaviours.Clear();
            autoInjectMonoBehaviours = target;
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(LifetimeScopeRefInject))]
    public class LifetimeScopeRefInjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button(("Auto Import Inject MonoBehaviour In Hierarchy To This Scope")))
            {
                var gameObject = (LifetimeScopeRefInject) target;
                MonoBehaviour[] monoBehaviours = ((LifetimeScopeRefInject)target).GetComponentsInChildren<MonoBehaviour>(true);
                List<MonoBehaviour> listInjected = new List<MonoBehaviour>();
                
                foreach (MonoBehaviour monoBehaviour in monoBehaviours)
                {
                    MethodInfo[] methods = monoBehaviour.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    foreach (MethodInfo method in methods)
                    {
                        
                        if (HasInjectAttribute(method))
                        {
                            // if (monoBehaviour.gameObject.GetComponent<LifetimeScope>() == null)
                                listInjected.Add(monoBehaviour);
                            break;
                        }
                    }
                    FieldInfo[] fields = monoBehaviour.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    foreach (var field in fields)
                    {
                        
                        if (HasInjectAttribute(field))
                        {
                            // if (monoBehaviour.gameObject.GetComponent<LifetimeScope>() == null)
                                listInjected.Add(monoBehaviour);
                            break;
                        }
                    }
                }
                
                gameObject.ReplaceMonoBehavioursInject(listInjected);

                EditorUtility.SetDirty(gameObject);
                AssetDatabase.SaveAssets();
            }
        }
        
        public bool HasInjectAttribute(MethodInfo methodInfo)
        {
            bool hasInjectAttribute = false;
            Attribute[] attributes = Attribute.GetCustomAttributes(methodInfo);
            foreach (Attribute attribute in attributes)
            {
                if (attribute.GetType() == typeof(InjectAttribute))
                {
                    hasInjectAttribute = true;
                    break;
                }
            }
            return hasInjectAttribute;
        }
        
        public bool HasInjectAttribute(FieldInfo methodInfo)
        {
            bool hasInjectAttribute = false;
            Attribute[] attributes = Attribute.GetCustomAttributes(methodInfo);
            foreach (Attribute attribute in attributes)
            {
                if (attribute.GetType() == typeof(InjectAttribute))
                {
                    hasInjectAttribute = true;
                    break;
                }
            }
            return hasInjectAttribute;
        }
    }
#endif

}

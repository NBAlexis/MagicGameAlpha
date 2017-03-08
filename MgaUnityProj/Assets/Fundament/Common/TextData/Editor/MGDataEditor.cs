using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEditor;

[AttributeUsage(AttributeTargets.Method)]
public class MGADataEditorAttribute : Attribute
{
    public readonly Type m_pType;

    public MGADataEditorAttribute(Type type)
    {
        m_pType = type;
    }
}

public static class MGAEditorGather
{
    public static Dictionary<Type, MethodInfo> GetAllEditorMethods()
    {
        Dictionary<Type, MethodInfo> ret = new Dictionary<Type, MethodInfo>();
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (type.BaseType != null 
             && type.BaseType.IsGenericType
             && type.BaseType.GetGenericTypeDefinition() == typeof(TMGTextDataEditor<>))
            {
                foreach (MethodInfo methodinfo in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
                {
                    if (methodinfo.IsStatic)
                    {
                        object[] attributes = methodinfo.GetCustomAttributes(typeof(MGADataEditorAttribute), true);
                        if (attributes.Length > 0)
                        {
                            foreach (object attribute in attributes)
                            {
                                MGADataEditorAttribute editorAttribute = attribute as MGADataEditorAttribute;
                                if (null != editorAttribute && !ret.ContainsKey(editorAttribute.m_pType))
                                {
                                    ret.Add(editorAttribute.m_pType, methodinfo);
                                }
                            }
                        }
                    }
                }
            }
        }
        return ret;
    }
}
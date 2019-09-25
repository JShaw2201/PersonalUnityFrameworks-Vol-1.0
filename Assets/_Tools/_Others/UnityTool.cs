using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class UnityTool : MonoSingleton<UnityTool>
{
    private Tool.Coroutine.GlobalCoroutineMgr GlobalCoroutine;
    public override void OnInit()
    {
        GlobalCoroutine = gameObject.AddComponent<Tool.Coroutine.GlobalCoroutineMgr>();
        GlobalCoroutine.OnInit();
    }

    public static void UTStartCoroutine(IEnumerator routine)
    {
        string id = Time.time.ToString() + Random.Range(1, 100).ToString();
        UnityTool.Instance.UTStartCoroutine(id,routine);
    }
  
    public void UTStartCoroutine(string id,IEnumerator routine)
    {
        GlobalCoroutine.UTStartCoroutine(id, routine);
    }
    public void UTStopCoroutine(string id)
    {
        GlobalCoroutine.UTStopCoroutine(id);
    }
    public static void GetChildComp<T>(Transform Trans, ref List<T> list) where T : Component
    {
        for (int i = 0; i < Trans.childCount; i++)
        {
            T[] ts = Trans.GetChild(i).GetComponents<T>();
            if (ts.Length > 0)
            {
                for (int j = 0; j < ts.Length; j++)
                {
                    T t = ts[j];
                    if (!list.Contains(t))
                        list.Add(t);
                }
            }
            GetChildComp<T>(Trans.GetChild(i), ref list);
        }
    }

    /// <summary>
    /// 查找子节点对象
    /// 内部使用“递归算法”
    /// </summary>
    /// <param name="goParent">父对象</param>
    /// <param name="chiildName">查找的子对象名称</param>
    /// <returns></returns>
    public static Transform FindTheChildNode(GameObject goParent, string chiildName)
    {
        Transform searchTrans = null;                   //查找结果

        searchTrans = goParent.transform.Find(chiildName);
        if (searchTrans == null)
        {
            foreach (Transform trans in goParent.transform)
            {
                searchTrans = FindTheChildNode(trans.gameObject, chiildName);
                if (searchTrans != null)
                {
                    return searchTrans;

                }
            }
        }
        return searchTrans;
    }

    /// <summary>
    /// 获取子节点（对象）脚本
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    /// <param name="goParent">父对象</param>
    /// <param name="childName">子对象名称</param>
    /// <returns></returns>
    public static T GetTheChildNodeComponetScript<T>(GameObject goParent, string childName) where T : Component
    {
        Transform searchTranformNode = null;            //查找特定子节点

        searchTranformNode = FindTheChildNode(goParent, childName);
        if (searchTranformNode != null)
        {
            return searchTranformNode.gameObject.GetComponent<T>();
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 给子节点添加脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="goParent">父对象</param>
    /// <param name="childName">子对象名称</param>
    /// <returns></returns>
    public static T AddChildNodeCompnent<T>(GameObject goParent, string childName) where T : Component
    {
        Transform searchTranform = null;                //查找特定节点结果

        //查找特定子节点
        searchTranform = FindTheChildNode(goParent, childName);
        //如果查找成功，则考虑如果已经有相同的脚本了，则先删除，否则直接添加。
        if (searchTranform != null)
        {
            //如果已经有相同的脚本了，则先删除
            T[] componentScriptsArray = searchTranform.GetComponents<T>();
            for (int i = 0; i < componentScriptsArray.Length; i++)
            {
                if (componentScriptsArray[i] != null)
                {
                    Destroy(componentScriptsArray[i]);
                }
            }
            return searchTranform.gameObject.AddComponent<T>();
        }
        else
        {
            return null;
        }
        //如果查找不成功，返回Null.
    }

    /// <summary>
    /// 给子节点添加父对象
    /// </summary>
    /// <param name="parents">父对象的方位</param>
    /// <param name="child">子对象的方法</param>
    public static void AddChildNodeToParentNode(Transform parents, Transform child)
    {
        child.SetParent(parents, false);
        child.localPosition = Vector3.zero;
        child.localScale = Vector3.one;
        child.localEulerAngles = Vector3.zero;
    }

    public static T GetParentComponentScript<T>(Transform go) where T : Component
    {
        T t = null;
        if (go.parent != null)
        {
            t = go.parent.GetComponent<T>();
            if (go.root != go.parent && t == null)
            {
                t = GetParentComponentScript<T>(go.parent);
            }
        }
        return t;
    }
}


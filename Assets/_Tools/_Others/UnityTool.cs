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
    /// �����ӽڵ����
    /// �ڲ�ʹ�á��ݹ��㷨��
    /// </summary>
    /// <param name="goParent">������</param>
    /// <param name="chiildName">���ҵ��Ӷ�������</param>
    /// <returns></returns>
    public static Transform FindTheChildNode(GameObject goParent, string chiildName)
    {
        Transform searchTrans = null;                   //���ҽ��

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
    /// ��ȡ�ӽڵ㣨���󣩽ű�
    /// </summary>
    /// <typeparam name="T">����</typeparam>
    /// <param name="goParent">������</param>
    /// <param name="childName">�Ӷ�������</param>
    /// <returns></returns>
    public static T GetTheChildNodeComponetScript<T>(GameObject goParent, string childName) where T : Component
    {
        Transform searchTranformNode = null;            //�����ض��ӽڵ�

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
    /// ���ӽڵ���ӽű�
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="goParent">������</param>
    /// <param name="childName">�Ӷ�������</param>
    /// <returns></returns>
    public static T AddChildNodeCompnent<T>(GameObject goParent, string childName) where T : Component
    {
        Transform searchTranform = null;                //�����ض��ڵ���

        //�����ض��ӽڵ�
        searchTranform = FindTheChildNode(goParent, childName);
        //������ҳɹ�����������Ѿ�����ͬ�Ľű��ˣ�����ɾ��������ֱ����ӡ�
        if (searchTranform != null)
        {
            //����Ѿ�����ͬ�Ľű��ˣ�����ɾ��
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
        //������Ҳ��ɹ�������Null.
    }

    /// <summary>
    /// ���ӽڵ���Ӹ�����
    /// </summary>
    /// <param name="parents">������ķ�λ</param>
    /// <param name="child">�Ӷ���ķ���</param>
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


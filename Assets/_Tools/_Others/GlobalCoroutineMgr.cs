using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tool.Coroutine
{
    public class GlobalCoroutineMgr : MonoBehaviour,IBehaviorHandle
    {
        private List<GlobalCoroutineBehavior> CoroutinePool;
        private Dictionary<string, GlobalCoroutineBehavior> curCoroutineDict;
        public void OnInit()
        {
            CoroutinePool = new List<GlobalCoroutineBehavior>();
            curCoroutineDict = new Dictionary<string, GlobalCoroutineBehavior>();
        }

        public void UTStartCoroutine(string id,IEnumerator routine)
        {
            GlobalCoroutineBehavior gcb = null;
            if (curCoroutineDict.ContainsKey(id))
            {
                gcb = curCoroutineDict[id];
            }
            else
            {
                if (CoroutinePool.Count == 0)
                {
                    GameObject go = new GameObject("GlobalCoroutineBehavior");
                    go.transform.SetParent(transform);
                    gcb = go.AddComponent<GlobalCoroutineBehavior>();
                    gcb.onComplete = OnCompleteCoroutineBehavior;
                }
                else
                {
                    gcb = CoroutinePool[0];
                    CoroutinePool.Remove(gcb);
                }
                curCoroutineDict[id] = gcb;
            }
            gcb.UTStartCoroutine(id, routine);
        }

        public IEnumerator GBStartCoroutine(string id, IEnumerator routine)
        {
            GlobalCoroutineBehavior gcb = null;
            if (curCoroutineDict.ContainsKey(id))
            {
                gcb = curCoroutineDict[id];
            }
            else
            {
                if (CoroutinePool.Count == 0)
                {
                    GameObject go = new GameObject("GlobalCoroutineBehavior");
                    go.transform.SetParent(transform);
                    gcb = go.AddComponent<GlobalCoroutineBehavior>();
                }
                else
                {
                    gcb = CoroutinePool[0];
                    CoroutinePool.Remove(gcb);
                }
                curCoroutineDict[id] = gcb;
            }
            yield return gcb.GBStartCoroutine(id, routine);
        }
        public void UTStopCoroutine(string id)
        {
            GlobalCoroutineBehavior gcb = null;
            curCoroutineDict.TryGetValue(id,out gcb);
            if (gcb != null)
                gcb.UTStopCoroutine();
        }
        public void OnDispose()
        {
            List<GlobalCoroutineBehavior> gcbList = new List<GlobalCoroutineBehavior>(curCoroutineDict.Values);
            foreach (var gcb in gcbList)
            {
                gcb.UTStopCoroutine();
            }
            curCoroutineDict.Clear();
        }     
        public void OnUpdate()
        {
            
        }

        private void OnCompleteCoroutineBehavior(string id)
        {
            GlobalCoroutineBehavior gcb = null;
            curCoroutineDict.TryGetValue(id, out gcb);
            if (gcb != null)
            {
                if(!CoroutinePool.Contains(gcb))
                    CoroutinePool.Add(gcb);
            }
            if (curCoroutineDict.ContainsKey(id))
            {
                curCoroutineDict.Remove(id);
            }

        }
    }
}

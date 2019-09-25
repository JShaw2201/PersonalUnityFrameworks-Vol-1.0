using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tool.Coroutine
{
    public class GlobalCoroutineBehavior : MonoBehaviour
    {
        public string status { get { return m_Status; } }
        [SerializeField]
        private string id;
        [SerializeField]
        private string m_Status = "END";
        public System.Action<string> onComplete;
        public void UTStartCoroutine(string id,IEnumerator routine)
        {
            if (m_Status == "WORKING")
            {
                StopAllCoroutines();
            }
            StartCoroutine(GBStartCoroutine(id,routine));
        }

        public void UTStopCoroutine()
        {
            StopAllCoroutines();
            m_Status = "END";
            if (onComplete != null)
                onComplete(id);
            id = null;
        }
        public IEnumerator GBStartCoroutine(string id,IEnumerator routine)
        {
            this.id = id;
            m_Status = "WORKING";
            yield return routine;
            m_Status = "END";
            if (onComplete != null)
                onComplete(id);
            id = null;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JS.Tool
{
    public class QueueAction : IDisposable
    {
        private class ActionObj
        {
            public byte stateId { get { return _stateId; } }
            private byte _stateId;

            public byte Working { get { return _Working; } }
            private byte _Working;

            private object[] data;
            private Delegate callback;

            public ActionObj(byte stateId, object[] data, Delegate callback)
            {
                this.data = data;
                this._stateId = stateId;
                this.callback = callback;
                this._Working = 1;
            }

            public void StartWorking()
            {
                _Working = 2;
                callback.DynamicInvoke(data);
            }

            public void EndWorking()
            {
                _Working = 3;
            }
        }

        public byte CurState
        {
            get
            {
                if (headNode == null || _IsWorking)
                    return _CurState;
                return headNode.stateId;
            }
        }
        public bool IsWorking
        {
            get
            {
                if (headNode == null || _IsWorking)
                    return _IsWorking;
                return headNode.Working == 2;
            }
        }
        private byte _CurState = 0;
        private bool _IsWorking = false;
        private ActionObj headNode;
        private List<ActionObj> nodes = new List<ActionObj>();

        public void Append(byte stateId, Delegate callback,params object[] data)
        {
            if (_IsWorking)
            {
                nodes.Add(new ActionObj(stateId, data, callback));
            }
            else
            {
                _CurState = stateId;
                _IsWorking = true;
                callback.DynamicInvoke(data);
            }
            OnUpdate();
        }

        public void OnUpdate()
        {
            if (!_IsWorking)
            {
                if (nodes.Count == 0)
                    return;
                if (headNode == null)
                    headNode = nodes[0];
                switch (headNode.Working)
                {
                    case 1:
                        nodes.RemoveAt(0);
                        headNode.StartWorking();
                        break;
                    case 3:                      
                        headNode = nodes.Count > 0 ? nodes[0] : null;                  
                        OnUpdate();
                        break;
                }

            }
        }

        public void OnEndWorking(byte state)
        {
            if (CurState == state)
            {
                if (headNode != null && !_IsWorking)
                {
                    headNode.EndWorking();
                }
                else
                {
                    _IsWorking = false;
                    _CurState = 0;
                }
            }
            OnUpdate();
        }

        public void Dispose()
        {
            headNode = null;
            nodes.Clear();
        }

    }
}

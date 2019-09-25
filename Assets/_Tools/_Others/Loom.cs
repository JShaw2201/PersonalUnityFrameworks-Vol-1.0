using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;

namespace JX.Tool
{
    public class Loom : MonoBehaviour
    {
        private static volatile Loom instance;
        private static object syncRoot = new object();
        private static bool _applicationIsQuitting = false;
        private static GameObject singletonObj = null;
        public static Loom Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    return null;
                }
                if (instance == null)
                {

                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            Loom[] instance1 = FindObjectsOfType<Loom>();
                            if (instance1 != null)
                            {
                                for (var i = 0; i < instance1.Length; i++)
                                {
                                    Destroy(instance1[i].gameObject);
                                }
                            }
                        }
                    }

                    GameObject go = new GameObject(typeof(Loom).FullName);
                    singletonObj = go;
                    instance = go.AddComponent<Loom>();
                    DontDestroyOnLoad(go);
                    _applicationIsQuitting = false;
                }
                return instance;
            }

        }

        private void Awake()
        {

            Loom t = gameObject.GetComponent<Loom>();
            if (singletonObj == null)
            {
                singletonObj = gameObject;
                DontDestroyOnLoad(gameObject);
                singletonObj.name = typeof(Loom).FullName;
                instance = t;
                _applicationIsQuitting = false;
            }
            else if (singletonObj != gameObject)
            {
                MonoBehaviour[] monos = gameObject.GetComponents<MonoBehaviour>();
                if (monos.Length > 1)
                {
                    Destroy(t);
                }
                else
                {
                    Destroy(gameObject);
                }
            }

        }

        public static int maxMainThreads
        {
            get { return _maxMainThreads; }
            set { _maxMainThreads = value; if (_mainThead != null) _mainThead.maxThread = _maxMainThreads; }
        }
        public static int maxAsyncThreads
        {
            get { return _maxAsyncThreads; }
            set { _maxAsyncThreads = value; if (_asyncThead != null) _asyncThead.maxThread = _maxAsyncThreads; }
        }

        private static int _maxMainThreads = 8;
        private static int _maxAsyncThreads = 10;

        public static int asyncThreadTimeOut = 100;
        public static int mainThreadTimeOut = 100;

        private static LoomBehavior<ThreadObject, Action> _asyncThead = new LoomBehavior<ThreadObject, Action>();
        private static LoomBehavior<ActionObject, Action<Action>> _mainThead = new LoomBehavior<ActionObject, Action<Action>>();

        private void Update()
        {
            if (_mainThead.hasAction)
                _mainThead.OnUpdate();
            if (_asyncThead.hasAction)
                _asyncThead.OnUpdate();

        }

        private void OnDestroy()
        {
            _asyncThead.OnDispose();
            _mainThead.OnDispose();
        }

        /// <summary>
        /// 添加到默认系列
        /// </summary>
        /// <param name="callback"></param>
        public static void QueueOnAsyncThread(Action callback)
        {
            _asyncThead.QueueOnThread(callback);
        }

        /// <summary>
        /// 添加到自定义系列
        /// </summary>
        /// <param name="id"></param>
        /// <param name="callback"></param>
        public static void QueueOnAsyncThread(int id, Action callback)
        {
            _asyncThead.QueueOnThread(id, callback);
        }

        /// <summary>
        /// 停止运行这个系列
        /// </summary>
        /// <param name="id"></param>
        public static void StopOnAsyncThread(int id)
        {
            _asyncThead.StopOnThread(id);
        }

        /// <summary>
        /// 停止运行单个
        /// </summary>
        /// <param name="id"></param>
        /// <param name="callback"></param>
        public static void StopOnAsyncThread(int id, Action callback)
        {
            _asyncThead.StopOnThread(id, callback);
        }

        /// <summary>
        /// 添加到默认系列
        /// </summary>
        /// <param name="callback"></param>
        public static void QueueOnMainThread(Action<Action> callback)
        {
            _mainThead.QueueOnThread(callback);
        }

        /// <summary>
        /// 添加到自定义系列
        /// </summary>
        /// <param name="id"></param>
        /// <param name="callback"></param>
        public static void QueueOnMainThread(int id, Action<Action> callback)
        {
            _mainThead.QueueOnThread(id, callback);
        }

        /// <summary>
        /// 停止运行这个系列
        /// </summary>
        /// <param name="id"></param>
        public static void StopOnMainThread(int id)
        {
            _mainThead.StopOnThread(id);
        }

        /// <summary>
        /// 停止运行单个
        /// </summary>
        /// <param name="id"></param>
        /// <param name="callback"></param>
        public static void StopOnMainThread(int id, Action<Action> callback)
        {
            _mainThead.StopOnThread(id, callback);
        }

        class LoomBehavior<T1, T2> where T1 : LoomThreadObject<T2>, new()
        {
            public int maxThread = 8;
            public bool hasAction = false;
            private object lockObj = new object();
            private Dictionary<int, LoomLinkList<T1, T2>> _current = new Dictionary<int, LoomLinkList<T1, T2>>();
            private Dictionary<int, LoomLinkList<T1, T2>> _Actions = new Dictionary<int, LoomLinkList<T1, T2>>();

            public LoomBehavior()
            {
                //string idStr = System.DateTime.Now.ToString() + System.Guid.NewGuid().ToString();
                //defaultID = idStr.GetHashCode();
            }

            public void OnUpdate()
            {
                if (_current.Count == 0 && _Actions.Count == 0)
                {
                    hasAction = false;
                    return;
                }
                List<int> _currentKeys = new List<int>(_current.Keys);
                for (int i = 0; i < _currentKeys.Count; i++)
                {
                    LoomLinkList<T1, T2> linkList = _current[_currentKeys[i]];
                    if (linkList.status == "END")
                    {
                        lock (_current)
                        {
                            _current.Remove(_currentKeys[i]);
                        }
                        break;
                    }
                    linkList.OnUpdate();
                }
                if (_currentKeys.Count < maxMainThreads)
                {
                    if (_Actions.Count > 0)
                    {
                        List<int> _ActionKeys = new List<int>(_Actions.Keys);
                        lock (_current)
                        {
                            _current[_ActionKeys[0]] = _Actions[_ActionKeys[0]];
                            _Actions.Remove(_ActionKeys[0]);
                        }
                    }
                }
            }

            /// <summary>
            /// 添加到默认系列
            /// </summary>
            /// <param name="callback"></param>
            public void QueueOnThread(T2 callback)
            {
                string idStr = System.DateTime.Now.ToString() + System.Guid.NewGuid().ToString();
                int defaultID = idStr.GetHashCode();
                QueueOnThread(defaultID, callback);
            }

            /// <summary>
            /// 添加到自定义系列
            /// </summary>
            /// <param name="id"></param>
            /// <param name="callback"></param>
            public void QueueOnThread(int id, T2 callback)
            {
                lock (lockObj)
                {
                    LoomLinkList<T1, T2> actions = null;
                    if (!_current.ContainsKey(id))
                    {
                        if (_current.Count >= maxThread)
                        {
                            if (!_Actions.ContainsKey(id))
                                _Actions[id] = new LoomLinkList<T1, T2>();
                            actions = _Actions[id];
                            T1 t1 = new T1();
                            t1.callback = callback;
                            actions.AppendNode(t1);
                        }
                        else
                        {
                            _current[id] = new LoomLinkList<T1, T2>();
                            actions = _current[id];
                            T1 t1 = new T1();
                            t1.callback = callback;
                            actions.AppendNode(t1);
                            actions.OnStart();
                        }
                    }
                    else
                    {
                        actions = _current[id];
                        T1 t1 = new T1();
                        t1.callback = callback;
                        actions.AppendNode(t1);
                        actions.OnStart();
                    }
                    hasAction = true;
                }
            }

            /// <summary>
            /// 停止运行这个系列
            /// </summary>
            /// <param name="id"></param>
            public void StopOnThread(int id)
            {
                _current.Remove(id);
                _Actions.Remove(id);
            }

            /// <summary>
            /// 停止运行单个
            /// </summary>
            /// <param name="id"></param>
            /// <param name="callback"></param>
            public void StopOnThread(int id, T2 callback)
            {
                LoomLinkList<T1, T2> linkList = null;
                if (_current.ContainsKey(id))
                {
                    linkList = _current[id];
                }
                else if (_Actions.ContainsKey(id))
                {
                    linkList = _Actions[id];
                }
                if (linkList == null)
                    return;
                LoomThreadObject<T2> currentNode = linkList.Head as LoomThreadObject<T2>;
                while (currentNode != null)
                {
                    if ((object)currentNode.callback == (object)callback)
                    {
                        currentNode.OnStop();
                        break;
                    }
                    currentNode = currentNode.Next as LoomThreadObject<T2>;
                }
            }

            public virtual void OnDispose()
            {
                lock (lockObj)
                {
                    List<LoomLinkList<T1, T2>> _curs = new List<LoomLinkList<T1, T2>>(_current.Values);
                    List<LoomLinkList<T1, T2>> _acts = new List<LoomLinkList<T1, T2>>(_Actions.Values);
                    for (int i = 0; i < _curs.Count; i++)
                    {
                        if (_curs[i] != null)
                            _curs[i].OnDispose();
                    }
                    for (int i = 0; i < _acts.Count; i++)
                    {
                        if (_acts[i] != null)
                            _acts[i].OnDispose();
                    }
                    _current.Clear();
                    _Actions.Clear();
                }
            }
        }

        class ThreadObject : LoomThreadObject<Action>
        {
            private Thread thread;

            public ThreadObject()
            {
                thread = new Thread((new ParameterizedThreadStart(RunAction)));
                thread.IsBackground = true;
                TimeOut = asyncThreadTimeOut;
            }
            public ThreadObject(Action callback) : base(callback)
            {
                thread = new Thread((new ParameterizedThreadStart(RunAction)));
                thread.IsBackground = true;
                TimeOut = asyncThreadTimeOut;
            }

            protected override void OnInvoke()
            {
                thread.Start(_callback);
            }

            private void RunAction(object action)
            {
                try { ((Action)action)(); }
                catch { }
                _status = "END";
                //if (Next != null) ((ThreadObject)Next).Invoke();
            }

            public override void OnStop()
            {
                base.OnStop();
                if (thread.ThreadState == ThreadState.Running)
                    thread.Abort();
            }
        }

        class ActionObject : LoomThreadObject<Action<Action>>
        {
            public ActionObject()
            {
                TimeOut = mainThreadTimeOut;
            }
            public ActionObject(Action<Action> callback) : base(callback)
            {
                TimeOut = mainThreadTimeOut;
            }

            protected override void OnInvoke()
            {
                if (_callback == null)
                {
                    OnStop();
                    return;
                }
                _callback(() => {

                    OnStop();
                    //if (Next != null) ((ActionObject)Next).Invoke();
                });
            }
        }

        class LoomThreadObject<T> : Node<T>
        {
            protected int TimeOut = 10;

            private float _Timer;

            public string status { get { return _status; } }
            protected string _status = "NOTWORKING";

            public T callback { get { return _callback; } set { _callback = value; } }
            protected T _callback;

            public LoomThreadObject() : base()
            {

            }

            public LoomThreadObject(T callback) : base()
            {
                this._callback = callback;
            }

            public void Invoke()
            {
                if (_status == "NOTWORKING")
                {
                    _Timer = 0;
                    _status = "WORKING";
                    OnInvoke();
                }
            }

            public void OnUpdate()
            {
                if (_status == "WORKING")
                {
                    _Timer += Time.deltaTime;
                    if (_Timer >= TimeOut)
                        OnStop();
                }
            }
            public virtual void OnStop()
            {
                _status = "END";
            }

            protected virtual void OnInvoke() { }
        }

        /// <summary>
        /// 链表
        /// </summary>
        class LoomLinkList<T1, T2> : LinkList<T2> where T1 : LoomThreadObject<T2>
        {
            public string status { get { return _status; } }
            private string _status = "NOTWORKING";
            private LoomThreadObject<T2> currentNode;

            public void OnStart()
            {
                switch (_status)
                {
                    case "NOTWORKING":
                        _status = "WORKING";
                        currentNode = Head as LoomThreadObject<T2>;
                        break;
                    case "END":
                        if (currentNode == null || currentNode.Next == null)
                        {
                            return;
                        }
                        _status = "WORKING";
                        currentNode = currentNode.Next as LoomThreadObject<T2>;
                        break;
                }
            }

            public void OnUpdate()
            {
                switch (_status)
                {
                    case "NOTWORKING":
                        _status = "WORKING";
                        currentNode = Head as LoomThreadObject<T2>;
                        break;
                    case "WORKING":
                        if (currentNode == null)
                        {
                            _status = "END";
                            return;
                        }
                        switch (currentNode.status)
                        {
                            case "NOTWORKING":
                                currentNode.Invoke();
                                break;
                            case "WORKING":
                                currentNode.OnUpdate();
                                break;
                            case "END":
                                if (currentNode.Next != null)
                                {
                                    currentNode = currentNode.Next as LoomThreadObject<T2>;
                                }
                                else
                                {
                                    _status = "END";
                                }
                                break;
                        }
                        break;
                }
            }

            public void OnDispose()
            {
                LoomThreadObject<T2> loomObj = Head as LoomThreadObject<T2>;
                loomObj.OnStop();
                while (loomObj.Next != null)
                {
                    loomObj = loomObj.Next as LoomThreadObject<T2>;
                    loomObj.OnStop();
                }
                currentNode = null;
            }
        }
        class LinkList<T>
        {
            public Node<T> Head { set; get; } //单链表头

            //构造
            public LinkList()
            {
                Head = null;
            }

            /// <summary>
            /// 增加新元素到单链表末尾
            /// </summary>
            public virtual void Append(T item)
            {
                Node<T> foot = new Node<T>(item);
                Node<T> A = null;
                if (Head == null)
                {
                    Head = foot;
                    return;
                }
                A = Head;
                while (A.Next != null)
                {

                    A = A.Next;
                }
                A.Next = foot;
                A.Next.Parent = A;
            }

            public virtual void AppendNode(Node<T> item)
            {
                Node<T> foot = item;
                Node<T> A = null;
                if (Head == null)
                {
                    Head = foot;
                    return;
                }
                A = Head;
                while (A.Next != null)
                {

                    A = A.Next;
                }
                A.Next = foot;
                A.Next.Parent = A;
            }
        }
        class Node<T>
        {
            public T Data { set; get; }          //数据域,当前结点数据
            public Node<T> Next { set; get; }    //位置域,下一个结点地址
            public Node<T> Parent { set; get; }

            public Node(T item)
            {
                this.Data = item;
                this.Next = null;
                this.Parent = null;
            }

            public Node()
            {
                this.Data = default(T);
                this.Next = null;
                this.Parent = null;
            }
        }
    }
}

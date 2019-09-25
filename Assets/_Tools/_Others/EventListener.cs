using System.Collections;
using UnityEngine;
/**事件管理器*/
public class EventListener{
    private Hashtable map;
    public delegate void Callback(object target);
    private static EventListener _instance;
    private static EventListener instance
    {
        get
        {
            if(_instance==null)
            {
                _instance = new EventListener();
            }
            return _instance;
        }
    }
            
    public EventListener()
    { 
        map = new Hashtable();
    }

    public static void registerEvent(string type,Callback callback)
    {
        lock (instance)
        {
            if (!instance.map.Contains(type))
            {
                instance.map.Add(type, ArrayList.Synchronized(new ArrayList()));
            }
            ArrayList array = (ArrayList)instance.map[type];
            if(!array.Contains(callback))
            {
               array.Add(callback);
            }
        }
    }

    public static int deleteEvent(string type, Callback callback)
    {
        lock (instance)
        {
            ArrayList array = null;
            if (instance.map.Contains(type))
            {
                array = (ArrayList)instance.map[type];
                array.Remove(callback);
                if (array.Count == 0)
                {
                    instance.map.Remove(type);
                }
            }
            return array == null ? 0 : array.Count;
        }
    }

    public static void dispatchEvent(string type,object data)
    {
        lock (instance)
        {
            if (instance.map.Contains(type))
            {
                ArrayList array = (ArrayList)instance.map[type];
                for (int i = 0; i < array.Count; i++)
                {
                    Callback callback = (Callback)array[i];
                    callback(data);

                }
            }
        }
    }

}

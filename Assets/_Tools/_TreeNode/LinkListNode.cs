using System.Collections;
using System.Collections.Generic;

namespace JS.Container.LinkList
{
    public class Node<T>
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

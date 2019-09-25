using System.Collections;
using System.Collections.Generic;

namespace JS.Container.LinkList
{
    public class LinkList<T>
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
}

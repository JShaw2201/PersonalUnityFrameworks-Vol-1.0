using System;
using System.Collections;
using System.Collections.Generic;

namespace JS.Container.Tree
{
    public class TreeNodeSingleLeafObject : BaseTreeNodeSingleLeaf<object, TreeNodeSingleLeafObject>
    {
        private object _data;
        public TreeNodeSingleLeafObject() : base() { }
        public TreeNodeSingleLeafObject(object leave): base(leave)   {           }
        public TreeNodeSingleLeafObject(TreeNodeSingleLeafObject child) : base(child) { }
        public TreeNodeSingleLeafObject(TreeNodeSingleLeafObject[] childs) : base(childs) { }
        public TreeNodeSingleLeafObject(object leave, TreeNodeSingleLeafObject[] childs): base(leave,childs) {     }

        protected override object data
        {
            get
            {
                return _data;
            }

            set
            {
                _data = value;
            }
        }
    }
}

using JS.Container.Tree;
using System;

public class TreeNodeTest  {

   
	public void Start () {
        TreeNodeMultiLeafObject head = new TreeNodeMultiLeafObject(new object[] { "节点-" + "Head" });
        for (int i = 0; i < 5; i++)
        {
            TreeNodeMultiLeafObject nodei = new TreeNodeMultiLeafObject(new object[] {"节点-"+i.ToString() });          
            for (int j = 0; j < 5; j++)
            {
                TreeNodeMultiLeafObject nodej = new TreeNodeMultiLeafObject(new object[] { "节点-" + i.ToString() +"-"+j.ToString()});
                nodei.InsertBranch(nodej);
            }
            head.InsertBranch(nodei);
        }
        int HeadDepth = head.GetBranchDepth();
        Console.WriteLine("Depth--"+ HeadDepth);
        for (int i = 0; i <= HeadDepth; i++)
        {
            int wid = head.GetBranchWidth(i);
            Console.WriteLine("Depth--" + i+ "-Width-"+wid);
            for (int j = 0; j < wid; j++)
            {
                TreeNodeMultiLeafObject node = null;
                head.GetBranch(i,j,ref node);
                if (node != null)
                {
                    object data = node.GetLeave(0);
                    if (data != null)
                    {
                        Console.WriteLine("---"+(string)data+ "-id:" + node.TreeNodeID.ToString());
                    }
                }
            }
        }
    }
	

}

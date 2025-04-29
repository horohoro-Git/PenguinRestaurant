using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NodePool
{

    public static Queue<Node> nodePool = new Queue<Node>(2000); 
    // Start is called before the first frame update
    
    public static Node GetNode(int Y, int X)
    {
        if(nodePool.Count > 0)
        {
            Node node = nodePool.Dequeue();
            node.r = Y;
            node.c = X;
            return node;
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                Node newNode = new Node(0, 0, false);
                nodePool.Enqueue(newNode);
            }
            Node node = nodePool.Dequeue();
            node.r = Y;
            node.c = X;
            return node;
        }
    }


    public static void ReturnNode(Node usedNode)
    {
        usedNode.Init(false);
        if(nodePool.Count < 999)
        {
            nodePool.Enqueue(usedNode);
        }
        else
        {
            usedNode = null;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IComparable<Node>
{
    public int r, c;
    public bool blocked;

    int? cachedF;

    public int F
    {
        get
        {
            if (!cachedF.HasValue)
            {
                cachedF = G + H;
            }
            return cachedF.Value;
        }
    }
    public int G { get; set; }
    public int H { get; set; }

    public void ResetF()
    {
        cachedF = null;
    }
    public Node parentNode = null;

    public static Node node = new Node(1, 1, true);
    public Node(int r, int c, bool blocked = false)
    {
        this.r = r;
        this.c = c;
        this.blocked = blocked;
        this.G = int.MaxValue;
        parentNode = null;
    }


    public void Init(bool isBlocked)
    {
        blocked = isBlocked;
        r = 0;
        c = 0;
        G = 0;
        H = 0;
        cachedF = null;
        parentNode = null;
    }


    public int CompareTo(Node other)
    {
        return F.CompareTo(other.F);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || this.GetType() != obj.GetType())
            return false;

        Node other = (Node)obj;
        return this.r == other.r && this.c == other.c;
    }
    public override int GetHashCode()
    {
        return (r, c).GetHashCode();
    }
}
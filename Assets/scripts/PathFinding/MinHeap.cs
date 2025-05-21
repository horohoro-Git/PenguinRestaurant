using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Loading;
using UnityEngine;

public class MinHeap<T> where T : IComparable<T>
{
    public List<T> contents = new List<T>();
    private HashSet<T> set = new HashSet<T>(); 
    public int Count { get { return contents.Count; } }

    public MinHeap(int capacity = 0)
    {
        contents = new List<T>(capacity);
        set = new HashSet<T>(capacity);
    }
    public void Add(T item)
    {
        if (!set.Contains(item))
        {
            contents.Add(item);
            set.Add(item);
            HeapUp(contents.Count - 1);
        }
    }

    public void Clear()
    {
        contents.Clear();
        set.Clear();
    }

    public T PopMin()
    {
        T returnItem = contents[0];
        set.Remove(returnItem);
        contents[0] = contents[contents.Count - 1];
        contents.RemoveAt(contents.Count - 1);
        if (contents.Count > 1) HeapDown(0);

        return returnItem;
    }

    public T Peek()
    {
        return contents[0];
    }

    public bool Contains(T item)
    {
        return set.Contains(item);
     /*   for (int i = 0; i < contents.Count; i++)
        {
            if (contents[i].Equals(item)) return true;
        }

        return false;*/
    }

    void HeapUp(int index)
    {
        while (index > 0)
        {
            int parentIndex = index / 2;
            if (contents[index].CompareTo(contents[parentIndex]) >= 0) break;

            Swap(index, parentIndex);
            index = parentIndex;
        }
    }

    void HeapDown(int index)
    {
        while(index < contents.Count - 1)
        {
            int smallestIndex = index;
            int leftChildIndex = index * 2 + 1;
            int rightChildIndex = index * 2 + 2;

            if (leftChildIndex < contents.Count &&  contents[leftChildIndex].CompareTo(contents[smallestIndex]) < 0)
            {
                smallestIndex = leftChildIndex;
            }
            if (rightChildIndex < contents.Count && contents[rightChildIndex].CompareTo(contents[smallestIndex]) < 0)
            {
                smallestIndex = rightChildIndex;
            }

            if (index == smallestIndex) break;
            Swap(index, smallestIndex);
            index = smallestIndex;

        }
    }

    void Swap(int item1, int item2)
    {
        T temp = contents[item1]; 
        contents[item1] = contents[item2]; 
        contents[item2] = temp;
    }
}

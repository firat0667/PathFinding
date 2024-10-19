using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int CurrentItemCount;

    public Heap(int maxHeapSize)
    {
        items=new T[maxHeapSize];
    }
    public void Add(T item) 
    {
        item.HeapIndex = CurrentItemCount;
        items[CurrentItemCount] = item;

        SortUp(item);
        CurrentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        CurrentItemCount--;
        items[0] = items[CurrentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }
    public void UpdateItem(T item)
    {
        SortUp(item);
    }
    public int Count
    {
        get
        {
            return CurrentItemCount;
        }
    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex],item);
    }


    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft=item.HeapIndex*2+1;
            int childIndexRight=item.HeapIndex*2+2;
            int swapIndex = 0;
            if (childIndexLeft < CurrentItemCount)
            {
                swapIndex = childIndexLeft;
                if (childIndexRight < CurrentItemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                    return;

            }
            else return;

        }
    }

     void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;
        while (true)
        {
            T parent = items[parentIndex];
            if (item.CompareTo(parent) > 0)
            {
                Swap(item, parent);
            }
            else
                break;
            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }
    void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;

        int itemAIndex =itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int currentItemCount;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    // Adds node to last element of array, then calls SortUp() to put node in correct location in the heap
    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public void Clear()
    {
        currentItemCount = 0;
    }

    // Removes the first element of array, copy last element to first element, then call SortDown() to put node in correct location in the heap
    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
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
        get { return currentItemCount; }
    }

    public bool Contains(T item)
    {
        if (item.HeapIndex < currentItemCount)
        {
            return Equals(items[item.HeapIndex], item);
        }
        else
        {
            return false;
        }
    }

    void SortDown(T item)
    {
        while (true)
        {
            // (n * 2) + 1 & (n * 2) + 2 allows you to calculate the child nodes of an array element
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            // Checks if current location of item has any children, if not, item is in correct location
            if (childIndexLeft < currentItemCount)
            {
                // Set swapIndex equal to child with higher priority. If item has a lower priority, it is swapped with the child with the highest priority.
                swapIndex = childIndexLeft;

                if (childIndexRight < currentItemCount)
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
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    void SortUp(T item)
    {
        // (n - 1) / 2 allows you to calculate the parent node of an array element
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            // If item has higher priority than parent, swap them
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }
            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}

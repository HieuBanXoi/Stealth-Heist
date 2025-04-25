using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<TElement, TPriority>
{
    private List<(TElement, TPriority)> _elements = new List<(TElement, TPriority)>();
    private readonly IComparer<TPriority> _comparer;

    public PriorityQueue() : this(Comparer<TPriority>.Default) { }
    public PriorityQueue(IComparer<TPriority> comparer) => _comparer = comparer;

    public void Enqueue(TElement item, TPriority priority)
    {
        _elements.Add((item, priority));
        int child = _elements.Count - 1;
        while (child > 0)
        {
            int parent = (child - 1) / 2;
            if (_comparer.Compare(_elements[child].Item2, _elements[parent].Item2) >= 0) break;
            (_elements[child], _elements[parent]) = (_elements[parent], _elements[child]);
            child = parent;
        }
    }

    public TElement Dequeue()
    {
        var result = _elements[0].Item1;
        int last = _elements.Count - 1;
        _elements[0] = _elements[last];
        _elements.RemoveAt(last);

        int parent = 0;
        while (true)
        {
            int left = 2 * parent + 1;
            if (left >= _elements.Count) break;
            int right = left + 1;
            int min = (right < _elements.Count && _comparer.Compare(_elements[right].Item2, _elements[left].Item2) < 0) ? right : left;
            if (_comparer.Compare(_elements[parent].Item2, _elements[min].Item2) <= 0) break;
            (_elements[parent], _elements[min]) = (_elements[min], _elements[parent]);
            parent = min;
        }
        return result;
    }

    public int Count => _elements.Count;
}
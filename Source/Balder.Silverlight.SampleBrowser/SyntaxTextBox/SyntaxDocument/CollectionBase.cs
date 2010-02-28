#region Fireball License
//    Copyright (C) 2005  Sebastian Faltoni sebastian{at}dotnetfireball{dot}net
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

#endregion

using System;
using System.Collections;
using System.Collections.Generic;

namespace Fireball.Syntax
{

  #region EventArgs and Delegate

  public class CollectionEventArgs : EventArgs
  {
    public CollectionEventArgs()
    {
    }

    public CollectionEventArgs(object item, int index)
    {
      this.Index = index;
      this.Item = item;
    }

    public object Item = null;
    public int Index = 0;
  }

  public delegate void CollectionEventHandler(object sender, CollectionEventArgs e);

  #endregion

  public abstract class BaseCollection : CollectionBase, IList
  {
    public BaseCollection(){
    }

    #region Events

    public event CollectionEventHandler ItemAdded = null;

    protected virtual void OnItemAdded(int index, object item)
    {
      if (this.ItemAdded != null)
      {
        CollectionEventArgs e = new CollectionEventArgs(item, index);

        this.ItemAdded(this, e);
      }
    }

    public event CollectionEventHandler ItemRemoved = null;

    protected virtual void OnItemRemoved(int index, object item)
    {
      if (this.ItemRemoved != null)
      {
        CollectionEventArgs e = new CollectionEventArgs(item, index);

        this.ItemRemoved(this, e);
      }
    }

    public event EventHandler ItemsCleared = null;

    protected virtual void OnItemsCleared()
    {
      if (this.ItemsCleared != null)
        this.ItemsCleared(this, EventArgs.Empty);
    }

    #endregion

    #region Overrides

    protected override void OnClearComplete()
    {
      base.OnClearComplete();
      this.OnItemsCleared();
    }

    protected override void OnRemoveComplete(int index, object value)
    {
      base.OnRemoveComplete(index, value);
      this.OnItemRemoved(index, value);
    }

    protected override void OnInsertComplete(int index, object value)
    {
      base.OnInsertComplete(index, value);
      this.OnItemAdded(index, value);
    }

    #endregion
  }

  public abstract class CollectionBase : CollectionBaseGeneric<object>
  {
    protected CollectionBase(){}

    protected CollectionBase(int capacity):base(capacity){}  
  }

  #region CollectionBaseGeneric<T>

  public abstract class CollectionBaseGeneric<T> : IList, ICollection, IEnumerable
  {
    // Fields
    private System.Collections.Generic.List<T> list;

    // Methods
    protected CollectionBaseGeneric()
    {
      this.list = new List<T>();
    }

    protected CollectionBaseGeneric(int capacity)
    {
      this.list = new List<T>(capacity);
    }

    public void Clear()
    {
      this.OnClear();
      this.InnerList.Clear();
      this.OnClearComplete();
    }

    public IEnumerator GetEnumerator()
    {
      return this.InnerList.GetEnumerator();
    }

    protected virtual void OnClear()
    {
    }

    protected virtual void OnClearComplete()
    {
    }

    protected virtual void OnInsert(int index, object value)
    {
    }

    protected virtual void OnInsertComplete(int index, object value)
    {
    }

    protected virtual void OnRemove(int index, object value)
    {
    }

    protected virtual void OnRemoveComplete(int index, object value)
    {
    }

    protected virtual void OnSet(int index, object oldValue, object newValue)
    {
    }

    protected virtual void OnSetComplete(int index, object oldValue, object newValue)
    {
    }

    protected virtual void OnValidate(object value)
    {
      if (value == null)
      {
        throw new ArgumentNullException("value");
      }
    }

    public void RemoveAt(int index)
    {
      if ((index < 0) || (index >= this.InnerList.Count))
      {
        throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_Index");
      }
      T obj2 = this.InnerList[index];
      this.OnValidate(obj2);
      this.OnRemove(index, obj2);
      this.InnerList.RemoveAt(index);
      try
      {
        this.OnRemoveComplete(index, obj2);
      }
      catch
      {
        this.InnerList.Insert(index, obj2);
        throw;
      }
    }

    void ICollection.CopyTo(Array array, int index)
    {
      this.InnerList.CopyTo((T[])array, index);
    }

    int IList.Add(object value)
    {
      this.OnValidate(value);
      this.OnInsert(this.InnerList.Count, value);
      this.InnerList.Add((T)value);
      int index = this.InnerList.IndexOf((T)value);
      try
      {
        this.OnInsertComplete(index, value);
      }
      catch
      {
        this.InnerList.RemoveAt(index);
        throw;
      }
      return index;
    }

    bool IList.Contains(object value)
    {
      return this.InnerList.Contains((T)value);
    }

    int IList.IndexOf(object value)
    {
      return this.InnerList.IndexOf((T)value);
    }

    void IList.Insert(int index, object value)
    {
      if ((index < 0) || (index > this.InnerList.Count))
      {
        throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_Index");
      }
      this.OnValidate(value);
      this.OnInsert(index, value);
      this.InnerList.Insert(index, (T)value);
      try
      {
        this.OnInsertComplete(index, value);
      }
      catch
      {
        this.InnerList.RemoveAt(index);
        throw;
      }
    }

    void IList.Remove(object value)
    {
      this.OnValidate(value);
      int index = this.InnerList.IndexOf((T)value);
      if (index < 0)
      {
        throw new ArgumentException("Arg_RemoveArgNotFound");
      }
      this.OnRemove(index, value);
      this.InnerList.RemoveAt(index);
      try
      {
        this.OnRemoveComplete(index, value);
      }
      catch
      {
        this.InnerList.Insert(index, (T)value);
        throw;
      }
    }

    // Properties
    public int Capacity
    {
      get
      {
        return this.InnerList.Capacity;
      }
      set
      {
        this.InnerList.Capacity = value;
      }
    }

    public int Count
    {
      get
      {
        if (this.list != null)
        {
          return this.list.Count;
        }
        return 0;
      }
    }

    protected List<T> InnerList
    {
      get
      {
        if (this.list == null)
        {
          this.list = new List<T>();
        }
        return this.list;
      }
    }

    protected IList List
    {
      get
      {
        return this;
      }
    }

    bool ICollection.IsSynchronized
    {
      get
      {
        return false;
      }
    }

    object ICollection.SyncRoot
    {
      get
      {
        return null;
      }
    }

    bool IList.IsFixedSize
    {
      get
      {
        return false;
      }
    }

    bool IList.IsReadOnly
    {
      get
      {
        return false;
      }
    }

    object IList.this[int index]
    {
      get
      {
        if ((index < 0) || (index >= this.InnerList.Count))
        {
          throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_Index");
        }
        return this.InnerList[index];
      }
      set
      {
        if ((index < 0) || (index >= this.InnerList.Count))
        {
          throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_Index");
        }
        this.OnValidate(value);
        object oldValue = this.InnerList[index];
        this.OnSet(index, oldValue, value);
        this.InnerList[index] = (T)value;
        try
        {
          this.OnSetComplete(index, oldValue, value);
        }
        catch
        {
          this.InnerList[index] = (T)oldValue;
          throw;
        }
      }
    }
  }
  #endregion
}
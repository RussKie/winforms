// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using static System.Windows.Forms.ListBox.ItemArray;

namespace System.Windows.Forms
{
    public partial class ListBox
    {
        /// <summary>
        ///  This is similar to ArrayList except that it also
        ///  mantains a bit-flag based state element for each item
        ///  in the array.
        ///
        ///  The methods to enumerate, count and get data support
        ///  virtualized indexes.  Indexes are virtualized according
        ///  to the state mask passed in.  This allows ItemArray
        ///  to be the backing store for one read-write "master"
        ///  collection and serveral read-only collections based
        ///  on masks.  ItemArray supports up to 31 masks.
        /// </summary>
        internal partial class ItemArray : List<Entry>, IComparer<Entry>
        {
            private static int s_lastMask = 1;
            private readonly ListControl _listControl;

            public ItemArray(ListControl listControl)
            {
                _listControl = listControl;
            }

            /// <summary>
            ///  The version of this array.  This number changes with each
            ///  change to the item list.
            /// </summary>
            public int Version { get; private set; }

            /// <summary>
            ///  Adds the given item to the array.  The state is initially
            ///  zero.
            /// </summary>
            public object Add(object item)
            {
                Version++;
                return Add(new Entry(item));
            }

            /// <summary>
            ///  Adds the given collection of items to the array.
            /// </summary>
            public void AddRange(ICollection items)
            {
                if (items == null)
                {
                    throw new ArgumentNullException(nameof(items));
                }

                foreach (object item in items)
                {
                    Add(item);
                }
                Version++;
            }

            /// <summary>
            ///  Clears this array.
            /// </summary>
            public new void Clear()
            {
                base.Clear();
                Version++;
            }

            /// <summary>
            ///  Allocates a new bitmask for use.
            /// </summary>
            public static int CreateMask()
            {
                int mask = s_lastMask;
                s_lastMask <<= 1;
                Debug.Assert(s_lastMask > mask, "We have overflowed our state mask.");
                return mask;
            }

            /// <summary>
            ///  Turns a virtual index into an actual index.
            /// </summary>
            public int GetActualIndex(int virtualIndex, int stateMask)
            {
                if (stateMask == 0)
                {
                    return virtualIndex;
                }

                // More complex; we must compute this index.
                int calcIndex = -1;
                for (int i = 0; i < Count; i++)
                {
                    if ((this[i].state & stateMask) != 0)
                    {
                        calcIndex++;
                        if (calcIndex == virtualIndex)
                        {
                            return i;
                        }
                    }
                }

                return -1;
            }

            /// <summary>
            ///  Gets the count of items matching the given mask.
            /// </summary>
            public int GetCount(int stateMask)
            {
                // If mask is zero, then just give the main count
                if (stateMask == 0)
                {
                    return Count;
                }

                // more complex:  must provide a count of items
                // based on a mask.

                int filteredCount = 0;

                for (int i = 0; i < Count; i++)
                {
                    if ((this[i].state & stateMask) != 0)
                    {
                        filteredCount++;
                    }
                }

                return filteredCount;
            }

            /// <summary>
            ///  Retrieves an enumerator that will enumerate based on
            ///  the given mask.
            /// </summary>
            public IEnumerator GetEnumerator(int stateMask)
            {
                return GetEnumerator(stateMask, false);
            }

            /// <summary>
            ///  Retrieves an enumerator that will enumerate based on
            ///  the given mask.
            /// </summary>
            public IEnumerator GetEnumerator(int stateMask, bool anyBit)
            {
                return new EntryEnumerator(this, stateMask, anyBit);
            }

            /// <summary>
            ///  Gets the item at the given index.  The index is
            ///  virtualized against the given mask value.
            /// </summary>
            public object GetItem(int virtualIndex, int stateMask)
            {
                int actualIndex = GetActualIndex(virtualIndex, stateMask);

                if (actualIndex == -1)
                {
                    throw new IndexOutOfRangeException();
                }

                return this[actualIndex].item;
            }
            /// <summary>
            ///  Gets the item at the given index.  The index is
            ///  virtualized against the given mask value.
            /// </summary>
            internal object GetEntryObject(int virtualIndex, int stateMask)
            {
                int actualIndex = GetActualIndex(virtualIndex, stateMask);

                if (actualIndex == -1)
                {
                    throw new IndexOutOfRangeException();
                }

                return this[actualIndex];
            }
            /// <summary>
            ///  Returns true if the requested state mask is set.
            ///  The index is the actual index to the array.
            /// </summary>
            public bool GetState(int index, int stateMask)
            {
                return ((this[index].state & stateMask) == stateMask);
            }

            /// <summary>
            ///  Returns the virtual index of the item based on the
            ///  state mask.
            /// </summary>
            public int IndexOf(object item, int stateMask)
            {
                int virtualIndex = -1;

                for (int i = 0; i < Count; i++)
                {
                    if (stateMask == 0 || (this[i].state & stateMask) != 0)
                    {
                        virtualIndex++;
                        if (this[i].item.Equals(item))
                        {
                            return virtualIndex;
                        }
                    }
                }

                return -1;
            }

            /// <summary>
            ///  Returns the virtual index of the item based on the
            ///  state mask. Uses reference equality to identify the
            ///  given object in the list.
            /// </summary>
            public int IndexOfIdentifier(object identifier, int stateMask)
            {
                int virtualIndex = -1;

                for (int i = 0; i < Count; i++)
                {
                    if (stateMask == 0 || (this[i].state & stateMask) != 0)
                    {
                        virtualIndex++;
                        if (this[i] == identifier)
                        {
                            return virtualIndex;
                        }
                    }
                }

                return -1;
            }

            /// <summary>
            ///  Inserts item at the given index.  The index
            ///  is not virtualized.
            /// </summary>
            public void Insert(int index, object item)
            {
                Insert(index, new Entry(item));
                Version++;
            }

            /// <summary>
            ///  Removes the given item from the array.  If
            ///  the item is not in the array, this does nothing.
            /// </summary>
            public void Remove(object item)
            {
                int index = IndexOf(item, 0);
                if (index != -1)
                {
                    RemoveAt(index);
                }
            }

            /// <summary>
            ///  Removes the item at the given index.
            /// </summary>
            public new void RemoveAt(int index)
            {
                base.RemoveAt(index);
                Version++;
            }

            /// <summary>
            ///  Sets the item at the given index to a new value.
            /// </summary>
            public void SetItem(int index, object item)
            {
                this[index].item = item;
            }

            /// <summary>
            ///  Sets the state data for the given index.
            /// </summary>
            public void SetState(int index, int stateMask, bool value)
            {
                if (value)
                {
                    this[index].state |= stateMask;
                }
                else
                {
                    this[index].state &= ~stateMask;
                }
                Version++;
            }

            ///// <summary>
            /////  Find element in sorted array. If element is not found returns a binary complement of index for inserting
            ///// </summary>
            //public int BinarySearch(object element)
            //{
            //    return base.BinarySearch(element, this);
            //}

            ///// <summary>
            /////  Sorts our array.
            ///// </summary>
            //public void Sort()
            //{
            //    Array.Sort(_entries, 0, _count, this);
            //}

            //public void Sort(Array externalArray)
            //{
            //    Array.Sort(externalArray, this);
            //}

            int IComparer<Entry>.Compare(Entry item1, Entry item2)
            {
                if (item1 == null)
                {
                    if (item2 == null)
                    {
                        return 0; //both null, then they are equal
                    }

                    return -1; //item1 is null, but item2 is valid (greater)
                }
                if (item2 == null)
                {
                    return 1; //item2 is null, so item 1 is greater
                }

                string itemName1 = _listControl.GetItemText(item1);
                string itemName2 = _listControl.GetItemText(item2);

                CompareInfo compInfo = Application.CurrentCulture.CompareInfo;
                return compInfo.Compare(itemName1, itemName2, CompareOptions.StringSort);
            }
        }
    }
}

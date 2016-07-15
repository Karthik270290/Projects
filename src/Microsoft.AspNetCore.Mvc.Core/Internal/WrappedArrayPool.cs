// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Buffers;
using System;

namespace Microsoft.AspNetCore.Mvc.Internal
{
    internal class WrappedArrayPool<T> : ArrayPool<T>
    {
        private const int _slots = 2;
        [ThreadStatic]
        private static T[][] _arrays;
        private ArrayPool<T> _inner = ArrayPool<T>.Shared;

        public override T[] Rent(int minimumLength)
        {
            if (_arrays == null)
            {
                _arrays = new T[_slots][];
            }

            for (int i = 0; i < _slots; i++)
            {
                if (_arrays[i] != null && _arrays[i].Length >= minimumLength)
                {
                    var rental = _arrays[i];
                    _arrays[i] = null;
                    return rental;
                }
            }

            return _inner.Rent(minimumLength);
        }

        public override void Return(T[] array, bool clearArray = false)
        {
            if (_arrays == null)
            {
                _arrays = new T[_slots][];
            }

            for (int i = 0; i < _slots; i++)
            {
                if (_arrays[i] == null)
                {
                    _arrays[i] = array;
                    return;
                }
            }

            for (int i = 0; i < _slots; i++)
            {
                if (_arrays[i].Length < array.Length)
                {
                    _inner.Return(_arrays[i]);
                    _arrays[i] = array;
                    return;
                }
            }

            _inner.Return(array);
        }
    }
}
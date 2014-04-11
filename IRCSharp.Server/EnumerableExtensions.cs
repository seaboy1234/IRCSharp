//    Project:     IRC# Server 
//    File:        EnumerableExtensions.cs
//    Copyright:   Copyright (C) 2014 Christian Wilson. All rights reserved.
//    Website:     https://github.com/seaboy1234/IRCSharp
//    Description: An Internet Relay Chat (IRC) Server written in C#.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRCSharp.Server
{
    public static class EnumerableExtensions
    {
        public static void RemoveGroup<T>(this List<T> list, IEnumerable<T> range)
        {
            foreach (T item in range)
            {
                list.Remove(item);
            }
        }

        public static T[] First<T>(this IEnumerable<T> collection, int n)
        {
            T[] array = new T[n];
            for (int i = 0; i < n; i++)
            {
                array[i] = collection.ElementAt(i);
            }
            return array;
        }

        public static bool TrueForAny<T>(this IEnumerable<T> item, Predicate<T> condition)
        {
            foreach (T obj in item)
            {
                if (condition(obj))
                {
                    return true;
                }
            }
            return false;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> item, Action<T> action)
        {
            foreach (var obj in item)
            {
                action.Invoke(obj);
            }
            return item;
        }

    }
}

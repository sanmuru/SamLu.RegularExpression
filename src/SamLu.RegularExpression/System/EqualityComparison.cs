using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// 表示用于比较相同类型的两个对象是否相等的方法。
    /// </summary>
    /// <typeparam name="T">要比较的对象的类型。</typeparam>
    /// <param name="x">要比较的第一个对象。</param>
    /// <param name="y">要比较的第二个对象。</param>
    /// <returns>一个布尔值，指示 <paramref name="x"/> 和 <paramref name="y"/> 是否相等。</returns>
    public delegate bool EqualityComparison<in T>(T x, T y);
}

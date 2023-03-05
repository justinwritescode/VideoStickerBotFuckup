/* 
 * ReflectionExtensions.cs
 * 
 *   Created: 2023-03-03-01:02:35
 *   Modified: 2023-03-03-01:02:36
 * 
 *   Author: Justin Chase <justin@justinwritescode.com>
 *   
 *   Copyright © 2022-2023 Justin Chase, All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

using System;
using System.Reflection;
namespace ReflecitionExtensions;

public static class ReflectionExt
{
    public static object getattr(this object obj, string name)
    {
        Type type = obj.GetType();
        BindingFlags flags = BindingFlags.Instance |
                                 BindingFlags.Public |
                                 BindingFlags.GetProperty;

        return type.InvokeMember(name, flags, Type.DefaultBinder, obj, null);
    }
}

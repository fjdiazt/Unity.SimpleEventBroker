#region File Header

//
// WinRT Reflector Shim - a library to assist in porting frameworks from .NET to WinRT.
// https://github.com/mbrit/WinRTReflectionShim
//
// *** USE THIS FILE IN YOUR METRO-STYLE PROJECT ***
//
// Copyright (c) 2012 Matthew Baxter-Reynolds 2012 (@mbrit)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

#endregion

#region Using statements

using System.Collections.Generic;
using System.Linq;

#endregion

namespace System.Reflection
{
    /// <summary>   A type extender. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public static class TypeExtender
    {
        /// <summary>   List of types of the empties. </summary>
        public static Type[] EmptyTypes = {};

        /// <summary>   A Type extension method that assemblies the given type. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   An Assembly. </returns>
        public static Assembly Assembly(this Type type)
        {
            return type.GetTypeInfo().Assembly;
        }

        /// <summary>   A Type extension method that query if 'type' is generic type. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   true if generic type, false if not. </returns>
        public static bool IsGenericType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType;
        }

        /// <summary>   A Type extension method that query if 'type' is generic type definition. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   true if generic type definition, false if not. </returns>
        public static bool IsGenericTypeDefinition(this Type type)
        {
            return type.GetTypeInfo().IsGenericTypeDefinition;
        }

        /// <summary>   A Type extension method that query if 'type' is assignable from. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type">     The type to act on. </param>
        /// <param name="toCheck">  to check. </param>
        /// <returns>   true if assignable from, false if not. </returns>
        public static bool IsAssignableFrom(this Type type, Type toCheck)
        {
            return type.GetTypeInfo().IsAssignableFrom(toCheck.GetTypeInfo());
        }

        /// <summary>   A Type extension method that query if 'type' is instance of type. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <exception cref="NotImplementedException">
        ///     Thrown when the requested operation is
        ///     unimplemented.
        /// </exception>
        /// <param name="type">     The type to act on. </param>
        /// <param name="toCheck">  to check. </param>
        /// <returns>   true if instance of type, false if not. </returns>
        public static bool IsInstanceOfType(this Type type, object toCheck)
        {
            throw new NotImplementedException("This operation has not been implemented.");
        }

        /// <summary>   A Type extension method that query if 'type' is subclass of. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type">     The type to act on. </param>
        /// <param name="toCheck">  to check. </param>
        /// <returns>   true if subclass of, false if not. </returns>
        public static bool IsSubclassOf(this Type type, Type toCheck)
        {
            return type.GetTypeInfo().IsSubclassOf(toCheck);
        }

        /// <summary>   A Type extension method that gets a method. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type">     The type to act on. </param>
        /// <param name="name">     The name. </param>
        /// <param name="flags">    (Optional) The flags. </param>
        /// <returns>   The method. </returns>
        public static MethodInfo GetMethod(this Type type, string name, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
        {
            return GetMember<MethodInfo>(type, name, flags);
        }

        /// <summary>   A Type extension method that gets a method. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type">         The type to act on. </param>
        /// <param name="name">         The name. </param>
        /// <param name="parameters">   Options for controlling the operation. </param>
        /// <returns>   The method. </returns>
        public static MethodInfo GetMethod(this Type type, string name, Type[] parameters)
        {
            return type.GetMethod(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, null, parameters, null);
        }

        /// <summary>   A Type extension method that gets a method. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type">         The type to act on. </param>
        /// <param name="name">         The name. </param>
        /// <param name="flags">        The flags. </param>
        /// <param name="binder">       The binder. </param>
        /// <param name="parameters">   Options for controlling the operation. </param>
        /// <param name="modifiers">    The modifiers. </param>
        /// <returns>   The method. </returns>
        public static MethodInfo GetMethod(this Type type, string name, BindingFlags flags, object binder, Type[] parameters, object[] modifiers)
        {
            return type.GetMethod(name, flags, null, CallingConventions.Any, parameters, modifiers);
        }

        /// <summary>   A Type extension method that gets a method. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type">             The type to act on. </param>
        /// <param name="name">             The name. </param>
        /// <param name="flags">            The flags. </param>
        /// <param name="binder">           The binder. </param>
        /// <param name="callConvention">   The call convention. </param>
        /// <param name="parameters">       Options for controlling the operation. </param>
        /// <param name="modifiers">        The modifiers. </param>
        /// <returns>   The method. </returns>
        public static MethodInfo GetMethod(this Type type,
                string name,
                BindingFlags flags,
                object binder,
                CallingConventions callConvention,
                Type[] parameters,
                object[] modifiers)
        {
            foreach(var method in type.GetMethods(flags))
            {
                if(method.Name == name && CheckParameters(method, parameters))
                    return method;
            }

            return null;
        }

        /// <summary>   A Type extension method that query if 'type' is interface. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   true if interface, false if not. </returns>
        public static bool IsInterface(this Type type)
        {
            return type.GetTypeInfo().IsInterface;
        }

        /// <summary>   A Type extension method that query if 'type' is class. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   true if class, false if not. </returns>
        public static bool IsClass(this Type type)
        {
            return type.GetTypeInfo().IsClass;
        }

        /// <summary>   A Type extension method that query if 'type' is public. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   true if public, false if not. </returns>
        public static bool IsPublic(this Type type)
        {
            return type.GetTypeInfo().IsPublic;
        }

        /// <summary>   A Type extension method that query if 'type' is array. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   true if array, false if not. </returns>
        public static bool IsArray(this Type type)
        {
            return type.GetTypeInfo().IsArray;
        }

        /// <summary>   A Type extension method that query if 'type' is enum. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   true if enum, false if not. </returns>
        public static bool IsEnum(this Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }

        /// <summary>   A Type extension method that query if 'type' is nested public. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   true if nested public, false if not. </returns>
        public static bool IsNestedPublic(this Type type)
        {
            return type.GetTypeInfo().IsNestedPublic;
        }

        /// <summary>   A Type extension method that query if 'type' is nested assembly. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   true if nested assembly, false if not. </returns>
        public static bool IsNestedAssembly(this Type type)
        {
            return type.GetTypeInfo().IsNestedAssembly;
        }

        /// <summary>   A Type extension method that query if 'type' is nested fam or assem. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   true if nested fam or assem, false if not. </returns>
        public static bool IsNestedFamORAssem(this Type type)
        {
            return type.GetTypeInfo().IsNestedFamORAssem;
        }

        /// <summary>   A Type extension method that query if 'type' is visible. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   true if visible, false if not. </returns>
        public static bool IsVisible(this Type type)
        {
            return type.GetTypeInfo().IsVisible;
        }

        /// <summary>   A Type extension method that query if 'type' is abstract. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   true if abstract, false if not. </returns>
        public static bool IsAbstract(this Type type)
        {
            return type.GetTypeInfo().IsAbstract;
        }

        /// <summary>   A Type extension method that query if 'type' is sealed. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   true if sealed, false if not. </returns>
        public static bool IsSealed(this Type type)
        {
            return type.GetTypeInfo().IsSealed;
        }

        /// <summary>   A Type extension method that query if 'type' is primitive. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   true if primitive, false if not. </returns>
        public static bool IsPrimitive(this Type type)
        {
            return type.GetTypeInfo().IsPrimitive;
        }

        /// <summary>   A Type extension method that query if 'type' is value type. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   true if value type, false if not. </returns>
        public static bool IsValueType(this Type type)
        {
            return type.GetTypeInfo().IsValueType;
        }

        /// <summary>   A Type extension method that modules the given type. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   A Module. </returns>
        public static Module Module(this Type type)
        {
            return type.GetTypeInfo().Module;
        }

        /// <summary>   A Type extension method that declaring method. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   A MethodBase. </returns>
        public static MethodBase DeclaringMethod(this Type type)
        {
            return type.GetTypeInfo().DeclaringMethod;
        }

        /// <summary>   A Type extension method that underlying system type. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   A Type. </returns>
        public static Type UnderlyingSystemType(this Type type)
        {
            // @mbrit - 2012-05-30 - this needs more science... UnderlyingSystemType isn't supported
            // in WinRT, but unclear why this was used...
            return type;
        }

        /// <summary>   A Type extension method that query if 'type' contains generic parameters. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   true if it succeeds, false if it fails. </returns>
        public static bool ContainsGenericParameters(this Type type)
        {
            return type.GetTypeInfo().ContainsGenericParameters;
        }

        /// <summary>   A Type extension method that generic parameter attributes. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   The GenericParameterAttributes. </returns>
        public static GenericParameterAttributes GenericParameterAttributes(this Type type)
        {
            return type.GetTypeInfo().GenericParameterAttributes;
        }

        /// <summary>   A Type extension method that base type. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   A Type. </returns>
        public static Type BaseType(this Type type)
        {
            return type.GetTypeInfo().BaseType;
        }

        /// <summary>   A Type extension method that gets the interfaces. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   An array of type. </returns>
        public static Type[] GetInterfaces(this Type type)
        {
            return type.GetTypeInfo().ImplementedInterfaces.ToArray();
        }

        /// <summary>   A Type extension method that gets an interface. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <exception cref="NotImplementedException">
        ///     Thrown when the requested operation is
        ///     unimplemented.
        /// </exception>
        /// <param name="type">             The type to act on. </param>
        /// <param name="interfaceType">    Type of the interface. </param>
        /// <returns>   The interface. </returns>
        public static InterfaceMapping GetInterface(this Type type, Type interfaceType)
        {
            throw new NotImplementedException("This operation has not been implemented.");
        }

        /// <summary>   A Type extension method that gets custom attributes. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type">     The type to act on. </param>
        /// <param name="inherit">  (Optional) true to inherit. </param>
        /// <returns>   An array of object. </returns>
        public static object[] GetCustomAttributes(this Type type, bool inherit = false)
        {
            return type.GetTypeInfo().GetCustomAttributes(inherit).ToArray();
        }

        /// <summary>   A Type extension method that gets custom attributes. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type">             The type to act on. </param>
        /// <param name="attributeType">    Type of the attribute. </param>
        /// <param name="inherit">          (Optional) true to inherit. </param>
        /// <returns>   An array of object. </returns>
        public static object[] GetCustomAttributes(this Type type, Type attributeType, bool inherit = false)
        {
            return type.GetTypeInfo().GetCustomAttributes(attributeType, inherit).ToArray();
        }

        /// <summary>   A Type extension method that gets the constructors. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type">     The type to act on. </param>
        /// <param name="flags">    (Optional) The flags. </param>
        /// <returns>   An array of constructor information. </returns>
        public static ConstructorInfo[] GetConstructors(this Type type, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            return GetMembers<ConstructorInfo>(type, flags).ToArray();
        }

        /// <summary>   A Type extension method that gets the properties. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type">     The type to act on. </param>
        /// <param name="flags">    (Optional) The flags. </param>
        /// <returns>   An array of property information. </returns>
        public static PropertyInfo[] GetProperties(this Type type, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public)
        {
            return GetMembers<PropertyInfo>(type, flags).ToArray();
        }

        /// <summary>   A Type extension method that gets the methods. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type">     The type to act on. </param>
        /// <param name="flags">    (Optional) The flags. </param>
        /// <returns>   An array of method information. </returns>
        public static MethodInfo[] GetMethods(this Type type, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public)
        {
            return GetMembers<MethodInfo>(type, flags).ToArray();
        }

        /// <summary>   A Type extension method that gets the fields. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type">     The type to act on. </param>
        /// <param name="flags">    (Optional) The flags. </param>
        /// <returns>   An array of field information. </returns>
        public static FieldInfo[] GetFields(this Type type, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public)
        {
            return GetMembers<FieldInfo>(type, flags).ToArray();
        }

        /// <summary>   A Type extension method that gets the events. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type">     The type to act on. </param>
        /// <param name="flags">    (Optional) The flags. </param>
        /// <returns>   An array of event information. </returns>
        public static EventInfo[] GetEvents(this Type type, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public)
        {
            return GetMembers<EventInfo>(type, flags).ToArray();
        }

        /// <summary>   Gets the members. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="type">     The type to act on. </param>
        /// <param name="flags">    The flags. </param>
        /// <returns>   The members. </returns>
        private static List<T> GetMembers<T>(Type type, BindingFlags flags)
                where T : MemberInfo
        {
            var results = new List<T>();

            var info = type.GetTypeInfo();
            var inParent = false;
            while(true)
            {
                foreach(T member in info.DeclaredMembers.Where(v => typeof(T).IsAssignableFrom(v.GetType())))
                {
                    if(member.CheckBindings(flags, inParent))
                        results.Add(member);
                }

                // constructors never walk the hierarchy...
                if(typeof(T) == typeof(ConstructorInfo))
                    break;

                // up...
                if(info.BaseType == null)
                    break;
                info = info.BaseType.GetTypeInfo();
                inParent = true;
            }

            return results;
        }

        /// <summary>   A Type extension method that gets a constructor. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type">     The type to act on. </param>
        /// <param name="types">    The types. </param>
        /// <returns>   The constructor. </returns>
        public static ConstructorInfo GetConstructor(this Type type, Type[] types)
        {
            return type.GetConstructor(BindingFlags.Public, null, types, null);
        }

        /// <summary>   A Type extension method that gets a constructor. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type">         The type to act on. </param>
        /// <param name="flags">        The flags. </param>
        /// <param name="binder">       The binder. </param>
        /// <param name="types">        The types. </param>
        /// <param name="modifiers">    The modifiers. </param>
        /// <returns>   The constructor. </returns>
        public static ConstructorInfo GetConstructor(this Type type, BindingFlags flags, object binder, Type[] types, object[] modifiers)
        {
            // can't have static constructors...
            flags |= BindingFlags.Instance | BindingFlags.Static;
            flags ^= BindingFlags.Static;

            // walk...
            foreach(var info in type.GetTypeInfo().DeclaredConstructors)
            {
                if(info.CheckBindings(flags, false) && CheckParameters(info, types))
                    return info;
            }

            return null;
        }

        /// <summary>   Check parameters. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="method">       The method. </param>
        /// <param name="parameters">   Options for controlling the operation. </param>
        /// <returns>   true if it succeeds, false if it fails. </returns>
        private static bool CheckParameters(MethodBase method, Type[] parameters)
        {
            var methodParameters = method.GetParameters();
            if(methodParameters.Length == parameters.Length)
                if(parameters.Length == 0)
                    return true;
                else
                {
                    for(var index = 0; index < parameters.Length; index++)
                    {
                        if(parameters[index] != methodParameters[index].ParameterType)
                            return false;
                    }

                    // ok...
                    return true;
                }

            // nope...
            return false;
        }

        /// <summary>   A Type extension method that gets a property. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type">     The type to act on. </param>
        /// <param name="name">     The name. </param>
        /// <param name="flags">    (Optional) The flags. </param>
        /// <returns>   The property. </returns>
        public static PropertyInfo GetProperty(this Type type, string name, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
        {
            return GetMember<PropertyInfo>(type, name, flags);
        }

        /// <summary>   A Type extension method that gets an event. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type">     The type to act on. </param>
        /// <param name="name">     The name. </param>
        /// <param name="flags">    (Optional) The flags. </param>
        /// <returns>   The event. </returns>
        public static EventInfo GetEvent(this Type type, string name, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
        {
            return GetMember<EventInfo>(type, name, flags);
        }

        /// <summary>   A Type extension method that gets a field. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type">     The type to act on. </param>
        /// <param name="name">     The name. </param>
        /// <param name="flags">    (Optional) The flags. </param>
        /// <returns>   The field. </returns>
        public static FieldInfo GetField(this Type type, string name, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
        {
            return GetMember<FieldInfo>(type, name, flags);
        }

        /// <summary>   Gets a member. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="type">     The type to act on. </param>
        /// <param name="name">     The name. </param>
        /// <param name="flags">    The flags. </param>
        /// <returns>   The member. </returns>
        private static T GetMember<T>(Type type, string name, BindingFlags flags)
                where T : MemberInfo
        {
            // walk...
            foreach(var member in GetMembers<T>(type, flags))
            {
                if(member.Name == name)
                    return (T)member;
            }

            return null;
        }

        /// <summary>   A Type extension method that gets an interface. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type">         The type to act on. </param>
        /// <param name="name">         The name. </param>
        /// <param name="ignoreCase">   (Optional) true to ignore case. </param>
        /// <returns>   The interface. </returns>
        public static Type GetInterface(this Type type, string name, bool ignoreCase = false)
        {
            // walk up the hierarchy...
            var info = type.GetTypeInfo();
            while(true)
            {
                foreach(var iface in type.GetInterfaces())
                {
                    if(ignoreCase)
                    {
                        // this matches just the name...
                        if(string.Compare(iface.Name, name, StringComparison.CurrentCultureIgnoreCase) == 0)
                            return iface;
                    }
                    else if(iface.FullName == name || iface.Name == name)
                        return iface;
                }

                // up...
                if(info.BaseType == null)
                    break;
                info = info.BaseType.GetTypeInfo();
            }

            return null;
        }

        /// <summary>   A Type extension method that gets generic arguments. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   An array of type. </returns>
        public static Type[] GetGenericArguments(this Type type)
        {
            return type.GetTypeInfo().GenericTypeArguments;
        }

        /// <summary>   A Type extension method that gets generic parameter constraints. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   An array of type. </returns>
        public static Type[] GetGenericParameterConstraints(this Type type)
        {
            return type.GetTypeInfo().GetGenericParameterConstraints();
        }

        /// <summary>   A Type extension method that gets custom attribute. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="type">     The type to act on. </param>
        /// <param name="inherit">  (Optional) true to inherit. </param>
        /// <returns>   The custom attribute. </returns>
        public static object GetCustomAttribute<T>(this Type type, bool inherit = false)
                where T : Attribute
        {
            return type.GetTypeInfo().GetCustomAttribute<T>(inherit);
        }

        /// <summary>   A Type extension method that gets interface map. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type">             The type to act on. </param>
        /// <param name="interfaceType">    Type of the interface. </param>
        /// <returns>   The interface map. </returns>
        public static InterfaceMapping GetInterfaceMap(this Type type, Type interfaceType)
        {
            return type.GetTypeInfo().GetRuntimeInterfaceMap(interfaceType);
        }

        /// <summary>   A Type extension method that gets type code. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   The type code. </returns>
        public static TypeCode GetTypeCode(this Type type)
        {
            if(type == null)
                return TypeCode.Empty;

            if(typeof(bool).IsAssignableFrom(type))
                return TypeCode.Boolean;
            else if(typeof(char).IsAssignableFrom(type))
                return TypeCode.Char;
            else if(typeof(sbyte).IsAssignableFrom(type))
                return TypeCode.SByte;
            else if(typeof(byte).IsAssignableFrom(type))
                return TypeCode.Byte;
            else if(typeof(short).IsAssignableFrom(type))
                return TypeCode.Int16;
            else if(typeof(ushort).IsAssignableFrom(type))
                return TypeCode.UInt16;
            else if(typeof(int).IsAssignableFrom(type))
                return TypeCode.Int32;
            else if(typeof(uint).IsAssignableFrom(type))
                return TypeCode.UInt32;
            else if(typeof(long).IsAssignableFrom(type))
                return TypeCode.Int64;
            else if(typeof(ulong).IsAssignableFrom(type))
                return TypeCode.UInt64;
            else if(typeof(float).IsAssignableFrom(type))
                return TypeCode.Single;
            else if(typeof(double).IsAssignableFrom(type))
                return TypeCode.Double;
            else if(typeof(decimal).IsAssignableFrom(type))
                return TypeCode.Decimal;
            else if(typeof(DateTime).IsAssignableFrom(type))
                return TypeCode.DateTime;
            else if(typeof(string).IsAssignableFrom(type))
                return TypeCode.String;
            else
                return TypeCode.Object;
        }

        /// <summary>   A Type extension method that searches for the first interfaces. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type">     The type to act on. </param>
        /// <param name="filter">   Specifies the filter. </param>
        /// <param name="criteria"> The criteria. </param>
        /// <returns>   The found interfaces. </returns>
        public static Type[] FindInterfaces(this Type type, Func<Type, object, bool> filter, object criteria)
        {
            var results = new List<Type>();
            foreach(var walk in type.GetInterfaces())
            {
                if(filter(type, criteria))
                    results.Add(walk);
            }

            return results.ToArray();
        }

        /// <summary>   A Type extension method that metadata token. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="type"> The type to act on. </param>
        /// <returns>   An int. </returns>
        public static int MetadataToken(this Type type)
        {
            // @mbrit - 2012-06-01 - no idea what to do with this...
            return type.GetHashCode();
        }
    }

    /// <summary>   A property information extender. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public static class PropertyInfoExtender
    {
        /// <summary>   A PropertyInfo extension method that gets method. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="prop">         The prop to act on. </param>
        /// <param name="nonPublic">    (Optional) true to non public. </param>
        /// <returns>   The method. </returns>
        public static MethodInfo GetGetMethod(this PropertyInfo prop, bool nonPublic = false)
        {
            // @mbrit - 2012-05-30 - non-public not supported in winrt...
            if(prop.GetMethod != null && (prop.GetMethod.IsPublic || nonPublic))
                return prop.GetMethod;
            else
                return null;
        }

        /// <summary>   A PropertyInfo extension method that gets set method. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="prop">         The prop to act on. </param>
        /// <param name="nonPublic">    (Optional) true to non public. </param>
        /// <returns>   The set method. </returns>
        public static MethodInfo GetSetMethod(this PropertyInfo prop, bool nonPublic = false)
        {
            // @mbrit - 2012-05-30 - non-public not supported in winrt...
            if(prop.SetMethod != null && (prop.SetMethod.IsPublic || nonPublic))
                return prop.SetMethod;
            else
                return null;
        }

        /// <summary>   A PropertyInfo extension method that reflected type. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="prop"> The prop to act on. </param>
        /// <returns>   A Type. </returns>
        public static Type ReflectedType(this PropertyInfo prop)
        {
            // this isn't right...
            return prop.DeclaringType;
        }
    }

    /// <summary>   A parameter information extender. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public static class ParameterInfoExtender
    {
        /// <summary>   A ParameterInfo extension method that query if 'info' has attribute. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <exception cref="NotImplementedException">
        ///     Thrown when the requested operation is
        ///     unimplemented.
        /// </exception>
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="info"> The info to act on. </param>
        /// <returns>   true if attribute, false if not. </returns>
        public static bool HasAttribute<T>(this ParameterInfo info)
                where T : Attribute
        {
            throw new NotImplementedException("This operation has not been implemented.");
        }
    }

    /// <summary>   A member information extender. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public static class MemberInfoExtender
    {
        /// <summary>   A MemberInfo extension method that member type. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <exception cref="NotSupportedException">
        ///     Thrown when the requested operation is not
        ///     supported.
        /// </exception>
        /// <param name="member">   The member to act on. </param>
        /// <returns>   The MemberTypes. </returns>
        public static MemberTypes MemberType(this MemberInfo member)
        {
            if(member is MethodInfo)
                return ((MethodInfo)member).MemberType();
            else
                throw new NotSupportedException(string.Format("Cannot handle '{0}'.", member.GetType()));
        }

        /// <summary>   A MemberInfo extension method that reflected type. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <exception cref="NotSupportedException">
        ///     Thrown when the requested operation is not
        ///     supported.
        /// </exception>
        /// <param name="member">   The member to act on. </param>
        /// <returns>   A Type. </returns>
        public static Type ReflectedType(this MemberInfo member)
        {
            // this isn't right...
            if(member is MethodInfo)
                return ((MethodInfo)member).ReflectedType();
            else
                throw new NotSupportedException(string.Format("Cannot handle '{0}'.", member.GetType()));
        }

        /// <summary>   A MemberInfo extension method that query if 'member' has attribute. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="member">   The member to act on. </param>
        /// <param name="inherit">  (Optional) true to inherit. </param>
        /// <returns>   true if attribute, false if not. </returns>
        public static bool HasAttribute<T>(this MemberInfo member, bool inherit = false)
                where T : Attribute
        {
            return member.HasAttribute(typeof(T), inherit);
        }

        /// <summary>   A MemberInfo extension method that query if 'member' has attribute. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <exception cref="NotImplementedException">
        ///     Thrown when the requested operation is
        ///     unimplemented.
        /// </exception>
        /// <param name="member">   The member to act on. </param>
        /// <param name="type">     The type. </param>
        /// <param name="inherit">  (Optional) true to inherit. </param>
        /// <returns>   true if attribute, false if not. </returns>
        public static bool HasAttribute(this MemberInfo member, Type type, bool inherit = false)
        {
            throw new NotImplementedException("This operation has not been implemented.");
        }

        /// <summary>   A MemberInfo extension method that check bindings. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="member">   The member to act on. </param>
        /// <param name="flags">    The flags. </param>
        /// <param name="inParent"> true to in parent. </param>
        /// <returns>   true if it succeeds, false if it fails. </returns>
        public static bool CheckBindings(this MemberInfo member, BindingFlags flags, bool inParent)
        {
            if((member.IsStatic() & (flags & BindingFlags.Static) == BindingFlags.Static) ||
               (!(member.IsStatic()) & (flags & BindingFlags.Instance) == BindingFlags.Instance))
            {
                // if we're static and we're in parent, and we haven't specified flatten hierarchy, we can't match...
                if(inParent && (int)(flags & BindingFlags.FlattenHierarchy) == 0 && member.IsStatic())
                    return false;

                if((member.IsPublic() & (flags & BindingFlags.Public) == BindingFlags.Public) ||
                   (!(member.IsPublic()) & (flags & BindingFlags.NonPublic) == BindingFlags.NonPublic))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>   A MemberInfo extension method that query if 'member' is static. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <exception cref="NotSupportedException">
        ///     Thrown when the requested operation is not
        ///     supported.
        /// </exception>
        /// <param name="member">   The member to act on. </param>
        /// <returns>   true if static, false if not. </returns>
        public static bool IsStatic(this MemberInfo member)
        {
            if(member is MethodBase)
                return ((MethodBase)member).IsStatic;
            else if(member is PropertyInfo)
            {
                var prop = (PropertyInfo)member;
                return (prop.GetMethod != null && prop.GetMethod.IsStatic) || (prop.SetMethod != null && prop.SetMethod.IsStatic);
            }
            else if(member is FieldInfo)
                return ((FieldInfo)member).IsStatic;
            else if(member is EventInfo)
            {
                var evt = (EventInfo)member;
                return (evt.AddMethod != null && evt.AddMethod.IsStatic) || (evt.RemoveMethod != null && evt.RemoveMethod.IsStatic);
            }
            else
                throw new NotSupportedException(string.Format("Cannot handle '{0}'.", member.GetType()));
        }

        /// <summary>   A MemberInfo extension method that query if 'member' is public. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <exception cref="NotSupportedException">
        ///     Thrown when the requested operation is not
        ///     supported.
        /// </exception>
        /// <param name="member">   The member to act on. </param>
        /// <returns>   true if public, false if not. </returns>
        public static bool IsPublic(this MemberInfo member)
        {
            if(member is MethodBase)
                return ((MethodBase)member).IsPublic;
            else if(member is PropertyInfo)
            {
                var prop = (PropertyInfo)member;
                return (prop.GetMethod != null && prop.GetMethod.IsPublic) || (prop.SetMethod != null && prop.SetMethod.IsPublic);
            }
            else if(member is FieldInfo)
                return ((FieldInfo)member).IsPublic;
            else if(member is EventInfo)
            {
                var evt = (EventInfo)member;
                return (evt.AddMethod != null && evt.AddMethod.IsPublic) || (evt.RemoveMethod != null && evt.RemoveMethod.IsPublic);
            }
            else
                throw new NotSupportedException(string.Format("Cannot handle '{0}'.", member.GetType()));
        }

        /// <summary>   A MemberInfo extension method that metadata token. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="member">   The member to act on. </param>
        /// <returns>   An int. </returns>
        public static int MetadataToken(this MemberInfo member)
        {
            // @mbrit - 2012-06-01 - no idea what to do with this...
            return member.GetHashCode();
        }
    }

    /// <summary>   A method information extender. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public static class MethodInfoExtender
    {
        /// <summary>   A MethodInfo extension method that member type. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="method">   The method to act on. </param>
        /// <returns>   The MemberTypes. </returns>
        public static MemberTypes MemberType(this MethodInfo method)
        {
            return MemberTypes.Method;
        }

        /// <summary>   A MethodInfo extension method that reflected type. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="method">   The method to act on. </param>
        /// <returns>   A Type. </returns>
        public static Type ReflectedType(this MethodInfo method)
        {
            // this isn't right...
            return method.DeclaringType;
        }

        /// <summary>   A MethodInfo extension method that gets base definition. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="method">   The method to act on. </param>
        /// <returns>   The base definition. </returns>
        public static MethodInfo GetBaseDefinition(this MethodInfo method)
        {
            var flags = BindingFlags.Instance;
            if(method.IsPublic)
                flags |= BindingFlags.Public;
            else
                flags |= BindingFlags.NonPublic;

            var parameters = new List<Type>();
            foreach(var parameter in method.GetParameters())
                parameters.Add(parameter.ParameterType);

            // get...
            var info = method.DeclaringType.GetTypeInfo();
            var found = new List<MethodInfo>();
            while(true)
            {
                // find...
                var inParent = info.AsType().GetMethod(method.Name, flags, null, parameters.ToArray(), null);
                if(inParent != null)
                    found.Add(inParent);

                // up...
                if(info.BaseType == null)
                    break;
                info = info.BaseType.GetTypeInfo();
            }

            // return the last one...
            return found.Last();
        }

        /// <summary>   A MethodBase extension method that query if 'method' is abstract. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="method">   The method to act on. </param>
        /// <returns>   true if abstract, false if not. </returns>
        public static bool IsAbstract(this MethodBase method)
        {
            return method.IsAbstract;
        }
    }

    /// <summary>   An event information extender. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public static class EventInfoExtender
    {
        /// <summary>   An EventInfo extension method that gets add method. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="e">            The e to act on. </param>
        /// <param name="nonPublic">    (Optional) true to non public. </param>
        /// <returns>   The add method. </returns>
        public static MethodInfo GetAddMethod(this EventInfo e, bool nonPublic = false)
        {
            if(e.AddMethod != null && (e.AddMethod.IsPublic || nonPublic))
                return e.AddMethod;
            else
                return null;
        }

        /// <summary>   An EventInfo extension method that gets remove method. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="e">            The e to act on. </param>
        /// <param name="nonPublic">    (Optional) true to non public. </param>
        /// <returns>   The remove method. </returns>
        public static MethodInfo GetRemoveMethod(this EventInfo e, bool nonPublic = false)
        {
            if(e.RemoveMethod != null && (e.RemoveMethod.IsPublic || nonPublic))
                return e.RemoveMethod;
            else
                return null;
        }
    }

    /// <summary>   A parameter information extension. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public static class ParameterInfoExtension
    {
        /// <summary>   A ParameterInfo extension method that query if 'param' has attribute. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <exception cref="NotImplementedException">
        ///     Thrown when the requested operation is
        ///     unimplemented.
        /// </exception>
        /// <param name="param">    The param to act on. </param>
        /// <param name="type">     The type. </param>
        /// <returns>   true if attribute, false if not. </returns>
        public static bool HasAttribute(this ParameterInfo param, Type type)
        {
            throw new NotImplementedException("This operation has not been implemented.");
        }
    }

    /// <summary>   An assembly extender. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public static class AssemblyExtender
    {
        /// <summary>   Gets the attributes. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <exception cref="NotImplementedException">
        ///     Thrown when the requested operation is
        ///     unimplemented.
        /// </exception>
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <returns>   An array of object. </returns>
        public static object[] GetAttributes<T>()
                where T : Attribute
        {
            throw new NotImplementedException("This operation has not been implemented.");
        }

        /// <summary>   An Assembly extension method that gets the types. </summary>
        /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
        /// <param name="asm">  The asm to act on. </param>
        /// <returns>   An array of type. </returns>
        public static Type[] GetTypes(this Assembly asm)
        {
            var results = new List<Type>();
            foreach(var type in asm.DefinedTypes)
                results.Add(type.AsType());

            return results.ToArray();
        }
    }

    /// <summary>   Bitfield of flags for specifying BindingFlags. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    [Flags]
    public enum BindingFlags
    {
        /// <summary>   A binary constant representing the instance flag. </summary>
        Instance = 4,

        /// <summary>   A binary constant representing the static flag. </summary>
        Static = 8,

        /// <summary>   A binary constant representing the public flag. </summary>
        Public = 16,

        /// <summary>   A binary constant representing the non public flag. </summary>
        NonPublic = 32,

        /// <summary>   A binary constant representing the flatten hierarchy flag. </summary>
        FlattenHierarchy = 64,
    }

    /// <summary>   Bitfield of flags for specifying MemberTypes. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    [Flags]
    public enum MemberTypes
    {
        /// <summary>   Event queue for all listeners interested in Constructor events. </summary>
        Constructor = 1,

        /// <summary>   Event queue for all listeners interested in Event events. </summary>
        Event = 2,

        /// <summary>   A binary constant representing the field flag. </summary>
        Field = 4,

        /// <summary>   A binary constant representing the method flag. </summary>
        Method = 8,

        /// <summary>   A binary constant representing the property flag. </summary>
        Property = 16
    }
}

namespace System
{
    /// <summary>   Values that represent TypeCode. </summary>
    /// <remarks>   Sander.struijk, 14.05.2014. </remarks>
    public enum TypeCode
    {
        /// <summary>   An enum constant representing the empty option. </summary>
        Empty = 0,

        /// <summary>   An enum constant representing the object option. </summary>
        Object = 1,

        /// <summary>   An enum constant representing the database null option. </summary>
        DBNull = 2,

        /// <summary>   An enum constant representing the boolean option. </summary>
        Boolean = 3,

        /// <summary>   An enum constant representing the character option. </summary>
        Char = 4,

        /// <summary>   An enum constant representing the byte option. </summary>
        SByte = 5,

        /// <summary>   An enum constant representing the byte option. </summary>
        Byte = 6,

        /// <summary>   An enum constant representing the int 16 option. </summary>
        Int16 = 7,

        /// <summary>   An enum constant representing the int 16 option. </summary>
        UInt16 = 8,

        /// <summary>   An enum constant representing the int 32 option. </summary>
        Int32 = 9,

        /// <summary>   An enum constant representing the int 32 option. </summary>
        UInt32 = 10,

        /// <summary>   An enum constant representing the int 64 option. </summary>
        Int64 = 11,

        /// <summary>   An enum constant representing the int 64 option. </summary>
        UInt64 = 12,

        /// <summary>   An enum constant representing the single option. </summary>
        Single = 13,

        /// <summary>   An enum constant representing the double option. </summary>
        Double = 14,

        /// <summary>   An enum constant representing the decimal option. </summary>
        Decimal = 15,

        /// <summary>   An enum constant representing the date time option. </summary>
        DateTime = 16,

        /// <summary>   An enum constant representing the string option. </summary>
        String = 18,
    }
}
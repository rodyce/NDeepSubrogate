#region License

/*
 * Copyright 2017 Rodimiro Cerrato Espinal.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

 
using System;
using System.Reflection;
using Spring.Context.Support;
using Spring.Objects.Factory.Config;

namespace NDeepSubrogate.Spring
{
    internal static class ExtensionMethods
    {
        public static void RemoveSingleton(this IConfigurableObjectFactory objectFactory, string name)
        {
            objectFactory.GetType()
                .InvokeMember("RemoveSingleton",
                    BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder,
                    objectFactory, new object[] { name });
        }

        public static void ReplaceObjectDefinition(this AbstractApplicationContext applicationContext, string name,
            IObjectDefinition definition)
        {
            applicationContext.ObjectFactory.RemoveSingleton(name);
            applicationContext.RegisterObjectDefinition(name, definition);
        }
    }
}

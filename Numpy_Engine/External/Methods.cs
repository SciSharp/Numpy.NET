/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.Engine.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using np = Numpy;

namespace BH.Engine.Numpy
{
    public static partial class External
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<MethodInfo> Methods()
        {
            Dictionary<string, MethodInfo> methodsDictionary = new Dictionary<string, MethodInfo>();

            List<Type> typesToExplore = new List<Type>
            {
                typeof(np.np), // order is important since we skip duplicates. We prefer np.np static methods
                typeof(np.Dtype),
                typeof(np.DtypeExtensions),
                typeof(np.Models.Flags),
                typeof(np.Models.Matrix),
                typeof(np.Models.MemMapMode),
                typeof(np.Models.Shape),
                typeof(np.Models.Slice),
                typeof(np.NDarray),
                typeof(np.NDarray<>),
                typeof(np.PythonObject),
            };

            foreach(Type type in typesToExplore)
            {
                List<MethodInfo> typeMethods = type.NestedMethods();
                // NDarray and NDarray<> contain methods that are duplicated in the np.np class
                // We are not adding them
                for (int i = 0; i < typeMethods.Count; i++)
                {
                    MethodInfo methodInstance = typeMethods[i];
                    if (!methodsDictionary.ContainsKey(methodInstance.Name) && !methodInstance.DeclaringType.Namespace.Contains("System"))
                        methodsDictionary.Add(methodInstance.Name, methodInstance);
                }
            }

            List<MethodInfo> methods = methodsDictionary.Values.ToList();
            methods.Add(typeof(Query).GetMethod("Slice", new Type[] { typeof(np.NDarray) }));
            methods.Add(typeof(Query).GetMethod("Slice", new Type[] { typeof(np.NDarray<>) }));

            return methods.Where(x => x != null).ToList();
        }

        /***************************************************/
    }
}

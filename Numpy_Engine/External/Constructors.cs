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

using System;
using System.Collections.Generic;
using System.Reflection;
using np = Numpy;

namespace BH.Engine.Numpy
{
    public static partial class External
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<ConstructorInfo> Constructors()
        {
            List<ConstructorInfo> constructors = new List<ConstructorInfo>();

            List<Type> typesToExplore = new List<Type>
            {
                typeof(np.Dtype),
                typeof(np.DtypeExtensions),
                typeof(np.Models.Flags),
                typeof(np.Models.Matrix),
                typeof(np.Models.MemMapMode),
                typeof(np.Models.Shape),
                typeof(np.Models.Slice),
                //typeof(np.NDarray),
                //typeof(np.NDarray<>),
                typeof(np.np),
                typeof(np.PythonObject),
            };

            foreach (Type type in typesToExplore)
                constructors.AddRange(type.GetConstructors());

            return constructors;
        }

        /***************************************************/
    }
}

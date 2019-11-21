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

using np = Numpy;

namespace BH.Engine.Numpy
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static np.NDarray Slice(this np.NDarray array, string sliceNotation)
        {
            return array[sliceNotation];
        }

        /***************************************************/

        public static np.NDarray<T> Slice<T>(this np.NDarray<T> array, string sliceNotation)
        {
            return array[sliceNotation];
        }

        /***************************************************/

        public static np.NDarray Slice(this np.NDarray array, np.NDarray notation)
        {
            return array[notation];
        }

        /***************************************************/

        public static np.NDarray ISlice(this np.NDarray array, object[] slice)
        {
            return array[slice as dynamic];
        }
    }
}

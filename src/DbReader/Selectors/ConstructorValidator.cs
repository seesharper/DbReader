/*****************************************************************************   
    The MIT License (MIT)
    Copyright (c) 2014 bernhard.richter@gmail.com
    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:
    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
******************************************************************************
    DbReader version 1.0.0.1
    https://github.com/seesharper/DbReader
    http://twitter.com/bernhardrichter
******************************************************************************/
namespace DbReader.Selectors
{
    using System;
    using System.Reflection;
    using Extensions;

    /// <summary>
    /// A <see cref="IConstructorSelector"/> decorator that validates
    /// the selected <see cref="ConstructorInfo"/>.
    /// </summary>
    public class ConstructorValidator : IConstructorSelector
    {
        private readonly IConstructorSelector constructorSelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorValidator"/> class.
        /// </summary>
        /// <param name="constructorSelector">The target <see cref="IConstructorSelector"/>.</param>
        public ConstructorValidator(IConstructorSelector constructorSelector)
        {
            this.constructorSelector = constructorSelector;
        }

        /// <summary>
        /// Gets a <see cref="ConstructorInfo"/> from the given <paramref name="type."/>
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to get a <see cref="ConstructorInfo"/>.</param>
        /// <returns><see cref="ConstructorInfo"/></returns>
        public ConstructorInfo Execute(Type type)
        {                        
            Require.IsNotNull(type, "type");                        
            var constructor = constructorSelector.Execute(type);
            return Ensure.IsNotNull(constructor, ErrorMessages.ConstructorNotFound.FormatWith(type));            
        }
    }
}
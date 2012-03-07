﻿/*
 * Copyright 2012 WildCard, LLC
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Nxdb.Node;

namespace Nxdb.Persistence
{
    /// <summary>
    /// Stores and fetches the field or property to/from a child element of the container element. If more
    /// than one element with the given name exists, the first one will be used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PersistentElementAttribute : PersistentAttributeBase
    {
        public PersistentElementAttribute()
        {
        }

        public PersistentElementAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets or sets the name of the element to use or create. If unspecified, the name of
        /// the field or property will be used (as converted to a valid XML name).
        /// </summary>
        public string Name { get; set; }

        internal override void Inititalize(MemberInfo memberInfo)
        {
            base.Inititalize(memberInfo);

            if (String.IsNullOrEmpty(Name))
            {
                Name = XmlConvert.EncodeName(memberInfo.Name);
            }
            else
            {
                XmlConvert.VerifyName(Name);
            }
        }

        internal override string FetchValue(Element element)
        {
            Element child = element.Children.OfType<Element>().Where(e => e.Name.Equals(Name)).FirstOrDefault();
            return child == null ? null : child.Value;
        }

        internal override void StoreValue(Element element, string value)
        {
            Element child = element.Children.OfType<Element>().Where(e => e.Name.Equals(Name)).FirstOrDefault();
            if (child == null)
            {
                element.Append(String.Format("<{0}>{1}</{0}>", Name, value));
            }
            else
            {
                child.Value = value;
            }
        }
    }
}

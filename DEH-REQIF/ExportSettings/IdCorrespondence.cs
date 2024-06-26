﻿//  -------------------------------------------------------------------------------------------------
//  <copyright file="IdCorrespondence.cs" company="Starion Group S.A.">
// 
//    Copyright 2022-2024 Starion Group S.A.
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
// 
//  </copyright>
//  -------------------------------------------------------------------------------------------------

namespace DEHReqIF.ExportSettings
{
    using System;

    using CDP4Common.CommonData;

    /// <summary>
    /// A class that defines the properties and methods of a <see cref="IdCorrespondence"/> which is part of an <see cref="ExternalIdentifierMap"/> class
    /// </summary>
    public class IdCorrespondence
    {
        /// <summary>
        /// Gets or sets the <see cref="Guid"/> of the internal 10-25 <see cref="Thing"/>
        /// </summary>
        public Guid InternalThing { get; set; }

        /// <summary>
        /// Gets or sets the id of the external ReqIF object
        /// </summary>
        public string ExternalId { get; set; }
    }
}

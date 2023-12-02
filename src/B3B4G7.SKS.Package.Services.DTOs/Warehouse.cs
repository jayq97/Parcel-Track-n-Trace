/*
 * Parcel Logistics Service
 *
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.22.1
 * 
 * Generated by: https://openapi-generator.tech
 */

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using B3B4G7.SKS.Package.Services.DTOs.Converters;
using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.Services.DTOs
{ 
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    [ExcludeFromCodeCoverage]
    public partial class Warehouse : Hop
    {
        /// <summary>
        /// Gets or Sets Level
        /// </summary>
        [Required]
        [DataMember(Name="level", EmitDefaultValue=true)]
        public int Level { get; set; }

        /// <summary>
        /// Next hops after this warehouse (warehouses or trucks).
        /// </summary>
        /// <value>Next hops after this warehouse (warehouses or trucks).</value>
        [Required]
        [DataMember(Name="nextHops", EmitDefaultValue=false)]
        public List<WarehouseNextHops> NextHops { get; set; }

    }
}
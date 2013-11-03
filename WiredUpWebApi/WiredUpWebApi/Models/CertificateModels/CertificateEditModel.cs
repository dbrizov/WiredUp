﻿using System;
using System.Linq;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.CertificateModels
{
    [DataContract]
    public class CertificateEditModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}
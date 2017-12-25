using System;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using CoreLocation;

namespace GSios.Models               
{
    public class Location
    {
		[JsonProperty(PropertyName = "Id")]
		public string Id { get; set; }

        [JsonProperty(PropertyName = "Coords")]
        public CLLocation Coords { get; set; }

		[Version]
		public string Version { get; set; }

    }
}

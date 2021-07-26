using System;

namespace RoboAdvisorApi.Models
{
    public class TefasModel
    {
        public float open { get; set; }
        public float close { get; set; }
        public float high { get; set; }
        public float low { get; set; }
        public float amount { get; set; }
        public float volume { get; set; }
        public string date { get; set; }
    }
}
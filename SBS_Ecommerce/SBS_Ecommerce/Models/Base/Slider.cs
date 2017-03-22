using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.Base
{
    public class Slider
    {
        public List<Picture> LstPicture { get; set; }
    }

    public class Picture
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Comment { get; set; }
    }
}
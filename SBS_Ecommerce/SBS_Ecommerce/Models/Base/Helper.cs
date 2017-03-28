﻿using Newtonsoft.Json;
using SBS_Ecommerce.Framework.Configurations;
using SBS_Ecommerce.Framework.Utilities;
using SBS_Ecommerce.Models.DTOs;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace SBS_Ecommerce.Models.Base
{
    public class Helper
    {
        //public List<Category> GetCategory()
        //{
        //    //Task<String> response = httpClient.GetStringAsync(uri);
        //    string value = RequestUtil.SendRequest(SBSConstants.GetListCategory);
        //    var json = JsonConvert.DeserializeObject<CategoryDTO>(value);
        //    return json.Items;
        //}

        /// <summary>
        /// Gets the products.
        /// </summary>
        /// <returns></returns>
        public List<Product> GetProducts()
        {
            int cId = 1;
            int pNo = 1;
            int pLength = 10;
            string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetListProduct, cId, pNo, pLength));
            var json = JsonConvert.DeserializeObject<ProductListDTO>(value);
            return json.Items;
        }

        /// <summary>
        /// Gets the best seller products.
        /// </summary>
        /// <returns></returns>
        public List<Product> GetBestSellerProducts()
        {
            int cId = 1;
            int pNo = 1;
            int pLength = 10;
            string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetBestSellerProduct, cId, pNo, pLength));
            var json = JsonConvert.DeserializeObject<ProductListDTO>(value);
            return json.Items;
        }

        public void Serialize(string filename, List<Theme> lstTheme)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<Theme>));
            TextWriter writer = new StreamWriter(filename);
            ser.Serialize(writer, lstTheme);
            writer.Close();
        }

        public void SerializeLayout(string filename, List<Layout> lstLayout)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<Layout>));
            TextWriter writer = new StreamWriter(filename);
            ser.Serialize(writer, lstLayout);
            writer.Close();
        }

        public List<Theme> DeSerialize(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Theme>));
            StreamReader reader = new StreamReader(filename);
            var cars = (List<Theme>)serializer.Deserialize(reader);
            reader.Close();
            return cars;
        }

        public List<Layout> DeSerializeLayout(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Layout>));
            StreamReader reader = new StreamReader(filename);
            var cars = (List<Layout>)serializer.Deserialize(reader);
            reader.Close();
            return cars;
        }

        public void SerializeSlider(string filename, Slider slider)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Slider));
            TextWriter writer = new StreamWriter(filename);
            ser.Serialize(writer, slider);
            writer.Close();
        }

        public Slider DeSerializeSlider(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Slider));
            StreamReader reader = new StreamReader(filename);
            var cars = (Slider)serializer.Deserialize(reader);
            reader.Close();
            return cars;
        }

        public void SerializeMenu(string filename, List<Menu> menu)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<Menu>));
            TextWriter writer = new StreamWriter(filename);
            ser.Serialize(writer, menu);
            writer.Close();
        }

        public List<Menu> DeSerializeMenu(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Menu>));
            StreamReader reader = new StreamReader(filename);
            var cars = (List<Menu>)serializer.Deserialize(reader);
            reader.Close();
            return cars;
        }


        public void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
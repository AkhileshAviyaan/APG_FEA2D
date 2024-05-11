using APG_FEA2D.Views;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FEA2D.Structures
{
    public class StructureIO
    {
        public StructureIO() { }
        public void OnCreate()
        {

        }
        public void OnUpgrade()
        {

        }
        public Structure OpenCache()
        {
            var abc=CustomSkiaPage.FileCache;
            return null;
        }
        public void SaveAsCache(Structure structure)
        {

        }
        public void WriteAttribute(string key, string value)
        {
        }
        public void GetAttribute(string key, string value)
        {
        }
        public Structure Open(string filePath)
        {
            return null;
        }
        public void Save(Structure structure)
        {

        }
    }
}

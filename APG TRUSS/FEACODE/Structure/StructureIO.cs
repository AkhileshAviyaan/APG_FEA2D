using APG_FEA2D.Views;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FEA2D.Elements;
using MathNet.Numerics.Distributions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using Tmds.DBus.Protocol;
using System.Runtime.ExceptionServices;
using CSparse.Ordering;
using System.Formats.Tar;
using FEA2D.CrossSections;
using FEA2D.Materials;
using FEA2D.Loads;
using DynamicData;

namespace FEA2D.Structures
{
    public class StructureIO
    {
        public readonly string path;
        SqliteConnection db;
        SqliteCommand command;

        public StructureIO()
        {
            this.path = Path.Combine(Path.GetTempPath(), "APG_FEA2D");
            Directory.CreateDirectory(path);
            path = Path.Combine(path, "structure.db");
            db = new SqliteConnection($"Data Source = {path};Foreign Keys=True;");
            db.Open();
            command = db.CreateCommand();
            OnCreate();
        }

        public void OnCreate()
        {
            bool abc = Convert.ToBoolean(1);
            string material = "CREATE TABLE  if not exists material(id integer primary key autoincrement, e number,g number, u number, label text, alpha number, gamma number);";
            string section = "CREATE TABLE  if not exists section(id integer primary key autoincrement, a number, ax number, ay text, ix number,iy number,j number, hmax number, wmax number); ";
            string nodes = "CREATE TABLE if not exists nodes(id integer primary key autoincrement, x number, y number, label text); ";
            string frame = "CREATE TABLE  if not exists frame(id integer primary key autoincrement, start_nodeX integer,start_nodeY integer,start_nodelabel text, end_nodeX integer,end_nodeY integer,end_nodelabel text, label text); ";
            string supports = "CREATE TABLE  if not exists supports(id integer primary key autoincrement, node_label text, restrain_x number,restrain_y number,restrain_R number); ";
            string nodalloads = "CREATE TABLE  if not exists nodalloads(id integer primary key autoincrement, node_label text, magnitude number, angle number); ";
            command = new SqliteCommand(material, db);
            command.ExecuteNonQuery();
            command = new SqliteCommand(section, db);
            command.ExecuteNonQuery();
            command = new SqliteCommand(nodes, db);
            command.ExecuteNonQuery();
            command = new SqliteCommand(frame, db);
            command.ExecuteNonQuery();
            command = new SqliteCommand(supports, db);
            command.ExecuteNonQuery();
            command = new SqliteCommand(nodalloads, db);
            command.ExecuteNonQuery();

        }
        public void OnUpgrade()
        {
            string material = "DROP TABLE  if exists material;";
            string section = "DROP TABLE  if exists section; ";
            string nodes = "DROP TABLE if exists nodes; ";
            string frame = "DROP TABLE  if exists frame; ";
            string supports = "DROP TABLE  if exists supports; ";
            string pointloads = "DROP TABLE  if exists nodalloads; ";
            command = new SqliteCommand(material, db);
            command.ExecuteNonQuery();
            command = new SqliteCommand(section, db);
            command.ExecuteNonQuery();
            command = new SqliteCommand(nodes, db);
            command.ExecuteNonQuery();
            command = new SqliteCommand(frame, db);
            command.ExecuteNonQuery();
            command = new SqliteCommand(supports, db);
            command.ExecuteNonQuery();
            command = new SqliteCommand(pointloads, db);
            command.ExecuteNonQuery();
            OnCreate();
        }
        public Structure OpenCache()
        {
            var abc = CustomSkiaPage.FileCache;
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
            //clear tables
            string material = "DELETE FROM material;";
            string section = "DELETE FROM section; ";
            string nodes = "DELETE FROM nodes; ";
            string frame = "DELETE FROM frame; ";
            string supports = "DELETE FROM supports; ";
            string pointloads = "DELETE FROM nodalloads; ";
            command = new SqliteCommand(material, db);
            command.ExecuteNonQuery();
            command = new SqliteCommand(section, db);
            command.ExecuteNonQuery();
            command = new SqliteCommand(nodes, db);
            command.ExecuteNonQuery();
            command = new SqliteCommand(frame, db);
            command.ExecuteNonQuery();
            command = new SqliteCommand(supports, db);
            command.ExecuteNonQuery();
            command = new SqliteCommand(pointloads, db);
            command.ExecuteNonQuery();

            //Insert Material and section
            using (SqliteTransaction insertTransaction = db.BeginTransaction())
            {
                foreach (FrameElement2D element in structure.Elements)
                {
                    command = db.CreateCommand();
                    command.Transaction = insertTransaction;
                    GenericIsotropicMaterial materialproperty = (GenericIsotropicMaterial)element.CrossSection.Material;
                    command.CommandText = @"INSERT INTO material(e,g,u,label,alpha,gamma) VALUES (@e,@g,@u,@label,@alpha,@gama)";
                    command.Parameters.AddWithValue("@e", materialproperty.E);
                    command.Parameters.AddWithValue("@g", materialproperty.G);
                    command.Parameters.AddWithValue("@u", materialproperty.U);
                    command.Parameters.AddWithValue("@label", materialproperty.Label);
                    command.Parameters.AddWithValue("@alpha", materialproperty.Alpha);
                    command.Parameters.AddWithValue("@gama", materialproperty.Gama);
                    command.ExecuteNonQuery();

                    IFrame2DSection crosssection = element.CrossSection;
                    command.CommandText = @"INSERT INTO section(a,ax,ay,ix,iy,j,hmax,wmax) VALUES (@a,@ax,@ay,@ix,@iy,@j,@hmax,@wmax)";
                    command.Parameters.AddWithValue("@a", crosssection.A);
                    command.Parameters.AddWithValue("@ax", crosssection.Ax);
                    command.Parameters.AddWithValue("@ay", crosssection.Ay);
                    command.Parameters.AddWithValue("@ix", crosssection.Ix);
                    command.Parameters.AddWithValue("@iy", crosssection.Iy);
                    command.Parameters.AddWithValue("@j", crosssection.J);
                    command.Parameters.AddWithValue("@hmax", crosssection.MaxHeight);
                    command.Parameters.AddWithValue("@wmax", crosssection.MaxWidth);
                    command.ExecuteNonQuery();
                }
            }
            //Insert nodes, nodal load and nodal supports

            using (SqliteTransaction insertTransaction = db.BeginTransaction())
            {
                foreach (Node2D node in structure.Nodes)
                {
                    command = db.CreateCommand();
                    command.Transaction = insertTransaction;
                    command.CommandText = @"INSERT INTO nodes(x,y,label) VALUES (@x,@y,@label)";
                    command.Parameters.AddWithValue("@x", node.X);
                    command.Parameters.AddWithValue("@y", node.Y);
                    command.Parameters.AddWithValue("@label", node.Label);
                    command.ExecuteNonQuery();
                    if (!node.IsFree)
                    {
                        foreach (NodalLoad nl in node.NodalLoads)
                        {
                            command.CommandText = @"INSERT INTO nodalloads(node_label,magnitude,angle) VALUES (@node_label,@magnitude,@angle)";
                            command.Parameters.AddWithValue("@node_label", node.Label);
                            command.Parameters.AddWithValue(@"magnitude", nl.F);
                            command.Parameters.AddWithValue(@"angle", node.RotaionAngle);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            //Insert frame
            using (SqliteTransaction insertTransaction = db.BeginTransaction())
            {
                foreach (FrameElement2D frameElement in structure.Elements)
                {
                    command = db.CreateCommand();
                    command.Transaction = insertTransaction;
                    command.CommandText = @"INSERT INTO frame(start_nodeX,start_nodeY,start_nodelabel,end_nodeX,end_nodeY,end_nodelabel,label) VALUES (@start_nodeX,@start_nodeY,@start_nodelabel,@end_nodeX,@end_nodeY,@end_nodelabel,@label)";
                    command.Parameters.AddWithValue("@start_nodeX", frameElement.StartNode.x);
                    command.Parameters.AddWithValue("@start_nodeY", frameElement.StartNode.y);
                    command.Parameters.AddWithValue("@start_nodelabel", frameElement.StartNode.Label);
                    command.Parameters.AddWithValue("@end_nodeX", frameElement.EndNode.x);
                    command.Parameters.AddWithValue("@end_nodeX", frameElement.EndNode.y);
                    command.Parameters.AddWithValue("@end_nodeX", frameElement.EndNode.Label);
                    command.Parameters.AddWithValue("@label", frameElement.Label);
                    command.ExecuteNonQuery();

                }
            }

        }
    }
}

using APG_FEA2D.Views;
using Microsoft.Data.Sqlite;
using System;
using System.IO;
using FEA2D.Elements;
using FEA2D.CrossSections;
using FEA2D.Materials;
using FEA2D.Loads;
using System.Collections.Generic;
using System.Linq;
namespace FEA2D.Structures
{
	public class StructureIO
	{
		public readonly string path;
		SqliteConnection db;
		public StructureIO()
		{
			this.path = Path.Combine(Path.GetTempPath(), "APG_FEA2D");
			Directory.CreateDirectory(path);
			path = Path.Combine(path, "structure.db");
			db = new SqliteConnection($"Data Source = {path};Foreign Keys=True;");
			db.Open();
			//OnDelete();
			OnCreate();
		}
		public void OnDelete()
		{
			using (SqliteCommand command = db.CreateCommand())
			{
				bool abc = Convert.ToBoolean(1);
				string material = "DROP TABLE IF EXISTS  material;";
				string section = "DROP TABLE IF EXISTS   section; ";
				string nodes = "DROP TABLE IF EXISTS  nodes; ";
				string frame = "DROP TABLE IF EXISTS   frame; ";
				string frameTrapezoidalLoad = "DROP TABLE IF EXISTS   frameTrapezoidalLoad; ";
				string supports = "DROP TABLE IF EXISTS  supports; ";
				string nodalloads = "DROP TABLE IF EXISTS  nodalloads; ";
				command.CommandText = material;
				command.ExecuteNonQuery();
				command.CommandText = section;
				command.ExecuteNonQuery();
				command.CommandText = nodes;
				command.ExecuteNonQuery();
				command.CommandText = frame;
				command.ExecuteNonQuery();
				command.CommandText = frameTrapezoidalLoad;
				command.ExecuteNonQuery();
				command.CommandText = supports;
				command.ExecuteNonQuery();
				command.CommandText = nodalloads;
				command.ExecuteNonQuery();
			}

		}
		public void OnCreate()
		{
			using (SqliteCommand command = db.CreateCommand())
			{
				bool abc = Convert.ToBoolean(1);
				string material = "CREATE TABLE  if not exists material(id integer primary key autoincrement,element_label text, e number, u number, label text, alpha number, gamma number);";
				string section = "CREATE TABLE  if not exists section(id integer primary key autoincrement,element_label text, a number, ax number, ay text, ix number,iy number,j number, hmax number, wmax number); ";
				string nodes = "CREATE TABLE if not exists nodes(id integer primary key autoincrement, x number, y number, label text); ";
				string frame = "CREATE TABLE  if not exists frame(id integer primary key autoincrement, start_nodeX integer,start_nodeY integer,start_nodelabel text, end_nodeX integer,end_nodeY integer,end_nodelabel text, label text); ";
				string frameTrapezoidalLoad = "CREATE TABLE  if not exists frameTrapezoidalLoad(id integer primary key autoincrement, framelabel text,wy1 number, wy2 number); ";
				string supports = "CREATE TABLE  if not exists supports(id integer primary key autoincrement, node_label text, restrain_x number,restrain_y number,restrain_R number); ";
				string nodalloads = "CREATE TABLE  if not exists nodalloads(id integer primary key autoincrement, node_label text, magnitude number, angle number); ";
				command.CommandText = material;
				command.ExecuteNonQuery();
				command.CommandText = section;
				command.ExecuteNonQuery();
				command.CommandText = nodes;
				command.ExecuteNonQuery();
				command.CommandText = frame;
				command.ExecuteNonQuery();
				command.CommandText = frameTrapezoidalLoad;
				command.ExecuteNonQuery();
				command.CommandText = supports;
				command.ExecuteNonQuery();
				command.CommandText = nodalloads;
				command.ExecuteNonQuery();
			}

		}
		public void OnUpgrade()
		{
			using (SqliteCommand command = db.CreateCommand())
			{
				string material = "DROP TABLE  if exists material;";
				string section = "DROP TABLE  if exists section; ";
				string nodes = "DROP TABLE if exists nodes; ";
				string frame = "DROP TABLE  if exists frame; ";
				string frameTrapezoidalLoad = "DROP TABLE  if exists frameTrapezoidalLoad; ";
				string supports = "DROP TABLE  if exists supports; ";
				string nodalloads = "DROP TABLE  if exists nodalloads; ";
				command.CommandText = material;
				command.ExecuteNonQuery();
				command.CommandText = section;
				command.ExecuteNonQuery();
				command.CommandText = nodes;
				command.ExecuteNonQuery();
				command.CommandText = frame;
				command.ExecuteNonQuery();
				command.CommandText = frameTrapezoidalLoad;
				command.ExecuteNonQuery();
				command.CommandText = supports;
				command.ExecuteNonQuery();
				command.CommandText = nodalloads;
				command.ExecuteNonQuery();
				OnCreate();
			}
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
		public Structure Open()
		{
			Structure structure = new();
			LoadCase loadCase = new LoadCase("live", LoadCaseType.Live);
			string material = "SELECT * FROM material;";
			string section = "SELECT * FROM section; ";
			string frame = "SELECT * FROM frame; ";
			string frameTrapezoidalLoad = "SELECT * FROM frameTrapezoidalLoad; ";
			string nodes = "SELECT * FROM nodes; ";
			string supports = "SELECT * FROM supports; ";
			string nodalloads = "SELECT * FROM nodalloads; ";
			using (SqliteCommand command = db.CreateCommand())
			{
				command.CommandText = nodes;
				List<Node2D> nodeList = [];
				using (SqliteDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						Node2D nod = new();
						nod.X = Convert.ToDouble(reader["x"]);
						nod.Y = Convert.ToDouble(reader["y"]);
						nod.Label = (string)reader["label"];
						nodeList.Add(nod);
					}
				}


				command.CommandText = supports;
				using (SqliteDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						NodalSupport nodSupport = new();
						nodSupport.Ux = Convert.ToBoolean(reader["restrain_x"]);
						nodSupport.Uy = Convert.ToBoolean(reader["restrain_y"]);
						nodSupport.Rz = Convert.ToBoolean(reader["restrain_R"]);
						var node_label = (string)reader["node_label"];
						Node2D node = nodeList.Where(n => n.Label == node_label).FirstOrDefault();
						node.Support = nodSupport;
					}
				}

				command.CommandText = nodalloads;
				using (SqliteDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						NodalLoad nodload = new(Convert.ToDouble(reader["angle"]) * 180 / Math.PI, Convert.ToDouble(reader["magnitude"]));
						var node_label = (string)reader["node_label"];
						Node2D node = nodeList.Where(n => n.Label == node_label).FirstOrDefault();
						nodload.LoadCase = loadCase;
						node.NodalLoads.Add(nodload);
					}
				}

				command.CommandText = material;
				List<GenericIsotropicMaterial> matList = [];
				using (SqliteDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						GenericIsotropicMaterial mat = new();
						mat.E = Convert.ToDouble(reader["e"]);
						mat.U = Convert.ToDouble(reader["u"]);
						mat.Label = (string)reader["label"];
						mat.Alpha = Convert.ToDouble(reader["alpha"]);
						mat.Gama = Convert.ToDouble(reader["gamma"]);
						mat.MaterialType = MaterialType.Steel;
						matList.Add(mat);
					}
				}

				command.CommandText = section;
				List<Generic2DSection> secList = [];
				using (SqliteDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						Generic2DSection sec = new();
						sec.A = Convert.ToDouble(reader["a"]);
						sec.Ax = Convert.ToDouble(reader["ax"]);
						sec.Ay = Convert.ToDouble(reader["ay"]);
						sec.Ix = Convert.ToDouble(reader["ix"]);
						sec.Iy = Convert.ToDouble(reader["iy"]);
						sec.J = Convert.ToDouble(reader["j"]);
						sec.MaxHeight = Convert.ToDouble(reader["hmax"]);
						sec.MaxWidth = Convert.ToDouble(reader["wmax"]);
						sec.Material = matList[secList.Count];
						secList.Add(sec);
					}
				}
				command.CommandText = frame;
				List<FrameElement2D> frameList = [];
				using (SqliteDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						FrameElement2D fra = new(new Node2D(), new Node2D(), "");

						fra.StartNode = nodeList.Where(n => n.Label == (string)reader["start_nodelabel"]).FirstOrDefault();
						fra.EndNode = nodeList.Where(n => n.Label == (string)reader["end_nodelabel"]).FirstOrDefault();
						fra.Label = (string)reader["label"];
						fra.CrossSection = secList[frameList.Count];
						frameList.Add(fra);
					}
				}
				command.CommandText = frameTrapezoidalLoad;
				List<FrameTrapezoidalLoad> frameTrapezoidalLoadList = [];
				using (SqliteDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						FrameElement2D fra = frameList.Where(n => n.Label == (string)reader["framelabel"]).FirstOrDefault();

						FrameTrapezoidalLoad TrapezoidalLoad = new(0, 0, Convert.ToDouble(reader["wy1"]), Convert.ToDouble(reader["wy2"]), LoadDirection.Local,loadCase);
						fra.Loads.Add(TrapezoidalLoad);
					}
				}
				structure.AddNode(nodeList);
				structure.AddElement(frameList);
				structure.LoadCasesToRun.Add(loadCase);
				structure.LinearMesher.NumberSegements = 20;
				return structure;
			}
			return null;
		}
		public void Save(Structure structure)
		{
			OnUpgrade();
			//clear tables
			using (SqliteCommand command = db.CreateCommand())
			{
				string material = "DELETE FROM material;";
				string section = "DELETE FROM section; ";
				string nodes = "DELETE FROM nodes; ";
				string frame = "DELETE FROM frame; ";
				string frameTrapezoidalLoad = "DELETE FROM frameTrapezoidalLoad; ";
				string supports = "DELETE FROM supports; ";
				string nodalloads = "DELETE FROM nodalloads; ";
				command.CommandText = material;
				command.ExecuteNonQuery();
				command.CommandText = section;
				command.ExecuteNonQuery();
				command.CommandText = nodes;
				command.ExecuteNonQuery();
				command.CommandText = frame;
				command.ExecuteNonQuery();
				command.CommandText = frameTrapezoidalLoad;
				command.ExecuteNonQuery();
				command.CommandText = supports;
				command.ExecuteNonQuery();
				command.CommandText = nodalloads;
				command.ExecuteNonQuery();
			}
			//Insert nodes, nodal load and nodal supports
			foreach (Node2D node in structure.Nodes)
			{
				using (SqliteTransaction insertTransaction = db.BeginTransaction())
				{
					using (SqliteCommand command = db.CreateCommand())
					{
						command.Transaction = insertTransaction;
						command.CommandText = @"INSERT INTO nodes(x,y,label) VALUES (@x,@y,@label)";
						command.Parameters.AddWithValue("@x", node.X);
						command.Parameters.AddWithValue("@y", node.Y);
						command.Parameters.AddWithValue("@label", node.Label);
						command.ExecuteNonQuery();
					}

					insertTransaction.Commit();
				}
				foreach (NodalLoad nl in node.NodalLoads)
				{
					using (SqliteTransaction insertTransaction = db.BeginTransaction())
					{
						using (SqliteCommand command = db.CreateCommand())
						{
							command.Transaction = insertTransaction;
							command.CommandText = @"INSERT INTO nodalloads(node_label,magnitude,angle) VALUES (@node_label,@magnitude,@angle)";
							command.Parameters.AddWithValue("@node_label", node.Label);
							command.Parameters.AddWithValue(@"magnitude", nl.F);
							command.Parameters.AddWithValue(@"angle", nl.Angle);
							command.ExecuteNonQuery();
						}
						insertTransaction.Commit();
					}
				}
				if (!node.IsFree)
				{
					using (SqliteTransaction insertTransaction = db.BeginTransaction())
					{
						using (SqliteCommand command = db.CreateCommand())
						{
							command.Transaction = insertTransaction;
							command.CommandText = @"INSERT INTO supports(node_label,restrain_x,restrain_y,restrain_R) VALUES (@node_label,@restrain_x,@restrain_y,@restrain_R)";
							command.Parameters.AddWithValue("@node_label", node.Label);
							command.Parameters.AddWithValue("@restrain_x", node.Support.Ux);
							command.Parameters.AddWithValue("@restrain_y", node.Support.Uy);
							command.Parameters.AddWithValue("@restrain_R", node.Support.Rz);
							command.ExecuteNonQuery();
						}

						insertTransaction.Commit();
					}
				}
			}

			//Insert Material and section
			foreach (FrameElement2D element in structure.Elements)
			{
				using (SqliteTransaction insertTransaction = db.BeginTransaction())
				{
					GenericIsotropicMaterial materialproperty = (GenericIsotropicMaterial)element.CrossSection.Material;

					// Insert material data
					using (SqliteCommand command = db.CreateCommand())
					{
						command.Transaction = insertTransaction;
						command.CommandText = @"INSERT INTO material(element_label,e,u,label,alpha,gamma) VALUES (@element_label,@e,@u,@label,@alpha,@gamma)";
						command.Parameters.AddWithValue("@element_label", element.Label);
						command.Parameters.AddWithValue("@e", materialproperty.E);
						command.Parameters.AddWithValue("@u", materialproperty.U);
						command.Parameters.AddWithValue("@label", materialproperty.Label);
						command.Parameters.AddWithValue("@alpha", materialproperty.Alpha);
						command.Parameters.AddWithValue("@gamma", materialproperty.Gama);
						command.ExecuteNonQuery();
					}

					IFrame2DSection crosssection = element.CrossSection;

					// Insert section data
					using (SqliteCommand command = db.CreateCommand())
					{
						command.Transaction = insertTransaction;
						command.CommandText = @"INSERT INTO section(element_label,a,ax,ay,ix,iy,j,hmax,wmax) VALUES (@element_label,@a,@ax,@ay,@ix,@iy,@j,@hmax,@wmax)";
						command.Parameters.AddWithValue("@element_label", element.Label);
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
					insertTransaction.Commit();
				}
			}

			// Insert frame
			using (SqliteTransaction insertTransaction = db.BeginTransaction())
			{
				foreach (FrameElement2D frameElement in structure.Elements)
				{
					using (SqliteCommand command = db.CreateCommand())
					{
						command.Transaction = insertTransaction;
						command.CommandText = @"INSERT INTO frame(start_nodeX,start_nodeY,start_nodelabel,end_nodeX,end_nodeY,end_nodelabel,label) VALUES (@start_nodeX,@start_nodeY,@start_nodelabel,@end_nodeX,@end_nodeY,@end_nodelabel,@label)";
						command.Parameters.AddWithValue("@start_nodeX", frameElement.StartNode.X);
						command.Parameters.AddWithValue("@start_nodeY", frameElement.StartNode.Y);
						command.Parameters.AddWithValue("@start_nodelabel", frameElement.StartNode.Label);
						command.Parameters.AddWithValue("@end_nodeX", frameElement.EndNode.X);
						command.Parameters.AddWithValue("@end_nodeY", frameElement.EndNode.Y);
						command.Parameters.AddWithValue("@end_nodelabel", frameElement.EndNode.Label);
						command.Parameters.AddWithValue("@label", frameElement.Label);
						command.ExecuteNonQuery();
					}
				}
				insertTransaction.Commit();
			}
			// Insert frameTrapezoidalLoad
			using (SqliteTransaction insertTransaction = db.BeginTransaction())
			{
				foreach (FrameElement2D frameElement in structure.Elements)
				{
					foreach (FrameTrapezoidalLoad load in frameElement.Loads)
					{
						using (SqliteCommand command = db.CreateCommand())
						{
							command.Transaction = insertTransaction;
							command.CommandText = @"INSERT INTO frameTrapezoidalLoad(framelabel,wy1,wy2) VALUES (@framelabel,@wy1,@wy2)";
							command.Parameters.AddWithValue("@framelabel", frameElement.Label);
							command.Parameters.AddWithValue("@wy1", load.Wy1);
							command.Parameters.AddWithValue("@wy2", load.Wy2);
							command.ExecuteNonQuery();
						}
					}
				}
				insertTransaction.Commit();
			}

		}
	}
}

/*
 * Created by SharpDevelop.
 * User: Windows 10
 * Date: 5/23/2022
 * Time: 1:25 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Text;
using System.Data.OleDb;
using MySql.Data.MySqlClient;

namespace Tugas_PV2
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		string connectionSQL = "server = localhost; database = tugas_pv2; uid = root; password = ;";
		Bitmap img;
			string pathimg;
			//string SourceFilePath;
			//string inputid;
			//int intinputid;
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			readData();
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		void BtnopenuploadClick(object sender, EventArgs e)
		{
			if(DialogResult.OK ==openFileDialog1.ShowDialog())
			{
				img = new Bitmap(openFileDialog1.FileName);
				pictureBox2.Image=img;
				pathimg=openFileDialog1.FileName.ToString();
			}
		}
		
		void BtnsaveuploadClick(object sender, EventArgs e)
		{
			File2SqlBlob(pathimg);
			readData();
		}
		
		private void File2SqlBlob(string SourceFilePath)
		{
			try
			{
				MySqlConnection koneksi = new MySqlConnection(connectionSQL);
				MySqlCommand cmd = new MySqlCommand("INSERT into tabelgambar(id,datafile) values ("+incrementValueId()+",?Picture)",koneksi);
				System.IO.FileStream fs = new System.IO.FileStream(SourceFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
				Byte[] b = new Byte[fs.Length];
				fs.Read(b, 0, b.Length);
				fs.Close();
				MySqlParameter P = new MySqlParameter("?Picture", MySqlDbType.LongBlob, b.Length, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, b);
				cmd.Parameters.Add(P);
				koneksi.Open();
				if(cmd.ExecuteNonQuery() == 1)
					MessageBox.Show("Gambar berhasil disimpan ke database");
				
				koneksi.Close();
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.Message);
			}
			readData();
		}
		
		void BtnopendownloadClick(object sender, EventArgs e)
		{
			try
			{
				MySqlConnection koneksi = new MySqlConnection(connectionSQL);
				koneksi.Open();
				MySqlDataAdapter dataAdapter = new MySqlDataAdapter(new MySqlCommand("SELECT tugas_pv2 from tabelgambar WHERE id ="+Input_id.Text, koneksi));
				DataSet dataSet = new DataSet();
				dataAdapter.Fill(dataSet);
				if(dataSet.Tables[0].Rows.Count == 1)
				{
					Byte[] data = new Byte[0];
					data = (Byte[])(dataSet.Tables[0].Rows[0]["datafile"]);
					MemoryStream mem = new MemoryStream(data);
					pictureBox1.Image = Image.FromStream(mem);
				}
					koneksi.Close();
			}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
		}
		
		void BtnsavedownloadClick(object sender, EventArgs e)
		{
			DialogResult dr = saveFileDialog1.ShowDialog();
			if(dr == DialogResult.OK)
			{
				SaveFileDialog sfd = new SaveFileDialog();
				sfd.Filter = "Images|.png;.bmp;*.jpg";
				ImageFormat format = ImageFormat.Png;
				if(sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					string ext = System.IO.Path.GetExtension(sfd.FileName);
					switch(ext)
					{
						case ".jpg":
									format = ImageFormat.Jpeg;
									break;
						case ".bmp":
									format = ImageFormat.Bmp;
									break;
					}
					pictureBox2.Image.Save(sfd.FileName, format);
				}
			}
		}

		private int incrementValueId()
		{
			//int temp = 0;
			try
			{
				MySqlConnection db = new MySqlConnection(connectionSQL);
				db.Open();
				MySqlCommand dbcmd = db.CreateCommand();
				string sql = "SELECT max(id) from tabelgambar";
				dbcmd.CommandText = sql;
				MySqlDataReader reader = dbcmd.ExecuteReader();
				while(reader.Read())
				{
					if(!reader.IsDBNull(0))
					{
						return Convert.ToInt16(reader.GetString(0))+1;
					}
				}
				return 0;
			}
			catch(MySqlException error)
			{
				MessageBox.Show(error.ToString());
				return 0;
			}
		}
		
		public void readData()
		{
			MySqlConnection koneksi = new MySqlConnection(connectionSQL);
			koneksi.Open();
			DataTable dt = new DataTable();
			MySqlDataAdapter da = new MySqlDataAdapter("SELECT * from tabelgambar", koneksi);
			da.Fill(dt);
			dataGridView1.DataSource = dt.DefaultView;
			koneksi.Close();
		}
	}
}

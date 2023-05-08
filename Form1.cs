using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Data.SqlClient;
using OfficeOpenXml;

namespace ABCtest
{
    public partial class Form1 : Form
    {        
        public Form1()
        {
            InitializeComponent();
            txtPassword.Text = "";
            txtPassword.PasswordChar = '*';           
        }
        private void btnGir_Click(object sender, EventArgs e)
        {
            #region Excelden veri okuma kodları

            string path = @"C:\XYZ\config.xlsx"; // config excelinden veri okumak için
            FileInfo fileInfo = new FileInfo(path);
            ExcelPackage package = new ExcelPackage(fileInfo);
            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
            string Username = worksheet.Cells[2, 2].Value.ToString();
            string Password = worksheet.Cells[3, 2].Value.ToString();

            #endregion

            #region Şifre Kontrolü

            bool DogruSifre = true;
            while (DogruSifre)
            {
                if (txtUsername.Text == Username && txtPassword.Text == Password)
                {
                    this.Close();
                    DogruSifre = false;
                    //this.Dispose();
                }
                else
                {
                    MessageBox.Show("YANLIŞ KULLANICI ADI VE ŞİFRE TEKRAR DENEYİNİZ");
                    txtPassword.Text = "";
                    txtUsername.Text = "";
                    txtUsername.Focus();
                    break;
                }
            }
            #endregion





        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }



}

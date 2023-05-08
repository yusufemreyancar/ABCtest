using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.IO;

namespace ABCtest
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());


            #region Excelden veri okuma kodları

            string path = @"C:\XYZ\config.xlsx"; // config excelinden veri okumak için
            FileInfo fileInfo = new FileInfo(path);
            ExcelPackage package = new ExcelPackage(fileInfo);
            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
            string JsonAdress = worksheet.Cells[12, 2].Value.ToString();

            #endregion
                                   

            #region Dataları Json lambda ile stoğu 100 den büyük olanları filtreleyip bunların stock, brand ve category bilgileri getirir

            var data = HttpHelper.GetDataFromApi<Root>(JsonAdress).Result;//json api den verileri data değişkenine atar
            List<Product> b = new List<Product>(); // product sınıfından b adında bir nesne oluşturur


            foreach (Product a in data.Products)
            {
                b.Add(a); // data içinde dönerek b adında ki product nesnesine ekler
            }
            var filteredProducts = b.Where(p => p.Stock > 100 && !string.IsNullOrEmpty(p.Brand) && !string.IsNullOrEmpty(p.Category))
        .ToList(); // linq lamda ile stock adedi 100 den fazla olanları filteredProducts değişkenine alır

            T_ABC newRecord = new T_ABC(); // database e kayıt için Entity framework ün database first metodunu oluşturduğu
                                           // newrecord nesnesi T_ABC sınıfından oluşturulur
            #endregion

            #region Filtrelenmiş dataları username ve zaman damgası ile Mssql database e kaydetme süreci

            foreach (Product C in filteredProducts) // newrecord nesnesine filteredProducts tan ilgili alanlar eklenir
            {
                newRecord.BRAND = C.Brand;
                newRecord.CAREGORY = C.Category;
                newRecord.USERNAME = "yusuf";
                newRecord.STOCK = C.Stock;
                newRecord.DATETIME = DateTime.Now;
                XYZContex contex = new XYZContex(); // entity frameworkten contex nesnesi oluşturulur
                contex.T_ABC.Add(newRecord); // Add metodu ile contex e alınır
                contex.SaveChanges(); // database e kayıt işlemi yapılır

            }

            #endregion

        }
    }


    public class Product // json api den gelen product bilgileri için sınıf
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public double DiscountPercentage { get; set; }
        public double Rating { get; set; }
        public int Stock { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string Thumbnail { get; set; }
        public List<string> Images { get; set; }
    }
    public class Root // json api den gelen hem product hemde Total, Skip ve Limit bilgileri için sınıf
    {
        public List<Product> Products { get; set; }
        public int Total { get; set; }
        public int Skip { get; set; }
        public int Limit { get; set; }
    }
    public class HttpHelper  // json api den asenkron veri okumak için sınıf 
    {
        public static async Task<T> GetDataFromApi<T>(string url)
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync(url);
                result.EnsureSuccessStatusCode();
                string resultContentString = await result.Content.ReadAsStringAsync();
                T root = JsonConvert.DeserializeObject<T>(resultContentString);
                return root;
            }
        }

    }

}

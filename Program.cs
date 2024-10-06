using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportGenerator
{
    class QuarterlyIncomeReport
    {
        static void Main(string[] args)
        {
            // create a new instance of the class
            QuarterlyIncomeReport report = new QuarterlyIncomeReport();

            // call the GenerateSalesData method
            SalesData[] salesData = report.GenerateSalesData();

            // call the QuarterlySalesReport method
            report.QuarterlySalesReport(salesData);
        }

        /* public struct SalesData includes the following fields: date sold, department name, product ID, quantity sold, unit price */
        public struct SalesData
        {
            public DateOnly dateSold;
            public string departmentName;
            public string productID;
            public int quantitySold;
            public double unitPrice;
            public double baseCost;
            public int volumeDiscount;
        }

        public struct ProdDepartments
        {
            public static string[] Departments = new string[]
            {
            "Menswear",
            "Womenswear",
            "Childrenswear",
            "Footwear",
            "Accessories",
            "Sportswear",
            "Underwear",
            "Outerwear"
            };

            public static string[] Abbreviations = new string[]
            {
            "MENS",
            "WMNS",
            "CHLD",
            "FTWR",
            "ACCS",
            "SPRT",
            "UNDR",
            "OUTR"
            };
        }


        public struct ManufacturingSites
        {
            public static string[] ManSites = new string[]
            {
                "US1", "US2", "CA1", "CA2", "MX1", "MX2", "DE1", "DE2", "JP1", "JP2"
            };
        }
        /* the GenerateSalesData method returns 1000 SalesData records. It assigns random values to each field of the data structure */
        public SalesData[] GenerateSalesData()
        {
            SalesData[] salesData = new SalesData[1000];
            Random random = new Random();

            for (int i = 0; i < 1000; i++)
            {
                salesData[i].dateSold = new DateOnly(2023, random.Next(1, 13), random.Next(1, 29));
                salesData[i].departmentName = ProdDepartments.Departments[random.Next(ProdDepartments.Departments.Length)];

                int indexOfDept = Array.IndexOf(ProdDepartments.Departments, salesData[i].departmentName);
                string deptAbb = ProdDepartments.Abbreviations[indexOfDept];
                string firstDigit = (indexOfDept + 1).ToString();
                string nextTwoDigits = random.Next(1, 100).ToString("D2");
                
                string[] sizes = { "XS", "S", "M", "L", "XL" };
                string sizeCode = sizes[random.Next(sizes.Length)];
                
                string[] colors = { "BK", "BL", "GR", "RD", "YL", "OR", "WT", "GY" };
                string colorCode = colors[random.Next(colors.Length)];
                
                string manufacturingSite = ManufacturingSites.ManSites[random.Next(ManufacturingSites.ManSites.Length)];

                salesData[i].productID = $"{deptAbb}-{firstDigit}{nextTwoDigits}-{sizeCode}-{colorCode}-{manufacturingSite}";
                salesData[i].quantitySold = random.Next(1, 101);
                salesData[i].unitPrice = random.Next(25, 300) + random.NextDouble();
                double discountPercentage = random.Next(5, 21) / 100.0;
                salesData[i].baseCost = salesData[i].unitPrice * (1 - discountPercentage);
                salesData[i].volumeDiscount = (int)(salesData[i].quantitySold * 0.1);
            }

            return salesData;
        }
        public void QuarterlySalesReport(SalesData[] salesData)
        {
            // create dictionaries to store the quarterly sales and profits data
            Dictionary<string, double> quarterlySales = new Dictionary<string, double>();
            Dictionary<string, double> quarterlyProfits = new Dictionary<string, double>();

            // create dictionaries to store the quarterly sales and profits data by department
            Dictionary<string, Dictionary<string, double>> departmentQuarterlySales = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, Dictionary<string, double>> departmentQuarterlyProfits = new Dictionary<string, Dictionary<string, double>>();

            // create a dictionary to store the top sales orders for each quarter
            Dictionary<string, List<SalesData>> topSalesOrders = new Dictionary<string, List<SalesData>>();

            // iterate through the sales data
            foreach (SalesData data in salesData)
            {
                // calculate the total sales and profit for each quarter
                string quarter = GetQuarter(data.dateSold.Month);
                double totalSales = data.quantitySold * data.unitPrice;
                double totalProfit = (data.unitPrice - data.baseCost) * data.quantitySold;

                if (quarterlySales.ContainsKey(quarter))
                {
                    quarterlySales[quarter] += totalSales;
                    quarterlyProfits[quarter] += totalProfit;
                }
                else
                {
                    quarterlySales.Add(quarter, totalSales);
                    quarterlyProfits.Add(quarter, totalProfit);
                }

                // calculate the total sales and profit for each quarter by department
                if (!departmentQuarterlySales.ContainsKey(data.departmentName))
                {
                    departmentQuarterlySales[data.departmentName] = new Dictionary<string, double>();
                    departmentQuarterlyProfits[data.departmentName] = new Dictionary<string, double>();
                }

                if (departmentQuarterlySales[data.departmentName].ContainsKey(quarter))
                {
                    departmentQuarterlySales[data.departmentName][quarter] += totalSales;
                    departmentQuarterlyProfits[data.departmentName][quarter] += totalProfit;
                }
                else
                {
                    departmentQuarterlySales[data.departmentName].Add(quarter, totalSales);
                    departmentQuarterlyProfits[data.departmentName].Add(quarter, totalProfit);
                }

                // track the top sales orders for each quarter
                if (!topSalesOrders.ContainsKey(quarter))
                {
                    topSalesOrders[quarter] = new List<SalesData>();
                }

                topSalesOrders[quarter].Add(data);
                topSalesOrders[quarter] = topSalesOrders[quarter].OrderByDescending(o => (o.unitPrice - o.baseCost) * o.quantitySold).Take(3).ToList();
            }

            // display the quarterly sales and profits report
            Console.WriteLine("Quarterly Sales and Profits Report");
            Console.WriteLine("----------------------------------");

            // sort the quarters in order
            var orderedQuarters = quarterlySales.OrderBy(q => q.Key);

            foreach (var quarter in orderedQuarters)
            {
                string quarterKey = quarter.Key;
                double sales = quarter.Value;
                double profit = quarterlyProfits[quarterKey];
                double profitPercentage = (profit / sales) * 100;

                Console.WriteLine("{0}: Sales: {1:C}, Profit: {2:C}, Profit Percentage: {3:F2}%", quarterKey, sales, profit, profitPercentage);

                // display the quarterly sales and profits report by department for the current quarter
                Console.WriteLine("  By Department:");
                Console.WriteLine("  +-----------------+-----------------+-----------------+---------------------+");
                Console.WriteLine("  | {0,-15} | {1,15} | {2,15} | {3,20} |", "Department", "Sales", "Profit", "Profit Percentage");
                Console.WriteLine("  +-----------------+-----------------+-----------------+---------------------+");

                foreach (var department in departmentQuarterlySales)
                {
                    if (department.Value.ContainsKey(quarterKey))
                    {
                        string departmentName = department.Key;
                        double deptSales = department.Value[quarterKey];
                        double deptProfit = departmentQuarterlyProfits[departmentName][quarterKey];
                        double deptProfitPercentage = (deptProfit / deptSales) * 100;

                        Console.WriteLine("  | {0,-15} | {1,15:C} | {2,15:C} | {3,20:F2} |", departmentName, deptSales, deptProfit, deptProfitPercentage);
                    }
                }
                Console.WriteLine("  +-----------------+-----------------+-----------------+---------------------+");
                Console.WriteLine();

                // display the top 3 sales orders for the current quarter
                Console.WriteLine("  Top 3 Sales Orders:");
                Console.WriteLine("  +-----------------+-----------------+-----------------+---------------------+-----------------+---------------------+");
                Console.WriteLine("  | {0,-15} | {1,15} | {2,15} | {3,20} | {4,15} | {5,20} |", "Product ID", "Quantity Sold", "Unit Price", "Total Sales", "Profit", "Profit Percentage");
                Console.WriteLine("  +-----------------+-----------------+-----------------+---------------------+-----------------+---------------------+");

                foreach (var order in topSalesOrders[quarterKey])
                {
                    double totalOrderSales = order.quantitySold * order.unitPrice;
                    double orderProfit = (order.unitPrice - order.baseCost) * order.quantitySold;
                    double orderProfitPercentage = (orderProfit / totalOrderSales) * 100;
                    Console.WriteLine("  | {0,-15} | {1,15} | {2,15:C} | {3,20:C} | {4,15:C} | {5,20:F2} |", order.productID, order.quantitySold, order.unitPrice, totalOrderSales, orderProfit, orderProfitPercentage);
                }
                Console.WriteLine("  +-----------------+-----------------+-----------------+---------------------+-----------------+---------------------+");
                Console.WriteLine();
            }
        }

        public string GetQuarter(int month)
        {
            if (month >= 1 && month <= 3)
            {
                return "Q1";
            }
            else if (month >= 4 && month <= 6)
            {
                return "Q2";
            }
            else if (month >= 7 && month <= 9)
            {
                return "Q3";
            }
            else
            {
                return "Q4";
            }
        }
    }
}

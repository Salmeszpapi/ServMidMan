using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using OfficeOpenXml;
using System.Text;
using ServMidMan.Models;
using ServMidMan.Data;

namespace ServMidMan.Helper
{
	public class ExcelDataTransmitter
	{
		private Dictionary<string, string> DisctrictShorteningDictionary = new Dictionary<string, string>()
		{
			{"BC","Banskobystrický" },
			{"TA","Trnavský" },
			{"PV","Prešovský" },
			{"ZI","Žilinský" },
			{"TC","Trenčiansky" },
			{"NI","Nitriansky" },
			{"KL","Košický" },
			{"BL","Bratislavský" }
		};
        public ExcelDataTransmitter(DataProviderContext dataProviderContext)
        {
			uploadDataset(dataProviderContext);

		}
        public void uploadDataset(DataProviderContext dataProviderContext)
		{
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			using (ExcelPackage xlPackage = new ExcelPackage(new FileInfo(@"D:\OBCE.xlsx")))
			{
				var myWorksheet = xlPackage.Workbook.Worksheets.First(); //select sheet here
				var totalRows = myWorksheet.Dimension.End.Row;
				var totalColumns = myWorksheet.Dimension.End.Column;

				var sb = new StringBuilder(); //this is your data
				List<Location> myLocations = new List<Location>();
				for (int rowNum = 2; rowNum <= totalRows; rowNum++) //select starting row here
				{
					var rowValues = new List<string>();
					for (int col = 1; col <= 8; col++) // Read the first 7 columns explicitly
					{
						var cellValue = myWorksheet.Cells[rowNum, col].Value;
						rowValues.Add(cellValue == null ? string.Empty : cellValue.ToString());
					}

					Location location = new Location();
					location.Cities = rowValues.ElementAtOrDefault(1); // City is in the first column
					location.PostalCode = rowValues.ElementAtOrDefault(3); // Postal code is in the third column

					// Attempt to get the district abbreviation
					var districtAbbreviation = rowValues.ElementAtOrDefault(7); // District abbreviation is in the seventh column
					if (DisctrictShorteningDictionary.TryGetValue(districtAbbreviation, out string district))
					{
						location.Disctrict = district;
					}
					else
					{
						// Handle the case where the district abbreviation is not found in the dictionary
						// For example, you might assign a default district or log an error
						location.Disctrict = "Unknown"; // Assign a default district
						Console.WriteLine($"District abbreviation '{districtAbbreviation}' not found in the dictionary.");
					}

					myLocations.Add(location);
					dataProviderContext.Locations.Add(location);
				}
			}
			dataProviderContext.SaveChanges();

		}
	}
}

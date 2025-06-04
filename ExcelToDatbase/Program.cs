using System;
using System.Data;
using System.Data.OleDb;
using Microsoft.Data.SqlClient;
using System.IO; // Required for Path.GetExtension

class Program
{
    static void Main(string[] args) // Main method to execute the program
    {
        try
        {
            string excelFilePath = @"S:\Sandesh\ExcelToDatbase\Data\CompanyWise_HR_Info.xlsx";
            string connectionString = "Server=LAPTOP-SMTECU8B\\SQLEXPRESS;Database=College;TrustServerCertificate=True;Trusted_Connection=True;";
            string tableName = "CompanyWise_HR_Info";

            DataTable dataTable = ReadExcelToDataTable(excelFilePath); // Read data from Excel file into DataTable
            if (dataTable.Rows.Count == 0)
            {
                Console.WriteLine("No data found in the Excel file.");
                return;
            }
            BulkInsertIntoSqlServer(dataTable, connectionString, tableName);// Bulk insert the DataTable into SQL Server

            Console.WriteLine("Operation completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }

    static DataTable ReadExcelToDataTable(string filePath)// Method to read Excel file and return a DataTable
    {
        if (!File.Exists(filePath))// Check if the file exists
            throw new FileNotFoundException("The specified Excel file does not exist.", filePath);

        string conString = string.Empty;// Initialize connection string
        // Determine the file extension and set the connection string accordingly

        string extension = Path.GetExtension(filePath);// Get the file extension of the Excel file

        switch (extension.ToLower())// Check the file extension and set the connection string accordingly
        {

            case ".xls":
                conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES;'";// For older Excel files (.xls)
                break;
            case ".xlsx":
                conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 12.0 Xml;HDR=YES;'";// For newer Excel files (.xlsx)
                break;
            default:
                throw new NotSupportedException("Unsupported file format.");
        }

        using (OleDbConnection conn = new OleDbConnection(conString))// Create a new OleDbConnection with the connection string
        {
            conn.Open();
            DataTable schemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);// Get the schema of the Excel file to find the worksheets

            if (schemaTable == null || schemaTable.Rows.Count == 0)// Check if there are any worksheets in the Excel file
                throw new InvalidOperationException("No worksheets found in the Excel file.");

            string sheetName = schemaTable.Rows[0]["TABLE_NAME"].ToString();// Get the name of the first worksheet

            using (OleDbCommand cmd = new OleDbCommand($"SELECT * FROM [{sheetName}]", conn))// Create a command to select all data from the worksheet
            
            using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))// Create an OleDbDataAdapter to fill the DataTable
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);// Fill the DataTable with data from the worksheet
                return dt;
            }
        }
    }

    static void BulkInsertIntoSqlServer(DataTable dt, string connString, string destinationTable)// Method to perform bulk insert of DataTable into SQL Server
    {
        using (SqlConnection connection = new SqlConnection(connString)) // Ensure SqlConnection is properly referenced  
        {
            connection.Open();

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection)) // Ensure SqlBulkCopy is properly referenced  
            {
                bulkCopy.DestinationTableName = destinationTable;// Set the destination table name in SQL Server

                // Map columns by name  
                foreach (DataColumn column in dt.Columns)// Iterate through each column in the DataTable
                {
                    bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);// Map the column in the DataTable to the corresponding column in the SQL Server table
                }

                bulkCopy.WriteToServer(dt);// Write the data from the DataTable to the SQL Server table
                Console.WriteLine("Data successfully imported.");
            }
        }
    }
}

// Note: Ensure that the SQL Server table structure matches the DataTable structure for successful bulk insert.
// Make sure to have the necessary permissions to access the SQL Server and the Excel file.
// Ensure that the Microsoft.ACE.OLEDB.12.0 provider is installed for .xlsx files.
// If you encounter issues with the OleDb provider, consider installing the Microsoft Access Database Engine Redistributable.
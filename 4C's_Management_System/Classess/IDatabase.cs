using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;


namespace _4C_s_Management_System
{
    /// <summary>Synchronous database abstraction for SQL Server.</summary>
    public interface IDatabase : IDisposable
    {
        int CommandTimeoutSeconds { get; set; }
        Action<string>? Logger { get; set; }

        int CUD(string query, object? parameters = null);
        object? Scalar(string query, object? parameters = null);
        DataTable TableData(string query, object? parameters = null);
        SqlDataReader select(string query);
        void Close();

        bool Save(string table, object data);
        bool Update(string table, object data, string keyColumn = "Id");
        bool Delete(string table, object data, string keyColumn = "Id");
        DataRow? GetById(string table, object id, string keyColumn = "Id");

        object? Max(string table, string column);
        object? Min(string table, string column);
        int Count(string table, string condition = "");
        bool Exists(string table, string condition);
        DataTable Search(string table, string[] columns, string keyword);
        decimal Sum(string table, string column, string condition = "");
        decimal Average(string table, string column, string condition = "");

        DataTable GetTop(string table, int top = 10, string orderBy = "Id DESC");
        DataTable Paginate(string table, int pageNumber, int pageSize, string orderBy = "Id ASC");
        (DataTable Rows, int TotalCount) PaginateWithCount(
            string table, int pageNumber, int pageSize,
            string where = "", string orderBy = "Id ASC");
        DataTable GetDistinct(string table, string column);
        DataTable Join(
            string table1, string table2, string onCondition,
            string columns = "*", string where = "");

        bool ClearTable(string table);
        bool BulkInsert(string table, DataTable data);
        bool Backup(string filePath);
        bool Ping();

        bool ExecuteTransaction(
            params (string query, object? parameters)[] commands);
        bool ExecuteTransaction(Func<SqlTransaction, bool> work);

        bool Upsert(string table, string keyColumn, object data);
        bool SoftDelete(
            string table, object id,
            string keyColumn = "Id", string deletedColumn = "IsDeleted");
        bool Restore(
            string table, object id,
            string keyColumn = "Id", string deletedColumn = "IsDeleted");
        bool Touch(
            string table, object id,
            string keyColumn = "Id", string column = "UpdatedAt");

        int ExecStoredProcedure(
            string procName, object? parameters = null, int? timeoutSeconds = null);
        DataTable QueryStoredProcedure(
            string procName, object? parameters = null, int? timeoutSeconds = null);

        T? QuerySingle<T>(string query, object? parameters = null) where T : new();
        List<T> QueryList<T>(string query, object? parameters = null) where T : new();

        T? ExecuteWithRetry<T>(
            Func<T> operation, int maxRetries = 3, int delayMs = 200);

        DataTable GetSchemaTables();
        DataTable GetSchemaColumns(string table);
        bool ColumnExists(string table, string column);

        void Table(DataTable dt, DataGridView dgv, string[]? header = null);
        void Table(string query, DataGridView dgv, string[]? header = null);
        void Table(
            string query, object? parameters,
            DataGridView dgv, string[]? header = null);
        void Table(string query, ListView lv, string[]? headers = null);
        void list(string query, ComboBox comboBox);

    }
}
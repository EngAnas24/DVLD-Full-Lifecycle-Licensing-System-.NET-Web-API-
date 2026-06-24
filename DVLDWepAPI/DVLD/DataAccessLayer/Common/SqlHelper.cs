using DAL.DataAccessLayer.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;
using DAL.DataAccessLayer.Mapping;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace DAL.DataAccessLayer.Common
{
    public class DeleteConflictException : Exception
    {
        public string DependentTable { get; private set; }

        public DeleteConflictException(string dependentTable, string message) : base(message)
        {
            DependentTable = dependentTable;
        }
    }

    public static class SqlHelper
    {
        private static string _connectionString;

        public static void Init(string connectionString)
        {
            _connectionString = connectionString;

            if (string.IsNullOrEmpty(_connectionString))
                throw new Exception("Connection string not found");
        }

        public static int ExecuteNonQuery(string spName, object value, bool isInsert = false)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(spName, conn)
                { CommandType = CommandType.StoredProcedure };

                cmd.AddParamsFromObject(value, isInsert);

                var pkProp = SqlCommandExtensions.FindPrimaryKeyProperty(value.GetType());
                int currentId = pkProp != null ? Convert.ToInt32(pkProp.GetValue(value) ?? 0) : 0;

                var outputParam = new SqlParameter("@ID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.InputOutput,
                    Value = (isInsert || currentId <= 0) ? (object)DBNull.Value : currentId
                };
                cmd.Parameters.Add(outputParam);

                conn.Open();
                cmd.ExecuteNonQuery();

                int returnedId = (outputParam.Value == DBNull.Value) ? 0 : Convert.ToInt32(outputParam.Value);

                if (isInsert && returnedId > 0 && pkProp != null)
                {
                    pkProp.SetValue(value, returnedId);
                }

                return returnedId;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                {
                    string dependentTable = ExtractTableNameFromConstraint(ex.Message);

                    throw new DeleteConflictException(dependentTable, ex.Message);
                }

                throw new Exception("Database Error: " + ex.Message);
            }
        }

        public static List<T> ExecuteQueryAll<T>(string spName, object parameters = null) where T : new()
        {
            List<T> list = new List<T>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(spName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null) cmd.AddParamsFromObject(parameters);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(MappingHelper.ToDto<T>(reader));
                }
            }
            return list;
        }

        public static T ExecuteeQuerySingl<T>(string spName, object parameters, out int outId) where T : new()
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(spName, conn) { CommandType = CommandType.StoredProcedure };

                cmd.AddParamsFromObject(parameters);

                var pkProp = SqlCommandExtensions.FindPrimaryKeyProperty(parameters.GetType());
                int currentId = pkProp != null ? Convert.ToInt32(pkProp.GetValue(parameters) ?? 0) : 0;

                var outputParam = new SqlParameter("@ID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.InputOutput,
                    Value = currentId
                };
                cmd.Parameters.Add(outputParam);

                conn.Open();
                T result = default;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result = MappingHelper.ToDto<T>(reader);
                    }
                }

                outId = (int)outputParam.Value;
                return result;
            }
            catch (SqlException ex)
            {
                throw new Exception("Database Error: " + ex.Message);
            }
        }

        public static string ExtractTableNameFromConstraint(string errorMessage)
        {
            var match = Regex.Match(errorMessage, @"table\s+""dbo\.([^""]+)""");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            var altMatch = Regex.Match(errorMessage, @"table\s+'dbo\.([^']+)'");
            if (altMatch.Success)
            {
                return altMatch.Groups[1].Value;
            }

            return "جداول أخرى مرتبطة";
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;
using WDE.Common.Database;
using WDE.MySqlDatabaseCommon.Providers;

namespace WDE.MySqlDatabaseCommon.Database
{
    public class MySqlExecutor : IMySqlExecutor
    {
        private readonly IMySqlConnectionStringProvider connectionString;

        public MySqlExecutor(IMySqlConnectionStringProvider connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task ExecuteSql(string query)
        {
            using var writeLock = await DatabaseLock.WriteLock();
            
            MySqlConnection conn = new(connectionString.ConnectionString);
            MySqlTransaction transaction;
            try
            {
                conn.Open();
                transaction = await conn.BeginTransactionAsync();
            }
            catch (Exception e)
            {
                throw new IMySqlExecutor.CannotConnectToDatabaseException(e);
            }

            try
            {
                MySqlCommand cmd = new(query, conn, transaction);
                await cmd.ExecuteNonQueryAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                await conn.CloseAsync();
                throw new IMySqlExecutor.QueryFailedDatabaseException(ex);
            }
            await conn.CloseAsync();
        }

        public async Task<IList<Dictionary<string, object>>> ExecuteSelectSql(string query)
        {
            using var writeLock = await DatabaseLock.WriteLock();
            
            MySqlConnection conn = new(connectionString.ConnectionString);
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                throw new IMySqlExecutor.CannotConnectToDatabaseException(e);
            }

            MySqlDataReader reader;
            try
            {
                MySqlCommand cmd = new(query, conn);
                reader = await cmd.ExecuteReaderAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new IMySqlExecutor.QueryFailedDatabaseException(ex);
            }

            List<Dictionary<string, object>> result = new();
            while (reader.Read())
            {
                var fields = new Dictionary<string, object>(reader.FieldCount);
                for (int i = 0; i < reader.FieldCount; ++i)
                    fields.Add(reader.GetName(i), reader.GetValue(i));
                
                result.Add(fields);
            }

            await reader.CloseAsync();
            await conn.CloseAsync();
            return result;
        }
    }
}
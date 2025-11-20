using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Interfaces;
using API.Models;
using Microsoft.Data.Sqlite;

namespace API.Repositories
{
    public class LogSQLiteRepository(SqliteConnection _connection) : ILogRepo
    {
        public async Task<AppLog?> Create(AddLogEntryReq req)
        {
            var insertCommand = _connection.CreateCommand();
            insertCommand.CommandText =
                @"
                INSERT INTO logs(username, timestamp)
                VALUES(@username, @timestamp);
                SELECT last_insert_rowid();";

            var ts = DateTime.Now;

            insertCommand.Parameters.AddWithValue("@username", req.UserName);
            insertCommand.Parameters.AddWithValue("@timestamp", ts);

            var newId = await insertCommand.ExecuteScalarAsync();
            if (newId == null)
            {
                return null;
            }

            var id = (long)newId;

            return new AppLog
            {
                Id = id,
                Timestamp = ts.ToString(),
                UserName = req.UserName,
            };
        }
    }
}

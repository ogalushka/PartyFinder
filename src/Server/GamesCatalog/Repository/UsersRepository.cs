﻿using GamesCatalog.Dto;
using System.Reflection;
using GamesCatalog.Entity;
using System.Data.SqlClient;

namespace GamesCatalog.Repository
{
    public class UsersRepository
    {
        // TODO move to config
        // TODO multiple repositories
        // TODO api and db objects separation
        public static string DataBaseName = "Players";
        public static string Connection = "Server=localhost,1433;User Id=sa;Password=change_this_password;";
        public UsersRepository() { }

        public async Task AddGame(string userId, int gameId, string name, string? coverUrl)
        {
            var query = "INSERT INTO Games (GameId, Name, CoverUrl) Values (@GameId, @Name, @CoverUrl);" + 
                "INSERT INTO PlayerGame (PlayerId, GameId) Values (@UserId, @GameId);";
            var param = new Dictionary<string, object?>
            {
                { "@UserId", userId },
                { "@GameId", gameId },
                { "@Name", name },
                { "@CoverUrl", coverUrl }
            };
            await ExecuteQuerry(query, param);
        }

        public async Task<bool> GameCached(int gameId)
        {
            var query = "SELECT 1 FROM Games WHERE GameId=@GameId;";
            var param = new Dictionary<string, object>
            {
                { "@GameId", gameId }
            };
            var result = await ExecuteQuerry(query, param);
            return result.Count > 0;
        }

        public async Task AddGame(string userId, int gameId)
        {
            var query = "INSERT INTO PlayerGame (PlayerId, GameId) Values (@UserId, @GameId);";
            var param = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@GameId", gameId }
            };
            await ExecuteQuerry(query, param);
        }

        public async Task RemoveGame(string userId, int gameId)
        {
            var query = "DELETE FROM PlayerGame WHERE PlayerId=@UserId AND GameId=@GameId;";
            var param = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@GameId", gameId }
            };
            await ExecuteQuerry(query, param);
        }
        
        public async Task AddTimeRange(string userId, TimeWindowDto timeWindow)
        {
            var query = "INSERT INTO PlayerTime (PlayerId, StartTime, EndTime) Values (@UserId, @StartTime, @EndTime);";
            var param = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@StartTime", timeWindow.StartTime },
                { "@EndTime", timeWindow.EndTime }
            };
            await ExecuteQuerry(query, param);
        }

        public async Task RemoteTimeRange(string userId, TimeWindowDto timeWindow)
        {
            var query = "DELETE FROM PlayerTime WHERE PlayerId=@UserId AND StartTime=@StartTime AND EndTime=EndTime;";
            var param = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@StartTime", timeWindow.StartTime },
                { "@EndTime", timeWindow.EndTime }
            };
            await ExecuteQuerry(query, param);
        }

        public async Task<UserMatchDto[]> GetUserRecomendations(string userId)
        {
            var query = @"
SELECT pt2.PlayerId, pg.GameId, pt2.StartTime, pt2.EndTime,
	-- get common start time (max)
	CASE 
		WHEN pt1.StartTime > pt2.StartTime 
			THEN pt1.StartTime 
			ELSE pt2.StartTime
	END as CommonStartTime,
	-- get common end time (min)
	CASE 
		WHEN pt1.EndTime > pt2.EndTime  
			THEN pt2.EndTime  
			ELSE pt1.EndTime 
	END as CommonEndTime
FROM PlayerTime pt1
-- get all time window pairs with player
JOIN PlayerTime pt2 
	ON pt1.PlayerId = @UserId
	AND pt2.PlayerId != @UserId
	AND pt1.StartTime < pt2.EndTime 
	AND pt1.EndTime > pt2.StartTime
-- join games for other players if they match player  games
JOIN PlayerGame pg
	ON pg.PlayerId = pt2.PlayerId 
	AND pg.GameId IN (SELECT GameId FROM PlayerGame WHERE PlayerId = @UserId)
	-- TODO a way to check filter out small timewindows, 
	-- considering time windows will be split if they go from sunday to monday
";
            var param = new Dictionary<string, object>
            {
                { "@UserId", userId },
            };
            var records = await ExecuteQuerry(query, param);

            //TODO check if possible to group with SQL
            var parsedRecords = records.Select(r => Field.CastRecord<UserMatch>(r));
            var result = new Dictionary<string, UserMatchDto>();
            foreach (var record in parsedRecords)
            {
                if (result.TryGetValue(record.PlayerId, out var user))
                {
                    user.MatchingGames.Add(record.GameId);
                    if (!user.MatchingTimeWindows.Any(w => w.StartTime == record.CommonStartTime && w.EndTime == record.CommonEndTime))
                    {
                        user.MatchingTimeWindows.Add(new TimeWindowDto { StartTime = record.CommonStartTime, EndTime = record.CommonEndTime });
                    }
                }
                else
                {
                    var userDto = new UserMatchDto(
                        record.PlayerId,
                        new() { record.GameId },
                        new() { new TimeWindowDto {
                            StartTime = record.CommonStartTime,
                            EndTime = record.CommonEndTime
                            }
                        });
                    result.Add(record.PlayerId, userDto);
                }
            }

            return result.Values.ToArray();
        }

        // TODO test new connection vs persistent connection
        private async Task<List<Dictionary<string, Field>>> ExecuteQuerry(string query, Dictionary<string, object?> queryParams)
        {
            var records = new List<Dictionary<string, Field>>();
            
            using var connection = new SqlConnection(Connection);
            await connection.OpenAsync();
            await connection.ChangeDatabaseAsync(DataBaseName);
            using var sqlCommand = new SqlCommand(query, connection);
            foreach (var pair in queryParams)
            {
                sqlCommand.Parameters.AddWithValue(pair.Key, pair.Value);
            }
            using var sqlReader = await sqlCommand.ExecuteReaderAsync();
            while (sqlReader.Read())
            {
                var fields = new Dictionary<string, Field>();
                for (var i = 0; i < sqlReader.FieldCount; i++)
                {
                    fields.Add(sqlReader.GetName(i), new Field { Type = sqlReader.GetFieldType(i), Value = sqlReader.GetValue(i) });
                }
                records.Add(fields);
            }

            return records;
        }

        public async Task<GameDto[]> GetGames(string userId)
        {
            var query =
                "SELECT g.GameId as Id, g.Name, g.CoverUrl FROM PlayerGame pg " +
                "JOIN Games g " +
                "ON g.GameId = pg.GameId " +
                "WHERE pg.PlayerId = @UserId";
            var param = new Dictionary<string, object>
            {
                { "@UserId", userId }
            };
            var records = await ExecuteQuerry(query, param);
            return records.Select(r => Field.CastRecord<GameDto>(r)).ToArray();
        }

        public async Task<TimeWindowDto[]> GetTimes(string userId)
        {
            var query =
                "SELECT t.StartTime, t.EndTime FROM PlayerTime t " +
                "WHERE t.PlayerId = @UserId";
            var param = new Dictionary<string, object>
            {
                { "@UserId", userId }
            };

            var records = await ExecuteQuerry(query, param);

            return records.Select(r => Field.CastRecord<TimeWindowDto>(r)).ToArray();
        }
    }

    class Field
    {
        public Type Type = typeof(int);
        public object Value = 0;

        public static T CastRecord<T>(Dictionary<string, Field> recrodFields) where T : new()
        {
            var result = new T();

            var typeFields = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public); 
            foreach (var typeField in typeFields)
            {
                if (!recrodFields.TryGetValue(typeField.Name, out var recordField))
                {
                    continue;
                }

                if (typeField.PropertyType != recordField.Type)
                {
                    continue;
                }

                typeField.SetValue(result, recordField.Value);
            }

            return result;
        }
    }
}

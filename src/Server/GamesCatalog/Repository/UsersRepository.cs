using GamesCatalog.Dto;
using System.Reflection;
using GamesCatalog.Entity;
using System.Data.SqlClient;

namespace GamesCatalog.Repository
{
    public class UsersRepository
    {
        // TODO move to config
        // TODO docker config with database setup
        public static string DataBaseName = "Players";
        public static string Connection = "Server=localhost,1433;User Id=sa;Password=change_this_password;";
        public UsersRepository() { }

        public async Task AddGame(string userId, int gameId)
        {
            using var connection = new SqlConnection(Connection);
            await connection.OpenAsync();
            await connection.ChangeDatabaseAsync(DataBaseName);
            var querry = "INSERT INTO PlayerGame (PlayerId, GameId) Values (@UserId, @GameId);";
            using var sqlCommand = new SqlCommand(querry, connection);
            sqlCommand.Parameters.AddWithValue("@UserId", userId);
            sqlCommand.Parameters.AddWithValue("@GameId", gameId);
            using var sqlDataReader = await sqlCommand.ExecuteReaderAsync();
        }

        public async Task RemoveGame(string userId, int gameId)
        {
            using var connection = new SqlConnection(Connection);
            await connection.OpenAsync();
            await connection.ChangeDatabaseAsync(DataBaseName);
            var querry = "DELETE FROM PlayerGame WHERE PlayerId=@UserId AND GameId=@GameId;";
            using var sqlCommand = new SqlCommand(querry, connection);
            sqlCommand.Parameters.AddWithValue("@UserId", userId);
            sqlCommand.Parameters.AddWithValue("@GameId", gameId);
            using var sqlDataReader = await sqlCommand.ExecuteReaderAsync();
        }
        
        public async Task AddTimeRange(string userId, TimeWindowDto timeWindow)
        {
            using var connection = new SqlConnection(Connection);
            await connection.OpenAsync();
            await connection.ChangeDatabaseAsync(DataBaseName);
            var querry = "INSERT INTO PlayerTime (PlayerId, StartTime, EndTime) Values (@UserId, @StartTime, @EndTime);";
            using var sqlCommand = new SqlCommand(querry, connection);
            sqlCommand.Parameters.AddWithValue("@UserId", userId);
            sqlCommand.Parameters.AddWithValue("@StartTime", timeWindow.StartTime);
            sqlCommand.Parameters.AddWithValue("@EndTime", timeWindow.EndTime);
            using var sqlDataReader = await sqlCommand.ExecuteReaderAsync();
        }

        public async Task RemoteTimeRange(string userId, TimeWindowDto timeWindow)
        {
            using var connection = new SqlConnection(Connection);
            await connection.OpenAsync();
            await connection.ChangeDatabaseAsync(DataBaseName);
            var querry = "DELETE FROM PlayerTime WHERE PlayerId=@UserId AND StartTime=@StartTime AND EndTime=EndTime;";
            using var sqlCommand = new SqlCommand(querry, connection);
            sqlCommand.Parameters.AddWithValue("@UserId", userId);
            sqlCommand.Parameters.AddWithValue("@StartTime", timeWindow.StartTime);
            sqlCommand.Parameters.AddWithValue("@EndTime", timeWindow.EndTime);
            using var sqlDataReader = await sqlCommand.ExecuteReaderAsync();
        }

        public async Task<UserMatchDto[]> GetUserRecomendations(string userId)
        {
            var querry = @"
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
            
            using var connection = new SqlConnection(Connection);
            await connection.OpenAsync();
            await connection.ChangeDatabaseAsync(DataBaseName);
            using var sqlCommand = new SqlCommand(querry, connection);
            sqlCommand.Parameters.AddWithValue("@UserId", userId);
            using var sqlReader = await sqlCommand.ExecuteReaderAsync();
            var records = new List<UserMatch>();
            var fields = new Dictionary<string, Field>();
            while (sqlReader.Read())
            {
                fields.Clear();
                for (var i = 0; i < sqlReader.FieldCount; i++)
                {
                    fields.Add(sqlReader.GetName(i), new Field { Type = sqlReader.GetFieldType(i), Value = sqlReader.GetValue(i) });
                }
                records.Add(Field.CastRecrod<UserMatch>(fields));
            }

            //TODO check if possible to group with SQL
            var result = new Dictionary<string, UserMatchDto>();
            foreach (var record in records)
            {
                if (result.TryGetValue(record.PlayerId, out var user))
                {
                    user.MatchingGames.Add(record.GameId);
                    if (!user.MatchingTimeWindows.Any(w => w.StartTime == record.CommonStartTime && w.EndTime == record.CommonEndTime))
                    {
                        user.MatchingTimeWindows.Add(new TimeWindowDto(record.CommonStartTime, record.CommonEndTime));
                    }
                }
                else
                {
                    var userDto = new UserMatchDto(record.PlayerId, new() { record.GameId }, new() { new TimeWindowDto(record.CommonStartTime, record.CommonEndTime) });
                    result.Add(record.PlayerId, userDto);
                }
            }

            return result.Values.ToArray();
        }
    }

    class Field
    {
        public Type Type = typeof(int);
        public object Value = 0;

        public static T CastRecrod<T>(Dictionary<string, Field> recrodFields) where T : new()
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

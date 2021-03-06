﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UltimateTicTacToe.DataAccess.DBOs;
using UltimateTicTacToe.Models.DTOs;

namespace UltimateTicTacToe.DataAccess
{
    class SQLDataBaseProvider : IDatabaseProvider
    {
        public void saveGameResult(int Player1, int Player2, int? Winner)
        {
            int gameCount = getNoGames(Player1, Player2);
            using (SqlConnection connection = new SqlConnection(getConnectionString()))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(null, connection);
                if (Winner != null)
                {
                    command.CommandText = "Insert into Games (GameNo, PlayerOne, PlayerTwo, Winner) Values (" + gameCount + "," + Player1 + "," + Player2 + "," + Winner + ")";
                } else
                {
                    command.CommandText = "Insert into Games (GameNo, PlayerOne, PlayerTwo) Values (" + gameCount + "," + Player1 + "," + Player2+ ")";
                }
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void saveMove(int PlayerId, double moveReward, int movePlace)
        {
            using (SqlConnection connection = new SqlConnection(getConnectionString()))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(null, connection);
                command.CommandText = "Insert into moves (Owner, Reward, Place) Values (" + PlayerId + "," + moveReward + "," + movePlace + ")";
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        private List<MoveDBO> getMoves(int PlayerID)
        {
            List<MoveDBO> result = new List<MoveDBO>();
            using(SqlConnection connection = new SqlConnection(getConnectionString()))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(null, connection);
                command.CommandText = "select Owner, Reward, Place from moves where Owner = " + PlayerID;
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new MoveDBO { Owner = PlayerID, place = (int)reader["Place"], reward = (double)reader["Reward"] });
                }
                connection.Close();
            }
            return result;
        }
        

        private int getNoGames(int Player1, int Player2)
        {
            string stmt = "SELECT COUNT(*) FROM GAMES";
            int count = 0;
            using (SqlConnection connection = new SqlConnection(getConnectionString()))
            {
                using (SqlCommand cmdCount = new SqlCommand(stmt, connection))
                {
                    connection.Open();
                    count = (int)cmdCount.ExecuteScalar();
                    connection.Close();
                }
            }
            return count;
        }

         public int createUser()
        {
            int UId;
            using (SqlConnection connection = new SqlConnection(getConnectionString()))
            {
                UId = getNumberOfUsers();
                connection.Open();
                SqlCommand command = new SqlCommand(null, connection);
                command.CommandText = "INSERT INTO RATINGS (UserId, TotalScore, LatestScore, TotalMoves)" +
                    "VALUES ( "+ UId + ", " + 0 + ", " + 0 + ", " + 0 + ") ";
                command.ExecuteNonQuery();
                connection.Close();
            }
            return UId;
        }

        public RatingDTO getUser(int UserId)
        {
            RatingDTO result = null;
            RatingDBO row = getUserRow(UserId);
            if (row != null)
            {
                result = new RatingDTO();
                result.userId = row.UserId;
                result.average = row.TotalScore / row.TotalMoves;
                result.latest = row.LatestScore;
                result.moves = getMoves(UserId);
            }
            return result;
        }

        public RatingDTO updateUser(int UserId, double LatestScore)
        {
            RatingDBO row = getUserRow(UserId);
            RatingDTO result = null;
            if (row != null)
            {
                using (SqlConnection connection = new SqlConnection(getConnectionString()))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(null, connection);
                    double total = row.TotalScore + LatestScore;
                    int moves = row.TotalMoves + 1;
                    command.CommandText = "UPDATE RATINGS SET LatestScore = " + LatestScore + ", TotalScore = " + total + ", TotalMoves = " + moves + " WHERE UserId = " + UserId;
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                result = getUser(UserId);
            }
            return result;
        }

        private RatingDBO getUserRow(int UserId)
        {
            RatingDBO result = null;
            using (SqlConnection connection = new SqlConnection(getConnectionString()))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(null, connection);
                command.CommandText = "SELECT * FROM RATINGS WHERE UserId = " + UserId;
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    result = new RatingDBO();
                    result.LatestScore = (double)reader["LatestScore"];
                    result.TotalMoves = (int)reader["TotalMoves"];
                    result.TotalScore = (double)reader["TotalScore"];
                    result.UserId = (int)reader["UserId"];
                }
                connection.Close();
            }
            return result;
        }

        private int getNumberOfUsers()
        {

            string stmt = "SELECT COUNT(*) FROM RATINGS";
            int count = 0;

            using (SqlConnection connection = new SqlConnection(getConnectionString()))
            {
                using (SqlCommand cmdCount = new SqlCommand(stmt, connection))
                {
                    connection.Open();
                    count = (int)cmdCount.ExecuteScalar();
                    connection.Close();
                }
            }
            return count;

        }

        private string getConnectionString()
        {
            string connectionString = "";
            using (StreamReader sr = new StreamReader("./DataAccess/connectionString.json"))
            {
                connectionString = JsonConvert.DeserializeObject<JObject>(sr.ReadToEnd())["connectionString"].Value<string>();
            }
            return connectionString;
        }
    }
}

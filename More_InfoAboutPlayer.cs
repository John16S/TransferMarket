using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;


namespace TransferMarket
{
    public partial class More_InfoAboutPlayer : Form
    {
        int id;

        public More_InfoAboutPlayer(int id)
        {
            this.id = id;
            InitializeComponent();
            load();
            dataGridView1.Columns[4].DefaultCellStyle.Format = "d"; //"short date pattern" (короткий формат даты), чтобы без времени выводить
        }

        private void load()
        {
            //Создаём соединение
            string connectionToDB = "provider=Microsoft.Jet.OLEDB.4.0;Data Source=TransferMarket.mdb"; //Строка соединение
            OleDbConnection dbConnection = new OleDbConnection(connectionToDB); //создаём соединение

            //Выполняем запрос в БД
            dbConnection.Open(); //открываем соединение
            string queryMainInfo = "SELECT player_info.last_name, player_info.first_name, player_info.fathers_name, [country].[country_name], player_info.date_of_birth, [position].[position_Name], [football_info].[height], [football_info].[kicking_leg], football_info.price, football_info.salary, [transfer_status].[Status_Name] " +
                "FROM transfer_status INNER JOIN([position] INNER JOIN (country INNER JOIN (player_info INNER JOIN football_info ON player_info.player_id = football_info.player_id) ON [country].country_id = player_info.[country]) ON [position].position_id = football_info.[position]) ON [transfer_status].[TrSt_id] = [football_info].[transfer_status] " +
                "WHERE player_info.player_id = " + id + ";";
            string queryPlayerStats = "SELECT player_stats.ps_id, player_stats.season, [club].[club_name], player_stats.played_matches, player_stats.goals, player_stats.assists " +
                "FROM club INNER JOIN player_stats ON [club].[club_id] = player_stats.club " +
                "where player_stats.player = " + id + ";";

            OleDbCommand dbCommandMainInfo = new OleDbCommand(queryMainInfo, dbConnection); //команда
            OleDbDataReader dbReaderMainInfo = dbCommandMainInfo.ExecuteReader();   //считываем данные
            OleDbCommand dbCommandPlayerStats = new OleDbCommand(queryPlayerStats, dbConnection); //команда
            OleDbDataReader dbReaderPlayerStats = dbCommandPlayerStats.ExecuteReader();   //считываем данные

            //Проверяем данные 
            if (dbReaderMainInfo.HasRows == false)
            {
                MessageBox.Show("Данные не найдены!", "Ошибка!");
            }
            else
            {
                dataGridView1.Rows.Clear();
                //Запищем данные в таблицу формы
                while (dbReaderMainInfo.Read())
                {
                    dataGridView1.Rows.Add(dbReaderMainInfo["last_name"], dbReaderMainInfo["first_name"], dbReaderMainInfo["fathers_name"], dbReaderMainInfo["country_name"], dbReaderMainInfo["date_of_birth"], dbReaderMainInfo["position_Name"], dbReaderMainInfo["height"], dbReaderMainInfo["kicking_leg"], dbReaderMainInfo["price"], dbReaderMainInfo["salary"], dbReaderMainInfo["Status_Name"]);
                }
            }

            //Проверяем данные 
            if (dbReaderPlayerStats.HasRows == false)
            {
                MessageBox.Show("Данные не найдены!", "Ошибка!");
            }
            else
            {
                dataGridView2.Rows.Clear();
                //Запищем данные в таблицу формы
                while (dbReaderPlayerStats.Read())
                {
                    dataGridView2.Rows.Add(dbReaderPlayerStats["ps_id"], dbReaderPlayerStats["season"], dbReaderPlayerStats["club_name"], dbReaderPlayerStats["played_matches"], dbReaderPlayerStats["goals"], dbReaderPlayerStats["assists"]);
                }
            }

            //Закрываем соединение
            dbReaderMainInfo.Close();
            dbReaderPlayerStats.Close();
            dbConnection.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            load();
        }

        //Удалить
        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить запись?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                string connectionToDB = "provider=Microsoft.Jet.OLEDB.4.0;Data Source=TransferMarket.mdb"; //Строка соединение
                OleDbConnection dbConnection = new OleDbConnection(connectionToDB); //создаём соединение
                dbConnection.Open();
                string query = "DELETE FROM player_info WHERE player_id = " + id;
                OleDbCommand cmd = new OleDbCommand(query, dbConnection);

                if (cmd.ExecuteNonQuery() != 1)
                {
                    MessageBox.Show("Ошибка выполнения запроса!", "Внимание!");
                }
                else
                {
                    MessageBox.Show("Запись удалён!", "Внимание!");
                    this.Close();
                }
                dbConnection.Close();
            }
            else
            {
                
            }    
        }

        //Обноваление 
        private void button3_Click(object sender, EventArgs e)
        {
            //Считаем данные
            //For player_info table
            string last_name = dataGridView1.Rows[0].Cells[0].Value.ToString();
            string first_name = dataGridView1.Rows[0].Cells[1].Value.ToString();
            string fathers_name = dataGridView1.Rows[0].Cells[2].Value.ToString();
            string country_name = dataGridView1.Rows[0].Cells[3].Value.ToString();
            DateTime dob = Convert.ToDateTime(dataGridView1.Rows[0].Cells[4].Value);
            string dobSql = dob.ToString("MM/dd/yyyy");
            //For football_info table
            string position_name = dataGridView1.Rows[0].Cells[5].Value.ToString();
            int height = Convert.ToInt32(dataGridView1.Rows[0].Cells[6].Value);
            string kicking_leg = dataGridView1.Rows[0].Cells[7].Value.ToString();
            int price = Convert.ToInt32(dataGridView1.Rows[0].Cells[8].Value);
            double salary = Convert.ToInt32(dataGridView1.Rows[0].Cells[9].Value);
            string transfer_status_name = dataGridView1.Rows[0].Cells[10].Value.ToString();

            int index = dataGridView2.CurrentCell.RowIndex; //выбираем id выбранной строчки

            int ps_id = Convert.ToInt32(dataGridView2.Rows[index].Cells[0].Value.ToString());
            string season = dataGridView2.Rows[index].Cells[1].Value.ToString();
            string club_name = dataGridView2.Rows[index].Cells[2].Value.ToString();
            int played_matches = Convert.ToInt32(dataGridView2.Rows[index].Cells[3].Value.ToString());
            int goals = Convert.ToInt32(dataGridView2.Rows[index].Cells[4].Value.ToString());
            int assists = Convert.ToInt32(dataGridView2.Rows[index].Cells[5].Value.ToString());

            string connectionToDB = "provider=Microsoft.Jet.OLEDB.4.0;Data Source=TransferMarket.mdb"; //Строка соединение
            OleDbConnection dbConnection = new OleDbConnection(connectionToDB); //создаём соединение
            dbConnection.Open();

            int country_id = addToTableCountry(dbConnection, country_name);
            int transfer_status_id = addToTableTransferSatus(dbConnection, transfer_status_name);
            int position_id = addToTablePosition(dbConnection, position_name);
            int club_id = addToTableClub(dbConnection, club_name);

            //Update player_info table
            string queryPlayer_info = "UPDATE player_info SET last_name = @last_name, first_name = @first_name, " +
                "fathers_name = @fathers_name, country = @country_id, date_of_birth = @dob WHERE player_id = @id";
            OleDbCommand cmdPlayer_info = new OleDbCommand(queryPlayer_info, dbConnection);
            cmdPlayer_info.Parameters.AddWithValue("@last_name", last_name);
            cmdPlayer_info.Parameters.AddWithValue("@first_name", first_name);
            cmdPlayer_info.Parameters.AddWithValue("@fathers_name", fathers_name);
            cmdPlayer_info.Parameters.AddWithValue("@country_id", country_id);
            cmdPlayer_info.Parameters.AddWithValue("@dob", dobSql);
            cmdPlayer_info.Parameters.AddWithValue("@id", id);
            cmdPlayer_info.ExecuteNonQuery();

            //Update football_info table
            string queryFootball_info = "UPDATE [football_info] SET [position] = @position, [height] = @height, " +
                "[kicking_leg] = @kicking_leg, [price] = @price, [salary] = @salary, [transfer_status] = @transfer_status WHERE [player_id] = @id";
            OleDbCommand cmdFootball_info = new OleDbCommand(queryFootball_info, dbConnection);
            cmdFootball_info.Parameters.AddWithValue("@position", position_id);
            cmdFootball_info.Parameters.AddWithValue("@height", height);
            cmdFootball_info.Parameters.AddWithValue("@kicking_leg", kicking_leg);
            cmdFootball_info.Parameters.AddWithValue("@price", price);
            cmdFootball_info.Parameters.AddWithValue("@salary", salary);
            cmdFootball_info.Parameters.AddWithValue("@transfer_status", transfer_status_id);
            cmdFootball_info.Parameters.AddWithValue("@id", id);
            cmdFootball_info.ExecuteNonQuery();

            //Update player_stats table
            string queryPlayer_stats = "UPDATE [player_stats] SET [season] = @season, [club] = @club, [played_matches] = @played_matches, " +
                "[goals] = @goals, [assists] = @assists WHERE (([ps_id] = @ps_id1) AND ([player] = @id))";
            OleDbCommand cmdPlayer_stats = new OleDbCommand(queryPlayer_stats, dbConnection);
            cmdPlayer_stats.Parameters.AddWithValue("@season", season);
            cmdPlayer_stats.Parameters.AddWithValue("@club", club_id);
            cmdPlayer_stats.Parameters.AddWithValue("@played_matches", played_matches);
            cmdPlayer_stats.Parameters.AddWithValue("@goals", goals);
            cmdPlayer_stats.Parameters.AddWithValue("@assists", assists);
            cmdPlayer_stats.Parameters.AddWithValue("@ps_id1", ps_id);
            cmdPlayer_stats.Parameters.AddWithValue("@id", id);
            cmdPlayer_stats.ExecuteNonQuery();

            if (cmdPlayer_info.ExecuteNonQuery() != 1 || cmdFootball_info.ExecuteNonQuery() != 1 || cmdPlayer_stats.ExecuteNonQuery() != 1)
            {
                MessageBox.Show("Ошибка выполнения запроса!", "Внимание!");
            }
            else
            {
                MessageBox.Show("Данные изменены!", "Внимание!");
            }

            dbConnection.Close();
        }

        private int addToTableCountry(OleDbConnection dbConnection, string _country_name)
        {
            int country_id;
            string queryCountry = "SELECT country_id FROM country WHERE country_name = @country_name";
            OleDbCommand cmdCountry = new OleDbCommand(queryCountry, dbConnection);
            cmdCountry.Parameters.AddWithValue("@country_name", _country_name);
            object result = cmdCountry.ExecuteScalar();

            if (result == null) // country_name не существует в таблице country
            {
                queryCountry = "INSERT INTO country (country_name) VALUES (@country_name)";
                cmdCountry = new OleDbCommand(queryCountry, dbConnection);
                cmdCountry.Parameters.AddWithValue("@country_name", _country_name);
                cmdCountry.ExecuteNonQuery();

                // получаем id только что добавленной записи
                cmdCountry = new OleDbCommand("SELECT @@IDENTITY", dbConnection);
                country_id = (int)(cmdCountry.ExecuteScalar());
            }
            else // country_name уже существует в таблице country
            {
                country_id = (int)(result);
            }

            return country_id;
        }

        private int addToTableTransferSatus(OleDbConnection dbConnection, string _transfer_status_name)
        {
            int transfer_status_id;
            string query = "SELECT TrSt_id FROM transfer_status WHERE Status_Name = @transfer_status_name";
            using (OleDbCommand cmd = new OleDbCommand(query, dbConnection))
            {
                cmd.Parameters.AddWithValue("@transfer_status_name", _transfer_status_name);
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    transfer_status_id = Convert.ToInt32(result);
                }
                else
                {
                    query = "INSERT INTO transfer_status (Status_Name) VALUES (@transfer_status_name)";
                    cmd.CommandText = query;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@transfer_status_name", _transfer_status_name);
                    cmd.ExecuteNonQuery();

                    // получаем id только что добавленной записи
                    cmd.CommandText = "SELECT @@IDENTITY";
                    transfer_status_id = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            return transfer_status_id;
        }

        private int addToTablePosition(OleDbConnection dbConnection, string _position_name)
        {
            int _position_name_id;
            string query = "SELECT [position_id] FROM [position] WHERE [position_Name] = @position_name";
            using (OleDbCommand cmd = new OleDbCommand(query, dbConnection))
            {
                cmd.Parameters.AddWithValue("@position_name", _position_name);
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    _position_name_id = Convert.ToInt32(result);
                }
                else
                {
                    query = "INSERT INTO position (position_Name) VALUES (@position_name)";
                    cmd.CommandText = query;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@position_name", _position_name);
                    cmd.ExecuteNonQuery();

                    // получаем id только что добавленной записи
                    cmd.CommandText = "SELECT @@IDENTITY";
                    _position_name_id = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            return _position_name_id;
        }

        private int addToTableClub(OleDbConnection dbConnection, string _club_name)
        {
            int club_id;
            string query = "SELECT [club_id] FROM [club] WHERE [club_name] = @club_name";
            using (OleDbCommand cmd = new OleDbCommand(query, dbConnection))
            {
                cmd.Parameters.AddWithValue("@club_name", _club_name);
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    club_id = Convert.ToInt32(result);
                }
                else
                {
                    query = "INSERT INTO club (club_name) VALUES (@club_name)";
                    cmd.CommandText = query;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@city_name", _club_name);
                    cmd.ExecuteNonQuery();

                    // получаем id только что добавленной записи
                    cmd.CommandText = "SELECT @@IDENTITY";
                    club_id = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            return club_id;
        }

    }
}

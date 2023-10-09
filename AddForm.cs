using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TransferMarket
{
    public partial class AddForm : Form
    {
        public AddForm()
        {
            InitializeComponent();
            //чтобы можно было только выбирать из выпадающего листа, и не вводить свои значения
            region_comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            position_comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            kicking_leg_comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            transfer_status_comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            season_comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private int addToTableRegion(OleDbConnection dbConnection, string _region_name)
        {
            int region_id;
            string query = "SELECT region_id FROM region WHERE region_name = @region_name";
            OleDbCommand cmd = new OleDbCommand(query, dbConnection);
            cmd.Parameters.AddWithValue("@country_name", _region_name);
            object result = cmd.ExecuteScalar();

            if (result == null) // region_name не существует в таблице region
            {
                query = "INSERT INTO region (region_name) VALUES (@region_name)";
                cmd = new OleDbCommand(query, dbConnection);
                cmd.Parameters.AddWithValue("@region_name", _region_name);
                cmd.ExecuteNonQuery();

                // получаем id только что добавленной записи
                cmd = new OleDbCommand("SELECT @@IDENTITY", dbConnection);
                region_id = (int)(cmd.ExecuteScalar());
            }
            else // region_name уже существует в таблице country
            {
                region_id = (int)(result);
            }

            return region_id;
        }

        private int addToTableCountry(OleDbConnection dbConnection, string _country_name, int _region_id)
        {
            int country_id;
            string queryCountry = "SELECT country_id FROM country WHERE country_name = @country_name";
            OleDbCommand cmdCountry = new OleDbCommand(queryCountry, dbConnection);
            cmdCountry.Parameters.AddWithValue("@country_name", _country_name);
            object result = cmdCountry.ExecuteScalar();

            if (result == null) // country_name не существует в таблице country
            {
                queryCountry = "INSERT INTO country (country_name, region) VALUES (@country_name, @region_id)";
                cmdCountry = new OleDbCommand(queryCountry, dbConnection);
                cmdCountry.Parameters.AddWithValue("@country_name", _country_name);
                cmdCountry.Parameters.AddWithValue("@region_id", _region_id);
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

        private void addPlayerInfo(OleDbConnection dbConnection, string _last_name, string _first_name, string _fathers_name, int _country_id, DateTime _dob)
        {
            string query = "INSERT INTO player_info (last_name, first_name, fathers_name, country, date_of_birth) " +
              "VALUES (@last_name, @first_name, @father_name, @country_id, @date_of_birth)";
            OleDbCommand cmd = new OleDbCommand(query, dbConnection);
            cmd.Parameters.AddWithValue("@last_name", _last_name);
            cmd.Parameters.AddWithValue("@first_name", _first_name);
            cmd.Parameters.AddWithValue("@fathers_name", _fathers_name);
            cmd.Parameters.AddWithValue("@country_id", _country_id);
            cmd.Parameters.AddWithValue("@date_of_birth", _dob);
            cmd.ExecuteNonQuery();
        }

        private int getThisId(OleDbConnection dbConnection)
        {
            int thisID;

            string query = "SELECT player_id FROM player_info WHERE (((player_info.last_name) = @last_name) AND ((player_info.first_name) = @first_name) AND ((player_info.fathers_name) = @fathers_name))";
            OleDbCommand cmd = new OleDbCommand(query, dbConnection);
            cmd = new OleDbCommand("SELECT @@IDENTITY", dbConnection);
            thisID = (int)(cmd.ExecuteScalar());

            return thisID;
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

        private void addFootballInfo(OleDbConnection dbConnection, int _thisID, int _position_id, int _height, 
            string _kicking_leg, int _price, int _salary, int _transfer_status_id)
        {
            string query = "INSERT INTO football_info (player_id, [position], [height], [kicking_leg], [price], [salary], [transfer_status]) " +
              "VALUES (@thisID, @position_name, @height, @kicking_leg, @price, @salary, @transfer_status_name)";
            OleDbCommand cmd = new OleDbCommand(query, dbConnection);
            cmd.Parameters.AddWithValue("@thisID", _thisID);
            cmd.Parameters.AddWithValue("@position_name", _position_id);
            cmd.Parameters.AddWithValue("@height", _height);
            cmd.Parameters.AddWithValue("@kicking_leg", _kicking_leg);
            cmd.Parameters.AddWithValue("@price", _price);
            cmd.Parameters.AddWithValue("@salary", _salary);
            cmd.Parameters.AddWithValue("@transfer_status_name", _transfer_status_id);
            cmd.ExecuteNonQuery();
        }

        private int addToTableNationalLeague(OleDbConnection dbConnection, string _national_league_name)
        {
            int _national_league_id;
            string query = "SELECT [league_id] FROM [national_league] WHERE [league_name] = @national_league_name";
            using (OleDbCommand cmd = new OleDbCommand(query, dbConnection))
            {
                cmd.Parameters.AddWithValue("@national_league_name", _national_league_name);
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    _national_league_id = Convert.ToInt32(result);
                }
                else
                {
                    query = "INSERT INTO national_league (league_name) VALUES (@national_league_name)";
                    cmd.CommandText = query;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@national_league_name", _national_league_name);
                    cmd.ExecuteNonQuery();

                    // получаем id только что добавленной записи
                    cmd.CommandText = "SELECT @@IDENTITY";
                    _national_league_id = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            return _national_league_id;
        }

        private int addToTableCity(OleDbConnection dbConnection, string _city_name, int _countryClub_id)
        {
            int city_id;
            string query = "SELECT [city_id] FROM [city] WHERE [city_name] = @city_name";
            using (OleDbCommand cmd = new OleDbCommand(query, dbConnection))
            {
                cmd.Parameters.AddWithValue("@city_name", _city_name);
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    city_id = Convert.ToInt32(result);
                }
                else
                {
                    query = "INSERT INTO city (city_name, country) VALUES (@city_name, @countryClub_id)";
                    cmd.CommandText = query;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@city_name", _city_name);
                    cmd.Parameters.AddWithValue("@countryClub_id", _countryClub_id);
                    cmd.ExecuteNonQuery();

                    // получаем id только что добавленной записи
                    cmd.CommandText = "SELECT @@IDENTITY";
                    city_id = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            return city_id;
        }

        private int addToTableClub(OleDbConnection dbConnection, string _club_name, int _city_id, int _national_laegue_id)
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
                    query = "INSERT INTO club (club_name, city, national_league) VALUES (@club_name, @city_id, @national_laegue_id)";
                    cmd.CommandText = query;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@city_name", _club_name);
                    cmd.Parameters.AddWithValue("@city_id", _city_id);
                    cmd.Parameters.AddWithValue("@national_laegue_id", _national_laegue_id);
                    cmd.ExecuteNonQuery();

                    // получаем id только что добавленной записи
                    cmd.CommandText = "SELECT @@IDENTITY";
                    club_id = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            return club_id;
        }

        private void addPlayer_stats(OleDbConnection dbConnection, string _season, int _club_id, int _played_matches, int _goals, 
            int _assists, int _thisID)
        {
            string query = "INSERT INTO player_stats ([season], [club], [played_matches], [goals], [assists], [player]) " +
              "VALUES (@season, @club_id, @played_matches, @goals, @assists, @thisID)";
            OleDbCommand cmd = new OleDbCommand(query, dbConnection);
            cmd.Parameters.AddWithValue("@season", _season);
            cmd.Parameters.AddWithValue("@club_id", _club_id);
            cmd.Parameters.AddWithValue("@played_matches", _played_matches);
            cmd.Parameters.AddWithValue("@goals", _goals);
            cmd.Parameters.AddWithValue("@assists", _assists);
            cmd.Parameters.AddWithValue("@thisID", _thisID);
            cmd.ExecuteNonQuery();
        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            // Создаем соединение
            string connectionToDB = "provider=Microsoft.Jet.OLEDB.4.0;Data Source=TransferMarket.mdb"; //Строка соединение
            OleDbConnection dbConnection = new OleDbConnection(connectionToDB); //создаём соединение
            dbConnection.Open();

            //Проверяем, все ли необходимые поля заполнены
            if (last_name_textBox.Text == "" ||
                first_name_textBox.Text == "" ||
                country_textBox.Text == "" ||
                region_comboBox.Text == "" ||
                kicking_leg_comboBox.Text == "" ||
                transfer_status_comboBox.Text == "" ||
                position_comboBox.Text == "" ||
                season_comboBox.Text == "" ||
                club_textBox.Text == "" ||
                city_textBox.Text == "" ||
                national_league_textBox.Text == "" ||
                countryForClub_textBox.Text == "" ||
                comboBox1.Text == "")
            {
                MessageBox.Show("Не все данны введены!", "Внимание!");
                dbConnection.Close();
                return;
            }
            else
            {
                //Считаем данные
                string last_name = last_name_textBox.Text;
                string first_name = first_name_textBox.Text;
                string fathers_name = father_name_textBox.Text;
                string country_name = country_textBox.Text;
                string region_name = region_comboBox.Text;
                DateTime dob = dob_dateTimePicker.Value;
                int height = (int)height_numericUpDown.Value;
                string kicking_leg = kicking_leg_comboBox.Text;
                string transfer_status_name = transfer_status_comboBox.Text;
                string position_name = position_comboBox.Text;
                int price = (int)price_numericUpDown.Value;
                int salary = (int)salary_numericUpDown.Value;
                string season = season_comboBox.Text;
                string club_name = club_textBox.Text;
                string city_name = city_textBox.Text;
                string national_league_name = national_league_textBox.Text;
                string countryClub_name = countryForClub_textBox.Text;
                string regionClub = comboBox1.Text;
                int played_matches = (int)played_matches_numericUpDown.Value;
                int goals = (int)goals_numericUpDown.Value;
                int assists = (int)assists_numericUpDown.Value;
                

                //Проверка, существует ли такой же игрок в БД
                string queryCheckForPlayer = "SELECT COUNT(*) FROM player_info WHERE (((player_info.last_name) = @last_name) AND ((player_info.first_name) = @first_name) AND ((player_info.fathers_name) = @fathers_name))";
                OleDbCommand cmd = new OleDbCommand(queryCheckForPlayer, dbConnection);
                cmd.Parameters.AddWithValue("@last_name", last_name);
                cmd.Parameters.AddWithValue("@first_name", first_name);
                cmd.Parameters.AddWithValue("@fathers_name", fathers_name);
                int count = (int)cmd.ExecuteScalar();

                if (count > 0) // Такой игрок уже существует.
                {
                    MessageBox.Show("Такой игрок уже существует!", "Внимание!");
                }
                else  // Можно добавить.
                {
                    int region_id = addToTableRegion(dbConnection, region_name);
                    int country_id = addToTableCountry(dbConnection, country_name, region_id);
                    addPlayerInfo(dbConnection, last_name, first_name, fathers_name, country_id, dob);
                    int thisID = getThisId(dbConnection);

                    int transfer_status_id = addToTableTransferSatus(dbConnection, transfer_status_name);
                    int position_id = addToTablePosition(dbConnection, position_name);
                    addFootballInfo(dbConnection, thisID, position_id, height, kicking_leg, price, salary, transfer_status_id);
                    
                    int national_laegue_id = addToTableNationalLeague(dbConnection, national_league_name);
                    int regionClub_id = addToTableRegion(dbConnection, regionClub);
                    int countryClub_id = addToTableCountry(dbConnection, countryClub_name, regionClub_id);
                    int city_id = addToTableCity(dbConnection, city_name, countryClub_id);
                    int club_id = addToTableClub(dbConnection, club_name, city_id, national_laegue_id);
                    addPlayer_stats(dbConnection, season, club_id, played_matches, goals, assists, thisID);

                    MessageBox.Show("Данные добавлены!", "Внимание!");
                    this.Close();
                }

                dbConnection.Close();
            }
        }
    }
}

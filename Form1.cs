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
    public partial class Form1 : Form
    {
        More_InfoAboutPlayer more_InfoAboutPlayer;

         public Form1()
        {
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
            string query = "SELECT player_info.last_name, player_info.first_name, player_info.fathers_name, country.country_name, player_info.date_of_birth " +
                            "FROM country INNER JOIN player_info ON country.country_id = player_info.country ";
            OleDbCommand dbCommand = new OleDbCommand(query, dbConnection); //команда
            OleDbDataReader dbReader = dbCommand.ExecuteReader();   //считываем данные

            //Проверяем данные 
            if (dbReader.HasRows == false)
            {
                MessageBox.Show("Данные не найдены!", "Ошибка!");
            }
            else
            {
                dataGridView1.Rows.Clear();
                //Запищем данные в таблицу формы
                while (dbReader.Read())
                {
                    dataGridView1.Rows.Add(dbReader["last_name"], dbReader["first_name"], dbReader["fathers_name"], dbReader["country_name"], dbReader["date_of_birth"]);
                }
            }

            //Закрываем соединение
            dbReader.Close();
            dbConnection.Close();
        }

        /*Загрузить*/
        private void button1_Click(object sender, EventArgs e)
        {
            load();
        }

        /*Добваить*/
        private void button2_Click(object sender, EventArgs e)
        {
            AddForm addForm = new AddForm();
            addForm.Show();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Программа для работы с реляционной базой данных.\n\n" +
                "Эта база данных хранит в себе футболистов на трансферном рынке, их футбольных характеристик " +
                "и статистик, которые с ними связаны.\n\nПрограмма позволяет:\n\t-добавлять записи в БД;\n\t-обновлять записи в БД;\n" +
                "\t-удалять записи из БД;\n\t-загружать актуальную БД;\n\nПрограмма написана в рамках курсовой работы по предмету" +
                " \"Проектирование баз данных\".\n\n\tАвтор: Бобохонов Амин.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        //Подробнее
        private void button5_Click(object sender, EventArgs e)
        {
            if(dataGridView1.SelectedRows.Count != 1)
            {
                MessageBox.Show("Выберите одну строку, чтобы увидеть больше информации об игроке!", "Внимание!");
            }
            else
            {
                //объект more_InfoAboutPlayer делаем глобальным "переменным"
                int id = dataGridView1.CurrentCell.RowIndex + 1; //выбираем id выбранной строчки
                more_InfoAboutPlayer = new More_InfoAboutPlayer(id);   //передаём id в объект more_InfoAboutPlayer, чтоб по id работать сдругими таблицами
                more_InfoAboutPlayer.Show();
            }
        }
    }
}
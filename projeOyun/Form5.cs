
//Öğrenci Numarası: B231200061
//Adı - Soyadı: Hatice Hüsna Özdemir
//Bölüm: Bilişim Sistemleri Mühendisliği
//Ders: Nesneye Dayalı Programlama

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using System.IO; // Dosya işlemleri için gerekli

namespace projeOyun
{
    public partial class Form5 : Form
    {
        private const string HighScoresFilePath = "HighScores.txt";
        public Form5()
        {
            InitializeComponent();
            //LoadHighScores();
            LoadHighScoresFromFile(); // Skorları dosyadan yükle
            LoadHighScoresToTextBoxes();
        }

        // Dosyadan skorları yükle
        private void LoadHighScoresFromFile()
        {
            if (File.Exists(HighScoresFilePath))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(HighScoresFilePath))
                    {
                        Form2.HighScores.Clear(); // Listeyi temizle
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var parts = line.Split(':');
                            if (parts.Length == 2 && int.TryParse(parts[1], out int score))
                            {
                                Form2.HighScores.Add((parts[0], score));
                            }
                        }

                        // Listeyi büyükten küçüğe sırala
                        Form2.HighScores = Form2.HighScores.OrderByDescending(h => h.Score).ToList();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Skorlar yüklenirken bir hata oluştu: " + ex.Message);
                }
            }
        }

        // Skorları TextBox'lara yükle
        private void LoadHighScoresToTextBoxes()
        {
            TextBox[] textBoxes = { textBox1, textBox2, textBox3, textBox4, textBox5 };

            for (int i = 0; i < textBoxes.Length; i++)
            {
                if (i < Form2.HighScores.Count)
                {
                    var (playerName, score) = Form2.HighScores[i];
                    textBoxes[i].Text = $"{playerName} - {score}";
                }
                else
                {
                    textBoxes[i].Text = ""; // Boş bırak
                }
            }
        }
        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}

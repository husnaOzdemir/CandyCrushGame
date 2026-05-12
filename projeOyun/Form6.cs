
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
using System.IO; // Dosya işlemleri için gerekli

namespace projeOyun
{
    
    public partial class Form6 : Form
    {
        private const string HighScoresFilePath = "HighScores.txt";
        public Form6()
        {
            InitializeComponent();
        }
        private void Form6_Load(object sender, EventArgs e)
        {
            btnHome.Click += pictureBox1_Click;
            btnRetry.Click += pictureBox2_Click;
        }

        private int currentScore; // Oyuncunun puanı

        public void SetScore(int score)
        {
            lblScore.Text = score.ToString();
            currentScore = score;
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            SaveCurrentScore(); // Puanı kaydet
            Form1 form1 = new Form1(); // Ana menü
            form1.Show();
            this.Close();
        }


        private string playerName; // Oyuncu adını saklamak için

        public void SetPlayerName(string name)
        {
            playerName = name; // Oyuncu adını sakla
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(playerName); // Aynı oyuncu adıyla yeni Form2
            form2.Show();
            this.Close();
        }


        // Skoru kaydet
        private void SaveCurrentScore()
        {
            // Oyuncunun adını ve mevcut skorunu listeye ekle
            Form2.HighScores.Add((playerName, currentScore));

            // Listeyi büyükten küçüğe sırala
            Form2.HighScores = Form2.HighScores.OrderByDescending(h => h.Score).ToList();

            // Skorları dosyaya yaz
            SaveHighScoresToFile();
        }

        // Skorları dosyaya kaydet
        private void SaveHighScoresToFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(HighScoresFilePath))
                {
                    foreach (var (playerName, score) in Form2.HighScores)
                    {
                        writer.WriteLine($"{playerName}:{score}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Skorlar kaydedilirken bir hata oluştu: " + ex.Message);
            }
        }



    }
}

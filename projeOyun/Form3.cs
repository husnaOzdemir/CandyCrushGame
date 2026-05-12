
//Öğrenci Numarası: B231200061
//Adı - Soyadı: Hatice Hüsna Özdemir
//Bölüm: Bilişim Sistemleri Mühendisliği
//Ders: Nesneye Dayalı Programlama

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection.Emit;
using System.Windows.Forms;
using System.IO; // Dosya işlemleri için gerekli

namespace projeOyun
{
    public partial class Form3 : Form
    {
        public event Action ResumeGame; // Oyun devam etme olayı
        private const string HighScoresFilePath = "HighScores.txt";

        public Form3()
        {
            InitializeComponent();

            this.KeyPreview = true; // Klavye girişlerini yakalamak için
            this.KeyDown += Form3_KeyDown; // KeyDown olayını tanımlayın


            // Devam etme resmine tıklama olayı
            pictureBoxPause.Click += (sender, e) =>
            {
                ResumeGame?.Invoke(); // Olayı tetikle
                this.Close(); // Form3'ü kapat
            };
            pictureBoxPause.MouseEnter += PictureBoxPause_MouseEnter;
            pictureBoxPause.MouseLeave += PictureBoxPause_MouseLeave;
        }

        // Ana menüye dönme resmi
        private void pictureBoxHome_Click(object sender, EventArgs e)
        {
            SaveScore(); // Skoru kaydet

            // Ana menüye dönmek için Form1'i aç
            Form1 form1 = new Form1();
            form1.Show();
            Application.OpenForms["Form2"]?.Close(); // Form2'yi kapat
            this.Close(); // Form3'ü kapat
        }

        // Skoru kaydeden metot
        private void SaveScore()
        {
            if (Application.OpenForms["Form2"] is Form2 form2)
            {
                // Oyuncunun adını ve mevcut skorunu listeye ekle
                Form2.HighScores.Add((form2.PlayerName, form2.Score));

                // Listeyi büyükten küçüğe sırala
                Form2.HighScores = Form2.HighScores.OrderByDescending(h => h.Score).ToList();

                // Skorları dosyaya yaz
                SaveHighScoresToFile();
            }
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

        // Puanı güncellemek için bir yöntem
        public void UpdateScore(int score)
        {
            lblScore.Text = score.ToString(); // Puanı Label'da göster
        }

        //// Skoru kaydetmek için yeni bir metot ekleyin
        //private void SaveScore()
        //{
        //    if (Application.OpenForms["Form2"] is Form2 form2)
        //    {
        //        Form2.HighScores.Add((form2.PlayerName, form2.Score)); // Oyuncu adı ve skoru ekle
        //        Form2.HighScores = Form2.HighScores.OrderByDescending(h => h.Score).ToList(); // Skorları sırala
        //    }
        //}

        private void Form3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.P) // 'P' tuşuna basıldığında
            {
                ResumeGame?.Invoke(); // Form2'de oyunu devam ettir
                this.Close(); // Form3'ü kapat
            }
        }

        private void PictureBoxPause_MouseEnter(object sender, EventArgs e)
        {
            pictureBoxPause.Cursor = Cursors.Hand; // İmleci tıklanabilir şekle değiştir
        }

        // pictureBox5 üzerinden ayrıldığında orijinal boyuta döndür
        private void PictureBoxPause_MouseLeave(object sender, EventArgs e)
        {
            pictureBoxPause.Cursor = Cursors.Default; // İmleci varsayılan yap
        }

        private void PictureBoxHome_MouseEnter(object sender, EventArgs e)
        {
            pictureBoxHome.Cursor = Cursors.Hand; // İmleci tıklanabilir şekle değiştir
        }

        // pictureBox5 üzerinden ayrıldığında orijinal boyuta döndür
        private void PictureBoxHome_MouseLeave(object sender, EventArgs e)
        {
            pictureBoxHome.Cursor = Cursors.Default; // İmleci varsayılan yap
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // Form yüklendiğinde yapılacak işlemler
        }

        private void lblScore_Click(object sender, EventArgs e)
        {
            // lblScore tıklama olayı
        }
    }
}

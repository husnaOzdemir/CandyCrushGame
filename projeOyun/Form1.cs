
//Öğrenci Numarası: B231200061
//Adı - Soyadı: Hatice Hüsna Özdemir
//Bölüm: Bilişim Sistemleri Mühendisliği
//Ders: Nesneye Dayalı Programlama

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace projeOyun
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Başla butonunu başlangıçta pasif yap
            btnStart.Enabled = false;

            // TextBox değişikliğini dinle
            txtPlayerName.TextChanged += TxtPlayerName_TextChanged;

            // Enter tuşu ile başlatma işlevi
            this.KeyPreview = true; // Tuş girişlerini yakalamak için
            this.KeyDown += Form1_KeyDown;

            label2.Parent = pictureBox4; // Label'i PictureBox'un üstüne koyar.
            label2.BackColor = Color.Transparent; // Şeffaf arka plan ayarlar.

            // Butonun hover ve cursor olayları
            btnStart.MouseEnter += BtnStart_MouseEnter;
            btnStart.MouseLeave += BtnStart_MouseLeave;

            // pictureBox5'e olaylar ekle
            pictureBox5.Click += PictureBox5_Click;
            pictureBox5.MouseEnter += PictureBox5_MouseEnter;
            pictureBox5.MouseLeave += PictureBox5_MouseLeave;

            // Kenarları yuvarlak yapma
            MakeControlRounded(btnStart);

            pictureBox3.Click += PictureBox3_Click;
            pictureBox3.MouseEnter += PictureBox3_MouseEnter;
            pictureBox3.MouseLeave += PictureBox3_MouseLeave;

        }

        // pictureBox3 tıklama olayı
        private void PictureBox3_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5(); // Form5 örneği oluştur
            form5.StartPosition = FormStartPosition.CenterScreen; // Ortada açılması için
            form5.Size = new Size(1020, 783);
            form5.Show(); // Form5'i göster
        }

        private void PictureBox3_MouseEnter(object sender, EventArgs e)
        {
            pictureBox3.Size = new Size(pictureBox3.Width + 10, pictureBox3.Height + 10); // Hafif büyüt
            pictureBox3.Cursor = Cursors.Hand; // İmleci tıklanabilir şekle değiştir
        }

        // pictureBox5 üzerinden ayrıldığında orijinal boyuta döndür
        private void PictureBox3_MouseLeave(object sender, EventArgs e)
        {
            pictureBox3.Size = new Size(pictureBox3.Width - 10, pictureBox3.Height - 10); // Orijinal boyuta dön
            pictureBox3.Cursor = Cursors.Default; // İmleci varsayılan yap
        }

        // Buton üzerine gelindiğinde imleci değiştir
        private void BtnStart_MouseEnter(object sender, EventArgs e)
        {
            if (btnStart.Enabled) // Buton etkinse
            {
                btnStart.Cursor = Cursors.Hand; // İmleci el simgesine değiştir
            }
        }

        // Buton üzerinden ayrıldığında varsayılan imleci geri yükle
        private void BtnStart_MouseLeave(object sender, EventArgs e)
        {
            btnStart.Cursor = Cursors.Default; // İmleci varsayılan yap
        }

        // pictureBox5'e tıklanıldığında Form4'ü aç
        private void PictureBox5_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4(); // Form4 örneği oluştur
            form4.Size = new Size(1020, 783); // Formun boyutlarını belirle
            form4.StartPosition = FormStartPosition.CenterScreen;
            form4.Show(); // Form4'ü göster
        }

        // pictureBox5 üzerine gelindiğinde büyüt ve imleci değiştir
        private void PictureBox5_MouseEnter(object sender, EventArgs e)
        {
            pictureBox5.Size = new Size(pictureBox5.Width + 10, pictureBox5.Height + 10); // Hafif büyüt
            pictureBox5.Cursor = Cursors.Hand; // İmleci tıklanabilir şekle değiştir
        }

        // pictureBox5 üzerinden ayrıldığında orijinal boyuta döndür
        private void PictureBox5_MouseLeave(object sender, EventArgs e)
        {
            pictureBox5.Size = new Size(pictureBox5.Width - 10, pictureBox5.Height - 10); // Orijinal boyuta dön
            pictureBox5.Cursor = Cursors.Default; // İmleci varsayılan yap
        }


        // TextBox değişince butonun durumunu kontrol et
        private void TxtPlayerName_TextChanged(object sender, EventArgs e)
        {
            btnStart.Enabled = !string.IsNullOrWhiteSpace(txtPlayerName.Text);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Enter tuşuna basıldığında
            if (e.KeyCode == Keys.Enter && btnStart.Enabled)
            {
                StartGame();
            }
        }

        private void StartGame()
        {
            // Yeni bir Form2 aç ve kullanıcı adını ilet
            Form2 gameForm = new Form2(txtPlayerName.Text);
            gameForm.Show();
            this.Hide(); // Mevcut formu gizle
        }

        private void MakeControlRounded(Control control)
        {
            int cornerRadius = 50; // Yuvarlak köşe yarıçapı

            control.Paint += (sender, e) =>
            {
                GraphicsPath path = new GraphicsPath();
                path.AddArc(0, 0, cornerRadius, cornerRadius, 180, 90);
                path.AddArc(control.Width - cornerRadius, 0, cornerRadius, cornerRadius, 270, 90);
                path.AddArc(control.Width - cornerRadius, control.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
                path.AddArc(0, control.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
                path.CloseFigure();

                control.Region = new Region(path);
            };

            control.Invalidate();
        }

        private void txtPlayerName_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }
    }
}

//Öğrenci Numarası: B231200061
//Adı - Soyadı: Hatice Hüsna Özdemir
//Bölüm: Bilişim Sistemleri Mühendisliği
//Ders: Nesneye Dayalı Programlama

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO; // Dosya işlemleri için gerekli

namespace projeOyun
{
    public partial class Form2 : Form
    {
        private Timer countdownTimer; // Sayaç için Timer
        private Timer matchCheckTimer; // Otomatik eşleşme kontrolü için Timer
        private int remainingTime = 90; // Başlangıç süresi (100 saniye)
        private Button[,] tiles; // Kareleri tutacak dizi
        private int rows = 5; // Satır sayısı
        private int cols = 10; // Sütun sayısı
        private ResourceImage[] resourceImages; // Resim nesnelerini tutan dizi
        private int matchCount = 0; // Eşleşme sayısını izler
        private Random random = new Random(); // Rastgele değerler için
        private int score = 0; // Toplam puan
        private Button draggedTile = null; // Sürüklenen kutucuk
        private Button targetTile = null; // Hedef kutucuk
        private Point draggedTileStartLocation; // Sürüklenen kutucuğun başlangıç konumu
        public static List<(string PlayerName, int Score)> HighScores = new List<(string, int)>();
        private const string HighScoresFilePath = "HighScores.txt"; // Skorları saklamak için dosya yolu


        public Form2(string playerName)
        {
            InitializeComponent();

            // Form Başlığı
            this.Text = "Patlatma Oyunu";

            lblPlayerName.Text = playerName; // Oyuncu adını göster

            // Timer Başlatma
            InitializeTimers();

            // Resim nesnelerini yükle
            LoadResourceImages();

            // Panel üzerinde Grid Oluştur
            CreateGrid(panel1);

            // Form yeniden boyutlandığında karelerin boyutunu dinamik olarak ayarla
            this.Resize += Form2_Resize;

            this.KeyPreview = true; // Tuş girişlerini yakalamak için
            this.KeyDown += Form2_KeyDown; // KeyDown olayını tanımla

        }

        // Form2.cs
        public string PlayerName => lblPlayerName.Text; // Oyuncu adını döndüren bir property
        public int Score => score; // Skoru döndüren bir property

        private bool isForm6Opened = false; // Form6'nın açılıp açılmadığını kontrol etmek için

        private bool isGameActive = true; // Oyun aktif mi?

        private void InitializeTimers()
        {
            // Süre sayacı
            countdownTimer = new Timer
            {
                Interval = 1000 // 1 saniyede bir çalışır
            };
            countdownTimer.Tick += (s, e) =>
            {
                if (!isGameActive) return; // Oyun kapatıldıysa timer çalışmasın

                remainingTime--;
                lblTime.Text = remainingTime.ToString();

                if (remainingTime <= 0 && !isForm6Opened)
                {
                    isForm6Opened = true;

                    // Form6'yı aç
                    Form6 form6 = new Form6();
                    form6.SetScore(score); // Puanı gönder
                    form6.SetPlayerName(lblPlayerName.Text); // Oyuncu adını gönder
                    form6.Show();

                    // Mevcut formu kapat
                    this.Close();
                }
            };
            countdownTimer.Start();

            // Otomatik eşleşme kontrolü için Timer
            matchCheckTimer = new Timer
            {
                Interval = 500 // Her yarım saniyede bir eşleşme kontrolü yapar
            };
            matchCheckTimer.Tick += (s, e) =>
            {
                if (isGameActive) CheckAndHandleMatches(); // Oyun devam ediyorsa kontrol yap
            };
            matchCheckTimer.Start();
        }


        private void LoadResourceImages()
        {
            // Resim nesnelerini yükle
            resourceImages = new ResourceImage[]
            {
                new ResourceImage(Properties.Resources.kırmızıseker),
                new ResourceImage(Properties.Resources.sarısekill),
                new ResourceImage(Properties.Resources.mavisekerr),
                new ResourceImage(Properties.Resources.yesillseker),
                new ResourceImage(Properties.Resources.roket),
                new ResourceImage(Properties.Resources.yandan),
                new ResourceImage(Properties.Resources.dikey),
                new ResourceImage(Properties.Resources.dinamit),
                new ResourceImage(Properties.Resources.bumbum),
            };
        }

        private void EndGame()
        {
            // Süre dolduğunda veya oyun bittiğinde skoru ekle
            SaveCurrentScore();

            // Form5'i göster
            Form5 form5 = new Form5();
            form5.Show();

            // Mevcut formu kapat
            this.Close();
        }


        private void CreateGrid(Panel panel)
        {
            tiles = new Button[rows, cols]; // Kareleri tutan 2D dizi
            int tileWidth = panel.Width / cols; // Kare genişliği
            int tileHeight = panel.Height / rows; // Kare yüksekliği

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    Button tile = new Button
                    {
                        Size = new Size(tileWidth, tileHeight),
                        Location = new Point(col * tileWidth, row * tileHeight),
                        BackColor = Color.LightGray // Orijinal arka plan rengi
                    };

                    // Rastgele bir resim nesnesi seç ve uygula
                    tile.BackgroundImage = GetRandomImage().Image;
                    tile.BackgroundImageLayout = ImageLayout.Stretch;

                    // Fare olayları
                    tile.MouseDown += Tile_MouseDown;
                    tile.MouseUp += Tile_MouseUp;

                    // Hover efekti için MouseEnter ve MouseLeave olayları
                    tile.MouseEnter += (s, e) =>
                    {
                        tile.BackColor = Color.AliceBlue; // Oyun atmosferine uygun hover rengi
                    };

                    tile.MouseLeave += (s, e) =>
                    {
                        tile.BackColor = Color.LightGray; // Orijinal renk
                    };

                    // Diziye ekle
                    tiles[row, col] = tile;

                    // Panel içine ekle
                    panel.Controls.Add(tile);
                }
            }
        }

        private ResourceImage GetRandomImage()
        {
            // Çok nadir "bumbum" resmini üret
            if (random.Next(100) < 2) // %2 ihtimalle "bumbum" resmi
            {
                return resourceImages[8]; // Bumbum resmi
            }

            // Çok nadir "dinamit" resmini üret
            if (random.Next(100) < 2) // %2 ihtimalle "dinamit" resmi
            {
                return resourceImages[7]; // Dinamit resmi
            }

            // Çok nadir "dikey" resmini üret
            if (random.Next(100) < 3) // %3 ihtimalle "dikey" resmi
            {
                return resourceImages[6]; // Dikey resmi
            }

            // Çok nadir yandan resmini üret
            if (random.Next(100) < 3) // %3 ihtimalle "yandan" resmi
            {
                return resourceImages[5]; // Yandan resmi
            }

            // 2-3 patlatmada bir roket oluştur
            if (matchCount >= 6 && random.Next(7) == 0)
            {
                matchCount = 0; // Sayacı sıfırla
                return resourceImages[4]; // Roket resmi
            }

            // Normal resimlerden birini döndür
            return resourceImages[random.Next(4)];
        }


        private void Tile_MouseDown(object sender, MouseEventArgs e)
        {
            draggedTile = sender as Button;

            if (draggedTile != null)
            {
                draggedTileStartLocation = draggedTile.Location;
            }
        }

        private void Tile_MouseUp(object sender, MouseEventArgs e)
        {
            if (draggedTile != null)
            {
                targetTile = GetTileAtPoint(panel1, panel1.PointToClient(Cursor.Position));

                if (targetTile != null && IsAdjacent(draggedTile, targetTile))
                {
                    if (IsSpecialResourceImage(draggedTile.BackgroundImage))
                    {
                        Image tempImage = draggedTile.BackgroundImage;
                        draggedTile.BackgroundImage = targetTile.BackgroundImage;
                        targetTile.BackgroundImage = tempImage;

                        if (!CreatesMatch(draggedTile) && !CreatesMatch(targetTile))
                        {
                            targetTile.BackgroundImage = draggedTile.BackgroundImage;
                            draggedTile.BackgroundImage = tempImage;
                        }
                        else
                        {
                            CheckAndHandleMatches();
                            score += 20;
                            lblScore.Text = score.ToString();

                        }
                    }
                    else
                    {
                        if (draggedTile.BackgroundImage == resourceImages[8].Image) // Bumbum
                        {
                            score += 40; // Bumbum puanı
                            lblScore.Text = score.ToString();
                            PatlatAyniResimleri(targetTile.BackgroundImage);
                            draggedTile.BackgroundImage = null;
                        }
                        else
                        {
                            Image tempImage = draggedTile.BackgroundImage;
                            draggedTile.BackgroundImage = targetTile.BackgroundImage;
                            targetTile.BackgroundImage = tempImage;

                            if (targetTile.BackgroundImage == resourceImages[6].Image) // Dikey
                            {
                                score += 30; // Dik puanı
                                lblScore.Text = score.ToString();
                                PatlatSutundakiTumResimler(targetTile);
                                targetTile.BackgroundImage = null;
                            }
                            else if (targetTile.BackgroundImage == resourceImages[5].Image) // Yandan
                            {
                                score += 30; // Yandan puanı
                                lblScore.Text = score.ToString();
                                PatlatSatirdakiTumResimler(targetTile);
                                targetTile.BackgroundImage = null;
                            }
                            else if (targetTile.BackgroundImage == resourceImages[4].Image) // Roket
                            {
                                score += 25; // Roket puanı
                                lblScore.Text = score.ToString();
                                PatlatRastgeleResim();
                                targetTile.BackgroundImage = null;
                            }
                            else if (targetTile.BackgroundImage == resourceImages[7].Image) // Dinamit
                            {
                                score += 35; // Dinamit puanı
                                lblScore.Text = score.ToString();
                                PatlatEtrafindakiResimler(targetTile);
                                targetTile.BackgroundImage = null;
                            }
                            else
                            {
                                CheckAndHandleMatches();
                            }
                        }
                    }
                }

                draggedTile = null;
                targetTile = null;
            }
        }

        private bool IsSpecialResourceImage(Image image)
        {
            return image == resourceImages[0].Image || // Kırmızı şeker
                   image == resourceImages[1].Image || // Sarı şeker
                   image == resourceImages[2].Image || // Mavi şeker
                   image == resourceImages[3].Image;   // Yeşil şeker
        }

        private bool CreatesMatch(Button tile)
        {
            int row = tile.Location.Y / tile.Height;
            int col = tile.Location.X / tile.Width;

            Image currentImage = tile.BackgroundImage;
            if (currentImage == null) return false;

            // Yatay eşleşme kontrolü
            int horizontalCount = 1;
            for (int c = col - 1; c >= 0 && tiles[row, c].BackgroundImage == currentImage; c--) horizontalCount++;
            for (int c = col + 1; c < cols && tiles[row, c].BackgroundImage == currentImage; c++) horizontalCount++;

            // Dikey eşleşme kontrolü
            int verticalCount = 1;
            for (int r = row - 1; r >= 0 && tiles[r, col].BackgroundImage == currentImage; r--) verticalCount++;
            for (int r = row + 1; r < rows && tiles[r, col].BackgroundImage == currentImage; r++) verticalCount++;

            return horizontalCount >= 3 || verticalCount >= 3;
        }

        private void PatlatSutundakiTumResimler(Button button)
        {
            int col = button.Location.X / button.Width; // Sütun indeksini bul

            for (int row = 0; row < rows; row++)
            {
                if (tiles[row, col].BackgroundImage != resourceImages[6].Image) // "dikey" değilse
                {
                    tiles[row, col].BackgroundImage = null; // Resmi patlat
                }
            }
        }

        private void PatlatSatirdakiTumResimler(Button button)
        {
            int row = button.Location.Y / button.Height; // Satır indeksini bul

            for (int col = 0; col < cols; col++)
            {
                if (tiles[row, col].BackgroundImage != resourceImages[5].Image) // "yandan" değilse
                {
                    tiles[row, col].BackgroundImage = null; // Resmi patlat
                }
            }
        }

        private void PatlatEtrafindakiResimler(Button button)
        {
            int row = button.Location.Y / button.Height; // Satır indeksini bul
            int col = button.Location.X / button.Width; // Sütun indeksini bul

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue; // Dinamitin kendisini patlatma
                    int newRow = row + i;
                    int newCol = col + j;

                    // Geçerli bir kutucuk olup olmadığını kontrol et
                    if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols)
                    {
                        tiles[newRow, newCol].BackgroundImage = null; // Kutucuğu patlat
                    }
                }
            }
        }

        private bool isPaused = false; // Oyunun durup durmadığını kontrol eder


        private void PatlatRastgeleResim()
        {
            var patlatilacakResimler = tiles.Cast<Button>().Where(t => t.BackgroundImage != null).ToList();

            if (patlatilacakResimler.Any())
            {
                var rastgeleResim = patlatilacakResimler[random.Next(patlatilacakResimler.Count)];
                rastgeleResim.BackgroundImage = null;
            }
        }

        private void PatlatAyniResimleri(Image targetImage)
        {
            if (targetImage == null) return;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (tiles[row, col].BackgroundImage == targetImage)
                    {
                        tiles[row, col].BackgroundImage = null; // Aynı resimleri patlat
                    }
                }
            }
        }

        private Button GetTileAtPoint(Panel panel, Point point)
        {
            foreach (Button tile in tiles)
            {
                if (tile.Bounds.Contains(point))
                {
                    return tile;
                }
            }
            return null;
        }

        private bool IsAdjacent(Button tile1, Button tile2)
        {
            int dx = Math.Abs(tile1.Location.X - tile2.Location.X);
            int dy = Math.Abs(tile1.Location.Y - tile2.Location.Y);

            return (dx == tile1.Width && dy == 0) || (dx == 0 && dy == tile1.Height);
        }

        private void CheckAndHandleMatches()
        {
            bool hasMatch;

            do
            {
                hasMatch = false;
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        if (CheckMatch(row, col))
                        {
                            HandleMatch(row, col);
                            hasMatch = true;
                            matchCount++;
                        }
                    }
                }

                FillEmptySpaces(); // Boşlukları doldur
            } while (hasMatch);

            lblScore.Text = score.ToString(); // Skoru her
        }


        private bool CheckMatch(int row, int col)
        {
            Image currentImage = tiles[row, col].BackgroundImage;
            if (currentImage == null || currentImage == resourceImages[4].Image || currentImage == resourceImages[5].Image || currentImage == resourceImages[6].Image)
            {
                return false; // Roket, yandan ve dikey resimleri eşleşmez
            }

            // Yatay eşleşme
            if (col <= cols - 3 &&
                tiles[row, col + 1].BackgroundImage == currentImage &&
                tiles[row, col + 2].BackgroundImage == currentImage)
            {
                return true;
            }

            // Dikey eşleşme
            if (row <= rows - 3 &&
                tiles[row + 1, col].BackgroundImage == currentImage &&
                tiles[row + 2, col].BackgroundImage == currentImage)
            {
                return true;
            }

            return false;
        }


        private Form3 pauseScreen; // Form3 için bir değişken

        private void HandleMatch(int row, int col)
        {
            Image currentImage = tiles[row, col].BackgroundImage;
            int matchPoints = 20; // Her eşleşme için puan

            // Yatay eşleşme
            if (col <= cols - 3 &&
                tiles[row, col + 1].BackgroundImage == currentImage &&
                tiles[row, col + 2].BackgroundImage == currentImage)
            {
                tiles[row, col].BackgroundImage = null;
                tiles[row, col + 1].BackgroundImage = null;
                tiles[row, col + 2].BackgroundImage = null;

                score += matchPoints; // Puan ekle
            }

            // Dikey eşleşme
            if (row <= rows - 3 &&
                tiles[row + 1, col].BackgroundImage == currentImage &&
                tiles[row + 2, col].BackgroundImage == currentImage)
            {
                tiles[row, col].BackgroundImage = null;
                tiles[row + 1, col].BackgroundImage = null;
                tiles[row + 2, col].BackgroundImage = null;

                score += matchPoints; // Puan ekle
            }

            lblScore.Text = score.ToString(); // Güncellenen skoru göster
            System.Threading.Thread.Sleep(140); // Görsel etki için gecikme
        }

        private void FillEmptySpaces()
        {
            for (int col = 0; col < cols; col++)
            {
                for (int row = rows - 1; row >= 0; row--)
                {
                    if (tiles[row, col].BackgroundImage == null)
                    {
                        for (int k = row; k > 0; k--)
                        {
                            tiles[k, col].BackgroundImage = tiles[k - 1, col].BackgroundImage;
                        }

                        tiles[0, col].BackgroundImage = GetRandomImage().Image;
                    }
                }
            }
        }

        private void Form2_Resize(object sender, EventArgs e)
        {
            if (tiles != null)
            {
                int tileWidth = panel1.Width / cols;
                int tileHeight = panel1.Height / rows;

                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        Button tile = tiles[row, col];
                        tile.Size = new Size(tileWidth, tileHeight);
                        tile.Location = new Point(col * tileWidth, row * tileHeight);
                    }
                }
            }
        }

        private void DisableTileInteraction()
        {
            foreach (Button tile in tiles)
            {
                tile.Enabled = false; // Tıklanabilirliği devre dışı bırak
            }
        }

        private void EnableTileInteraction()
        {
            foreach (Button tile in tiles)
            {
                tile.Enabled = true; // Tıklanabilirliği etkinleştir
            }
        }
        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.P) // 'P' tuşuna basıldığında
            {
                TogglePause();
            }
        }
        private void TogglePause()
        {
            if (!isPaused)
            {
                // Oyunu durdur
                countdownTimer.Stop();
                matchCheckTimer.Stop();
                DisableTileInteraction();
                isPaused = true;

                // Form3'ü göster
                if (pauseScreen == null || pauseScreen.IsDisposed)
                {
                    pauseScreen = new Form3();
                    pauseScreen.ResumeGame += ResumeGame; // Devam etme olayını dinle
                }

                pauseScreen.UpdateScore(score); // Puanı Form3'e gönder
                pauseScreen.Show(); // Form3'ü modal olmadan göster
            }
            else
            {
                if (pauseScreen != null && !pauseScreen.IsDisposed)
                {
                    pauseScreen.Close(); // Form3'ü kapat
                }

                ResumeGame(); // Oyunu devam ettir
            }
        }

        private void ResumeGame()
        {
            // Oyunu devam ettir
            countdownTimer.Start();
            matchCheckTimer.Start();
            EnableTileInteraction();
            isPaused = false;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            LoadHighScoresFromFile();
            // Form yüklendiğinde yapılacak işlemler
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void homeicon_Click(object sender, EventArgs e)
        {
            // Timer'ları durdur
            countdownTimer.Stop();
            matchCheckTimer.Stop();

            // Oyunu tamamen durdur (Gerekirse diğer temizleme işlemleri yapılabilir)
            DisableTileInteraction();

            // Skoru kaydet
            SaveCurrentScore();


            // Ana sayfayı aç
            Form1 form1 = new Form1(); // Yeni bir Form1 örneği oluştur
            form1.Show(); // Form1'i göster

            // Form2'yi kapat
            this.Close();
        }


        private void pauseicon_Click(object sender, EventArgs e)
        {
            TogglePause();
        }

        // Oyuncunun skoru kaydedilirken dosyaya da yazılır
        private void SaveCurrentScore()
        {
            // Oyuncunun adını ve mevcut skorunu listeye ekle
            Form2.HighScores.Add((lblPlayerName.Text, score));

            // Listeyi büyükten küçüğe sırala
            Form2.HighScores = Form2.HighScores.OrderByDescending(h => h.Score).ToList();

            // Skorları dosyaya yaz
            SaveHighScoresToFile();
        }

        // Skorları dosyaya kaydeden metot
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

        // Dosyadan skorları yükleyen metot
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


        private void lblPlayerName_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}

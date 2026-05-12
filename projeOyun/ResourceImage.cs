
//Öğrenci Numarası: B231200061
//Adı - Soyadı: Hatice Hüsna Özdemir
//Bölüm: Bilişim Sistemleri Mühendisliği
//Ders: Nesneye Dayalı Programlama

using System.Drawing;

namespace projeOyun
{
    public interface IResourceImage
    {
        Image Image { get;} // Get ve Set metotları
    }

    public class ResourceImage : IResourceImage
    {
        public Image Image { get;} // Get ve Set metotları

        public ResourceImage(Image image)
        {
            Image = image;
        }
    }
}

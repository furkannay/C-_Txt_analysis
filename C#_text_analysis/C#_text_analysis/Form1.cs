using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C__text_analysis
{
    public partial class Form1 : Form
    {
        private TextBox textBox1;
        private Button buttonAnalizEt;
        private Button buttonTemizle;
        private Label labelAnalizSonuclari;

        // Analiz sonuçlarını tutmak için bir sözlük
        private Dictionary<string, string> analizSonuclari = new Dictionary<string, string>
        {
            {"Letter Count", "0"},
            {"Word Count", "0"},
            {"Word Frequency", "0"},
            {"Most Repeated Word", "N/A"},
            {"Most Repeated Letter", "N/A"}
        };

        public Form1()
        {
            InitializeComponent();
            InitializeUI();

            this.BackgroundImage = Properties.Resources.textanalysis; // YOUR_IMAGE_NAME, eklediğiniz resmin dosya adı olmalıdır
            this.BackgroundImageLayout = ImageLayout.Stretch; // İsteğe bağlı olarak arkaplan resminin boyutunu ayarlayabilirsiniz
        }

        private void InitializeUI()
        {
            // Özel kontrol öğelerini form üzerine ekleyin
            textBox1 = new TextBox();
            buttonAnalizEt = new Button();
            buttonTemizle = new Button();
            labelAnalizSonuclari = new Label();

            Controls.Add(textBox1);
            Controls.Add(buttonAnalizEt);
            Controls.Add(buttonTemizle);
            Controls.Add(labelAnalizSonuclari);

            // Kontrol öğelerini konumlandır ve özelliklerini ayarla
            textBox1.Location = new System.Drawing.Point(170, 100);
            textBox1.Size = new System.Drawing.Size(400, 100);

            buttonAnalizEt.Location = new System.Drawing.Point(200, 150);
            buttonAnalizEt.Size = new System.Drawing.Size(150, 60);
            buttonAnalizEt.Text = "Make Analysis";
            buttonAnalizEt.Click += buttonAnalizEt_Click;

            buttonTemizle.Location = new System.Drawing.Point(370, 150);
            buttonTemizle.Size = new System.Drawing.Size(150, 60);
            buttonTemizle.Text = "Clear";
            buttonTemizle.Click += buttonTemizle_Click;

            labelAnalizSonuclari.Location = new System.Drawing.Point(170, 260);
            labelAnalizSonuclari.Size = new System.Drawing.Size(400, 100);
            
            labelAnalizSonuclari.ForeColor = Color.Black;
            labelAnalizSonuclari.Font = new Font(labelAnalizSonuclari.Font.FontFamily, 14, FontStyle.Bold);



            // Analiz sonuçlarını göster
            UpdateAnalysisLabels();
        }

        private void buttonAnalizEt_Click(object sender, EventArgs e)
        {
            string metin = textBox1.Text;

            // Harf sayısı analizi
            analizSonuclari["Letter Count"] = HarfSayisiniHesapla(metin).ToString();

            // Kelime sayısı analizi
            analizSonuclari["Word Count"] = KelimeSayisiniHesapla(metin).ToString();

            // Kelime frekansı analizi
            var kelimeFrekanslari = KelimeFrekanslariniHesapla(metin);
            analizSonuclari["Word Frequency"] = kelimeFrekanslari.Count.ToString();

            // En çok tekrar eden kelime analizi
            var enCokTekrarEdenKelime = KelimeFrekanslariIcinEnCokTekrarEdeniBul(kelimeFrekanslari);
            analizSonuclari["Most Repeated Word"] = enCokTekrarEdenKelime;

            // En çok tekrar eden harf analizi
            var enCokTekrarEdenHarf = HarfFrekanslariIcinEnCokTekrarEdeniBul(metin);
            analizSonuclari["Most Repeated Letter"] = enCokTekrarEdenHarf;

            // Analiz sonuçlarını göster
            UpdateAnalysisLabels();
        }

        private void buttonTemizle_Click(object sender, EventArgs e)
        {
            // Metin kutusunu ve analiz sonuçlarını temizle
            textBox1.Text = "";
            ResetAnalysisResults();
            UpdateAnalysisLabels();
        }

        private void UpdateAnalysisLabels()
        {
            // Analiz sonuçlarını metin olarak oluştur
            string analizSonuclariMetin = "";
            foreach (var entry in analizSonuclari)
            {
                analizSonuclariMetin += $"{entry.Key}: {entry.Value}\n";
            }

            // Analiz sonuçlarını Label kontrolüne yaz
            labelAnalizSonuclari.Text = analizSonuclariMetin;


        }

        private void ResetAnalysisResults()
        {
            // Analiz sonuçlarını sıfırla
            foreach (var key in analizSonuclari.Keys.ToList())
            {
                analizSonuclari[key] = "N/A";
            }
        }

        private int HarfSayisiniHesapla(string metin)
        {
            int harfSayisi = 0;

            foreach (char karakter in metin)
            {
                if (char.IsLetter(karakter))
                {
                    harfSayisi++;
                }
            }

            return harfSayisi;
        }

        private int KelimeSayisiniHesapla(string metin)
        {
            // Boşluklara göre metni parçala ve kelime sayısını bul
            string[] kelimeler = metin.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return kelimeler.Length;
        }

        private Dictionary<string, int> KelimeFrekanslariniHesapla(string metin)
        {
            // Metni küçük harfe çevir ve noktalama işaretlerini temizle
            string temizMetin = new string(metin.Select(c => char.IsLetter(c) || char.IsWhiteSpace(c) ? char.ToLower(c) : ' ').ToArray());

            // Boşluklara göre metni parçala ve kelime frekanslarını hesapla
            string[] kelimeler = temizMetin.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, int> kelimeFrekanslari = new Dictionary<string, int>();

            foreach (string kelime in kelimeler)
            {
                if (kelimeFrekanslari.ContainsKey(kelime))
                {
                    kelimeFrekanslari[kelime]++;
                }
                else
                {
                    kelimeFrekanslari[kelime] = 1;
                }
            }

            return kelimeFrekanslari;
        }

        private string HarfFrekanslariIcinEnCokTekrarEdeniBul(string metin)
        {
            // Metni küçük harfe çevir ve sadece harf karakterlerini al
            string temizMetin = new string(metin.Where(char.IsLetter).Select(char.ToLower).ToArray());

            // Harf frekanslarını hesapla
            Dictionary<char, int> harfFrekanslari = new Dictionary<char, int>();
            foreach (char harf in temizMetin)
            {
                if (harfFrekanslari.ContainsKey(harf))
                {
                    harfFrekanslari[harf]++;
                }
                else
                {
                    harfFrekanslari[harf] = 1;
                }
            }

            // En çok tekrar eden harfi bul
            var enCokTekrarEdenHarf = harfFrekanslari.Aggregate((x, y) => x.Value > y.Value ? x : y);

            return enCokTekrarEdenHarf.Key.ToString();
        }

        private string KelimeFrekanslariIcinEnCokTekrarEdeniBul(Dictionary<string, int> kelimeFrekanslari)
        {
            // En çok tekrar eden kelimeyi bul
            var enCokTekrarEdenKelime = kelimeFrekanslari.Aggregate((x, y) => x.Value > y.Value ? x : y);

            return enCokTekrarEdenKelime.Key;
        }
    }
}

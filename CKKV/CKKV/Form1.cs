﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections; //arraylist kullanımı için ekledim
using System.Text.RegularExpressions;
using Microsoft.Office.Core;
//using Excel = Microsoft.Office.Interop.Excel;
using System.Data.OleDb;
using GemBox.Spreadsheet;
using GemBox.Spreadsheet.WinFormsUtilities;

namespace CKKV
{
    public partial class Form1 : Form
    {
        string text = "Eklemek istediğiniz kriteri giriniz";
        string text2 = "Eklemek istediğiniz alternatifi giriniz";
        public static ArrayList kriterler = new ArrayList(); //
        public static ArrayList alternatifler = new ArrayList();//
        public static ArrayList faydaMaliyet = new ArrayList(); //
        public static ArrayList agirliklar = new ArrayList();
        public static ArrayList maxList = new ArrayList();
        public static ArrayList minList = new ArrayList();
        public static ArrayList optimalList = new ArrayList(); //
        public static ArrayList paydaListesi = new ArrayList(); //yüzde önem dağılımlarını hesaplamak için gereken sutun toplamlarını tutar    //   
        int rbtnDiziboyut, rbtnDizi1boyut = 1;
        int x, y, rbtn, rbtn1 = 0;
        string yontem;
        int duzenleIndex; // kriter ve alternatif değerlerini düzenlemek için
        RadioButton[] radioButton;
        RadioButton[] radioButton1;
        double max, min, lamda, CI, CR, agirlikToplam;
        public Form1()
        {
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY"); //import için

            InitializeComponent();
            text = txtKriter.Text;
            gridTasarim(dataGridViewIdealUzaklik);
            gridTasarim(dataGridViewNegatifIdealUzaklık);
            gridTasarimSirasiz(dataGridViewKararMat);
            gridTasarimSirasiz(dataGridViewOptimalKararMat);
            gridTasarim(dataGridViewSonucQi);
            gridTasarim(dataGridViewVikorKosulDenetle);
            gridTasarim(dataGridViewAgirlik);
            gridTasarim(dataGridViewAgirlikliNormalizeKMat);
            gridTasarim(dataGridViewNormalize);
            gridTasarim(dataGridViewOptimalFonkDegerleri);
            gridTasarim(dataGridViewKarsilastirmaMat);
            gridTasarim(dataGridViewC);
            gridTasarim(dataGridViewWVektörü);
            gridTasarim(dataGridViewAyrintiKmat);
            gridTasarim(dataGridViewDVektör);
            gridTasarim(dataGridViewKriterAgirliklari);
            gridTasarim(dataGridViewSonucOptimalKararMat);
            gridTasarim(dataGridViewSonucNormalizeMat);
            gridTasarim(dataGridViewSonucOptimalFonkDegerleri);
            gridTasarim(dataGridViewSonucAgirlikliNormalizeKMat);
            gridTasarimSirasiz(dataGridViewQiSiralama);
            gridTasarimSirasiz(dataGridViewSiSiralama);
            gridTasarimSirasiz(dataGridViewRiSiralama);

        }
        public void coprasQiDegerleriMat()
        {
            try
            {
                dataGridViewQiSiralama.Rows.Clear();
                dataGridViewQiSiralama.ColumnCount = 2;
                dataGridViewQiSiralama.Columns[0].Name = " ";
                dataGridViewQiSiralama.Columns[1].Name = "Göreceli Önem Değerleri(Qİ)";


                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewQiSiralama.Rows.Add(alternatifler[i].ToString());
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Göreceli Önem Değerleri matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void coprasQiDegerleri()
        {
            try
            {
                coprasQiDegerleriMat();
                //qi hesaplama
                double siEksiTopla = 0;
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    siEksiTopla += Convert.ToDouble(dataGridViewOptimalKararMat.Rows[i].Cells[2].Value);
                }
                double siEksiBireBolTopla = 0;
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    siEksiBireBolTopla += Convert.ToDouble(1 / Convert.ToDouble(dataGridViewOptimalKararMat.Rows[i].Cells[2].Value));
                }


                for (int i = 0; i < alternatifler.Count; i++)
                {
                    double pi = (siEksiTopla / (Convert.ToDouble(dataGridViewOptimalKararMat.Rows[i].Cells[2].Value) * siEksiBireBolTopla));
                    dataGridViewQiSiralama.Rows[i].Cells[1].Value = Convert.ToDouble(dataGridViewOptimalKararMat.Rows[i].Cells[1].Value) + pi;
                }


            }
            catch (Exception ex)
            {

                MessageBox.Show("Göreceli Önem Değerleri matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        //public void coprasQiDegerleri()
        //{
        //    try
        //    {
        //        coprasQiDegerleriMat();
        //        //qi hesaplama
        //        double siEksiTopla = 0;
        //        for (int i = 0; i < alternatifler.Count; i++)
        //        {
        //            siEksiTopla = Convert.ToDouble(dataGridViewOptimalKararMat.Rows[i].Cells[2].Value);
        //        }
        //        double siEksiBireBolTopla = 0;
        //        for (int i = 0; i < alternatifler.Count; i++)
        //        {
        //            siEksiBireBolTopla = Convert.ToDouble(1 / Convert.ToDouble(dataGridViewOptimalKararMat.Rows[i].Cells[2].Value));
        //        }


        //        for (int i = 0; i < alternatifler.Count; i++)
        //        {
        //            dataGridViewQiSiralama.Rows[i].Cells[1].Value = (Convert.ToDouble(dataGridViewOptimalKararMat.Rows[i].Cells[1].Value) + siEksiTopla) / (Convert.ToDouble(dataGridViewOptimalKararMat.Rows[i].Cells[2].Value) * siEksiBireBolTopla);
        //        }


        //    }
        //    catch (Exception ex)
        //    {

        //        MessageBox.Show("Göreceli Önem Değerleri matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return;
        //    }
        //}
        public void coprasPiMat()
        {
            try
            {
                dataGridViewSiSiralama.Rows.Clear();
                dataGridViewSiSiralama.ColumnCount = 2;
                dataGridViewSiSiralama.Columns[0].Name = " ";
                dataGridViewSiSiralama.Columns[1].Name = "Performans İndexi(Pİ)";


                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewSiSiralama.Rows.Add(alternatifler[i].ToString());
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Performans İndexi(Pİ) matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void coprasPi()
        {
            try
            {
                coprasPiMat();
                double maxGoreliOncelik = Convert.ToDouble(dataGridViewQiSiralama.Rows[0].Cells[1].Value);
                for (int i = 1; i < alternatifler.Count; i++)
                {

                    if (maxGoreliOncelik < Convert.ToDouble(dataGridViewQiSiralama.Rows[i].Cells[1].Value))
                    {
                        maxGoreliOncelik = Convert.ToDouble(dataGridViewQiSiralama.Rows[i].Cells[1].Value);
                    }

                }

                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewSiSiralama.Rows[i].Cells[1].Value = Convert.ToDouble((Convert.ToDouble(Convert.ToDouble(dataGridViewQiSiralama.Rows[i].Cells[1].Value) / maxGoreliOncelik)) * 100);
                }

            }

            catch (Exception ex)
            {

                MessageBox.Show("Performans İndexi(Pİ) matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        public void coprasAlternatifTercihSiraMat()
        {
            try
            {
                dataGridViewRiSiralama.Rows.Clear();
                dataGridViewRiSiralama.ColumnCount = 2;
                dataGridViewRiSiralama.Columns[0].Name = "Alternatifler";
                dataGridViewRiSiralama.Columns[1].Name = "Pİ";


                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewRiSiralama.Rows.Add(alternatifler[i].ToString());
                }


            }
            catch (Exception ex)
            {

                MessageBox.Show("Alternatifler için tercih sırası matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void coprasAlternatifTercihSira()
        {
            try
            {
                coprasAlternatifTercihSiraMat();
                double maxGoreliOncelik = Convert.ToDouble(dataGridViewQiSiralama.Rows[0].Cells[1].Value);
                for (int i = 1; i < alternatifler.Count; i++)
                {

                    if (maxGoreliOncelik < Convert.ToDouble(dataGridViewQiSiralama.Rows[i].Cells[1].Value))
                    {
                        maxGoreliOncelik = Convert.ToDouble(dataGridViewQiSiralama.Rows[i].Cells[1].Value);
                    }

                }

                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewRiSiralama.Rows[i].Cells[1].Value = Convert.ToDouble((Convert.ToDouble(Convert.ToDouble(dataGridViewQiSiralama.Rows[i].Cells[1].Value) / maxGoreliOncelik)) * 100);
                }
                dataGridViewRiSiralama.Sort(dataGridViewRiSiralama.Columns[1], ListSortDirection.Descending);//Normal Sıralama


            }
            catch (Exception ex)
            {

                MessageBox.Show("Alternatifler için tercih sırası matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void coprasSiDegerMat()
        {
            try
            {
                dataGridViewOptimalKararMat.Rows.Clear();
                dataGridViewOptimalKararMat.ColumnCount = 3;
                dataGridViewOptimalKararMat.Columns[0].Name = " ";
                dataGridViewOptimalKararMat.Columns[1].Name = "Si+";
                dataGridViewOptimalKararMat.Columns[2].Name = "Sİ-";

                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewOptimalKararMat.Rows.Add(alternatifler[i].ToString());
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("İdeal çözüme göreli yakınlık matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        public void coprasSiDegerleri()
        {
            try
            {
                coprasSiDegerMat();
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    double siArti = 0; double siEksi = 0;

                    for (int j = 1; j < kriterler.Count + 1; j++)
                    {
                        if (faydaMaliyet[j - 1].ToString() == rbtnFayda.Text)
                        {
                            siArti += Convert.ToDouble(dataGridViewAgirlikliNormalizeKMat.Rows[i].Cells[j].Value);
                        }
                        else if (faydaMaliyet[j - 1].ToString() == rbtnMaliyet.Text)
                        {
                            siEksi += Convert.ToDouble(dataGridViewAgirlikliNormalizeKMat.Rows[i].Cells[j].Value);
                        }
                    }
                    dataGridViewOptimalKararMat.Rows[i].Cells[1].Value = siArti;
                    dataGridViewOptimalKararMat.Rows[i].Cells[2].Value = siEksi;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("İdeal çözüme göreli yakınlık matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void coprasDuzenle()
        {
            btnOptimalGit.Text = "NORMALİZE ET";
            btnOptimallikFonkDegerHesapla.Text = "Her alternatif için  Si+  ve  Si-  değerleri ";
            tabPageOptimal.Text = "Sİ DEĞERLERİ";
            lblOpKararMat.Text = "Sİ DEĞERLERİ";
            btnKararMatNormalize.Text = "Qİ Değerlerini Görüntüle";
            label26.Text = " ";
            label27.Text = "GÖRECELİ ÖNEM DEĞERLERİ Qİ";
            label35.Text = "Pİ DEĞERLERİ";
            label39.Text = "ALTERNATİFLERİN TERCİH SIRASI";
            tabPageVikorSiralama.Text = "Alternatif Tercih Sıralaması";
            btnKosulDenetle.Visible = false;
        }
        public void idealCozumGoreliYakinlikMatCerceve()
        {
            try
            {
                dataGridViewOptimalFonkDegerleri.Rows.Clear();
                dataGridViewOptimalFonkDegerleri.ColumnCount = 4;
                dataGridViewOptimalFonkDegerleri.Columns[0].Name = " ";
                dataGridViewOptimalFonkDegerleri.Columns[1].Name = "Si*";
                dataGridViewOptimalFonkDegerleri.Columns[2].Name = "Sİ-";
                dataGridViewOptimalFonkDegerleri.Columns[3].Name = "Ci*";
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewOptimalFonkDegerleri.Rows.Add(alternatifler[i].ToString());
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("İdeal çözüme göreli yakınlık matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        public void idealCozumGoreliYakinlik()
        {
            try
            {
                idealCozumGoreliYakinlikMatCerceve();
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewOptimalFonkDegerleri.Rows[i].Cells[1].Value = dataGridViewIdealUzaklik.Rows[i].Cells[1].Value;
                    dataGridViewOptimalFonkDegerleri.Rows[i].Cells[2].Value = dataGridViewNegatifIdealUzaklık.Rows[i].Cells[1].Value;
                    dataGridViewOptimalFonkDegerleri.Rows[i].Cells[3].Value = Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[i].Cells[2].Value) / (Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[i].Cells[2].Value) + Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[i].Cells[1].Value));
                }
                dataGridViewOptimalFonkDegerleri.Sort(dataGridViewOptimalFonkDegerleri.Columns[3], ListSortDirection.Descending);

            }
            catch (Exception ex)
            {

                MessageBox.Show("İdeal çözüme göreli yakınlık matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        public void idealUzaklikMatCerceve()
        {
            try
            {
                dataGridViewIdealUzaklik.Rows.Clear();
                dataGridViewIdealUzaklik.ColumnCount = 2;
                dataGridViewIdealUzaklik.Columns[0].Name = " ";
                dataGridViewIdealUzaklik.Columns[1].Name = "Si*";

                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewIdealUzaklik.Rows.Add(alternatifler[i].ToString());
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("İdeal uzaklık matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void negatifIdealUzaklikMatCerceve()
        {
            try
            {
                dataGridViewNegatifIdealUzaklık.Rows.Clear();
                dataGridViewNegatifIdealUzaklık.ColumnCount = 2;
                dataGridViewNegatifIdealUzaklık.Columns[0].Name = " ";
                dataGridViewNegatifIdealUzaklık.Columns[1].Name = "Si-";

                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewNegatifIdealUzaklık.Rows.Add(alternatifler[i].ToString());
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Negatif ideal uzaklık matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void idealUzaklik()
        {
            try
            {
                idealUzaklikMatCerceve();
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    double s2Yildiz = 0;
                    for (int j = 1; j < kriterler.Count + 1; j++)
                    {
                        s2Yildiz += (Convert.ToDouble(dataGridViewAgirlikliNormalizeKMat.Rows[i].Cells[j].Value) - Convert.ToDouble(dataGridViewOptimalKararMat.Rows[0].Cells[j].Value)) * (Convert.ToDouble(dataGridViewAgirlikliNormalizeKMat.Rows[i].Cells[j].Value) - Convert.ToDouble(dataGridViewOptimalKararMat.Rows[0].Cells[j].Value));
                    }
                    dataGridViewIdealUzaklik.Rows[i].Cells[1].Value = (double)Math.Sqrt(s2Yildiz);
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("İdeal uzaklık matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        public void negatifIdealUzaklık()
        {
            try
            {
                negatifIdealUzaklikMatCerceve();
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    double s2Ussu = 0;
                    for (int j = 1; j < kriterler.Count + 1; j++)
                    {
                        s2Ussu += (double)Math.Pow((Convert.ToDouble(dataGridViewAgirlikliNormalizeKMat.Rows[i].Cells[j].Value) - Convert.ToDouble(dataGridViewOptimalKararMat.Rows[1].Cells[j].Value)), 2);
                    }
                    dataGridViewNegatifIdealUzaklık.Rows[i].Cells[1].Value = (double)Math.Sqrt(s2Ussu);
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("İdeal uzaklık matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void topsisDuzenle()
        {
            btnOptimalGit.Text = "NORMALİZE ET";
            btnKararMatNormalize.Text = "Uzaklık değerleri";
            btnOptimallikFonkDegerHesapla.Text = "İdeal ve negatif ideal çözüm";
            lblOpKararMat.Text = "İDEAL VE NEGATİF İDEAL ÇÖZÜM DEĞERLERİ";
            lblOptFonkDeger.Text = "TOPSIS YÖNTEMİ SONUÇ TABLOSU ";


        }
        public void normalizeMaxMin() //AĞIRLIKLANDIRILMIŞ NORMALİZE MATRİSİNDEKİ HER SUTUNDAKİ MAX VE MİN DEĞERLERİ BULUP ARRAYLİSTLERE ATAN METOD
        {
            //ağırlıklandırılmış normalize matrisindeki bir sutundaki max ve min değerleri bulan döngüler
            for (int j = 1; j < kriterler.Count + 1; j++)
            {
                max = Convert.ToDouble(dataGridViewAgirlikliNormalizeKMat.Rows[0].Cells[j].Value);
                min = Convert.ToDouble(dataGridViewAgirlikliNormalizeKMat.Rows[0].Cells[j].Value);

                for (int i = 0; i < alternatifler.Count; i++)
                {
                    if (Convert.ToDouble(dataGridViewAgirlikliNormalizeKMat.Rows[i].Cells[j].Value) > max)
                    {
                        max = Convert.ToDouble(dataGridViewAgirlikliNormalizeKMat.Rows[i].Cells[j].Value);
                    }
                }
                maxList.Add(max);

                for (int i = 0; i < alternatifler.Count; i++)
                {
                    if (Convert.ToDouble(dataGridViewAgirlikliNormalizeKMat.Rows[i].Cells[j].Value) < min)
                    {
                        min = Convert.ToDouble(dataGridViewAgirlikliNormalizeKMat.Rows[i].Cells[j].Value);
                    }
                }
                minList.Add(min);
            }
        }
        public void topsisIdealNegatifIdealMatCerceve()
        {
            try
            {
                dataGridViewOptimalKararMat.Rows.Clear();

                dataGridViewOptimalKararMat.ColumnCount = kriterler.Count + 1;
                dataGridViewOptimalKararMat.Columns[0].Name = " ";
                int k = 1;
                for (int i = 0; i < kriterler.Count; i++)
                {
                    dataGridViewOptimalKararMat.Columns[k].Name = kriterler[i].ToString();
                    k++;
                }
                dataGridViewOptimalKararMat.Rows.Add("S*");
                dataGridViewOptimalKararMat.Rows.Add("S-");

            }
            catch (Exception ex)
            {

                MessageBox.Show("Hata!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void topsisIdealNegatifIdealCozumDegeri()
        {
            try
            {
                normalizeMaxMin(); //karar matrisindeki max ve min değerleri bulup listeye atan metod
                topsisIdealNegatifIdealMatCerceve();

                for (int i = 0; i < kriterler.Count; i++)
                {
                    if (faydaMaliyet[i].ToString() == rbtnFayda.Text)
                    {
                        dataGridViewOptimalKararMat.Rows[0].Cells[i + 1].Value = Convert.ToDouble(maxList[i]);
                        dataGridViewOptimalKararMat.Rows[1].Cells[i + 1].Value = Convert.ToDouble(minList[i]);
                    }
                    else if (faydaMaliyet[i].ToString() == rbtnMaliyet.Text)
                    {
                        dataGridViewOptimalKararMat.Rows[0].Cells[i + 1].Value = Convert.ToDouble(minList[i]);
                        dataGridViewOptimalKararMat.Rows[1].Cells[i + 1].Value = Convert.ToDouble(maxList[i]);
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("İdeal ve negatif ideal çözüm değerleri belirlenemedi!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        public void topsisNormalize()
        {
            try
            {
                vikorNormalizeMatCerceve();
                for (int j = 1; j < kriterler.Count + 1; j++)
                {
                    double payda = 0;
                    for (int i = 0; i < alternatifler.Count; i++)
                    {
                        payda += (double)Math.Pow(Convert.ToDouble(dataGridViewKararMat.Rows[i].Cells[j].Value), 2);
                    }
                    for (int i = 0; i < alternatifler.Count; i++)
                    {
                        double nij = (Convert.ToDouble(dataGridViewKararMat.Rows[i].Cells[j].Value) / (double)Math.Sqrt(payda));

                        dataGridViewNormalize.Rows[i].Cells[j].Value = nij;
                    }
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Matris Normalize Edilemedi!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        public void coprasNormalize()
        {
            try
            {
                vikorNormalizeMatCerceve();
                for (int j = 1; j < kriterler.Count + 1; j++)
                {
                    double payda = 0;
                    for (int i = 0; i < alternatifler.Count; i++)
                    {
                        payda += Convert.ToDouble(dataGridViewKararMat.Rows[i].Cells[j].Value);
                    }
                    for (int i = 0; i < alternatifler.Count; i++)
                    {
                        double dij = Convert.ToDouble(dataGridViewKararMat.Rows[i].Cells[j].Value) / payda;

                        dataGridViewNormalize.Rows[i].Cells[j].Value = dij;
                    }
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Matris Normalize Edilemedi!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void topsisAgirlikliNormalize()
        {
            try
            {
                vikorAgirlikliNormalizeKararMatCerceve();
                for (int j = 1; j < kriterler.Count + 1; j++)
                {
                    for (int i = 0; i < alternatifler.Count; i++)
                    {
                        dataGridViewAgirlikliNormalizeKMat.Rows[i].Cells[j].Value = Convert.ToDouble(dataGridViewNormalize.Rows[i].Cells[j].Value) * Convert.ToDouble(agirliklar[j - 1]);
                    }
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Ağırlıklı normalize matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


        }
        public void coprasAgirlikliNormalize()
        {
            try
            {
                vikorAgirlikliNormalizeKararMatCerceve();
                for (int j = 1; j < kriterler.Count + 1; j++)
                {
                    for (int i = 0; i < alternatifler.Count; i++)
                    {
                        dataGridViewAgirlikliNormalizeKMat.Rows[i].Cells[j].Value = Convert.ToDouble(dataGridViewNormalize.Rows[i].Cells[j].Value) * Convert.ToDouble(agirliklar[j - 1]);
                    }
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Ağırlıklı normalize matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


        }
        public void vikorKosulDenetleMat()
        {
            //matris
            try
            {
                dataGridViewVikorKosulDenetle.Rows.Clear();
                dataGridViewVikorKosulDenetle.ColumnCount = 2;
                dataGridViewVikorKosulDenetle.Columns[0].Name = " ";
                dataGridViewVikorKosulDenetle.Columns[1].Name = "Sonuçlar";

                dataGridViewVikorKosulDenetle.Rows.Add("Q(A2)");
                dataGridViewVikorKosulDenetle.Rows.Add("Q(A1)");
                dataGridViewVikorKosulDenetle.Rows.Add("Q(A2)-Q(A1)");
                dataGridViewVikorKosulDenetle.Rows.Add("DQ");
                dataGridViewVikorKosulDenetle.Rows.Add("Koşul 1");
                dataGridViewVikorKosulDenetle.Rows.Add("Koşul 2");


                dataGridViewVikorKosulDenetle.Rows[0].Cells[1].Value = dataGridViewSonucQi.Rows[1].Cells[1].Value;
                dataGridViewVikorKosulDenetle.Rows[1].Cells[1].Value = dataGridViewSonucQi.Rows[0].Cells[1].Value;
                dataGridViewVikorKosulDenetle.Rows[2].Cells[1].Value = (Convert.ToDouble(dataGridViewSonucQi.Rows[1].Cells[1].Value) - Convert.ToDouble(dataGridViewSonucQi.Rows[0].Cells[1].Value));
                double say = alternatifler.Count - 1;
                dataGridViewVikorKosulDenetle.Rows[3].Cells[1].Value = Convert.ToDouble(1 / say);
                if (Convert.ToDouble(dataGridViewVikorKosulDenetle.Rows[2].Cells[1].Value) >= Convert.ToDouble(dataGridViewVikorKosulDenetle.Rows[3].Cells[1].Value))
                {
                    dataGridViewVikorKosulDenetle.Rows[4].Cells[1].Value = "Sağlandı";
                }
                else if (Convert.ToDouble(dataGridViewVikorKosulDenetle.Rows[2].Cells[1].Value) < Convert.ToDouble(dataGridViewVikorKosulDenetle.Rows[3].Cells[1].Value))
                {
                    dataGridViewVikorKosulDenetle.Rows[4].Cells[1].Value = "Sağlanmadı";
                    richTextBoxKosulDenetle.Text = "Kabul edilebilir avantaj koşulu sağlanmadığından " + dataGridViewSonucQi.Rows[0].Cells[0].Value.ToString() + " , " + dataGridViewSonucQi.Rows[1].Cells[0].Value.ToString() + " , ... , " + dataGridViewSonucQi.Rows[alternatifler.Count - 1].Cells[0].Value.ToString() + " alternatiflerinin tamamı uzlaşık çözüm kümesinde yer almaktadır.";

                }
                if (dataGridViewSonucQi.Rows[0].Cells[0].Value == dataGridViewSiSiralama.Rows[0].Cells[0].Value || dataGridViewSonucQi.Rows[0].Cells[0].Value == dataGridViewRiSiralama.Rows[0].Cells[0].Value)
                {
                    dataGridViewVikorKosulDenetle.Rows[5].Cells[1].Value = "Sağlandı";
                }
                else if (dataGridViewSonucQi.Rows[0].Cells[0].Value != dataGridViewSiSiralama.Rows[0].Cells[0].Value && dataGridViewSonucQi.Rows[0].Cells[0].Value != dataGridViewRiSiralama.Rows[0].Cells[0].Value)
                {
                    dataGridViewVikorKosulDenetle.Rows[5].Cells[1].Value = "Sağlanmadı";
                    richTextBoxKosulDenetle.AppendText(Environment.NewLine + "Kabul edilebilir istikrar koşulu sağlanmadığından " + dataGridViewSonucQi.Rows[0].Cells[0].Value.ToString() + " ve " + dataGridViewSonucQi.Rows[1].Cells[0].Value.ToString() + " alternatiflerinin her ikisi de uzlaşık ortam çözümü olarak kabul edilir.");

                }
                if (Convert.ToDouble(dataGridViewVikorKosulDenetle.Rows[2].Cells[1].Value) >= Convert.ToDouble(dataGridViewVikorKosulDenetle.Rows[3].Cells[1].Value) && dataGridViewSonucQi.Rows[0].Cells[0].Value == dataGridViewSiSiralama.Rows[0].Cells[0].Value || dataGridViewSonucQi.Rows[0].Cells[0].Value == dataGridViewRiSiralama.Rows[0].Cells[0].Value)
                {
                    richTextBoxKosulDenetle.AppendText(Environment.NewLine + "EN İYİ ALTERNATİF: " + dataGridViewSonucQi.Rows[0].Cells[0].Value.ToString());
                }


            }
            catch (Exception ex)
            {

                MessageBox.Show("Qİ değerleri için sıralama matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void vikorKosulQiSiralama()
        {
            try
            {
                dataGridViewSonucQi.Rows.Clear();
                dataGridViewSonucQi.ColumnCount = 2;
                dataGridViewSonucQi.Columns[0].Name = " ";
                dataGridViewSonucQi.Columns[1].Name = "Qİ";

                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewSonucQi.Rows.Add(dataGridViewQiSiralama.Rows[i].Cells[0].Value);
                }
                dataGridViewSonucQi.Rows.Add("Q(A2)");
                dataGridViewSonucQi.Rows.Add("Q(A1)");
                dataGridViewSonucQi.Rows.Add("Q(A2)-Q(A1)");
                dataGridViewSonucQi.Rows.Add("DQ");
                dataGridViewSonucQi.Rows.Add("Koşul 1");
                dataGridViewSonucQi.Rows.Add("Koşul 2");

                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewSonucQi.Rows[i].Cells[1].Value = dataGridViewQiSiralama.Rows[i].Cells[1].Value;
                }

                dataGridViewSonucQi.Rows[alternatifler.Count].Cells[1].Value = dataGridViewSonucQi.Rows[1].Cells[1].Value;
                dataGridViewSonucQi.Rows[alternatifler.Count + 1].Cells[1].Value = dataGridViewSonucQi.Rows[0].Cells[1].Value;
                dataGridViewSonucQi.Rows[alternatifler.Count + 2].Cells[1].Value = (Convert.ToDouble(dataGridViewSonucQi.Rows[1].Cells[1].Value) - Convert.ToDouble(dataGridViewSonucQi.Rows[0].Cells[1].Value));
                double say = alternatifler.Count - 1;
                dataGridViewSonucQi.Rows[alternatifler.Count + 3].Cells[1].Value = Convert.ToDouble(1 / say);

                if (Convert.ToDouble(dataGridViewSonucQi.Rows[alternatifler.Count + 2].Cells[1].Value) >= Convert.ToDouble(dataGridViewSonucQi.Rows[alternatifler.Count + 3].Cells[1].Value))
                {
                    dataGridViewSonucQi.Rows[alternatifler.Count + 4].Cells[1].Value = "Sağlandı";
                }
                else if (Convert.ToDouble(dataGridViewSonucQi.Rows[alternatifler.Count + 2].Cells[1].Value) < Convert.ToDouble(dataGridViewSonucQi.Rows[alternatifler.Count + 3].Cells[1].Value))
                {
                    dataGridViewSonucQi.Rows[alternatifler.Count + 4].Cells[1].Value = "Sağlanmadı";
                }
                if (dataGridViewSonucQi.Rows[0].Cells[0].Value == dataGridViewSiSiralama.Rows[0].Cells[0].Value || dataGridViewSonucQi.Rows[0].Cells[0].Value == dataGridViewRiSiralama.Rows[0].Cells[0].Value)
                {
                    dataGridViewSonucQi.Rows[alternatifler.Count + 5].Cells[1].Value = "Sağlandı";
                }
                else if (dataGridViewSonucQi.Rows[0].Cells[0].Value != dataGridViewSiSiralama.Rows[0].Cells[0].Value && dataGridViewSonucQi.Rows[0].Cells[0].Value != dataGridViewRiSiralama.Rows[0].Cells[0].Value)
                {
                    dataGridViewSonucQi.Rows[alternatifler.Count + 5].Cells[1].Value = "Sağlanmadı";
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Qİ değerleri için sıralama matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void qiSiralamaMatCerceve()
        {
            try
            {
                dataGridViewQiSiralama.Rows.Clear();
                dataGridViewQiSiralama.ColumnCount = 2;
                dataGridViewQiSiralama.Columns[0].Name = " ";
                dataGridViewQiSiralama.Columns[1].Name = "Qİ";

                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewQiSiralama.Rows.Add(alternatifler[i].ToString());
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Qİ değerleri için sıralama matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void siSiralamaMatCerceve()
        {
            try
            {
                dataGridViewSiSiralama.Rows.Clear();
                dataGridViewSiSiralama.ColumnCount = 2;
                dataGridViewSiSiralama.Columns[0].Name = " ";
                dataGridViewSiSiralama.Columns[1].Name = "Sİ";

                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewSiSiralama.Rows.Add(alternatifler[i].ToString());
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Sİ değerleri için sıralama matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void riSiralamaMatCerceve()
        {
            try
            {
                dataGridViewRiSiralama.Rows.Clear();
                dataGridViewRiSiralama.ColumnCount = 2;
                dataGridViewRiSiralama.Columns[0].Name = " ";
                dataGridViewRiSiralama.Columns[1].Name = "Rİ";

                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewRiSiralama.Rows.Add(alternatifler[i].ToString());
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Rİ değerleri için sıralama matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void qiSiralamaMat()
        {
            try
            {
                qiSiralamaMatCerceve();

                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewQiSiralama.Rows[i].Cells[1].Value = dataGridViewOptimalFonkDegerleri.Rows[i].Cells[5].Value;
                }
                dataGridViewQiSiralama.Sort(dataGridViewQiSiralama.Columns[1], ListSortDirection.Ascending);//Normal Sıralama

            }
            catch (Exception ex)
            {

                MessageBox.Show("Qİ değerleri için sıralama matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void siSiralamaMat()
        {
            try
            {
                siSiralamaMatCerceve();

                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewSiSiralama.Rows[i].Cells[1].Value = dataGridViewOptimalFonkDegerleri.Rows[i].Cells[1].Value;
                }
                dataGridViewSiSiralama.Sort(dataGridViewSiSiralama.Columns[1], ListSortDirection.Ascending);//Normal Sıralama

            }
            catch (Exception ex)
            {

                MessageBox.Show("Sİ değerleri için sıralama matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void riSiralamaMat()
        {
            try
            {
                riSiralamaMatCerceve();
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewRiSiralama.Rows[i].Cells[1].Value = dataGridViewOptimalFonkDegerleri.Rows[i].Cells[2].Value;
                }
                dataGridViewRiSiralama.Sort(dataGridViewRiSiralama.Columns[1], ListSortDirection.Ascending);//Normal Sıralama

            }
            catch (Exception ex)
            {

                MessageBox.Show("Rİ değerleri için sıralama matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void dataGridViewKararMat_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int index = dataGridViewKararMat.CurrentCell.ColumnIndex;
                int i = index - 1;
                if (index == 0)
                {
                    MessageBox.Show("Lütfen yönünü değiştirmek istediğiniz kriterin bulunduğu sutundaki değerlerden birinin üzerine tıklayınız!", "Uyarı ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (index != 0)
                {
                    if (faydaMaliyet[i].ToString() == rbtnFayda.Text)
                    {
                        faydaMaliyet.RemoveAt(i);
                        faydaMaliyet.Insert(i, rbtnMaliyet.Text);
                        dataGridViewKararMat.Columns[index].HeaderCell.Style.BackColor = Color.Plum;
                        listBoxKriter.Items.RemoveAt(i);
                        listBoxKriter.Items.Insert(i, (kriterler[i].ToString() + "  (" + rbtnMaliyet.Text + ")"));

                    }
                    else if (faydaMaliyet[i].ToString() == rbtnMaliyet.Text)
                    {
                        faydaMaliyet.RemoveAt(i);
                        faydaMaliyet.Insert(i, rbtnFayda.Text);
                        dataGridViewKararMat.Columns[index].HeaderCell.Style.BackColor = Color.LightBlue;
                        listBoxKriter.Items.RemoveAt(i);
                        listBoxKriter.Items.Insert(i, (kriterler[i].ToString() + "  (" + rbtnFayda.Text + ")"));

                    }
                }
                else
                {

                    MessageBox.Show("Hata!", "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception)
            {
            }
        }
        public void kararMatRenklendir()
        {
            for (int j = 1; j < kriterler.Count + 1; j++)
            {
                if (faydaMaliyet[j - 1].ToString() == rbtnFayda.Text)
                {
                    dataGridViewKararMat.Columns[j].HeaderCell.Style.BackColor = Color.LightBlue;

                }
                else if (faydaMaliyet[j - 1].ToString() == rbtnMaliyet.Text)
                {
                    dataGridViewKararMat.Columns[j].HeaderCell.Style.BackColor = Color.Plum;

                }
                else
                {
                    MessageBox.Show("boş");
                    return;
                }

            }
        }
        public void kararMatrisiOlustur()
        {
            try
            {
                dataGridViewKararMat.Rows.Clear();
                tabControl1.SelectedTab = tabPageKararMatrisi;
                dataGridViewKararMat.ColumnCount = kriterler.Count + 1;
                dataGridViewKararMat.Columns[0].Name = " ";
                int j = 1;
                for (int i = 0; i < kriterler.Count; i++)
                {
                    dataGridViewKararMat.Columns[j].Name = kriterler[i].ToString();
                    j++;
                }

                for (int i = 0; i < alternatifler.Count; i++)
                {

                    dataGridViewKararMat.Rows.Add(alternatifler[i].ToString());
                }

                kararMatRenklendir();
                ////İLK SATIR VE İLK SUTUNDAKİ DEĞERLERİN DEĞİŞTİRİLMESİNİ ENGELLEDİM
                //for (int rC = 0; rC < alternatifler.Count; rC++)
                //{
                //    dataGridViewKararMat.Rows[rC].Cells[0].ReadOnly = true;
                //}


                //BU KISMA FAYDA MALİYET İÇİN BUTON EKLE DEĞİŞEBİLSİN 
                //for (int i = 1; i < kriterler.Count+1; i++)
                //{

                //}

            }
            catch (Exception ex)
            {

                MessageBox.Show("Hata!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


        }
        //public void normalizeMatCerceve() //normalize matrisinin ilk satır ve sutununu kriterler ve alternatifler arraylistlerini kullanarak oluşturan metod
        //{
        //    try
        //    {
        //        dataGridViewNormalize.Rows.Clear();
        //        //normalize matrisi
        //        dataGridViewNormalize.ColumnCount = kriterler.Count + 1; //alternatif sutunuda bulunması gerektiğinden kriterler+1 tane sutun ekledim
        //        dataGridViewNormalize.Columns[0].Name = " ";
        //        int s = 1;
        //        for (int i = 0; i < kriterler.Count; i++)
        //        {
        //            dataGridViewNormalize.Columns[s].Name = kriterler[i].ToString();
        //            s++;
        //        }
        //        for (int i = 0; i < alternatifler.Count; i++)
        //        {

        //            dataGridViewNormalize.Rows.Add(alternatifler[i].ToString());
        //        }
        //        //İLK SATIR VE İLK SUTUNDAKİ DEĞERLERİN DEĞİŞTİRİLMESİNİ ENGELLEDİM
        //        for (int rC = 0; rC < alternatifler.Count; rC++)
        //        {
        //            dataGridViewNormalize.Rows[rC].Cells[0].ReadOnly = true;
        //        }


        //    }
        //    catch (Exception ex)
        //    {

        //        MessageBox.Show("Hata!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return;
        //    }

        //}
        private void btnKararMatOL_Click(object sender, EventArgs e)
        {
            boyutAyarlama();
            kararMatrisiOlustur();
        }
        public void boyutAyarlama()
        {
            //girilen alternatif sayısına göre karar matrisinin boyutunu büyüten ve butonları konumlandıran kod parçası
            if (alternatifler.Count > 5)
            {
                int say = alternatifler.Count - 5;
                int sayi = 0;
                int konum, konum1, konum2 = 0;
                konum = 364 + (say * 10);
                konum1 = 387 + (say * 10);
                konum2 = 432 + (say * 15);
                sayi = 260 + (say * 12);
                if (sayi > 386)
                {
                    sayi = 386;
                }
                if (konum > 500)
                {
                    konum = 500;
                }
                if (konum1 > 523)
                {
                    konum = 523;
                }
                if (konum2 > 568)
                {
                    konum2 = 568;
                }

                chkPasteToSelectedCells.Top = konum;
                dataGridViewImport.Top = konum;
                btnOptimalGit.Top = konum1;
                btnKararMatEAktar.Top = konum2;
                dataGridViewKararMat.Height = sayi;
            }
            //optimal karar mat için(ARAS YÖNTEMİ)
            if (yontem != btnVikor.Text)
            {
                //optimal karar mat için
                if (alternatifler.Count > 5)
                {
                    int say = alternatifler.Count - 5;
                    int sayi = 0;
                    int konum = 0;
                    konum = 290 + (say * 23);
                    sayi = 240 + (say * 22);
                    if (sayi > 511)
                    {
                        sayi = 511;
                    }
                    if (konum > 561)
                    {
                        konum = 561;
                    }
                    btnKararMatNormalize.Top = konum;
                    btnOptimalKararMatEAktar.Top = konum;
                    dataGridViewOptimalKararMat.Height = sayi;
                }
            }
            //En iyi en kötü değer matrisi için (VİKOR YÖNTEMİ)
            if (yontem == btnVikor.Text)
            {
                btnKararMatNormalize.Top = 190;
                btnOptimalKararMatEAktar.Top = 190;
                dataGridViewOptimalKararMat.Height = 140;

            }
            // normalize matrisi için
            if (alternatifler.Count > 5)
            {
                int say = alternatifler.Count - 5;
                int sayi = 0;
                int konum = 0;
                konum = 290 + (say * 23);
                sayi = 240 + (say * 22);
                if (sayi > 511)
                {
                    sayi = 511;
                }
                if (konum > 561)
                {
                    konum = 561;
                }

                btnKriterAgirlikBelirleme.Top = konum;
                btnNormalizeEAktar.Top = konum;
                dataGridViewNormalize.Height = sayi;
            }
            //ağırlıklı normalize mat için
            if (alternatifler.Count > 5)
            {
                int say = alternatifler.Count - 5;
                int sayi = 0;
                int konum = 0;
                konum = 290 + (say * 23);
                sayi = 240 + (say * 22);
                if (sayi > 511)
                {
                    sayi = 511;
                }
                if (konum > 561)
                {
                    konum = 561;
                }

                btnOptimallikFonkDegerHesapla.Top = konum;
                btnAgirlikliNormalizeKMatEAktar.Top = konum;
                dataGridViewAgirlikliNormalizeKMat.Height = sayi;
            }
            //optimallik fonk değerleri matrisi
            if (alternatifler.Count > 5)
            {
                int say = alternatifler.Count - 5;
                int sayi = 0;
                int konum, konum1, konum2 = 0;
                konum = 356 + (say * 13);
                konum1 = 392 + (say * 13);
                konum2 = 434 + (say * 13);
                sayi = 240 + (say * 12);
                if (sayi > 444)
                {
                    sayi = 444;
                }
                if (konum > 548)
                {
                    konum = 548;
                }
                if (konum1 > 585)
                {
                    konum = 585;
                }
                if (konum2 > 626)
                {
                    konum2 = 626;
                }

                btnAyrintiliCozum.Top = konum1;
                btnOptimalFonkDegerEAktar.Top = konum2;
                dataGridViewOptimalFonkDegerleri.Height = sayi;
            }
        }
        public void alternatifSil()
        {
            try
            {
                int secili = listBoxAlternatif.SelectedIndex;
                alternatifler.RemoveAt(secili);
                listBoxAlternatif.Items.RemoveAt(secili);

                txtAlternatif.Focus();

                if (listBoxAlternatif.Items.Count == 0)
                {
                    btnAlternatifSil.Enabled = false;
                    txtAlternatif.Focus();

                }
            }
            catch (Exception)
            {
                MessageBox.Show("Lütfen silmek istediğiniz alternatifi seçiniz!", "Uyarı ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

        }
        private void btnAlternatifSil_Click(object sender, EventArgs e)
        {
            alternatifSil();
        }
        private void btnAlternatifDuzenle_Click(object sender, EventArgs e)
        {
            duzenleIndex = listBoxAlternatif.SelectedIndex;
            txtAlternatif.Text = alternatifler[duzenleIndex].ToString();
            btnAlternatifEkle.Text = "Güncelle";
            btnAlternatifEkle.Font = new Font("Bahnschrift Light", 8, FontStyle.Bold);
        }
        private void btnKriterSil_Click(object sender, EventArgs e)
        {
            kriterSil();
        }
        public void maxMin() //KARAR MATRİSİNDEKİ HER SUTUNDAKİ MAX VE MİN DEĞERLERİ BULUP ARRAYLİSTLERE ATAN METOD
        {
            //karar matrisindeki bir sutundaki max ve min değerleri bulan döngüler
            for (int j = 1; j < kriterler.Count + 1; j++)
            {
                max = Convert.ToDouble(dataGridViewKararMat.Rows[0].Cells[j].Value);
                min = Convert.ToDouble(dataGridViewKararMat.Rows[0].Cells[j].Value);

                for (int i = 0; i < alternatifler.Count; i++)
                {
                    if (Convert.ToDouble(dataGridViewKararMat.Rows[i].Cells[j].Value) > max)
                    {
                        max = Convert.ToDouble(dataGridViewKararMat.Rows[i].Cells[j].Value);
                    }
                }
                maxList.Add(max);

                for (int i = 0; i < alternatifler.Count; i++)
                {
                    if (Convert.ToDouble(dataGridViewKararMat.Rows[i].Cells[j].Value) < min)
                    {
                        min = Convert.ToDouble(dataGridViewKararMat.Rows[i].Cells[j].Value);
                    }
                }
                minList.Add(min);
            }
        }
        //public void minMaxNormalization() //karar matrisini fayda ve maliyet kriteri olmasına göre ayrı formüllerle normalize edip normalizasyon matrisini oluşturan metod
        //{
        //    try
        //    {
        //        normalizeMatCerceve();//
        //        maxMin(); //her sutundaki max ve min değerleri bulup ilgili listelere atayan metod
        //        //normalizasyon
        //        for (int j = 1; j < kriterler.Count + 1; j++)
        //        {
        //            if (faydaMaliyet[j - 1].ToString() == "Fayda")
        //            {
        //                max1 = Convert.ToDouble(maxList[j - 1]);
        //                min1 = Convert.ToDouble(minList[j - 1]);

        //                for (int i = 0; i < alternatifler.Count; i++)
        //                {
        //                    if (max1 - min1 == 0)
        //                    {
        //                        dataGridViewNormalize.Rows[i].Cells[j].Value = 0;
        //                    }
        //                    else
        //                    {
        //                        dataGridViewNormalize.Rows[i].Cells[j].Value = (Convert.ToDouble(dataGridViewKararMat.Rows[i].Cells[j].Value) - min1) / (max1 - min1);

        //                    }
        //                }
        //            }
        //            else /*if (faydaMaliyet[j - 1].ToString() == "Maliyet")*/
        //            {
        //                max1 = Convert.ToDouble(maxList[j - 1]);
        //                min1 = Convert.ToDouble(minList[j - 1]);
        //                for (int i = 0; i < alternatifler.Count; i++)
        //                {
        //                    if (max1 - min1 == 0)
        //                    {
        //                        dataGridViewNormalize.Rows[i].Cells[j].Value = 0;
        //                    }
        //                    else
        //                    {
        //                        dataGridViewNormalize.Rows[i].Cells[j].Value = (max1 - (Convert.ToDouble(dataGridViewKararMat.Rows[i].Cells[j].Value))) / (max1 - min1);
        //                    }

        //                }

        //            }
        //        }
        //        tabControl1.SelectedTab = tabPageNormalize;// butona tıklanıldığında tabPageNormalize ye gönderen kod
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message + "  Matris Normalize Edilemedi! Lütfen metinsel değerler girmeyiniz.", "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return;
        //    }
        //}
        private void btnKararMatNormalize_Click(object sender, EventArgs e)
        {
            if (yontem == btnAras.Text)
            {
                normalizeToolStripMenuItem.Visible = true;
                arasNormalize();
                tabControl1.SelectedTab = tabPageNormalize;
            }

            if (yontem == btnVikor.Text)
            {
                normalizeToolStripMenuItem.Visible = true;
                vikorNormalize();
                tabControl1.SelectedTab = tabPageNormalize;

            }

            if (yontem == btnTopsis.Text)
            {
                idealVeNegatifİdealUzaklıkToolStripMenuItem.Visible = true;
                negatifIdealUzaklık();
                idealUzaklik();
                tabControl1.SelectedTab = tabPageIdealNegatifUzaklik;

            }
            if (yontem == btnCopras.Text)
            {
                optimallikFonksiyonDeğerleriToolStripMenuItem.Visible = false;
                sonuçlarToolStripMenuItem1.Visible = true;
                vikorSralamaSonuçlarıToolStripMenuItem.Text = "Alternatif Tercih Sıralaması";
                vikorSralamaSonuçlarıToolStripMenuItem.Visible = true;
                coprasQiDegerleri();
                coprasPi();
                coprasAlternatifTercihSira();
                tabControl1.SelectedTab = tabPageVikorSiralama;

            }


        }
        public void vikorNormalizeMatCerceve() //arasta diğer yöntemlerden farklı olarak ek optimal değerlerin yer aldığı bir satır olduğu için yeni bir çerçeve tanımladım
        {
            try
            {
                dataGridViewNormalize.Rows.Clear();
                //normalize matrisi
                dataGridViewNormalize.ColumnCount = kriterler.Count + 1; //alternatif sutunuda bulunması gerektiğinden kriterler+1 tane sutun ekledim
                dataGridViewNormalize.Columns[0].Name = " ";

                for (int i = 0; i < kriterler.Count; i++)
                {
                    dataGridViewNormalize.Columns[i + 1].Name = kriterler[i].ToString();

                }

                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewNormalize.Rows.Add(alternatifler[i].ToString());
                }


            }
            catch (Exception ex)
            {

                MessageBox.Show("Normalize matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void vikorNormalize()
        {
            try
            {
                vikorNormalizeMatCerceve();

                for (int j = 1; j < kriterler.Count + 1; j++)
                {
                    for (int i = 0; i < alternatifler.Count; i++)
                    {
                        dataGridViewNormalize.Rows[i].Cells[j].Value = (Convert.ToDouble(dataGridViewOptimalKararMat.Rows[0].Cells[j].Value) - Convert.ToDouble(dataGridViewKararMat.Rows[i].Cells[j].Value)) / (Convert.ToDouble(dataGridViewOptimalKararMat.Rows[0].Cells[j].Value) - Convert.ToDouble(dataGridViewOptimalKararMat.Rows[1].Cells[j].Value));
                    }
                }



            }
            catch (Exception ex)
            {

                MessageBox.Show("Normalize matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


        }
        public void arasNormalize() //karar matrisini fayda ve maliyet kriteri olmasına göre ayrı formüllerle normalize edip normalizasyon matrisini oluşturan metod
        {
            try
            {
                arasNormalizeMatCerceve();

                for (int j = 1; j < kriterler.Count + 1; j++)
                {
                    if (faydaMaliyet[j - 1].ToString() == "Fayda")
                    {
                        double sutunToplam = 0;
                        for (int i = 0; i < alternatifler.Count + 1; i++)
                        {
                            sutunToplam += Convert.ToDouble(dataGridViewOptimalKararMat.Rows[i].Cells[j].Value);
                        }
                        for (int i = 0; i < alternatifler.Count + 1; i++)
                        {
                            dataGridViewNormalize.Rows[i].Cells[j].Value = Convert.ToDouble(dataGridViewOptimalKararMat.Rows[i].Cells[j].Value) / sutunToplam;
                        }
                    }
                    else /*if (faydaMaliyet[j - 1].ToString() == "Maliyet")*/
                    {
                        double sutunToplam = 0;
                        for (int i = 0; i < alternatifler.Count + 1; i++)
                        {
                            sutunToplam += 1 / Convert.ToDouble(dataGridViewOptimalKararMat.Rows[i].Cells[j].Value);
                        }

                        for (int i = 0; i < alternatifler.Count + 1; i++)
                        {
                            dataGridViewNormalize.Rows[i].Cells[j].Value = (1 / Convert.ToDouble(dataGridViewOptimalKararMat.Rows[i].Cells[j].Value)) / sutunToplam;

                        }

                    }
                }
                tabControl1.SelectedTab = tabPageNormalize;// butona tıklanıldığında tabPageNormalize ye gönderen kod
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "  Matris Normalize Edilemedi! Lütfen metinsel değerler girmeyiniz.", "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void arasNormalizeMatCerceve() //normalize matrisinin ilk satır ve sutununu kriterler ve alternatifler arraylistlerini kullanarak oluşturan metod
        {
            try
            {
                dataGridViewNormalize.Rows.Clear();
                //normalize matrisi
                dataGridViewNormalize.ColumnCount = kriterler.Count + 1; //alternatif sutunuda bulunması gerektiğinden kriterler+1 tane sutun ekledim
                dataGridViewNormalize.Columns[0].Name = " ";

                for (int i = 0; i < kriterler.Count; i++)
                {
                    dataGridViewNormalize.Columns[i + 1].Name = kriterler[i].ToString();

                }
                dataGridViewNormalize.Rows.Add("Optimal Değer");
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewNormalize.Rows.Add(alternatifler[i].ToString());
                }
                // İLK SATIR VE SUTUNDAKİ DEĞERLERİN DEĞİŞTİRİLMESİNİ ENGELLEDİM
                for (int rC = 0; rC < alternatifler.Count + 1; rC++)
                {
                    dataGridViewNormalize.Rows[rC].Cells[rC].ReadOnly = true;
                }


            }
            catch (Exception ex)
            {

                MessageBox.Show("Hata!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        public void kriterSil()
        {
            try
            {
                int secili = listBoxKriter.SelectedIndex;
                faydaMaliyet.RemoveAt(secili);
                kriterler.RemoveAt(secili);
                listBoxKriter.Items.RemoveAt(secili);

                if (listBoxKriter.Items.Count == 0)
                {
                    txtKriter.Focus();
                    btnKriterSil.Enabled = false;

                }
            }
            catch (Exception)
            {
                MessageBox.Show("Lütfen silmek istediğiniz kriteri seçiniz!", "Uyarı ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        public void kararMatDirektAktar()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "XLS files (*.xls)|*.xls|XLT files (*.xlt)|*.xlt|XLSX files (*.xlsx)|*.xlsx|XLSM files (*.xlsm)|*.xlsm|XLTX (*.xltx)|*.xltx|XLTM (*.xltm)|*.xltm|ODS (*.ods)|*.ods|OTS (*.ots)|*.ots|CSV (*.csv)|*.csv|TSV (*.tsv)|*.tsv|HTML (*.html)|*.html|MHTML (.mhtml)|*.mhtml|PDF (*.pdf)|*.pdf|XPS (*.xps)|*.xps|BMP (*.bmp)|*.bmp|GIF (*.gif)|*.gif|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|TIFF (*.tif)|*.tif|WMP (*.wdp)|*.wdp";
                saveFileDialog.FilterIndex = 3;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var workbook = new ExcelFile();
                    var worksheet = workbook.Worksheets.Add("Sheet1");

                    // From DataGridView to ExcelFile.
                    DataGridViewConverter.ImportFromDataGridView(worksheet, this.dataGridViewKararMat, new ImportFromDataGridViewOptions() { ColumnHeaders = true });

                    workbook.Save(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Excel'e aktarma işlemi başarısız!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        private void btnKriterAgirlikBelirleme_Click(object sender, EventArgs e)
        {
            agırlıkBelirlemeToolStripMenuItem1.Visible = true;
            tabControl1.SelectedTab = tabPageAgirlikBelirleme;
        }
        public void normalizeMatDirektEAktar()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "XLS files (*.xls)|*.xls|XLT files (*.xlt)|*.xlt|XLSX files (*.xlsx)|*.xlsx|XLSM files (*.xlsm)|*.xlsm|XLTX (*.xltx)|*.xltx|XLTM (*.xltm)|*.xltm|ODS (*.ods)|*.ods|OTS (*.ots)|*.ots|CSV (*.csv)|*.csv|TSV (*.tsv)|*.tsv|HTML (*.html)|*.html|MHTML (.mhtml)|*.mhtml|PDF (*.pdf)|*.pdf|XPS (*.xps)|*.xps|BMP (*.bmp)|*.bmp|GIF (*.gif)|*.gif|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|TIFF (*.tif)|*.tif|WMP (*.wdp)|*.wdp";
                saveFileDialog.FilterIndex = 3;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var workbook = new ExcelFile();
                    var worksheet = workbook.Worksheets.Add("Sheet1");

                    // From DataGridView to ExcelFile.
                    DataGridViewConverter.ImportFromDataGridView(worksheet, this.dataGridViewNormalize, new ImportFromDataGridViewOptions() { ColumnHeaders = true });

                    workbook.Save(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Excel'e aktarma işlemi başarısız!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void btnNormalizeEAktar_Click(object sender, EventArgs e)
        {
            normalizeMatDirektEAktar();
        }
        private void btnManuel_Click(object sender, EventArgs e)
        {
            if (yontem == btnAras.Text)
            {
                arasAgirlikTemizle();
            }
            manuelAgirlikMatrisi();
        }
        public void arasAgirlikTemizle()
        {
            try
            {

                pnlYontemSec.Visible = true;
                panel12.Visible = false;

                btnKriterAgirlikKaydet.Visible = false;
                btnAhpAyrintiCozGoster.Visible = false;
                panel82.Visible = false;
                panel83.Visible = false;
                panel80.Visible = false;
                label24.Visible = false;
                lblTutOrani.Visible = false;
                btnKriterAgirlikKaydet.Visible = false;
                btnAhpAyrintiCozGoster.Visible = false;
                lblUyari.Visible = false;
                panel52.Visible = false;
                panel81.Visible = false;

                dataGridViewAgirlik.Columns.Clear();
                dataGridViewKarsilastirmaMat.Columns.Clear();
                dataGridViewKriterAgirliklari.Columns.Clear();
                lblTutOrani.Text = "";
                dataGridViewAyrintiKmat.Columns.Clear();
                dataGridViewC.Columns.Clear();
                dataGridViewWVektörü.Columns.Clear();
                dataGridViewDVektör.Columns.Clear();
                lblCI.Text = "";
                lblRI.Text = "";
                lblTutOrani.Text = "";
                agirliklar.Clear();
                paydaListesi.Clear();
                agirlikToplam = 0;
                dataGridViewAgirlikliNormalizeKMat.Columns.Clear();
                dataGridViewOptimalFonkDegerleri.Columns.Clear();
                dataGridViewSonucAgirlikliNormalizeKMat.Columns.Clear();
                dataGridViewSonucOptimalFonkDegerleri.Columns.Clear();

                rbtnDiziboyut = 1;
                rbtnDizi1boyut = 1;
                x = 0;
                y = 0;
                rbtn = 0;
                rbtn1 = 0;
                lamda = 0;
                CI = 0;
                CR = 0;
                //RadioButton[] radioButton;
                // RadioButton[] radioButton1;
            }
            catch (Exception ex)
            {

                MessageBox.Show("Hata!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void manuelAgirlikMatrisi()
        {
            try
            {
                pnlYontemSec.Visible = false;
                panel12.Visible = true;
                dataGridViewAgirlik.Rows.Clear();
                int k = 1;
                dataGridViewAgirlik.ColumnCount = kriterler.Count + 1;
                dataGridViewAgirlik.Columns[0].Name = " ";

                for (int j = 0; j < kriterler.Count; j++)
                {
                    dataGridViewAgirlik.Columns[k].Name = kriterler[j].ToString();
                    k++;
                }

                dataGridViewAgirlik.Rows.Add("Ağırlıklar (virgül ile ayırınız)");



            }
            catch (Exception)
            {

                MessageBox.Show("Hata!", "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        public void ahpTasarim()
        {
            pnlAhpTasarim.Controls.Clear();
            for (int i = 0; i < 9; i++)
            {
                Label lbl = new Label();
                int yi = 9 - i;
                lbl.Name = "lbls" + yi.ToString();
                lbl.Text = yi.ToString();
                lbl.Top = 33;
                lbl.Left = 285 + (22 * i);
                lbl.Width = 12;
                lbl.Height = 15;
                lbl.Font = new Font("Palatino Linotype", 7, FontStyle.Bold);
                pnlAhpTasarim.Controls.Add(lbl);
            }

            for (int i = 0; i < 8; i++)
            {
                Label lbl = new Label();
                int yi = i + 2;
                lbl.Name = "lblk" + yi.ToString();
                lbl.Text = yi.ToString();
                lbl.Top = 33;
                lbl.Left = 485 + (22 * i);
                lbl.Width = 12;
                lbl.Height = 15;
                lbl.Font = new Font("Palatino Linotype", 7, FontStyle.Bold);
                pnlAhpTasarim.Controls.Add(lbl);
            }
            ///BOYUT///////////////////////////////////

            for (int i = 1; i < kriterler.Count; i++)
            {
                for (int j = i; j < kriterler.Count; j++)
                {
                    for (int rb = 1; rb < 10; rb++) // 9 tane 
                    {
                        rbtnDiziboyut++;
                    }
                }

            }

            for (int i = 1; i < kriterler.Count; i++)
            {
                for (int j = i; j < kriterler.Count; j++)
                {
                    for (int rb = 1; rb < 9; rb++) //8 tane
                    {
                        rbtnDizi1boyut++;
                    }
                }

            }

            radioButton = new RadioButton[rbtnDiziboyut];
            radioButton1 = new RadioButton[rbtnDizi1boyut];
            ///////////////////////////////////
            int k = 0;
            for (int i = 1; i < kriterler.Count; i++)
            {
                for (int j = i; j < kriterler.Count; j++)
                {
                    Label lbl = new Label();
                    lbl.Name = "lblRow" + i + j;
                    lbl.Text = i.ToString() + ".Kriter";
                    lbl.Top = 53 + (29 * k);
                    lbl.Left = 218;
                    lbl.Width = 53;
                    lbl.Height = 17;
                    lbl.Font = new Font("Palatino Linotype", 9, FontStyle.Bold);
                    pnlAhpTasarim.Controls.Add(lbl);

                    Label lbl1 = new Label();
                    lbl1.Name = "lblCol" + i + j;
                    lbl1.Text = (j + 1).ToString() + ".Kriter";
                    lbl1.Top = 53 + (29 * k);
                    lbl1.Left = 666;
                    lbl1.Width = 53;
                    lbl1.Height = 17;
                    lbl1.Font = new Font("Palatino Linotype", 9, FontStyle.Bold);
                    pnlAhpTasarim.Controls.Add(lbl1);

                    GroupBox groupBox = new GroupBox();
                    int J = j + 1;
                    groupBox.Name = "groupBox" + i + J;
                    groupBox.Text = "";
                    groupBox.Top = 45 + (29 * k);
                    groupBox.Left = 277;
                    groupBox.Width = 380;
                    groupBox.Height = 30;

                    for (int rb = 1; rb < 10; rb++)
                    {
                        int s = rb - 1;
                        radioButton[rbtn] = new RadioButton();
                        radioButton[rbtn].Name = "rBtnR" + i + J + rb;
                        radioButton[rbtn].Text = "";
                        radioButton[rbtn].Top = 10;
                        radioButton[rbtn].Left = 7 + (22 * s);
                        radioButton[rbtn].Width = 14;
                        radioButton[rbtn].Height = 13;
                        radioButton[rbtn].CheckedChanged += new EventHandler(RadioChange);

                        groupBox.Controls.Add(radioButton[rbtn]);

                        rbtn++;
                    }

                    for (int rb = 1; rb < 9; rb++)
                    {
                        int s = rb - 1;

                        radioButton1[rbtn1] = new RadioButton();
                        radioButton1[rbtn1].Name = "rBtnC" + i + J + rb.ToString();
                        radioButton1[rbtn1].Text = "";
                        radioButton1[rbtn1].Top = 10;
                        radioButton1[rbtn1].Left = 206 + (22 * s);
                        radioButton1[rbtn1].Width = 14;
                        radioButton1[rbtn1].Height = 13;
                        radioButton1[rbtn1].CheckedChanged += new EventHandler(RadioChange);
                        groupBox.Controls.Add(radioButton1[rbtn1]);

                        rbtn1++;

                    }
                    pnlAhpTasarim.Controls.Add(groupBox);
                    k++;
                }
            }
            Button btnKarsilastirmaMatOlustur = new Button();
            btnKarsilastirmaMatOlustur.Name = "btnKararMatOlustur";
            btnKarsilastirmaMatOlustur.AutoSize = false;
            btnKarsilastirmaMatOlustur.Text = "KAYDET";
            btnKarsilastirmaMatOlustur.Top = 7;
            btnKarsilastirmaMatOlustur.Left = 370;
            btnKarsilastirmaMatOlustur.Width = 200;
            btnKarsilastirmaMatOlustur.Height = 36;
            btnKarsilastirmaMatOlustur.Font = new Font(" Bahnschrift Light", 9, FontStyle.Bold);
            btnKarsilastirmaMatOlustur.BackColor = Color.Gray;
            btnKarsilastirmaMatOlustur.ForeColor = Color.White;
            btnKarsilastirmaMatOlustur.FlatStyle = FlatStyle.Popup;
            btnKarsilastirmaMatOlustur.Click += new EventHandler(btnKarsilastirmaMatOlustur_Click);
            panel62.Controls.Add(btnKarsilastirmaMatOlustur);

        }
        private void btnKarsilastirmaMatOlustur_Click(object sender, EventArgs e)
        {
            try
            {
                //if (yontem==btnAras.Text)
                //{
                //    arasAgirlikTemizle();
                //}
                karsilastirmaMatrisiToolStripMenuItem.Visible = true;
                karsilastirmaMatOlustur();
                for (int i = 0; i < dataGridViewKarsilastirmaMat.Rows.Count; i++)
                {
                    for (int j = 1; j < dataGridViewKarsilastirmaMat.Columns.Count; j++)
                    {
                        if (dataGridViewKarsilastirmaMat.Rows[i].Cells[j].Value == null)
                        {
                            MessageBox.Show("Lütfen boş hücreleri doldurunuz!", "Uyarı ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }
                wVektörü();
                ayrintiKMatDoldur();
                DVektörü();
                tutarlilikOrani();
                tabControl1.SelectedTab = tabPageAHP;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        public void karsilastirmaMatOlustur() //dinamik oluşturduğum buton
        {
            btnAhpAyrintiCozGoster.Visible = false;
            try
            {
                for (int i = 1; i < kriterler.Count; i++)
                {
                    for (int j = i; j < kriterler.Count; j++)
                    {
                        int J = j + 1;
                        for (int rb = 9; rb > 0; rb--)
                        {
                            if (radioButton[y].Checked == true)
                            {
                                if (i == 1)
                                {

                                }
                                dataGridViewKarsilastirmaMat.Rows[i - 1].Cells[J].Value = Convert.ToDouble(rb);
                                dataGridViewKarsilastirmaMat.Rows[j].Cells[i].Value = Convert.ToDouble(1 / Convert.ToDouble(rb));
                            }

                            y++;
                        }

                        for (int rb = 2; rb < 10; rb++)
                        {
                            if (radioButton1[x].Checked == true)
                            {
                                dataGridViewKarsilastirmaMat.Rows[j].Cells[i].Value = Convert.ToDouble(rb);
                                dataGridViewKarsilastirmaMat.Rows[i - 1].Cells[J].Value = Convert.ToDouble(1 / Convert.ToDouble(rb));
                            }
                            x++;
                        }
                    }

                }
                y = 0;
                x = 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void paydaHesapla() //yüzde önem dağılımlarını hesaplamak için gereken sutun toplamlarını hesaplayıp paydaListesi' ne ekleyen metod
        {
            try
            {
                for (int j = 1; j < kriterler.Count + 1; j++)
                {
                    double payda = 0;
                    for (int i = 0; i < kriterler.Count; i++)
                    {
                        payda += Convert.ToDouble(dataGridViewKarsilastirmaMat.Rows[i].Cells[j].Value);
                    }
                    paydaListesi.Add(payda);

                    payda = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        //kriterlerin birbirlerine göre önem değerlerini gösterir
        public void cMatrisi()
        {
            try
            {
                cMatrisTasarim();
                double pay, sonuc;
                paydaHesapla();
                for (int j = 1; j < kriterler.Count + 1; j++)
                {
                    for (int i = 0; i < kriterler.Count; i++)
                    {
                        pay = Convert.ToDouble(dataGridViewKarsilastirmaMat.Rows[i].Cells[j].Value);
                        sonuc = pay / Convert.ToDouble(paydaListesi[j - 1]);
                        dataGridViewC.Rows[i].Cells[j].Value = sonuc;
                        sonuc = 0;
                        pay = 0;
                    }

                }
            }
            catch (Exception)
            {

                MessageBox.Show("C matrisi oluşturulamadı.!", "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        public void cMatrisTasarim()
        {
            dataGridViewC.Rows.Clear();
            int k = 1;
            dataGridViewC.ColumnCount = kriterler.Count + 1;
            dataGridViewC.Columns[0].Name = " ";

            for (int i = 0; i < kriterler.Count; i++)
            {
                k = 1;
                for (int j = 0; j < kriterler.Count; j++)
                {
                    dataGridViewC.Columns[k].Name = kriterler[j].ToString();
                    k++;
                }
            }
            //dataGridViewC.Rows.Clear();
            for (int j = 0; j < kriterler.Count; j++)
            {
                dataGridViewC.Rows.Add(kriterler[j].ToString());
            }
            for (int cR = 0; cR < kriterler.Count; cR++)
            {
                dataGridViewC.Rows[cR].Cells[0].ReadOnly = true;
            }


        }
        public void wVektörüTasarim() //öncelik (w) vektörünü hesaplayan metod
        {
            dataGridViewWVektörü.Rows.Clear();
            int k = 1;
            dataGridViewWVektörü.ColumnCount = kriterler.Count + 1;
            dataGridViewWVektörü.Columns[0].Name = " ";

            for (int j = 0; j < kriterler.Count; j++)
            {
                dataGridViewWVektörü.Columns[k].Name = kriterler[j].ToString();
                k++;
            }
            for (int j = 0; j < 1; j++)
            {
                //dataGridViewWVektörü.Rows.Clear();
                dataGridViewWVektörü.Rows.Add("Ağırlıklar");
            }

            dataGridViewWVektörü.Rows[0].Cells[0].ReadOnly = true;
        }
        public void wVektörü() //öncelik (w) vektörünü hesaplayan metod
        {
            try
            {
                cMatrisi();
                wVektörüTasarim();
                for (int i = 0; i < kriterler.Count; i++)
                {
                    double satirToplam = 0;
                    for (int j = 1; j < kriterler.Count + 1; j++)
                    {
                        satirToplam += Convert.ToDouble(dataGridViewC.Rows[i].Cells[j].Value);
                    }
                    dataGridViewWVektörü.Rows[0].Cells[i + 1].Value = (satirToplam / kriterler.Count);
                    agirliklar.Add(satirToplam / kriterler.Count);
                    satirToplam = 0;

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("V vektörü oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void ayrintiKMatDoldur()
        {
            try
            {
                dataGridViewAyrintiKmat.Rows.Clear();
                int k = 1;
                dataGridViewAyrintiKmat.ColumnCount = dataGridViewKarsilastirmaMat.Columns.Count;
                for (int i = 0; i < kriterler.Count; i++)
                {
                    k = 1;
                    for (int j = 0; j < kriterler.Count; j++)
                    {
                        dataGridViewAyrintiKmat.Columns[k].Name = kriterler[j].ToString();
                        k++;
                    }
                }
                for (int j = 0; j < kriterler.Count; j++)
                {
                    dataGridViewAyrintiKmat.Rows.Add(kriterler[j].ToString());
                }
                for (int cR = 0; cR < kriterler.Count; cR++)
                {
                    dataGridViewAyrintiKmat.Rows[cR].Cells[0].ReadOnly = true;
                }
                for (int i = 0; i < dataGridViewKarsilastirmaMat.Rows.Count; i++)
                {
                    for (int j = 0; j < dataGridViewKarsilastirmaMat.Columns.Count; j++)
                    {
                        dataGridViewAyrintiKmat.Rows[i].Cells[j].Value = dataGridViewKarsilastirmaMat.Rows[i].Cells[j].Value.ToString();
                    }
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Hata!", "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        public void DVektörüTasarim() //öncelik (w) vektörünü hesaplayan metod
        {
            dataGridViewDVektör.Rows.Clear();
            int k = 1;
            dataGridViewDVektör.ColumnCount = kriterler.Count + 1;
            dataGridViewDVektör.Columns[0].Name = " ";

            for (int j = 0; j < kriterler.Count; j++)
            {
                dataGridViewDVektör.Columns[k].Name = kriterler[j].ToString();
                k++;
            }
            for (int j = 0; j < 1; j++)
            {
                //dataGridViewDVektör.Rows.Clear();
                dataGridViewDVektör.Rows.Add("D satır vektörü");
            }

            dataGridViewDVektör.Rows[0].Cells[0].ReadOnly = true;

        }
        public void DVektörü()
        {
            try
            {
                DVektörüTasarim();
                for (int i = 0; i < kriterler.Count; i++)
                {
                    double carpim, toplam = 0;
                    for (int j = 1; j < kriterler.Count + 1; j++)
                    {
                        carpim = Convert.ToDouble(dataGridViewAyrintiKmat.Rows[i].Cells[j].Value) * Convert.ToDouble(dataGridViewWVektörü.Rows[0].Cells[j].Value);
                        toplam += carpim;
                    }
                    dataGridViewDVektör.Rows[0].Cells[i + 1].Value = toplam;
                    carpim = 0;
                    toplam = 0;

                }

            }
            catch (Exception)
            {

                MessageBox.Show("D vektörü oluşturulamadı!", "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        public void eToplamı()
        {
            double e, eToplam = 0;
            for (int j = 1; j < kriterler.Count + 1; j++)
            {
                e = Convert.ToDouble(dataGridViewDVektör.Rows[0].Cells[j].Value) / Convert.ToDouble(dataGridViewWVektörü.Rows[0].Cells[j].Value);
                eToplam += e;
                e = 0;
            }
            lamda = eToplam / kriterler.Count;
            //eToplam = 0;
        }
        public void tutarlilikOrani()
        {
            try
            {
                double RI = 0;
                eToplamı();
                CI = (lamda - kriterler.Count) / (kriterler.Count - 1);
                lamda = 0;
                if (kriterler.Count == 1)
                {
                    RI = 0;
                }
                else if (kriterler.Count == 2)
                {
                    RI = 0;
                }
                else if (kriterler.Count == 3)
                {
                    RI = 0.58;
                }
                else if (kriterler.Count == 4)
                {
                    RI = 0.90;
                }
                else if (kriterler.Count == 5)
                {
                    RI = 1.12;
                }
                else if (kriterler.Count == 6)
                {
                    RI = 1.24;
                }
                else if (kriterler.Count == 7)
                {
                    RI = 1.32;
                }
                else if (kriterler.Count == 8)
                {
                    RI = 1.41;
                }
                else if (kriterler.Count == 9)
                {
                    RI = 1.45;
                }
                else if (kriterler.Count == 10)
                {
                    RI = 1.49;
                }
                else if (kriterler.Count == 11)
                {
                    RI = 1.51;
                }
                else if (kriterler.Count == 12)
                {
                    RI = 1.48;
                }
                else if (kriterler.Count == 13)
                {
                    RI = 1.56;
                }
                else if (kriterler.Count == 14)
                {
                    RI = 1.57;
                }
                else if (kriterler.Count == 15)
                {
                    RI = 1.59;
                }
                CR = Convert.ToDouble(CI / RI);
                lblCI.Text = CI.ToString();
                lblRI.Text = RI.ToString();
                lblTutarlilikOrani.Text = "";
                lblTutarlilikOrani.Text = CR.ToString();
                lblTutOrani.Text = "";
                lblTutOrani.Text = CR.ToString();
                CR = 0;
                CI = 0;
                RI = 0;
            }
            catch (Exception)
            {

                MessageBox.Show("Tutarlılık oranı hesaplanamadı!", "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


        }
        protected void RadioChange(object sender, EventArgs e)
        {

        }
        private void btnAhp_Click(object sender, EventArgs e)
        {
            aHPToolStripMenuItem.Visible = true;

            arasAgirlikTemizle();
            try
            {
                dataGridViewKarsilastirmaMat.Rows.Clear();
                pnlAhpTasarim.Controls.Clear();
                int k = 1;
                dataGridViewKarsilastirmaMat.ColumnCount = kriterler.Count + 1;
                dataGridViewKarsilastirmaMat.Columns[0].Name = " ";

                for (int i = 0; i < kriterler.Count; i++)
                {
                    k = 1;
                    for (int j = 0; j < kriterler.Count; j++)
                    {
                        dataGridViewKarsilastirmaMat.Columns[k].Name = kriterler[j].ToString();
                        k++;
                    }
                }
                for (int i = 0; i < kriterler.Count; i++)
                {
                    k = 1;
                    for (int j = 0; j < kriterler.Count; j++)
                    {
                        dataGridViewKarsilastirmaMat.Columns[k].Name = kriterler[j].ToString();
                        k++;
                    }
                }
                for (int j = 0; j < kriterler.Count; j++)
                {
                    dataGridViewKarsilastirmaMat.Rows.Add(kriterler[j].ToString());
                }
                for (int cR = 0; cR < kriterler.Count; cR++)
                {
                    dataGridViewKarsilastirmaMat.Rows[cR].Cells[0].ReadOnly = true;
                }
                for (int i = 0; i < kriterler.Count; i++)
                {
                    dataGridViewKarsilastirmaMat.Rows[i].Cells[i + 1].Value = 1;
                }

                ahpTasarim();

                tabControl1.SelectedTab = tabPageKarsilastirmaMat;
                agirlikDeğerleriToolStripMenuItem.Visible = true;
            }
            catch (Exception)
            {

                MessageBox.Show("AHP Hesaplanamadı. !", "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        private void txtAlternatif_Leave(object sender, EventArgs e)
        {
            if (txtAlternatif.Text == "")
            {
                txtAlternatif.Text = text2;
            }
        }
        private void txtAlternatif_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                btnAlternatifEkle.PerformClick();
            }
        }
        private void txtAlternatif_Enter(object sender, EventArgs e)
        {
            if (txtAlternatif.Text == text2)
            {
                txtAlternatif.Text = "";
            }
        }
        private void btnAgirlikKaydet_Click(object sender, EventArgs e)
        {
            agirlikliNormalizeMatrisiToolStripMenuItem.Visible = true;
            pnlYontemSec.Visible = true;
            agirliklar.Clear();

            //ağırlıklar listesine ağırlık değerlerini ekledim
            try
            {
                for (int j = 1; j < kriterler.Count + 1; j++)
                {
                    agirliklar.Add(dataGridViewAgirlik.Rows[0].Cells[j].Value.ToString());
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Lütfen boş hücreleri doldurunuz!", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //AĞIRLIK TOPLAMALARININ 1 EŞİT OLUP OLMADIĞININ KONTROLÜ
            agirlikToplam = 0;
            foreach (var item in agirliklar)
            {
                agirlikToplam += Convert.ToDouble(item);
            }
            if (agirlikToplam > 1.1 || agirlikToplam < 0.99)
            {
                MessageBox.Show("Toplam ağırlık: " + agirlikToplam + " Lütfen girilen değerleri kontrol edip tekrar deneyiniz!", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (yontem == btnAras.Text)
            {
                agirlikliNormalizeKararMat();
                tabControl1.SelectedTab = tabPageAgirlikliNormalizeKMat;
            }
            if (yontem == btnVikor.Text)
            {
                vikorAgirlikliNormalizeKararMat();
                tabControl1.SelectedTab = tabPageAgirlikliNormalizeKMat;
            }
            if (yontem == btnCopras.Text)
            {
                coprasAgirlikliNormalize();
                tabControl1.SelectedTab = tabPageAgirlikliNormalizeKMat;
            }
            if (yontem == btnTopsis.Text)
            {
                topsisAgirlikliNormalize();
                tabControl1.SelectedTab = tabPageAgirlikliNormalizeKMat;
            }
        }
        public void vikorAgirlikliNormalizeKararMatCerceve()
        {
            try
            {
                dataGridViewAgirlikliNormalizeKMat.Rows.Clear();
                dataGridViewAgirlikliNormalizeKMat.ColumnCount = kriterler.Count + 1; //alternatif sutunuda bulunması gerektiğinden kriterler+1 tane sutun ekledim
                dataGridViewAgirlikliNormalizeKMat.Columns[0].Name = " ";

                for (int i = 0; i < kriterler.Count; i++)
                {
                    dataGridViewAgirlikliNormalizeKMat.Columns[i + 1].Name = kriterler[i].ToString();

                }

                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewAgirlikliNormalizeKMat.Rows.Add(alternatifler[i].ToString());
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Hata!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void vikorAgirlikliNormalizeKararMat()
        {
            try
            {
                vikorAgirlikliNormalizeKararMatCerceve();
                for (int j = 1; j < kriterler.Count + 1; j++)
                {
                    for (int i = 0; i < alternatifler.Count; i++)
                    {
                        dataGridViewAgirlikliNormalizeKMat.Rows[i].Cells[j].Value = Convert.ToDouble(dataGridViewNormalize.Rows[i].Cells[j].Value) * Convert.ToDouble(agirliklar[j - 1]);
                    }
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Ağırlıklı normalize matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void agirlikliNormalizeKararMatCerceve()
        {
            try
            {
                dataGridViewAgirlikliNormalizeKMat.Rows.Clear();
                dataGridViewAgirlikliNormalizeKMat.ColumnCount = kriterler.Count + 1; //alternatif sutunuda bulunması gerektiğinden kriterler+1 tane sutun ekledim
                dataGridViewAgirlikliNormalizeKMat.Columns[0].Name = " ";

                for (int i = 0; i < kriterler.Count; i++)
                {
                    dataGridViewAgirlikliNormalizeKMat.Columns[i + 1].Name = kriterler[i].ToString();

                }
                dataGridViewAgirlikliNormalizeKMat.Rows.Add("Optimal Değer");
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewAgirlikliNormalizeKMat.Rows.Add(alternatifler[i].ToString());
                }



            }
            catch (Exception ex)
            {

                MessageBox.Show("Ağırlıklı normalize matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        } //aras
        public void agirlikliNormalizeKararMat() //aras
        {
            //normalize matrisinin ağırlık değerleriyle çarpılmış hali
            try
            {
                agirlikliNormalizeKararMatCerceve();
                for (int j = 1; j < kriterler.Count + 1; j++)
                {
                    for (int i = 0; i < alternatifler.Count + 1; i++)
                    {
                        dataGridViewAgirlikliNormalizeKMat.Rows[i].Cells[j].Value = Convert.ToDouble(dataGridViewNormalize.Rows[i].Cells[j].Value) * (Convert.ToDouble(agirliklar[j - 1]));
                    }
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Hata!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void dgvKriterAgirlikDoldur()
        {
            try
            {
                dataGridViewKriterAgirliklari.ColumnCount = dataGridViewWVektörü.Columns.Count;
                //dataGridViewKriterAgirliklari.Columns[0].Name = " ";
                int k = 1;
                for (int j = 0; j < kriterler.Count; j++)
                {
                    dataGridViewKriterAgirliklari.Columns[k].Name = kriterler[j].ToString();
                    k++;
                }

                dataGridViewKriterAgirliklari.Rows.Add("Ağırlıklar");

                for (int i = 0; i < dataGridViewWVektörü.Rows.Count; i++)
                {
                    for (int j = 0; j < dataGridViewWVektörü.Columns.Count; j++)
                    {
                        dataGridViewKriterAgirliklari.Rows[i].Cells[j].Value = dataGridViewWVektörü.Rows[i].Cells[j].Value.ToString();
                    }
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Kriter ağırlıkları getirilemedi!", "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        private void btnAhpHesapla_Click(object sender, EventArgs e)
        {
            try
            {

                btnKriterAgirlikKaydet.Visible = true;
                btnAhpAyrintiCozGoster.Visible = true;
                dataGridViewKriterAgirliklari.Rows.Clear();
                panel82.Visible = true;
                panel81.Visible = true;
                panel83.Visible = true;
                dgvKriterAgirlikDoldur();
                panel80.Visible = true;
                label24.Visible = true;
                lblTutOrani.Visible = true;


                if (Convert.ToDouble(lblTutOrani.Text) >= 0.10)
                {
                    lblUyari.Visible = true;
                    panel52.Visible = true;
                    btnKriterAgirlikKaydet.Text = "Bu şekilde devam et";
                }


            }
            catch (Exception)
            {

                MessageBox.Show("AHP Hesaplanamadı!", "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void kriterAgirlikKaydet()
        {
            agirliklar.Clear();
            try
            {
                for (int j = 1; j < kriterler.Count + 1; j++)
                {
                    agirliklar.Add(dataGridViewKriterAgirliklari.Rows[0].Cells[j].Value.ToString());
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Lütfen boş hücreleri doldurunuz!", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


        }
        private void btnKriterAgirlikKaydet_Click(object sender, EventArgs e)
        {
            agirlikDeğerleriToolStripMenuItem.Visible = true;
            AHPAyrintiliCozumlerToolStripMenuItem.Visible = true;
            agirlikliNormalizeMatrisiToolStripMenuItem.Visible = true;

            kriterAgirlikKaydet();
            if (yontem == btnAras.Text)
            {
                agirlikliNormalizeKararMat();
                tabControl1.SelectedTab = tabPageAgirlikliNormalizeKMat;
            }
            if (yontem == btnVikor.Text)
            {
                vikorAgirlikliNormalizeKararMat();
                tabControl1.SelectedTab = tabPageAgirlikliNormalizeKMat;
            }
            if (yontem == btnCopras.Text)
            {
                coprasAgirlikliNormalize();
                tabControl1.SelectedTab = tabPageAgirlikliNormalizeKMat;
            }
            if (yontem == btnTopsis.Text)
            {
                topsisAgirlikliNormalize();
                tabControl1.SelectedTab = tabPageAgirlikliNormalizeKMat;
            }
        }
        private void btnAgirlikDuzenle_Click(object sender, EventArgs e)
        {
            panel82.Visible = false;
            panel83.Visible = false;
            panel80.Visible = false;
            label24.Visible = false;
            lblTutOrani.Visible = false;
            btnKriterAgirlikKaydet.Visible = false;
            lblUyari.Visible = false;
            panel52.Visible = false;
            btnKriterAgirlikKaydet.Text = "AĞIRLIKLARI KAYDET";


            tabControl1.SelectedTab = tabPageKarsilastirmaMat;
        }
        private void btnAhpAyrintiCozGoster_Click(object sender, EventArgs e)
        {
            AHPAyrintiliCozumlerToolStripMenuItem.Visible = true;

            //tutarlılık oranı hesaplanırken zaten gridler doldurulmuştu.
            tabControl1.SelectedTab = tabPageAhpAyrinti;
        }
        //public void ikiliKMatExcelAktar()
        //{
        //    try
        //    {
        //        if (dataGridViewKarsilastirmaMat.Rows.Count > 0)
        //        {

        //            Microsoft.Office.Interop.Excel.Application xcelApp = new Microsoft.Office.Interop.Excel.Application();
        //            xcelApp.Application.Workbooks.Add(Type.Missing);

        //            for (int i = 1; i < dataGridViewKarsilastirmaMat.Columns.Count + 1; i++)
        //            {
        //                xcelApp.Cells[1, i] = dataGridViewKarsilastirmaMat.Columns[i - 1].HeaderText;
        //            }

        //            for (int i = 0; i < dataGridViewKarsilastirmaMat.Rows.Count; i++)
        //            {
        //                for (int j = 0; j < dataGridViewKarsilastirmaMat.Columns.Count; j++)
        //                {
        //                    xcelApp.Cells[i + 2, j + 1] = dataGridViewKarsilastirmaMat.Rows[i].Cells[j].Value.ToString();
        //                }
        //            }
        //            if (dataGridViewKriterAgirliklari.Rows.Count > 0)
        //            {

        //                for (int i = 1; i < dataGridViewKriterAgirliklari.Columns.Count + 1; i++)
        //                {
        //                    xcelApp.Cells[dataGridViewKarsilastirmaMat.Rows.Count + 4, i] = dataGridViewKriterAgirliklari.Columns[i - 1].HeaderText;
        //                }
        //                int a = 0;
        //                for (int i = dataGridViewKarsilastirmaMat.Rows.Count; i < dataGridViewKarsilastirmaMat.Rows.Count + dataGridViewKriterAgirliklari.Rows.Count; i++)
        //                {
        //                    int s = 0;
        //                    for (int j = 0; j < dataGridViewKriterAgirliklari.Columns.Count; j++)
        //                    {
        //                        xcelApp.Cells[i + 5, j + 1] = dataGridViewKriterAgirliklari.Rows[a].Cells[s].Value.ToString();
        //                        s++;
        //                    }
        //                    a++;
        //                }
        //            }
        //            xcelApp.Columns.AutoFit();
        //            xcelApp.Visible = true;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        MessageBox.Show("Excel'e aktarılamadı!", "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return;
        //    }
        //}
        public void ikiliKMatExcelDirektAktar()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "XLS files (*.xls)|*.xls|XLT files (*.xlt)|*.xlt|XLSX files (*.xlsx)|*.xlsx|XLSM files (*.xlsm)|*.xlsm|XLTX (*.xltx)|*.xltx|XLTM (*.xltm)|*.xltm|ODS (*.ods)|*.ods|OTS (*.ots)|*.ots|CSV (*.csv)|*.csv|TSV (*.tsv)|*.tsv|HTML (*.html)|*.html|MHTML (.mhtml)|*.mhtml|PDF (*.pdf)|*.pdf|XPS (*.xps)|*.xps|BMP (*.bmp)|*.bmp|GIF (*.gif)|*.gif|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|TIFF (*.tif)|*.tif|WMP (*.wdp)|*.wdp";
                saveFileDialog.FilterIndex = 3;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var workbook = new ExcelFile();
                    var worksheet = workbook.Worksheets.Add("Sheet1");

                    // From DataGridView to ExcelFile.
                    DataGridViewConverter.ImportFromDataGridView(worksheet, this.dataGridViewKarsilastirmaMat, new ImportFromDataGridViewOptions() { ColumnHeaders = true });
                    DataGridViewConverter.ImportFromDataGridView(worksheet, this.dataGridViewKriterAgirliklari, new ImportFromDataGridViewOptions()
                    {
                        ColumnHeaders = true,
                        StartRow = dataGridViewKarsilastirmaMat.Rows.Count + 3
                    });
                    workbook.Save(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Excel'e aktarma işlemi başarısız!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void btnIkiliKMatEAktar_Click(object sender, EventArgs e)
        {
            ikiliKMatExcelDirektAktar();
            //    ikiliKMatExcelAktar();
        }
        public void optimalListeDoldur()
        {
            for (int j = 0; j < kriterler.Count; j++)
            {
                double optimal = Convert.ToDouble(dataGridViewKararMat.Rows[0].Cells[j + 1].Value);
                if (faydaMaliyet[j].ToString() == rbtnFayda.Text)
                {
                    for (int i = 1; i < alternatifler.Count; i++)
                    {
                        if (optimal < Convert.ToDouble(dataGridViewKararMat.Rows[i].Cells[j + 1].Value))
                        {
                            optimal = Convert.ToDouble(dataGridViewKararMat.Rows[i].Cells[j + 1].Value);
                        }
                    }

                    optimalList.Add(optimal);
                }

                else if (faydaMaliyet[j].ToString() == rbtnMaliyet.Text)
                {

                    for (int i = 1; i < alternatifler.Count; i++)
                    {
                        if (optimal > Convert.ToDouble(dataGridViewKararMat.Rows[i].Cells[j + 1].Value))
                        {
                            optimal = Convert.ToDouble(dataGridViewKararMat.Rows[i].Cells[j + 1].Value);
                        }
                    }

                    optimalList.Add(optimal);
                }
            }
        }
        public void optimalKararMatOlustur()
        {
            try
            {
                optimalListeDoldur();
                dataGridViewOptimalKararMat.Rows.Clear();

                dataGridViewOptimalKararMat.ColumnCount = kriterler.Count + 1;
                dataGridViewOptimalKararMat.Columns[0].Name = " ";
                int k = 1;
                for (int i = 0; i < kriterler.Count; i++)
                {
                    dataGridViewOptimalKararMat.Columns[k].Name = kriterler[i].ToString();
                    k++;
                }
                dataGridViewOptimalKararMat.Rows.Add("Optimal Değer");
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewOptimalKararMat.Rows.Add(alternatifler[i].ToString());
                }
                for (int j = 1; j < kriterler.Count + 1; j++)
                {
                    dataGridViewOptimalKararMat.Rows[0].Cells[j].Value = optimalList[j - 1];
                }

                for (int j = 1; j < kriterler.Count + 1; j++)
                {
                    for (int i = 1; i < alternatifler.Count + 1; i++)
                    {
                        dataGridViewOptimalKararMat.Rows[i].Cells[j].Value = dataGridViewKararMat.Rows[i - 1].Cells[j].Value;
                    }
                }

                ////İLK SATIR VE İLK SUTUNDAKİ DEĞERLERİN DEĞİŞTİRİLMESİNİ ENGELLEDİM
                //for (int rC = 0; rC < alternatifler.Count; rC++)
                //{
                //    dataGridViewKararMat.Rows[rC].Cells[0].ReadOnly = true;
                //}

            }
            catch (Exception ex)
            {

                MessageBox.Show("Hata!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        private void btnOptimalGit_Click(object sender, EventArgs e)
        {
            //boş hücre kontrolü
            for (int i = 0; i < alternatifler.Count; i++) //satır
            {
                for (int j = 1; j < kriterler.Count + 1; j++) //sutun
                {
                    if (dataGridViewKararMat.Rows[i].Cells[j].Value == null)
                    {
                        //dataGridViewKararMat.Visible = false;
                        MessageBox.Show("Lütfen boş hücreleri doldurunuz!", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                }
            }
            if (yontem == btnAras.Text)
            {
                optimalKararMatrisiToolStripMenuItem.Visible = true;
                optimalKararMatOlustur();
                tabControl1.SelectedTab = tabPageOptimal;
            }

            if (yontem == btnVikor.Text)
            {
                optimalKararMatrisiToolStripMenuItem.Text = "En iyi en kötü kriter değerleri";
                optimalKararMatrisiToolStripMenuItem.Visible = true;
                vikorEnİyiEnKotuKriterMat();
                tabControl1.SelectedTab = tabPageOptimal;

            }

            if (yontem == btnTopsis.Text)
            {
                normalizeToolStripMenuItem.Visible = true;
                topsisNormalize();
                tabControl1.SelectedTab = tabPageNormalize;
            }

            if (yontem == btnCopras.Text)
            {
                normalizeToolStripMenuItem.Visible = true;
                coprasNormalize();
                tabControl1.SelectedTab = tabPageNormalize;
            }



        }
        public void vikorEnİyiEnKotuKriterMatCerceve()
        {
            try
            {
                dataGridViewOptimalKararMat.Rows.Clear();

                dataGridViewOptimalKararMat.ColumnCount = kriterler.Count + 1;
                dataGridViewOptimalKararMat.Columns[0].Name = " ";
                int k = 1;
                for (int i = 0; i < kriterler.Count; i++)
                {
                    dataGridViewOptimalKararMat.Columns[k].Name = kriterler[i].ToString();
                    k++;
                }
                dataGridViewOptimalKararMat.Rows.Add("fj*");
                dataGridViewOptimalKararMat.Rows.Add("fj-");

            }
            catch (Exception ex)
            {

                MessageBox.Show("Hata!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void vikorEnİyiEnKotuKriterMat()
        {
            try
            {
                maxMin(); //karar matrisindeki max ve min değerleri bulup listeye atan metod
                vikorEnİyiEnKotuKriterMatCerceve();

                for (int i = 0; i < kriterler.Count; i++)
                {
                    if (faydaMaliyet[i].ToString() == rbtnFayda.Text)
                    {
                        dataGridViewOptimalKararMat.Rows[0].Cells[i + 1].Value = Convert.ToDouble(maxList[i]);
                        dataGridViewOptimalKararMat.Rows[1].Cells[i + 1].Value = Convert.ToDouble(minList[i]);
                    }
                    else if (faydaMaliyet[i].ToString() == rbtnMaliyet.Text)
                    {
                        dataGridViewOptimalKararMat.Rows[0].Cells[i + 1].Value = Convert.ToDouble(minList[i]);
                        dataGridViewOptimalKararMat.Rows[1].Cells[i + 1].Value = Convert.ToDouble(maxList[i]);
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("En iyi ve en kötü kriter değerleri belirlenemedi!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


        }
        public void optimalKararMatEAktar()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "XLS files (*.xls)|*.xls|XLT files (*.xlt)|*.xlt|XLSX files (*.xlsx)|*.xlsx|XLSM files (*.xlsm)|*.xlsm|XLTX (*.xltx)|*.xltx|XLTM (*.xltm)|*.xltm|ODS (*.ods)|*.ods|OTS (*.ots)|*.ots|CSV (*.csv)|*.csv|TSV (*.tsv)|*.tsv|HTML (*.html)|*.html|MHTML (.mhtml)|*.mhtml|PDF (*.pdf)|*.pdf|XPS (*.xps)|*.xps|BMP (*.bmp)|*.bmp|GIF (*.gif)|*.gif|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|TIFF (*.tif)|*.tif|WMP (*.wdp)|*.wdp";
                saveFileDialog.FilterIndex = 3;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var workbook = new ExcelFile();
                    var worksheet = workbook.Worksheets.Add("Sheet1");

                    // From DataGridView to ExcelFile.
                    DataGridViewConverter.ImportFromDataGridView(worksheet, this.dataGridViewOptimalKararMat, new ImportFromDataGridViewOptions() { ColumnHeaders = true });

                    workbook.Save(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Excel'e aktarma işlemi başarısız!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void btnOptimalKararMatEAktar_Click(object sender, EventArgs e)
        {
            optimalKararMatEAktar();
        }
        private void btnKararMatEAktar_Click(object sender, EventArgs e)
        {
            kararMatDirektAktar();
        }
        public void optimalFonkDegerMatCerceve()
        {
            try
            {

                dataGridViewOptimalFonkDegerleri.Rows.Clear();

                dataGridViewOptimalFonkDegerleri.ColumnCount = 4;
                dataGridViewOptimalFonkDegerleri.Columns[0].Name = " ";
                dataGridViewOptimalFonkDegerleri.Columns[1].Name = "Si";
                dataGridViewOptimalFonkDegerleri.Columns[2].Name = "Ki";
                dataGridViewOptimalFonkDegerleri.Columns[3].Name = "%Ki";


                dataGridViewOptimalFonkDegerleri.Rows.Add("Optimal");
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewOptimalFonkDegerleri.Rows.Add(alternatifler[i].ToString());
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Optimal fonksiyon değerleri matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void siDegerleri()
        {
            for (int i = 0; i < alternatifler.Count + 1; i++)
            {
                double si = 0;
                for (int j = 1; j < kriterler.Count + 1; j++)
                {
                    si += Convert.ToDouble(dataGridViewAgirlikliNormalizeKMat.Rows[i].Cells[j].Value);
                }
                dataGridViewOptimalFonkDegerleri.Rows[i].Cells[1].Value = si;
            }
        }
        public void kiDegerleri()
        {
            for (int i = 1; i < alternatifler.Count + 1; i++)
            {
                double ki = 0;
                ki = Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[i].Cells[1].Value) / Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[0].Cells[1].Value);
                dataGridViewOptimalFonkDegerleri.Rows[i].Cells[2].Value = ki;

            }

        }
        public void kiYuzdeDegerleri()
        {
            for (int i = 1; i < alternatifler.Count + 1; i++)
            {
                double kiYuzde = 0;
                kiYuzde = Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[i].Cells[2].Value) * 100;
                dataGridViewOptimalFonkDegerleri.Rows[i].Cells[3].Value = Math.Round(kiYuzde, 2).ToString() + "%";

            }

        }
        public void optimalFonkEnİyiAlternatif()
        {
            DataGridViewCellStyle renk = new DataGridViewCellStyle();

            double enBuyuk = Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[1].Cells[2].Value);
            int index = dataGridViewOptimalFonkDegerleri.Rows[1].Cells[2].RowIndex;
            for (int j = 2; j < alternatifler.Count + 1; j++)
            {
                if (enBuyuk < Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[j].Cells[2].Value))
                {
                    enBuyuk = Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[j].Cells[2].Value);
                    index = dataGridViewOptimalFonkDegerleri.Rows[j].Cells[2].RowIndex;
                    renk.BackColor = Color.LightSteelBlue;

                }
            }
            dataGridViewOptimalFonkDegerleri.Rows[index].DefaultCellStyle = renk;
            lblEnİyiAlternatif.Text = dataGridViewOptimalFonkDegerleri.Rows[index].Cells[0].Value.ToString();
        }
        public void optimallikFonkDegerHesapla()
        {
            try
            {
                optimalFonkDegerMatCerceve();
                siDegerleri();
                kiDegerleri();
                kiYuzdeDegerleri();
                optimalFonkEnİyiAlternatif();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Hata!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        private void btnOptimallikFonkDegerHesapla_Click(object sender, EventArgs e)
        {
            if (yontem == btnAras.Text)
            {
                sonuçlarToolStripMenuItem1.Visible = true;
                optimallikFonksiyonDeğerleriToolStripMenuItem.Visible = true;
                optimallikFonkDegerHesapla();
                tabControl1.SelectedTab = tabPageOptimallikFonksiyonDeğerleri;
            }
            if (yontem == btnVikor.Text)
            {
                sonuçlarToolStripMenuItem1.Visible = true;
                optimallikFonksiyonDeğerleriToolStripMenuItem.Text = "Sİ, Rİ VE Qİ DEĞERLERİ";
                optimallikFonksiyonDeğerleriToolStripMenuItem.Visible = true;
                siRiQiDegerleri();
                tabControl1.SelectedTab = tabPageOptimallikFonksiyonDeğerleri;

            }
            if (yontem == btnTopsis.Text)
            {
                optimallikFonksiyonDeğerleriToolStripMenuItem.Visible = false;
                sonuçlarToolStripMenuItem1.Visible = true;
                optimalKararMatrisiToolStripMenuItem.Text = "İdeal ve Negatif İdeal Çözüm Değerleri";
                optimalKararMatrisiToolStripMenuItem.Visible = true;
                topsisIdealNegatifIdealCozumDegeri();
                tabControl1.SelectedTab = tabPageOptimal;

            }
            if (yontem == btnCopras.Text)
            {
                optimalKararMatrisiToolStripMenuItem.Visible = true;
                optimalKararMatrisiToolStripMenuItem.Text = "Sİ, Rİ VE Qİ DEĞERLERİ";
                coprasSiDegerleri();
                tabControl1.SelectedTab = tabPageOptimal;

            }
        }
        public void siRiQiDegerleriMatCerceve()
        {
            try
            {
                dataGridViewOptimalFonkDegerleri.Rows.Clear();
                dataGridViewOptimalFonkDegerleri.ColumnCount = 8;
                dataGridViewOptimalFonkDegerleri.Columns[0].Name = " ";
                dataGridViewOptimalFonkDegerleri.Columns[1].Name = "Sİ";
                dataGridViewOptimalFonkDegerleri.Columns[2].Name = "Rİ";
                dataGridViewOptimalFonkDegerleri.Columns[3].Name = "q=0,0";
                dataGridViewOptimalFonkDegerleri.Columns[4].Name = "q=0,25";
                dataGridViewOptimalFonkDegerleri.Columns[5].Name = "q=0,5";
                dataGridViewOptimalFonkDegerleri.Columns[6].Name = "q=0,75";
                dataGridViewOptimalFonkDegerleri.Columns[7].Name = "q=1";
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewOptimalFonkDegerleri.Rows.Add(alternatifler[i].ToString());
                }
                dataGridViewOptimalFonkDegerleri.Rows.Add("S*");
                dataGridViewOptimalFonkDegerleri.Rows.Add("S-");
                dataGridViewOptimalFonkDegerleri.Rows.Add("R*");
                dataGridViewOptimalFonkDegerleri.Rows.Add("R-");

            }
            catch (Exception ex)
            {

                MessageBox.Show("Sİ,Rİ,Qİ değerleri matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void siRiQiDegerleri()
        {
            try
            {
                siRiQiDegerleriMatCerceve();

                //si değerleri
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    double si = 0;
                    for (int j = 1; j < kriterler.Count + 1; j++)
                    {
                        si += Convert.ToDouble(dataGridViewAgirlikliNormalizeKMat.Rows[i].Cells[j].Value);
                    }
                    dataGridViewOptimalFonkDegerleri.Rows[i].Cells[1].Value = si;
                }


                //ri değerleri
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    double max = Convert.ToDouble(dataGridViewAgirlikliNormalizeKMat.Rows[i].Cells[1].Value);
                    for (int j = 1; j < kriterler.Count + 1; j++)
                    {
                        if (max < Convert.ToDouble(dataGridViewAgirlikliNormalizeKMat.Rows[i].Cells[j].Value))
                        {
                            max = Convert.ToDouble(dataGridViewAgirlikliNormalizeKMat.Rows[i].Cells[j].Value);
                        }
                    }
                    dataGridViewOptimalFonkDegerleri.Rows[i].Cells[2].Value = max;
                }


                //s*, s-,r*,r- değerleri
                //s* değeri
                double sYildiz = Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[1].Cells[1].Value);
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    if (sYildiz > Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[i].Cells[1].Value))
                    {
                        sYildiz = Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[i].Cells[1].Value);
                    }
                }
                dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count].Cells[1].Value = sYildiz;

                // s- değeri
                double sUssu = Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[1].Cells[1].Value);
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    if (sUssu < Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[i].Cells[1].Value))
                    {
                        sUssu = Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[i].Cells[1].Value);
                    }
                }
                dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count + 1].Cells[1].Value = sUssu;


                // rYidiz değeri

                double rYildiz = Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[1].Cells[2].Value);
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    if (rYildiz > Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[i].Cells[2].Value))
                    {
                        rYildiz = Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[i].Cells[2].Value);
                    }
                }
                dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count + 2].Cells[1].Value = rYildiz;


                // rUssu değeri

                double rUssu = Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[1].Cells[2].Value);
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    if (rUssu < Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[i].Cells[2].Value))
                    {
                        rUssu = Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[i].Cells[2].Value);
                    }
                }
                dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count + 3].Cells[1].Value = rUssu;


                //qi değerleri
                //q=0,0 için
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    double Q = 0;
                    Q = (Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[i].Cells[2].Value) - Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count + 2].Cells[1].Value)) / (Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count + 3].Cells[1].Value) - Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count + 2].Cells[1].Value));
                    dataGridViewOptimalFonkDegerleri.Rows[i].Cells[3].Value = Q;

                }

                //q=0,25 için

                for (int i = 0; i < alternatifler.Count; i++)
                {
                    double Q = 0;
                    Q = ((0.25 * (Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[i].Cells[1].Value) - Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count].Cells[1].Value))) / (Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count + 1].Cells[1].Value) - Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count].Cells[1].Value))) + ((0.75 * (Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[i].Cells[2].Value) - Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count + 2].Cells[1].Value))) / (Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count + 3].Cells[1].Value) - Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count + 2].Cells[1].Value)));
                    dataGridViewOptimalFonkDegerleri.Rows[i].Cells[4].Value = Q;

                }


                //q=0,5 için
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    double Q = 0;
                    Q = ((0.5 * (Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[i].Cells[1].Value) - Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count].Cells[1].Value))) / (Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count + 1].Cells[1].Value) - Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count].Cells[1].Value))) + ((0.5 * (Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[i].Cells[2].Value) - Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count + 2].Cells[1].Value))) / (Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count + 3].Cells[1].Value) - Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count + 2].Cells[1].Value)));
                    dataGridViewOptimalFonkDegerleri.Rows[i].Cells[5].Value = Q;

                }

                //q=0,75 için
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    double Q = 0;
                    Q = ((0.75 * (Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[i].Cells[1].Value) - Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count].Cells[1].Value))) / (Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count + 1].Cells[1].Value) - Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count].Cells[1].Value))) + ((0.75 * (Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[i].Cells[2].Value) - Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count + 2].Cells[1].Value))) / (Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count + 3].Cells[1].Value) - Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count + 2].Cells[1].Value)));
                    dataGridViewOptimalFonkDegerleri.Rows[i].Cells[6].Value = Q;

                }

                //q=1 için
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    double Q = 0;
                    Q = (((Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[i].Cells[1].Value) - Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count].Cells[1].Value))) / (Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count + 1].Cells[1].Value) - Convert.ToDouble(dataGridViewOptimalFonkDegerleri.Rows[alternatifler.Count].Cells[1].Value)));
                    dataGridViewOptimalFonkDegerleri.Rows[i].Cells[7].Value = Q;

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Sİ,Rİ,Qİ değerleri matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void sonucOptimalKararMatDoldur()
        {

            try
            {
                dataGridViewSonucOptimalKararMat.Rows.Clear();
                dataGridViewSonucOptimalKararMat.ColumnCount = kriterler.Count + 1;
                dataGridViewSonucOptimalKararMat.Columns[0].Name = " ";
                int k = 1;
                for (int i = 0; i < kriterler.Count; i++)
                {
                    dataGridViewSonucOptimalKararMat.Columns[k].Name = kriterler[i].ToString();
                    k++;
                }
                dataGridViewSonucOptimalKararMat.Rows.Add("Optimal Değer");
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewSonucOptimalKararMat.Rows.Add(alternatifler[i].ToString());
                }
                for (int j = 1; j < kriterler.Count + 1; j++)
                {
                    dataGridViewSonucOptimalKararMat.Rows[0].Cells[j].Value = optimalList[j - 1];
                }

                for (int j = 1; j < kriterler.Count + 1; j++)
                {
                    for (int i = 1; i < alternatifler.Count + 1; i++)
                    {
                        dataGridViewSonucOptimalKararMat.Rows[i].Cells[j].Value = dataGridViewKararMat.Rows[i - 1].Cells[j].Value;
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Hata!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


        }
        public void sonucNormalizeMatDoldur()
        {
            try
            {
                dataGridViewSonucNormalizeMat.Rows.Clear();
                //normalize matrisi
                dataGridViewSonucNormalizeMat.ColumnCount = kriterler.Count + 1; //alternatif sutunuda bulunması gerektiğinden kriterler+1 tane sutun ekledim
                dataGridViewSonucNormalizeMat.Columns[0].Name = " ";

                for (int i = 0; i < kriterler.Count; i++)
                {
                    dataGridViewSonucNormalizeMat.Columns[i + 1].Name = kriterler[i].ToString();

                }
                dataGridViewSonucNormalizeMat.Rows.Add("Optimal Değer");
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewSonucNormalizeMat.Rows.Add(alternatifler[i].ToString());
                }

                for (int j = 1; j <= kriterler.Count; j++)
                {
                    for (int i = 0; i <= alternatifler.Count; i++)
                    {
                        dataGridViewSonucNormalizeMat.Rows[i].Cells[j].Value = dataGridViewNormalize.Rows[i].Cells[j].Value;
                    }
                }

                // İLK SATIR VE SUTUNDAKİ DEĞERLERİN DEĞİŞTİRİLMESİNİ ENGELLEDİM
                for (int rC = 0; rC < alternatifler.Count + 1; rC++)
                {
                    dataGridViewSonucNormalizeMat.Rows[rC].Cells[rC].ReadOnly = true;
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Hata!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void sonucAgirlikliNormalizeMatDoldur()
        {
            try
            {
                dataGridViewSonucAgirlikliNormalizeKMat.Rows.Clear();
                dataGridViewSonucAgirlikliNormalizeKMat.ColumnCount = kriterler.Count + 1; //alternatif sutunuda bulunması gerektiğinden kriterler+1 tane sutun ekledim
                dataGridViewSonucAgirlikliNormalizeKMat.Columns[0].Name = " ";

                for (int i = 0; i < kriterler.Count; i++)
                {
                    dataGridViewSonucAgirlikliNormalizeKMat.Columns[i + 1].Name = kriterler[i].ToString();

                }
                dataGridViewSonucAgirlikliNormalizeKMat.Rows.Add("Optimal Değer");
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewSonucAgirlikliNormalizeKMat.Rows.Add(alternatifler[i].ToString());
                }
                for (int j = 1; j <= kriterler.Count; j++)
                {
                    for (int i = 0; i <= alternatifler.Count; i++)
                    {
                        dataGridViewSonucAgirlikliNormalizeKMat.Rows[i].Cells[j].Value = dataGridViewAgirlikliNormalizeKMat.Rows[i].Cells[j].Value;
                    }
                }
                // İLK SATIR VE SUTUNDAKİ DEĞERLERİN DEĞİŞTİRİLMESİNİ ENGELLEDİM
                for (int rC = 0; rC < alternatifler.Count + 1; rC++)
                {
                    dataGridViewSonucAgirlikliNormalizeKMat.Rows[rC].Cells[rC].ReadOnly = true;
                }


            }
            catch (Exception ex)
            {

                MessageBox.Show("Hata!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void sonucOptimallikFonkMatDoldur()
        {
            try
            {
                dataGridViewSonucOptimalFonkDegerleri.Rows.Clear();
                dataGridViewSonucOptimalFonkDegerleri.ColumnCount = 4;
                dataGridViewSonucOptimalFonkDegerleri.Columns[0].Name = " ";
                dataGridViewSonucOptimalFonkDegerleri.Columns[1].Name = "Si";
                dataGridViewSonucOptimalFonkDegerleri.Columns[2].Name = "Ki";
                dataGridViewSonucOptimalFonkDegerleri.Columns[3].Name = "%Ki";

                dataGridViewSonucOptimalFonkDegerleri.Rows.Add("Optimal");
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    dataGridViewSonucOptimalFonkDegerleri.Rows.Add(alternatifler[i].ToString());
                }

                for (int j = 1; j < 4; j++)
                {
                    for (int i = 0; i < alternatifler.Count + 1; i++)
                    {

                        dataGridViewSonucOptimalFonkDegerleri.Rows[i].Cells[j].Value = dataGridViewOptimalFonkDegerleri.Rows[i].Cells[j].Value;
                    }
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Optimal fonksiyon değerleri matrisi oluşturulamadı!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void btnAyrintiliCozum_Click(object sender, EventArgs e)
        {

            if (yontem == btnAras.Text)
            {
                sonuclarToolStripMenuItem2.Visible = true;
                sonucOptimalKararMatDoldur();
                sonucNormalizeMatDoldur();
                sonucAgirlikliNormalizeMatDoldur();
                sonucOptimallikFonkMatDoldur();
                tabControl1.SelectedTab = tabPageArasSonuc;
            }
            if (yontem == btnVikor.Text)
            {
                vikorSralamaSonuçlarıToolStripMenuItem.Visible = true;

                qiSiralamaMat();
                siSiralamaMat();
                riSiralamaMat();
                tabControl1.SelectedTab = tabPageVikorSiralama;
            }
        }
        public void alternatifDuzenle()
        {
            try
            {
                if (txtAlternatif.Text == text2)
                {
                    MessageBox.Show("Alternatif girmeden ekleme yapamazsınız !", "UYARI ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (txtAlternatif.Text == "")
                {
                    MessageBox.Show("Alternatif girmeden ekleme yapamazsınız !", "UYARI ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    listBoxAlternatif.Items.RemoveAt(duzenleIndex);
                    listBoxAlternatif.Items.Insert(duzenleIndex, txtAlternatif.Text);
                    alternatifler.RemoveAt(duzenleIndex);
                    alternatifler.Insert(duzenleIndex, txtAlternatif.Text);
                    //alternatif eklendikten sonra butonları aktif etsin
                    btnAlternatifSil.Enabled = true;
                    btnAlternatifDuzenle.Enabled = true;
                    //ekledikten sonra textbox ın içini temizleyip imleci oraya fokuslasın

                    txtAlternatif.Clear();
                    txtAlternatif.Focus();
                    pnlAlternatif.Visible = true;
                    pnlKriterAlternatif.Visible = true;
                }
                txtAlternatif.Clear();
                txtAlternatif.Focus();

                btnAlternatifEkle.Text = "Ekle";
                btnAlternatifEkle.Font = new Font("Bahnschrift Light", 9, FontStyle.Bold);
            }
            catch (Exception)
            {

                MessageBox.Show("Hata!", "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void alternatifEkle()
        {
            try
            {
                for (int i = 0; i < alternatifler.Count; i++)
                {
                    if (txtAlternatif.Text == alternatifler[i].ToString())
                    {
                        MessageBox.Show("Lütfen farklı alternatifler giriniz!", "UYARI ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                if (txtAlternatif.Text == text2)
                {
                    MessageBox.Show("Alternatif girmeden ekleme yapamazsınız !", "UYARI ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                else if (txtAlternatif.Text == "")
                {
                    MessageBox.Show("Alternatif girmeden ekleme yapamazsınız !", "UYARI ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    listBoxAlternatif.Items.Add(txtAlternatif.Text);
                    alternatifler.Add(txtAlternatif.Text);
                    //alternatif eklendikten sonra butonları aktif etsin
                    btnAlternatifSil.Enabled = true;
                    btnAlternatifDuzenle.Enabled = true;
                    //ekledikten sonra textbox ın içini temizleyip imleci oraya fokuslasın

                    txtAlternatif.Clear();
                    txtAlternatif.Focus();
                    pnlAlternatif.Visible = true;
                    pnlKriterAlternatif.Visible = true;
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Hata!", "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        private void btnAlternatifEkle_Click(object sender, EventArgs e)
        {
            if (btnAlternatifEkle.Text == "Ekle")
            {
                alternatifEkle();
            }
            else if (btnAlternatifEkle.Text == "Güncelle")
            {
                alternatifDuzenle();
            }
        }
        private void txtKriter_Leave(object sender, EventArgs e)
        {
            if (txtKriter.Text == "")
            {
                txtKriter.Text = text;
            }
        }
        private void txtKriter_Enter(object sender, EventArgs e)
        {
            if (txtKriter.Text == text)
            {
                txtKriter.Text = "";
            }
        }
        private void txtKriter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                btnKriterEkle.PerformClick();
            }
        }
        public void kriterDuzenle()
        {

            try
            {
                if (txtKriter.Text == text)
                {
                    MessageBox.Show("Kriter girmeden ekleme yapamazsınız !", "UYARI ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    //for (int i = 0; i < kriterler.Count; i++)
                    //{
                    //    if (txtKriter.Text == kriterler[i].ToString())
                    //    {
                    //        MessageBox.Show("Lütfen farklı kriterler giriniz!", "UYARI ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //        return;
                    //    }
                    //}
                    if (txtKriter.Text == "")
                    {
                        MessageBox.Show("Kriter girmeden ekleme yapamazsınız !", "UYARI ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    //uyarı
                    if (rbtnFayda.Checked == false && rbtnMaliyet.Checked == false)
                    {
                        MessageBox.Show("Lütfen kriter tipini seçiniz!", "UYARI ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }


                    //listbox a ekleme
                    if (rbtnFayda.Checked == true)
                    {
                        listBoxKriter.Items.RemoveAt(duzenleIndex);
                        listBoxKriter.Items.Insert(duzenleIndex, txtKriter.Text + "  (" + rbtnFayda.Text + ")");
                    }
                    if (rbtnMaliyet.Checked == true)
                    {
                        listBoxKriter.Items.RemoveAt(duzenleIndex);
                        listBoxKriter.Items.Insert(duzenleIndex, txtKriter.Text + "  (" + rbtnMaliyet.Text + ")");
                    }
                    kriterler.RemoveAt(duzenleIndex);
                    kriterler.Insert(duzenleIndex, txtKriter.Text);
                    //eklendikten sonra butonları aktif etsin
                    btnKriterSil.Enabled = true;
                    btnKriterDuzenle.Enabled = true;

                    //fayda ve maliyet kriterlerini arrayliste ekleme

                    if (rbtnFayda.Checked == true)
                    {
                        faydaMaliyet.RemoveAt(duzenleIndex);
                        faydaMaliyet.Insert(duzenleIndex, rbtnFayda.Text);
                    }
                    if (rbtnMaliyet.Checked == true)
                    {
                        faydaMaliyet.RemoveAt(duzenleIndex);
                        faydaMaliyet.Insert(duzenleIndex, rbtnMaliyet.Text);
                    }
                    pnlKriter.Visible = true;

                    //ekledikten sonra textbox ın içini temizleyip imleci oraya fokuslasın

                    txtKriter.Clear();
                    txtKriter.Focus();

                    btnKriterEkle.Text = "Ekle";
                    btnKriterEkle.Font = new Font("Bahnschrift Light", 9, FontStyle.Bold);

                }
            }
            catch (Exception)
            {

                MessageBox.Show("Kriter güncelleme işlemi başarısız!", "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void kriterEkle()
        {
            try
            {
                if (txtKriter.Text == text)
                {
                    MessageBox.Show("Kriter girmeden ekleme yapamazsınız !", "UYARI ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    for (int i = 0; i < kriterler.Count; i++)
                    {
                        if (txtKriter.Text == kriterler[i].ToString())
                        {
                            MessageBox.Show("Lütfen farklı kriterler giriniz!", "UYARI ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    if (txtKriter.Text == "")
                    {
                        MessageBox.Show("Kriter girmeden ekleme yapamazsınız !", "UYARI ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    //uyarı
                    if (rbtnFayda.Checked == false && rbtnMaliyet.Checked == false)
                    {
                        MessageBox.Show("Lütfen kriter tipini seçiniz!", "UYARI ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }


                    //listbox a ekleme
                    if (rbtnFayda.Checked == true)
                    {
                        listBoxKriter.Items.Add(txtKriter.Text + "  (" + rbtnFayda.Text + ")");
                    }
                    if (rbtnMaliyet.Checked == true)
                    {
                        listBoxKriter.Items.Add(txtKriter.Text + "  (" + rbtnMaliyet.Text + ")");
                    }

                    kriterler.Add(txtKriter.Text);
                    //eklendikten sonra butonları aktif etsin
                    btnKriterSil.Enabled = true;
                    btnKriterDuzenle.Enabled = true;

                    //fayda ve maliyet kriterlerini arrayliste ekleme

                    if (rbtnFayda.Checked == true)
                    {
                        faydaMaliyet.Add(rbtnFayda.Text);
                    }
                    if (rbtnMaliyet.Checked == true)
                    {
                        faydaMaliyet.Add(rbtnMaliyet.Text);
                    }
                    pnlKriter.Visible = true;

                    //ekledikten sonra textbox ın içini temizleyip imleci oraya fokuslasın

                    txtKriter.Clear();
                    txtKriter.Focus();

                }
            }
            catch (Exception)
            {

                MessageBox.Show("Kriter ekleme işlemi başarısız!", "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        private void btnKriterEkle_Click(object sender, EventArgs e)
        {
            if (btnKriterEkle.Text == "Ekle")
            {
                kriterEkle();
            }

            else if (btnKriterEkle.Text == "Güncelle")
            {
                kriterDuzenle();
            }
        }
        public void gridTasarim(DataGridView datagridview)
        {
            datagridview.RowHeadersVisible = false;  //ilk sutunu gizleme
            datagridview.BorderStyle = BorderStyle.None;
            datagridview.AlternatingRowsDefaultCellStyle.BackColor = Color.LightSlateGray;//varsayılan arka plan rengi verme
            datagridview.DefaultCellStyle.SelectionBackColor = Color.Silver; //seçilen hücrenin arkaplan rengini belirleme
            datagridview.DefaultCellStyle.SelectionForeColor = Color.White;  //seçilen hücrenin yazı rengini belirleme
            datagridview.EnableHeadersVisualStyles = false; //başlık özelliğini değiştirme
            datagridview.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None; //başlık çizgilerini ayarlama
            datagridview.ColumnHeadersDefaultCellStyle.BackColor = Color.Gray; //başlık arkaplan rengini belirleme

            datagridview.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;  //başlık yazı rengini belirleme
            datagridview.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;                                                                   // datagridview.SelectionMode = DataGridViewSelectionMode.FullRowSelect; //satırı tamamen seçmeyi sağlama
            datagridview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;  //her hangi bir sutunun genişliğini o sutunda yer alan en  uzun yazının genişliğine göre ayarlama

            datagridview.AllowUserToAddRows = false;  //ilk sutunu gizleme
            datagridview.AllowUserToOrderColumns = true;

        }
        public void gridTasarimSirasiz(DataGridView datagridview)
        {
            datagridview.RowHeadersVisible = false;  //ilk sutunu gizleme         
            datagridview.BorderStyle = BorderStyle.None;
            datagridview.AlternatingRowsDefaultCellStyle.BackColor = Color.LightSlateGray;//varsayılan arka plan rengi verme
            datagridview.DefaultCellStyle.SelectionBackColor = Color.Silver; //seçilen hücrenin arkaplan rengini belirleme
            datagridview.DefaultCellStyle.SelectionForeColor = Color.White;  //seçilen hücrenin yazı rengini belirleme
            datagridview.EnableHeadersVisualStyles = false; //başlık özelliğini değiştirme
            datagridview.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single; //başlık çizgilerini ayarlama
            datagridview.ColumnHeadersDefaultCellStyle.BackColor = Color.Gray; //başlık arkaplan rengini belirleme

            datagridview.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;  //başlık yazı rengini belirleme
            datagridview.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;                                                                   // datagridview.SelectionMode = DataGridViewSelectionMode.FullRowSelect; //satırı tamamen seçmeyi sağlama
            datagridview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;  //her hangi bir sutunun genişliğini o sutunda yer alan en  uzun yazının genişliğine göre ayarlama

            datagridview.AllowUserToAddRows = false;
            datagridview.AllowUserToOrderColumns = false;

        }
        private void btnSimdiOlustur_Click(object sender, EventArgs e)
        {
            kararMatrisiOlusturmaToolStripMenuItem.Visible = true;
            tumunuTemizle();
            tabControl1.SelectedTab = tabPageKararMatOlusturma;
        }
        public void tumunuTemizle()
        {
            try
            {
                //yontem = "";
                dataGridViewQiSiralama.Columns.Clear();
                dataGridViewSiSiralama.Columns.Clear();
                dataGridViewRiSiralama.Columns.Clear();
                dataGridViewSonucQi.Columns.Clear();
                dataGridViewVikorKosulDenetle.Columns.Clear();
                richTextBoxKosulDenetle.Text = "";
                dataGridViewIdealUzaklik.Columns.Clear();
                dataGridViewNegatifIdealUzaklık.Columns.Clear();
                dataGridViewOptimalKararMat.Columns.Clear();
                dataGridViewAgirlik.Columns.Clear();
                dataGridViewKararMat.Columns.Clear();
                dataGridViewNormalize.Columns.Clear();
                dataGridViewAyrintiKmat.Columns.Clear();
                dataGridViewC.Columns.Clear();
                dataGridViewWVektörü.Columns.Clear();
                dataGridViewDVektör.Columns.Clear();
                dataGridViewKarsilastirmaMat.Columns.Clear();
                dataGridViewKriterAgirliklari.Columns.Clear();
                dataGridViewAgirlikliNormalizeKMat.Columns.Clear();
                dataGridViewOptimalFonkDegerleri.Columns.Clear();
                dataGridViewSonucOptimalKararMat.Columns.Clear();
                dataGridViewSonucNormalizeMat.Columns.Clear();
                dataGridViewSonucAgirlikliNormalizeKMat.Columns.Clear();
                dataGridViewSonucOptimalFonkDegerleri.Columns.Clear();
                pnlYontemSec.Visible = true;
                panel12.Visible = false;
                btnKriterAgirlikKaydet.Visible = false;
                btnAhpAyrintiCozGoster.Visible = false;
                panel82.Visible = false;
                panel83.Visible = false;
                panel80.Visible = false;
                label24.Visible = false;
                lblTutOrani.Visible = false;
                btnKriterAgirlikKaydet.Visible = false;
                btnAhpAyrintiCozGoster.Visible = false;
                lblUyari.Visible = false;
                panel52.Visible = false;
                panel81.Visible = false;
                listBoxKriter.Items.Clear();
                listBoxAlternatif.Items.Clear();
                lblTutOrani.Text = "";
                lblCI.Text = "";
                lblRI.Text = "";
                lblTutOrani.Text = "";
                kriterler.Clear();
                alternatifler.Clear();
                faydaMaliyet.Clear();
                agirliklar.Clear();
                maxList.Clear();
                minList.Clear();
                paydaListesi.Clear();
                paydaListesi.Clear();
                max = 0;
                min = 0;
                agirlikToplam = 0;
                rbtnDiziboyut = 1;
                rbtnDizi1boyut = 1;
                x = 0;
                y = 0;
                rbtn = 0;
                rbtn1 = 0;
                lamda = 0;
                CI = 0;
                CR = 0;
                //RadioButton[] radioButton;
                // RadioButton[] radioButton1;
            }
            catch (Exception ex)
            {

                MessageBox.Show("Hata!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void btnExcelYukle_Click(object sender, EventArgs e)
        {

            try
            {
                kararMatrisiOlusturmaToolStripMenuItem.Visible = true;
                tumunuTemizle();
                kararMatImport();
                importKararMatDoldur();
                kararMatImportListeDoldurma();
                kararMatRenklendir();
                boyutAyarlama();
                tabControl1.SelectedTab = tabPageKararMatrisi;

                gridTasarimSirasiz(dataGridViewKararMat);
            }
            catch
            {
                MessageBox.Show("Dosya seçilmedi", "Uyarı ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

        }
        public void kararMatImport()
        {
            try
            {
                var openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "XLS files (*.xls, *.xlt)|*.xls;*.xlt|XLSX files (*.xlsx, *.xlsm, *.xltx, *.xltm)|*.xlsx;*.xlsm;*.xltx;*.xltm|ODS files (*.ods, *.ots)|*.ods;*.ots|CSV files (*.csv, *.tsv)|*.csv;*.tsv|HTML files (*.html, *.htm)|*.html;*.htm";
                openFileDialog.FilterIndex = 2;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var workbook = ExcelFile.Load(openFileDialog.FileName);

                    // From ExcelFile to DataGridView.
                    DataGridViewConverter.ExportToDataGridView(workbook.Worksheets.ActiveWorksheet, this.dataGridViewImport, new ExportToDataGridViewOptions() { ColumnHeaders = false });
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Karar matrisi yüklenemedi!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        public void kararMatImportListeDoldurma()
        {
            try
            {
                for (int i = 1; i < dataGridViewKararMat.Columns.Count; i++)
                {
                    kriterler.Add(dataGridViewKararMat.Columns[i].Name.ToString());
                }
                for (int i = 0; i < kriterler.Count; i++)
                {
                    faydaMaliyet.Add(rbtnFayda.Text);
                }
                for (int i = 0; i < dataGridViewKararMat.Rows.Count; i++)
                {
                    alternatifler.Add(dataGridViewKararMat.Rows[i].Cells[0].Value.ToString());
                }


            }
            catch (Exception)
            {
                MessageBox.Show("Lütfen boş hücreleri doldurunuz!", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        public void importKararMatDoldur()
        {
            gridTasarimSirasiz(dataGridViewImport);
            dataGridViewKararMat.ColumnCount = dataGridViewImport.Columns.Count;
            dataGridViewKararMat.Columns[0].Name = " ";

            for (int i = 1; i < dataGridViewImport.Columns.Count; i++)
            {
                dataGridViewKararMat.Columns[i].Name = (dataGridViewImport.Rows[0].Cells[i].Value.ToString());
            }

            for (int i = 1; i < dataGridViewImport.Rows.Count; i++)
            {

                dataGridViewKararMat.Rows.Add(dataGridViewImport.Rows[i].Cells[0].Value.ToString());
            }

            for (int i = 1; i < dataGridViewImport.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridViewImport.Columns.Count; j++)
                {

                    dataGridViewKararMat.Rows[i - 1].Cells[j].Value = dataGridViewImport.Rows[i].Cells[j].Value.ToString();


                }
            }
            dataGridViewImport.Visible = false;
        }
        private void btnKriterDuzenle_Click(object sender, EventArgs e)
        {
            duzenleIndex = listBoxKriter.SelectedIndex;
            txtKriter.Text = kriterler[duzenleIndex].ToString();
            btnKriterEkle.Text = "Güncelle";
            btnKriterEkle.Font = new Font("Bahnschrift Light", 8, FontStyle.Bold);
        }
        //KARAR MATRİSİ KES KOPYALA YAPIŞTIR KODLARI
        public void kararMatKopyalaYapistir()
        {

            try
            {
                DataGridViewRow selectedRow;
                /* Find first selected cell's row (or first selected row). */
                if (dataGridViewKararMat.SelectedRows.Count > 0)
                    selectedRow = dataGridViewKararMat.SelectedRows[0];
                else if (dataGridViewKararMat.SelectedCells.Count > 0)
                    selectedRow = dataGridViewKararMat.SelectedCells[0].OwningRow;
                else
                    return;
                /* Get clipboard Text */
                string clipText = Clipboard.GetText();
                /* Get Rows ( newline delimited ) */
                string[] rowLines = Regex.Split(clipText, "\r\n");
                foreach (string row in rowLines)
                {
                    /* Get Cell contents ( tab delimited ) */
                    string[] cells = Regex.Split(row, "\t");
                    DataGridViewRow r = new DataGridViewRow();
                    foreach (string sc in cells)
                    {
                        DataGridViewTextBoxCell c = new DataGridViewTextBoxCell();
                        c.Value = sc;
                        r.Cells.Add(c);
                    }
                    dataGridViewKararMat.Rows.Insert(selectedRow.Index, r);

                }


            }
            //catch (System.ArgumentException ex)
            //{

            //}
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public void agirlikliNormalizeKMatEAktar()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "XLS files (*.xls)|*.xls|XLT files (*.xlt)|*.xlt|XLSX files (*.xlsx)|*.xlsx|XLSM files (*.xlsm)|*.xlsm|XLTX (*.xltx)|*.xltx|XLTM (*.xltm)|*.xltm|ODS (*.ods)|*.ods|OTS (*.ots)|*.ots|CSV (*.csv)|*.csv|TSV (*.tsv)|*.tsv|HTML (*.html)|*.html|MHTML (.mhtml)|*.mhtml|PDF (*.pdf)|*.pdf|XPS (*.xps)|*.xps|BMP (*.bmp)|*.bmp|GIF (*.gif)|*.gif|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|TIFF (*.tif)|*.tif|WMP (*.wdp)|*.wdp";
                saveFileDialog.FilterIndex = 3;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var workbook = new ExcelFile();
                    var worksheet = workbook.Worksheets.Add("Sheet1");

                    // From DataGridView to ExcelFile.
                    DataGridViewConverter.ImportFromDataGridView(worksheet, this.dataGridViewAgirlikliNormalizeKMat, new ImportFromDataGridViewOptions() { ColumnHeaders = true });

                    workbook.Save(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Excel'e aktarma işlemi başarısız!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void btnAgirlikliNormalizeKMatEAktar_Click(object sender, EventArgs e)
        {
            agirlikliNormalizeKMatEAktar();
        }
        public void optimalFonkDegerEAktar()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "XLS files (*.xls)|*.xls|XLT files (*.xlt)|*.xlt|XLSX files (*.xlsx)|*.xlsx|XLSM files (*.xlsm)|*.xlsm|XLTX (*.xltx)|*.xltx|XLTM (*.xltm)|*.xltm|ODS (*.ods)|*.ods|OTS (*.ots)|*.ots|CSV (*.csv)|*.csv|TSV (*.tsv)|*.tsv|HTML (*.html)|*.html|MHTML (.mhtml)|*.mhtml|PDF (*.pdf)|*.pdf|XPS (*.xps)|*.xps|BMP (*.bmp)|*.bmp|GIF (*.gif)|*.gif|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|TIFF (*.tif)|*.tif|WMP (*.wdp)|*.wdp";
                saveFileDialog.FilterIndex = 3;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var workbook = new ExcelFile();
                    var worksheet = workbook.Worksheets.Add("Sheet1");

                    // From DataGridView to ExcelFile.
                    DataGridViewConverter.ImportFromDataGridView(worksheet, this.dataGridViewOptimalFonkDegerleri, new ImportFromDataGridViewOptions() { ColumnHeaders = true });

                    workbook.Save(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Excel'e aktarma işlemi başarısız!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void btnOptimalFonkDegerEAktar_Click(object sender, EventArgs e)
        {
            optimalFonkDegerEAktar();
        }
        public void ahpKriterAgirlikBulEAktar()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "XLS files (*.xls)|*.xls|XLT files (*.xlt)|*.xlt|XLSX files (*.xlsx)|*.xlsx|XLSM files (*.xlsm)|*.xlsm|XLTX (*.xltx)|*.xltx|XLTM (*.xltm)|*.xltm|ODS (*.ods)|*.ods|OTS (*.ots)|*.ots|CSV (*.csv)|*.csv|TSV (*.tsv)|*.tsv|HTML (*.html)|*.html|MHTML (.mhtml)|*.mhtml|PDF (*.pdf)|*.pdf|XPS (*.xps)|*.xps|BMP (*.bmp)|*.bmp|GIF (*.gif)|*.gif|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|TIFF (*.tif)|*.tif|WMP (*.wdp)|*.wdp";
                saveFileDialog.FilterIndex = 3;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var workbook = new ExcelFile();
                    var worksheet = workbook.Worksheets.Add("Sheet1");
                    int say = 0;
                    worksheet.Cells[0, 0].Value = "KRİTERLER İÇİN İKİLİ KARŞILAŞTIRMA MATRİSİ";
                    say += 2;
                    DataGridViewConverter.ImportFromDataGridView(worksheet, this.dataGridViewAyrintiKmat, new ImportFromDataGridViewOptions() { ColumnHeaders = true, StartRow = say });
                    say += dataGridViewAyrintiKmat.Rows.Count + 3;
                    worksheet.Cells[say, 0].Value = "C MATRİSİ";
                    say += 2;
                    DataGridViewConverter.ImportFromDataGridView(worksheet, this.dataGridViewC, new ImportFromDataGridViewOptions() { ColumnHeaders = true, StartRow = say });
                    say += dataGridViewC.Rows.Count + 3;
                    worksheet.Cells[say, 0].Value = "AĞIRLIKLAR";
                    say += 2;
                    DataGridViewConverter.ImportFromDataGridView(worksheet, this.dataGridViewWVektörü, new ImportFromDataGridViewOptions() { ColumnHeaders = true, StartRow = say });
                    say += dataGridViewWVektörü.Rows.Count + 3;
                    worksheet.Cells[say, 0].Value = "D MATRİSİ";
                    say += 2;
                    DataGridViewConverter.ImportFromDataGridView(worksheet, this.dataGridViewDVektör, new ImportFromDataGridViewOptions() { ColumnHeaders = true, StartRow = say });
                    say += dataGridViewDVektör.Rows.Count + 3;
                    worksheet.Cells[say, 0].Value = "CI: " + lblCI.Text;
                    say += 2;
                    worksheet.Cells[say, 0].Value = "RI: " + lblRI.Text;
                    say += 2;
                    worksheet.Cells[say, 0].Value = "TUTARLILIK ORANI: " + lblTutOrani.Text;


                    workbook.Save(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Excel'e aktarma işlemi başarısız!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void btnAhpKriterAgrBulEAktar_Click(object sender, EventArgs e)
        {
            ahpKriterAgirlikBulEAktar();
        }
        public void arasSonucExcelAktar()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "XLS files (*.xls)|*.xls|XLT files (*.xlt)|*.xlt|XLSX files (*.xlsx)|*.xlsx|XLSM files (*.xlsm)|*.xlsm|XLTX (*.xltx)|*.xltx|XLTM (*.xltm)|*.xltm|ODS (*.ods)|*.ods|OTS (*.ots)|*.ots|CSV (*.csv)|*.csv|TSV (*.tsv)|*.tsv|HTML (*.html)|*.html|MHTML (.mhtml)|*.mhtml|PDF (*.pdf)|*.pdf|XPS (*.xps)|*.xps|BMP (*.bmp)|*.bmp|GIF (*.gif)|*.gif|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|TIFF (*.tif)|*.tif|WMP (*.wdp)|*.wdp";
                saveFileDialog.FilterIndex = 3;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var workbook = new ExcelFile();
                    var worksheet = workbook.Worksheets.Add("Sheet1");
                    int say = 0;
                    worksheet.Cells[0, 0].Value = "OPTİMAL DEĞERLERDEN OLUŞAN KARAR MATRİSİ";
                    say += 2;
                    DataGridViewConverter.ImportFromDataGridView(worksheet, this.dataGridViewSonucOptimalKararMat, new ImportFromDataGridViewOptions() { ColumnHeaders = true, StartRow = say });
                    say += dataGridViewSonucOptimalKararMat.Rows.Count + 3;
                    worksheet.Cells[say, 0].Value = "NORMALİZE MATRİSİ";
                    say += 2;
                    DataGridViewConverter.ImportFromDataGridView(worksheet, this.dataGridViewSonucNormalizeMat, new ImportFromDataGridViewOptions() { ColumnHeaders = true, StartRow = say });
                    say += dataGridViewSonucNormalizeMat.Rows.Count + 3;
                    worksheet.Cells[say, 0].Value = "AĞIRLIKLI NORMALİZE MATRİSİ";
                    say += 2;
                    DataGridViewConverter.ImportFromDataGridView(worksheet, this.dataGridViewSonucAgirlikliNormalizeKMat, new ImportFromDataGridViewOptions() { ColumnHeaders = true, StartRow = say });
                    say += dataGridViewSonucAgirlikliNormalizeKMat.Rows.Count + 3;
                    worksheet.Cells[say, 0].Value = "OPTİMALLİK FONKSİYON DEĞERLERİ VE ALTERNATİF SIRALAMALARI";
                    say += 2;
                    DataGridViewConverter.ImportFromDataGridView(worksheet, this.dataGridViewSonucOptimalFonkDegerleri, new ImportFromDataGridViewOptions() { ColumnHeaders = true, StartRow = say });

                    workbook.Save(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Excel'e aktarma işlemi başarısız!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void btnArasSonucExcelAktar_Click(object sender, EventArgs e)
        {
            arasSonucExcelAktar();
        }
        private void btnYenile_Click(object sender, EventArgs e)
        {
            tumunuTemizle();
        }
        private void btnAhpAyrintiAgirlikKaydet_Click(object sender, EventArgs e)
        {
            kriterAgirlikKaydet();
            if(yontem==btnAras.Text)
            {
                agirlikliNormalizeKararMat();
                tabControl1.SelectedTab = tabPageAgirlikliNormalizeKMat;
            }
            if (yontem == btnVikor.Text)
            {
                vikorAgirlikliNormalizeKararMat();
                tabControl1.SelectedTab = tabPageAgirlikliNormalizeKMat;
            }
            if (yontem == btnCopras.Text)
            {
                coprasAgirlikliNormalize();
                tabControl1.SelectedTab = tabPageAgirlikliNormalizeKMat;
            }
            if (yontem == btnTopsis.Text)
            {
                topsisAgirlikliNormalize();
                tabControl1.SelectedTab = tabPageAgirlikliNormalizeKMat;
            }

        }
        public void vikorTasarimDegistir()
        {
            btnOptimalGit.Text = "En iyi ve en kötü kriter değerlerini görüntüle";
            lblOpKararMat.Text = "EN İYİ VE EN KÖTÜ KRİTER DEĞERLERİ";
            tabPageOptimal.Text = "En İyi ve En Kötü Kriter Değerleri";
            btnOptimallikFonkDegerHesapla.Text = "Sİ , Rİ VE Qİ DEĞERLERİ";
            btnAyrintiliCozum.Text = "Vikor Sıralama Sonuçları";
            tabPageOptimallikFonksiyonDeğerleri.Text = "Sİ , Rİ VE Qİ DEĞERLERİ";
            lblOptFonkDeger.Text = "Sİ , Rİ VE Qİ DEĞERLERİ";
            label10.Visible = false;
            lblEnİyiAlternatif.Visible = false;

        }
        private void btnKosulDenetle_Click(object sender, EventArgs e)
        {
            if (yontem == btnVikor.Text)
            {
                koşullarınDenetlenmesiToolStripMenuItem.Visible = true;
                vikorKosulQiSiralama();
                vikorKosulDenetleMat();
                tabControl1.SelectedTab = tabPageVikorKosulDenetle;
            }

        }
        private void btnAras_Click(object sender, EventArgs e)
        {
            agirlikliNormalizeMatrisiToolStripMenuItem.Visible = true;
            baslangicToolStripMenuItem.Visible = true;
            tumMenuleriGizle();
            tumunuTemizle();
            yontem = btnAras.Text;
            btnOptimalGit.Text = "OPTİMAL DEĞERLERDEN OLUŞAN KARAR MATRİSİNİ GÖRÜNTÜLE";
            tabControl1.SelectedTab = tabPageBaslangic;
        }
        private void btnVikor_Click(object sender, EventArgs e)
        {
            baslangicToolStripMenuItem.Visible = true;
            tumMenuleriGizle();
            tumunuTemizle();
            yontem = btnVikor.Text;
            vikorTasarimDegistir();
            tabControl1.SelectedTab = tabPageBaslangic;
        }
        private void btnCopras_Click(object sender, EventArgs e)
        {
            baslangicToolStripMenuItem.Visible = true;
            tumMenuleriGizle();
            tumunuTemizle();
            yontem = btnCopras.Text;
            coprasDuzenle();
            tabControl1.SelectedTab = tabPageBaslangic;
        }
        private void btnTopsis_Click(object sender, EventArgs e)
        {
            baslangicToolStripMenuItem.Visible = true;
            tumMenuleriGizle();
            tumunuTemizle();
            yontem = btnTopsis.Text;
            topsisDuzenle();
            tabControl1.SelectedTab = tabPageBaslangic;
        }
        private void btnGoreliYakinlikHesapla_Click(object sender, EventArgs e)
        {
            btnAyrintiliCozum.Visible = false;
            optimallikFonksiyonDeğerleriToolStripMenuItem.Visible = false;
            idealÇözümeGöreliYakınlıkToolStripMenuItem.Visible = true;
            idealCozumGoreliYakinlik();
            tabControl1.SelectedTab = tabPageOptimallikFonksiyonDeğerleri;
        }
        public void vikorQİSİRİEAktar()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "XLS files (*.xls)|*.xls|XLT files (*.xlt)|*.xlt|XLSX files (*.xlsx)|*.xlsx|XLSM files (*.xlsm)|*.xlsm|XLTX (*.xltx)|*.xltx|XLTM (*.xltm)|*.xltm|ODS (*.ods)|*.ods|OTS (*.ots)|*.ots|CSV (*.csv)|*.csv|TSV (*.tsv)|*.tsv|HTML (*.html)|*.html|MHTML (.mhtml)|*.mhtml|PDF (*.pdf)|*.pdf|XPS (*.xps)|*.xps|BMP (*.bmp)|*.bmp|GIF (*.gif)|*.gif|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|TIFF (*.tif)|*.tif|WMP (*.wdp)|*.wdp";
                saveFileDialog.FilterIndex = 3;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var workbook = new ExcelFile();
                    var worksheet = workbook.Worksheets.Add("Sheet1");
                    int say = 0;
                    worksheet.Cells[0, 0].Value = label27.Text;
                    say += 2;
                    DataGridViewConverter.ImportFromDataGridView(worksheet, this.dataGridViewQiSiralama, new ImportFromDataGridViewOptions() { ColumnHeaders = true, StartRow = say });
                    say += dataGridViewQiSiralama.Rows.Count + 3;
                    worksheet.Cells[say, 0].Value = label35.Text;
                    say += 2;
                    DataGridViewConverter.ImportFromDataGridView(worksheet, this.dataGridViewSiSiralama, new ImportFromDataGridViewOptions() { ColumnHeaders = true, StartRow = say });
                    say += dataGridViewSiSiralama.Rows.Count + 3;
                    worksheet.Cells[say, 0].Value = label39.Text;
                    say += 2;
                    DataGridViewConverter.ImportFromDataGridView(worksheet, this.dataGridViewRiSiralama, new ImportFromDataGridViewOptions() { ColumnHeaders = true, StartRow = say });


                    workbook.Save(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Excel'e aktarma işlemi başarısız!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void btnVikorQİSİRİEAktar_Click(object sender, EventArgs e)
        {
            vikorQİSİRİEAktar();
        }
        public void vikorSonucEAktar()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "XLS files (*.xls)|*.xls|XLT files (*.xlt)|*.xlt|XLSX files (*.xlsx)|*.xlsx|XLSM files (*.xlsm)|*.xlsm|XLTX (*.xltx)|*.xltx|XLTM (*.xltm)|*.xltm|ODS (*.ods)|*.ods|OTS (*.ots)|*.ots|CSV (*.csv)|*.csv|TSV (*.tsv)|*.tsv|HTML (*.html)|*.html|MHTML (.mhtml)|*.mhtml|PDF (*.pdf)|*.pdf|XPS (*.xps)|*.xps|BMP (*.bmp)|*.bmp|GIF (*.gif)|*.gif|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|TIFF (*.tif)|*.tif|WMP (*.wdp)|*.wdp";
                saveFileDialog.FilterIndex = 3;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var workbook = new ExcelFile();
                    var worksheet = workbook.Worksheets.Add("Sheet1");
                    int say = 0;
                    worksheet.Cells[0, 0].Value = "Qİ DEĞERLERİ İÇİN SIRALAMA MATRİSİ";
                    say += 2;
                    DataGridViewConverter.ImportFromDataGridView(worksheet, this.dataGridViewSonucQi, new ImportFromDataGridViewOptions() { ColumnHeaders = true, StartRow = say });
                    say += dataGridViewSonucQi.Rows.Count + 3;
                    worksheet.Cells[say, 0].Value = "SONUÇLAR";
                    say += 2;
                    DataGridViewConverter.ImportFromDataGridView(worksheet, this.dataGridViewVikorKosulDenetle, new ImportFromDataGridViewOptions() { ColumnHeaders = true, StartRow = say });

                    workbook.Save(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Excel'e aktarma işlemi başarısız!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void btnVikorSonucEAktar_Click(object sender, EventArgs e)
        {
            vikorSonucEAktar();
        }
        public void idealUzaklikEAktar()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "XLS files (*.xls)|*.xls|XLT files (*.xlt)|*.xlt|XLSX files (*.xlsx)|*.xlsx|XLSM files (*.xlsm)|*.xlsm|XLTX (*.xltx)|*.xltx|XLTM (*.xltm)|*.xltm|ODS (*.ods)|*.ods|OTS (*.ots)|*.ots|CSV (*.csv)|*.csv|TSV (*.tsv)|*.tsv|HTML (*.html)|*.html|MHTML (.mhtml)|*.mhtml|PDF (*.pdf)|*.pdf|XPS (*.xps)|*.xps|BMP (*.bmp)|*.bmp|GIF (*.gif)|*.gif|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|TIFF (*.tif)|*.tif|WMP (*.wdp)|*.wdp";
                saveFileDialog.FilterIndex = 3;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var workbook = new ExcelFile();
                    var worksheet = workbook.Worksheets.Add("Sheet1");
                    int say = 0;
                    worksheet.Cells[0, 0].Value = "İDEAL UZAKLIK DEĞERLERİ";
                    say += 2;
                    DataGridViewConverter.ImportFromDataGridView(worksheet, this.dataGridViewIdealUzaklik, new ImportFromDataGridViewOptions() { ColumnHeaders = true, StartRow = say });
                    say += dataGridViewIdealUzaklik.Rows.Count + 3;
                    worksheet.Cells[say, 0].Value = "NEGATİF İDEAL UZAKLIK DEĞERLERİ";
                    say += 2;
                    DataGridViewConverter.ImportFromDataGridView(worksheet, this.dataGridViewNegatifIdealUzaklık, new ImportFromDataGridViewOptions() { ColumnHeaders = true, StartRow = say });
                    workbook.Save(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Excel'e aktarma işlemi başarısız!" + ex.Message, "Hata ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void btnIdealUzaklikEAktar_Click(object sender, EventArgs e)
        {
            idealUzaklikEAktar();
        }
        public void tumMenuleriGizle()
        {
            kararMatGoruntuleToolStripMenuItem.Visible = false;
            optimalKararMatrisiToolStripMenuItem.Visible = false;
            normalizeToolStripMenuItem.Visible = false;
            agırlıkBelirlemeToolStripMenuItem1.Visible = false;
            aHPToolStripMenuItem.Visible = false;
            karsilastirmaMatrisiToolStripMenuItem.Visible = false;
            agirlikDeğerleriToolStripMenuItem.Visible = false;
            AHPAyrintiliCozumlerToolStripMenuItem.Visible = false;
            agirlikliNormalizeMatrisiToolStripMenuItem.Visible = false;
            sonuçlarToolStripMenuItem1.Visible = false;
            vikorSralamaSonuçlarıToolStripMenuItem.Visible = false;
            sonuclarToolStripMenuItem2.Visible = false;
            kararMatrisiOlusturmaToolStripMenuItem.Visible = false;
            koşullarınDenetlenmesiToolStripMenuItem.Visible = false;
            idealVeNegatifİdealUzaklıkToolStripMenuItem.Visible = false;
            idealÇözümeGöreliYakınlıkToolStripMenuItem.Visible = false;
        }
        private void yontemSeçToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPageYontemSec;
        }
        private void baslangicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPageBaslangic;
        }
        private void kararMatrisiOlusturmaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPageKararMatOlusturma;
        }
        private void kararMatGoruntuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPageKararMatrisi;
        }
        private void normalizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPageNormalize;
        }
        private void agirlikBelirlemeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPageAgirlikBelirleme;
        }
        private void agirlikliNormalizeMatrisiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPageAgirlikliNormalizeKMat;
        }
        private void karsilastirmaMatrisiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPageKarsilastirmaMat;
        }
        private void agirlikDeğerleriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPageAHP;
        }
        private void AHPAyrintiliCozumlerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPageAhpAyrinti;
        }
        private void sonuclarToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (yontem == btnAras.Text)
            {
                tabControl1.SelectedTab = tabPageArasSonuc;
            }

            if (yontem == btnVikor.Text)
            {
                tabControl1.SelectedTab = tabPageVikorKosulDenetle;
            }
            if (yontem == btnTopsis.Text)
            {
                tabControl1.SelectedTab = tabPageIdealNegatifUzaklik;
            }
        }
        private void optimallikFonksiyonDegerleriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPageOptimallikFonksiyonDeğerleri;
        }
        private void optimalKararMatrisiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPageOptimal;
        }
        private void vikorSiralamaSonuçlarıToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPageVikorSiralama;
        }
        private void koşullarınDenetlenmesiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPageVikorKosulDenetle;
        }
        private void idealVeNegatifİdealUzaklıkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPageIdealNegatifUzaklik;
        }
        private void idealÇözümeGöreliYakınlıkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPageOptimal;
        }
        private void dataGridViewKararMat_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridViewKararMat.SelectedCells.Count > 0)
                dataGridViewKararMat.ContextMenuStrip = contextMenuStrip1;
            chkPasteToSelectedCells.Visible = true;
        }
        private void kesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Copy to clipboard
            CopyToClipboard();

            //Clear selected cells
            foreach (DataGridViewCell dgvCell in dataGridViewKararMat.SelectedCells)
                dgvCell.Value = string.Empty;
        }
        private void kopyalaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyToClipboard();
        }
        private void yapıştırToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteClipboardValue();
        }
        private void dataGridViewKararMat_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Modifiers == Keys.Control)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.C:
                            CopyToClipboard();
                            break;

                        case Keys.V:
                            PasteClipboardValue();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kopyalama / yapıştırma işlemi başarısız oldu." + ex.Message, "Kopyala/Yapıştır", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void CopyToClipboard()
        {
            //Copy to clipboard
            DataObject dataObj = dataGridViewKararMat.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }
        private void PasteClipboardValue() //pano değerlerini yapıştır
        {
            //hiçbir hücre seçilmezse
            if (dataGridViewKararMat.SelectedCells.Count == 0)
            {
                MessageBox.Show("Lütfen bir hücre seçin", "Yapıştır",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //başlangıç hücresini alma
            DataGridViewCell startCell = GetStartCell(dataGridViewKararMat);
            //pano değerlerini sözlükten alma
            Dictionary<int, Dictionary<int, string>> cbValue = ClipBoardValues(Clipboard.GetText());

            int iRowIndex = startCell.RowIndex;
            foreach (int rowKey in cbValue.Keys)
            {
                int iColIndex = startCell.ColumnIndex;
                foreach (int cellKey in cbValue[rowKey].Keys)
                {
                    //dizinin sınırlar dahilinde olup olmadığını kontrol etme
                    if (iColIndex <= dataGridViewKararMat.Columns.Count - 1
                    && iRowIndex <= dataGridViewKararMat.Rows.Count - 1)
                    {
                        DataGridViewCell cell = dataGridViewKararMat[iColIndex, iRowIndex];

                        // 'chkPasteToSelectedCells' işaretliyse seçili hücrelere kopyala
                        if ((chkPasteToSelectedCells.Checked && cell.Selected) ||
                            (!chkPasteToSelectedCells.Checked))
                            cell.Value = cbValue[rowKey][cellKey];
                    }
                    iColIndex++;
                }
                iRowIndex++;
            }
        }
        private DataGridViewCell GetStartCell(DataGridView dgView)
        {

            // en küçük satırı, sütun dizinini al
            if (dgView.SelectedCells.Count == 0)
                return null;

            int rowIndex = dgView.Rows.Count - 1;
            int colIndex = dgView.Columns.Count - 1;

            foreach (DataGridViewCell dgvCell in dgView.SelectedCells)
            {
                if (dgvCell.RowIndex < rowIndex)
                    rowIndex = dgvCell.RowIndex;
                if (dgvCell.ColumnIndex < colIndex)
                    colIndex = dgvCell.ColumnIndex;
            }

            return dgView[colIndex, rowIndex];
        }
        private Dictionary<int, Dictionary<int, string>> ClipBoardValues(string clipboardValue)
        {
            Dictionary<int, Dictionary<int, string>>
            copyValues = new Dictionary<int, Dictionary<int, string>>();

            String[] lines = clipboardValue.Split('\n');

            for (int i = 0; i <= lines.Length - 1; i++)
            {
                copyValues[i] = new Dictionary<int, string>();
                String[] lineContent = lines[i].Split('\t');

                // boş bir hücre değeri kopyalandıysa, sözlüğü boş bir dize ile ayarlayın
                // else Değeri sözlüğe ayarla
                if (lineContent.Length == 0)
                    copyValues[i][0] = string.Empty;
                else
                {
                    for (int j = 0; j <= lineContent.Length - 1; j++)
                        copyValues[i][j] = lineContent[j];
                }
            }
            return copyValues;
        }
        //-------------------------------------------------------------------------











    }
}

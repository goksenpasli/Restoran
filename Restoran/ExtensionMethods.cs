﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;
using System.Xml.Linq;
using System.Xml.Serialization;
using DotLiquid;
using HandyControl.Controls;
using Restoran.Model;
using Restoran.ViewModel;
using dotTemplate = DotLiquid.Template;

namespace Restoran
{
    internal static class ExtensionMethods
    {
        public enum Format
        {
            Tiff = 0,

            TiffRenkli = 1,

            Jpg = 2,

            Png = 3
        }

        public static double DepoÜrünAdeti(this Veriler veriler, int ürünid)
        {
            return veriler.Ürünler.Ürün.Any() ? veriler.Ürünler.Ürün.FirstOrDefault(z => z.Id == ürünid).Adet : 0;
        }

        public static double DepoÜrünEşikAdeti(this Veriler veriler, int ürünid)
        {
            return veriler.Ürünler.Ürün.Any() ? veriler.Ürünler.Ürün.FirstOrDefault(z => z.Id == ürünid).UyarıAdet : 0;
        }

        public static double FiyatHesapla(this Sipariş sipariş)
        {
            return ÜrünleriYükle().FirstOrDefault(x => x.Id == sipariş.ÜrünId).Fiyat * sipariş.Adet;
        }

        public static string GenerateTemplate(this Hash context, string reportpath)
        {
            using FileStream stream = new(reportpath, FileMode.Open);
            using StreamReader reader = new(stream);
            dotTemplate template = dotTemplate.Parse(reader.ReadToEnd());
            Hash docContext = context;
            return template.Render(docContext);
        }

        public static string ResimYükle(this string file, double en, double boy)
        {
            string filename = Guid.NewGuid() + Path.GetExtension(file);
            BitmapImage image = new(new Uri(file));
            File.WriteAllBytes($"{Path.GetDirectoryName(MainViewModelBase.xmldatapath)}\\{filename}", image.Resize(en, boy).ToTiffJpegByteArray(Format.Jpg));
            return filename;
        }

        public static BitmapSource Resize(this BitmapSource bfPhoto, double nWidth, double nHeight, double rotate = 0, int dpiX = 96, int dpiY = 96)
        {
            RotateTransform rotateTransform = new(rotate);
            ScaleTransform scaleTransform = new(nWidth / 96 * dpiX / bfPhoto.PixelWidth, nHeight / 96 * dpiY / bfPhoto.PixelHeight, 0, 0);
            TransformGroup transformGroup = new();
            transformGroup.Children.Add(rotateTransform);
            transformGroup.Children.Add(scaleTransform);
            TransformedBitmap tb = new(bfPhoto, transformGroup);
            tb.Freeze();
            return tb;
        }

        public static BitmapSource Resize(this BitmapSource bfPhoto, double oran)
        {
            TransformedBitmap tb = new(bfPhoto, new ScaleTransform(oran, oran, 0, 0));
            tb.Freeze();
            return tb;
        }

        [DllImport("shell32.dll", SetLastError = true)]
        public static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder, uint cidl, [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl, uint dwFlags);

        [DllImport("shell32.dll", SetLastError = true)]
        public static extern void SHParseDisplayName([MarshalAs(UnmanagedType.LPWStr)] string name, IntPtr bindingContext, [Out] out IntPtr pidl, uint sfgaoIn, [Out] out uint psfgaoOut);

        public static IEnumerable<Siparişler> SiparişDurumuVerileriniAl(this Veriler veriler, int year)
        {
            return veriler.Salonlar.Masalar.SelectMany(z => z.Masa).SelectMany(z => z.Siparişler).Where(z => z.Tarih.Year == year).GroupBy(z => z.Tarih.Month).OrderBy(z => z.Key).Select(t => new Siparişler() { Id = t.Key, ToplamTutar = t.Sum(z => z.ToplamTutar) });
        }

        public static byte[] ToTiffJpegByteArray(this ImageSource bitmapsource, Format format)
        {
            using MemoryStream outStream = new();
            switch (format)
            {
                case Format.TiffRenkli:
                    TiffBitmapEncoder tifzipencoder = new() { Compression = TiffCompressOption.Zip };
                    tifzipencoder.Frames.Add(BitmapFrame.Create((BitmapSource)bitmapsource));
                    tifzipencoder.Save(outStream);
                    return outStream.ToArray();

                case Format.Tiff:
                    TiffBitmapEncoder tifccittencoder = new() { Compression = TiffCompressOption.Ccitt4 };
                    tifccittencoder.Frames.Add(BitmapFrame.Create((BitmapSource)bitmapsource));
                    tifccittencoder.Save(outStream);
                    return outStream.ToArray();

                case Format.Jpg:
                    JpegBitmapEncoder jpgencoder = new() { QualityLevel = 75 };
                    jpgencoder.Frames.Add(BitmapFrame.Create((BitmapSource)bitmapsource));
                    jpgencoder.Save(outStream);
                    return outStream.ToArray();

                case Format.Png:
                    PngBitmapEncoder pngencoder = new();
                    pngencoder.Frames.Add(BitmapFrame.Create((BitmapSource)bitmapsource));
                    pngencoder.Save(outStream);
                    return outStream.ToArray();

                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }

        public static void ÜrünAdetDüşümüYap(ObservableCollection<Sipariş> Siparişler, ObservableCollection<Ürün> Ürünler)
        {
            foreach (Ürün ürün in Ürünler)
            {
                foreach (Sipariş sipariş in Siparişler.Where(sipariş => ürün.Id == sipariş.ÜrünId))
                {
                    if (sipariş.Adet <= ürün.Adet)
                    {
                        ürün.Adet -= sipariş.Adet;
                    }
                }
            }
        }

        public static FixedDocumentSequence WriteXPS(this FlowDocument flowDocument)
        {
            Package package = Package.Open(new MemoryStream(), FileMode.Create, FileAccess.ReadWrite);
            Uri packUri = new("pack://temp.xps");
            PackageStore.RemovePackage(packUri);
            PackageStore.AddPackage(packUri, package);
            using XpsDocument xpsDocument = new(package, CompressionOption.SuperFast, packUri.ToString());
            DocumentPaginator paginator = ((IDocumentPaginatorSource)flowDocument).DocumentPaginator;
            using (XpsSerializationManager xpsSerializationManager = new(new XpsPackagingPolicy(xpsDocument), false))
            {
                xpsSerializationManager.SaveAsXaml(paginator);
            }
            return xpsDocument.GetFixedDocumentSequence();
        }

        internal static T DeSerialize<T>(this string xmldatapath) where T : class, new()
        {
            try
            {
                XmlSerializer serializer = new(typeof(T));
                using StreamReader stream = new(xmldatapath);
                return serializer.Deserialize(stream) as T;
            }
            catch (Exception Ex)
            {
                Growl.Fatal(Ex.Message);
                return null;
            }
        }

        internal static T DeSerialize<T>(this XElement xElement) where T : class, new()
        {
            XmlSerializer serializer = new(typeof(T));
            return serializer.Deserialize(xElement.CreateReader()) as T;
        }

        internal static ObservableCollection<T> DeSerialize<T>(this IEnumerable<XElement> xElement) where T : class, new()
        {
            ObservableCollection<T> list = new();
            foreach (XElement element in xElement)
            {
                list.Add(element.DeSerialize<T>());
            }
            return list;
        }

        internal static Kategoriler Kategoriler()
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject())
                           ? null
                           : File.Exists(MainViewModelBase.xmldatapath)
                           ? MainViewModelBase.xmldatapath.DeSerialize<Veriler>().Kategoriler
                           : new Kategoriler();
        }

        internal static ObservableCollection<Kategori> KategorileriYükle()
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject())
                   ? null
                   : File.Exists(MainViewModelBase.xmldatapath)
                   ? MainViewModelBase.xmldatapath.DeSerialize<Veriler>().Kategoriler.Kategori
                   : new ObservableCollection<Kategori>();
        }

        internal static ObservableCollection<Masalar> MasalarıYükle()
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject())
           ? null
           : File.Exists(MainViewModelBase.xmldatapath)
           ? MainViewModelBase.xmldatapath.DeSerialize<Veriler>().Salonlar.Masalar
           : new ObservableCollection<Masalar>();
        }

        internal static Müşteriler Müşteriler()
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject())
                  ? null
                  : File.Exists(MainViewModelBase.xmldatapath)
                  ? MainViewModelBase.xmldatapath.DeSerialize<Veriler>().Müşteriler
                  : new Müşteriler();
        }

        internal static ObservableCollection<Müşteri> MüşterileriYükle()
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject())
                     ? null
                     : File.Exists(MainViewModelBase.xmldatapath)
                     ? MainViewModelBase.xmldatapath.DeSerialize<Veriler>().Müşteriler.Müşteri
                     : new ObservableCollection<Müşteri>();
        }

        internal static string RandomColor()
        {
            return $"#{new Random(Guid.NewGuid().GetHashCode()).Next(0x1000000):X6}";
        }

        internal static int RandomNumber()
        {
            return new Random(Guid.NewGuid().GetHashCode()).Next(1, int.MaxValue);
        }

        internal static Salonlar Salonlar()
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject())
                    ? null
                    : File.Exists(MainViewModelBase.xmldatapath)
                    ? MainViewModelBase.xmldatapath.DeSerialize<Veriler>().Salonlar
                    : new Salonlar();
        }

        internal static void Serialize<T>(this T dataToSerialize) where T : class
        {
            XmlSerializer serializer = new(typeof(T));
            using TextWriter stream = new StreamWriter(MainViewModelBase.xmldatapath);
            serializer.Serialize(stream, dataToSerialize);
        }

        internal static double SiparişToplamları(this ObservableCollection<Sipariş> siparişler)
        {
            double toplamfiyat = 0;
            foreach (Sipariş sipariş in siparişler)
            {
                toplamfiyat += ÜrünleriYükle().FirstOrDefault(x => x.Id == sipariş.ÜrünId).Fiyat * sipariş.Adet;
            }
            return toplamfiyat;
        }

        internal static Ürünler Ürünler()
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject())
                    ? null
                    : File.Exists(MainViewModelBase.xmldatapath)
                    ? MainViewModelBase.xmldatapath.DeSerialize<Veriler>().Ürünler
                    : new Ürünler();
        }

        internal static ObservableCollection<Ürün> ÜrünleriYükle()
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject())
                     ? null
                     : File.Exists(MainViewModelBase.xmldatapath)
                     ? MainViewModelBase.xmldatapath.DeSerialize<Veriler>().Ürünler.Ürün
                     : new ObservableCollection<Ürün>();
        }
    }
}
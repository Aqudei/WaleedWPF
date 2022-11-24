using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Caliburn.Micro;

namespace WaleedWPF.ViewModels
{
    public sealed class PdfViewModel : Screen
    {
        private string _selectedFile;
        private int _currentPageIndex = 0;
        private readonly List<BitmapSource> _pdfPages = new List<BitmapSource>();
        private BitmapSource _currentPage;
        public ObservableCollection<string> Files { get; set; } = new ObservableCollection<string>();

        public int CurrentPageIndex
        {
            get => _currentPageIndex;
            set => Set(ref _currentPageIndex, value);
        }

        public string SelectedFile
        {
            get => _selectedFile;
            set => Set(ref _selectedFile, value);
        }

        public PdfViewModel()
        {
            PropertyChanged += PdfViewModel_PropertyChanged;
        }

        /// <summary>Deletes a logical pen, brush, font, bitmap, region, or palette, freeing all system resources associated with the object. After the object is deleted, the specified handle is no longer valid.</summary>
        /// <param name="hObject">A handle to a logical pen, brush, font, bitmap, region, or palette.</param>
        /// <returns>
        ///   <para>If the function succeeds, the return value is nonzero.</para>
        ///   <para>If the specified handle is not valid or is currently selected into a DC, the return value is zero.</para>
        /// </returns>
        /// <remarks>
        ///   <para>Do not delete a drawing object (pen or brush) while it is still selected into a DC.</para>
        ///   <para>When a pattern brush is deleted, the bitmap associated with the brush is not deleted. The bitmap must be deleted independently.</para>
        /// </remarks>
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);


        public static BitmapSource GetImageStream(Image myImage)
        {
            var bitmap = new Bitmap(myImage);
            IntPtr bmpPt = bitmap.GetHbitmap();
            BitmapSource bitmapSource =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    bmpPt,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

            //freeze bitmapSource and clear memory to avoid memory leaks
            bitmapSource.Freeze();
            DeleteObject(bmpPt);

            return bitmapSource;
        }
        private void PdfViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (nameof(SelectedFile) == e.PropertyName)
            {
                _pdfPages.Clear();
                CurrentPageIndex = 0;
                CurrentPage = null;

                if (string.IsNullOrWhiteSpace(SelectedFile) == false && File.Exists(SelectedFile))
                {
                    RenderPdf();
                }
            }
        }

        private void RenderPdf()
        {
            Task.Run(() =>
            {
                var pdfDocument = PdfiumViewer.PdfDocument.Load(SelectedFile);
                for (int i = 0; i < pdfDocument.PageCount; i++)
                {
                    var pageSize = pdfDocument.PageSizes[i];

                    var rendered = pdfDocument.Render(i, 300, 300, false);
                    _pdfPages.Add(GetImageStream(rendered));
                }

                if (_pdfPages.Any())
                {
                    Execute.OnUIThread(() => CurrentPage = _pdfPages[CurrentPageIndex]);
                }
            });

        }


        public void NextPage()
        {
            if (!_pdfPages.Any())
            {
                return;
            }

            CurrentPageIndex += 1;
            CurrentPageIndex %= _pdfPages.Count;
            CurrentPage = _pdfPages[CurrentPageIndex];
        }

        public void PreviousPage()
        {
            if (!_pdfPages.Any())
            {
                return;
            }

            CurrentPageIndex -= 1;
            if (CurrentPageIndex < 0)
            {
                CurrentPageIndex = _pdfPages.Count - 1;
            }

            CurrentPage = _pdfPages[CurrentPageIndex];
        }
        public BitmapSource CurrentPage
        {
            get => _currentPage;
            set => Set(ref _currentPage, value);
        }

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            var pdfs = Directory.EnumerateFiles("./FilesPdf");
            pdfs.Apply(Files.Add);
            return base.OnInitializeAsync(cancellationToken);
        }

        public void OpenInAcrobat()
        {
            Process.Start(SelectedFile);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace VanBan.View
{
    public class DocumentFramework : FrameworkElement
    {

        #region TextProperty
        /// <summary>
        /// Lấy hoặc cài đặt thuộc tính nội dung cho ...
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); InvalidateVisual(); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(DocumentFramework), new PropertyMetadata(string.Empty, TextChangedCallBack));

        private static void TextChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DocumentFramework).InvalidateVisual();
        }

        #endregion

        Point p1 = new Point(15, 6.8);//Tọa độ vị trí bắt đầu  kéo chuột
        Point p2 = new Point(25.5, 56.8);//Tọa độ kết thúc kéo chuột
        int _canLe = 1;//1 Trái 2 phải; 3 Giua; 4 Deu
        int _soCot = 1;//Số cột phân
        double widthCheck = 0;// Width của mỗi cột 
        public double tec { get; set; }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            widthCheck = ActualWidth / _soCot;
            double lineSpacing = 0;//Khoảng cách các hàng
            List<FormattedText> _listFormattedText = new List<FormattedText>();
            List<FormatText> _listFormatText = new List<FormatText>();//List chứa các Format
            List<ListText> _listText = new List<ListText>();//
            //Khai bao toa do boi den.
            tec += 2;
            List<ItemFormatText> listFont = new List<ItemFormatText>();//Gía trị khởi tạo ban đầu của text
            listFont.Add(new ItemFormatText { viTriKetThuc = 8, _fontSize = 10, _fontType = "Arial", _offset = 0, _line = 0 });
            listFont.Add(new ItemFormatText { viTriKetThuc = 11, _fontSize = 20, _fontType = "Arial", _offset = 0, _line = 0 });
            listFont.Add(new ItemFormatText { viTriKetThuc = 12, _fontSize = 20, _fontType = "Arial", _offset = 0, _line = 1 });
            listFont.Add(new ItemFormatText { viTriKetThuc = 13, _fontSize = 20, _fontType = "Arial", _offset = 50, _line = 0 });
            listFont.Add(new ItemFormatText { viTriKetThuc = 16, _fontSize = 18, _fontType = "Arial", _offset = 0, _line = 0 });
            listFont.Add(new ItemFormatText { viTriKetThuc = 32, _fontSize = 22, _fontType = "Arial", _offset = 0, _line = 1 });
            listFont.Add(new ItemFormatText { viTriKetThuc = 47, _fontSize = 18, _fontType = "Arial", _offset = 0, _line = 0 });
            List<TemplateFormattedText> listTu = new List<TemplateFormattedText>();// List các từ khi cắt ra
            //Xử lý trường hợp khoảng trắng nằm đầu tiên
            if (Text[0] == '_')
            {
                listTu.Add(new TemplateFormattedText() { Tu = "", KhoangTrang = Text.Substring(0, viTriDauTienKhacKT(Text, 0)) });
            }
            var listCat = Text.Split('_');// Cắt text ra theo khoảng trắng
            string sumSpace = "";
            //Thêm vào list từ
            for (int i = 0; i < listCat.Length; i++)
            {
                if (!string.IsNullOrEmpty(listCat[i]))
                {
                    sumSpace = "";
                    int j = i + 1;
                    for (j = i + 1; j < listCat.Length; j++)
                    {
                        if (string.IsNullOrEmpty(listCat[j]))
                        {
                            sumSpace += "_";
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (i < viTriKhacKTCuoiList(listCat))
                    {
                        sumSpace += "_";
                    }
                    listTu.Add(new TemplateFormattedText() { Tu = listCat[i], KhoangTrang = sumSpace });
                    i = j - 1;
                }
            }
            string xauHoanChinh = "";// Khai báo khi xâu đã hoàn chỉnh
            double sumWidth = 0;//Tổng width để xét xuống hàng
            int viTriCuoiChuoi = 0;//Vị trí cuối không phải khoảng trắng
            int viTriCuoiKT = 0;//Vị trí cuối của khoảng trắng
            int viTriDauChuoi = 0;//Vị trí đầu không phải khoảng trắng
            int viTriDauKT = 0;//Vị trí đầu của khoảng trắng
            int positionFontTu = 0;// Vị trí Font của từ khác khoảng trắng
            int positionFontKT = 0;// Vị trí Font của khoảng trắng
            double maxBaseline = 0;// Chiều cao lớn nhất của ký tự trong một hàng
            double _height = 0;//Tọa độ y của một từ
            double x = 0;//Tọa độ x của một từ
            double check = 0;
            //Xử lý xuống hàng
            foreach (var item in listTu)
            {
                if (item.Tu.Length > 0)
                {
                    viTriCuoiChuoi = viTriCuoiKT + item.Tu.Length;
                }
                if (viTriCuoiChuoi == 0)
                {
                    viTriCuoiKT = viTriCuoiChuoi + item.KhoangTrang.Length - 1;
                }
                else
                    viTriCuoiKT = viTriCuoiChuoi + item.KhoangTrang.Length;
                positionFontTu = viTriDinhDang(listFont, viTriCuoiChuoi);
                positionFontKT = viTriDinhDang(listFont, viTriCuoiKT);
                if (item.Tu.Length > 0)
                {
                    viTriDauChuoi = viTriCuoiChuoi - item.Tu.Length + 1;
                }
                if (item.KhoangTrang.Length > 0)
                {
                    viTriDauKT = viTriCuoiKT - item.KhoangTrang.Length + 1;
                }
                check = widthXau(item.Tu, listFont[positionFontTu]) - widthXau(listFont, item.Tu, viTriDauChuoi, viTriCuoiChuoi);
                sumWidth += widthXau(listFont, item.Tu, viTriDauChuoi, viTriCuoiChuoi);
                //Khi vượt quá width của vùng chứa thì add vào listText rồi xử lý xuống hàng
                if (sumWidth > widthCheck)
                {
                    x = 0;
                    // Add các từ vào trong list
                    for (int j = 0; j < _listFormattedText.Count; j++)
                    {
                        Typeface typeface = new Typeface(("Arial"));
                        _listText.Add(new ListText
                        {
                            text = _listFormatText[j].textToFormat,
                            size = _listFormatText[j].emSize,
                            typeface = typeface,
                            foreground = _listFormatText[j].foreground,
                            y = maxBaseline + _height + lineSpacing,
                            maxBaseline = maxBaseline,
                            x = x,
                            offset = _listFormatText[j].offset,
                            line = _listFormatText[j].line

                        });
                        x += _listFormattedText[j].WidthIncludingTrailingWhitespace;//Tăng tọa độ x lên để vẽ cho hàng tiếp theo
                    }
                    //Gan lai height,maxBaseline, clear _listFormattedText
                    _height += maxBaseline;
                    maxBaseline = 0;
                    _listFormattedText.Clear();
                    _listFormatText.Clear();
                    //Trường hợp một từ có width lớn hơn ActuaWidth
                    if (widthXau(listFont, item.Tu, viTriDauChuoi, viTriCuoiChuoi) > widthCheck)
                    {
                        //Gán lại giá trị sumWidth, maxBaseLine
                        maxBaseline = 0;
                        sumWidth = 0;
                        int vitri = 0;
                        //Duyệt qua các kí tự trong từ
                        for (int i = 0; i < item.Tu.Length;i++)
                        {
                            //
                            var charKiTu = new FormattedText(item.Tu[i].ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"),
                              listFont[viTriDinhDang(listFont, viTriDauChuoi + i)]._fontSize * (1 - ((int)listFont[viTriDinhDang(listFont, viTriDauChuoi + i)]._offset != 0 ? 1 : 0) / 3.0), Brushes.Black);

                            var charKiTu2 = new FormatText(item.Tu[i].ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), listFont[viTriDinhDang(listFont, viTriDauChuoi + i)]._fontSize * (1 - ((int)listFont[viTriDinhDang(listFont, viTriDauChuoi + i)]._offset != 0 ? 1 : 0) / 3.0), Brushes.Black, listFont[viTriDinhDang(listFont, viTriDauChuoi + i)]._offset, listFont[viTriDinhDang(listFont, viTriDauChuoi + i)]._line);
                            if (charKiTu.Baseline > maxBaseline) maxBaseline = charKiTu.Baseline;//Tìm chiều cao max trong hàng
                            sumWidth += charKiTu.WidthIncludingTrailingWhitespace;//Cộng thêm width cho kí tự đó
                            //Xử lý khi tổng width vượt quá phạm vi
                            if (sumWidth > widthCheck)
                            {
                                x = 0;// Gán lại tọa độ x=0
                                //Thêm vào list
                                for (int j = 0; j < _listFormattedText.Count; j++)
                                {
                                    Typeface typeface = new Typeface(("Arial"));
                                    _listText.Add(new ListText
                                    {
                                        text = _listFormatText[j].textToFormat,
                                        size = _listFormatText[j].emSize,
                                        typeface = typeface,
                                        foreground = _listFormatText[j].foreground,
                                        x = x,
                                        y = maxBaseline + _height + lineSpacing,
                                        maxBaseline = maxBaseline,
                                        offset = _listFormatText[j].offset,
                                        line = _listFormatText[j].line
                                    });
                                    x += _listFormattedText[j].WidthIncludingTrailingWhitespace;
                                }
                                //Gan lai height,maxBase, list
                                _height += maxBaseline;
                                maxBaseline = charKiTu.Baseline;
                                _listFormattedText.Clear();
                                _listFormatText.Clear();
                                sumWidth = charKiTu.WidthIncludingTrailingWhitespace;
                                _listFormattedText.Add(charKiTu);
                                _listFormatText.Add(charKiTu2);
                                vitri = i;// Vị trí cuối của từ khi bị cắt xuống hàng
                            }
                            else
                            {
                                //Add vào list
                                _listFormattedText.Add(charKiTu);
                                _listFormatText.Add(charKiTu2);
                            }

                        }
                        //Trường hợp khoảng trắng ở cuối
                        if (vitri < item.Tu.Length)
                        {
                            for (int i = 0; i < item.KhoangTrang.Length; i++)
                            {
                                var charKT = new FormattedText(item.KhoangTrang[i].ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), listFont[viTriDinhDang(listFont, viTriDauKT + i)]._fontSize * (1 - ((int)listFont[viTriDinhDang(listFont, viTriDauKT + i)]._offset != 0 ? 1 : 0) / 3.0), Brushes.Black);
                                var charKT2 = new FormatText(item.KhoangTrang[i].ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), listFont[viTriDinhDang(listFont, viTriDauKT + i)]._fontSize * (1 - ((int)listFont[viTriDinhDang(listFont, viTriDauKT + i)]._offset != 0 ? 1 : 0) / 3.0), Brushes.Black, listFont[viTriDinhDang(listFont, viTriDauKT + i)]._offset, listFont[viTriDinhDang(listFont, viTriDauKT + i)]._line);
                                sumWidth += charKT.WidthIncludingTrailingWhitespace;
                                _listFormattedText.Add(charKT);
                                _listFormatText.Add(charKT2);
                            }
                        }
                    }
                    else
                    {
                        //Gán lại giá trị
                        sumWidth = 0;
                        int toaDo = 0;
                        int i = 0;
                        for (i = viTriDinhDang(listFont, viTriDauChuoi); i < viTriDinhDang(listFont, viTriCuoiChuoi); i++)
                        {
                            var xau = new FormattedText(item.Tu.Substring(toaDo, listFont[i].viTriKetThuc - viTriDauChuoi - toaDo + 1), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), listFont[i]._fontSize * (1 -
                                ((int)listFont[i]._offset != 0 ? 1 : 0) / 3.0), Brushes.Black);
                            var xau21 = new FormatText(item.Tu.Substring(toaDo, listFont[i].viTriKetThuc - viTriDauChuoi - toaDo + 1), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), listFont[i]._fontSize * (1 -
                                ((int)listFont[i]._offset != 0 ? 1 : 0) / 3.0), Brushes.Black, listFont[i]._offset, listFont[i]._line);
                            toaDo = listFont[i].viTriKetThuc - viTriDauChuoi + 1;
                            if (xau.Baseline > maxBaseline) maxBaseline = xau.Baseline;
                            sumWidth += xau.WidthIncludingTrailingWhitespace;
                            _listFormattedText.Add(xau);
                            _listFormatText.Add(xau21);
                        }
                        if (item.Tu.Length > 0)
                        {
                            var xau2 = new FormattedText(item.Tu.Substring(toaDo, viTriCuoiChuoi - toaDo - viTriDauChuoi + 1), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), listFont[viTriDinhDang(listFont, viTriCuoiChuoi)]._fontSize * (1 - ((int)listFont[viTriDinhDang(listFont, viTriCuoiChuoi)]._offset != 0 ? 1 : 0) / 3.0), Brushes.Black);
                            var xau22 = new FormatText(item.Tu.Substring(toaDo, viTriCuoiChuoi - toaDo - viTriDauChuoi + 1), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), listFont[viTriDinhDang(listFont, viTriCuoiChuoi)]._fontSize * (1 - ((int)listFont[viTriDinhDang(listFont, viTriCuoiChuoi)]._offset != 0 ? 1 : 0) / 3.0), Brushes.Black, listFont[viTriDinhDang(listFont, viTriCuoiChuoi)]._offset, listFont[viTriDinhDang(listFont, viTriCuoiChuoi)]._line);
                            if (xau2.Baseline > maxBaseline) maxBaseline = xau2.Baseline;
                            sumWidth += xau2.WidthIncludingTrailingWhitespace;
                            _listFormattedText.Add(xau2);
                            _listFormatText.Add(xau22);
                        }

                        //dinh dang va add vao list cac kt
                        toaDo = 0;
                        i = 0;
                        if (!string.IsNullOrEmpty(item.KhoangTrang))
                        {
                            for (i = viTriDinhDang(listFont, viTriDauKT); i < viTriDinhDang(listFont, viTriCuoiKT); i++)
                            {
                                string t = item.KhoangTrang;
                                var kt = new FormattedText(item.KhoangTrang.Substring(toaDo, listFont[i].viTriKetThuc - viTriDauKT - toaDo + 1), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), listFont[i]._fontSize * (1 - ((int)listFont[i]._offset != 0 ? 1 : 0) / 3.0), Brushes.Black);
                                var kt21 = new FormatText(item.KhoangTrang.Substring(toaDo, listFont[i].viTriKetThuc - viTriDauKT - toaDo + 1), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), listFont[i]._fontSize * (1 - ((int)listFont[i]._offset != 0 ? 1 : 0) / 3.0), Brushes.Black, listFont[i]._offset, listFont[i]._line);
                                toaDo = listFont[i].viTriKetThuc - viTriDauKT + 1;
                                _listFormattedText.Add(kt);
                                _listFormatText.Add(kt21);
                                sumWidth += kt.WidthIncludingTrailingWhitespace;
                            }
                            if (item.KhoangTrang.Length > 0)
                            {
                                var kt2 = new FormattedText(item.KhoangTrang.Substring(toaDo, viTriCuoiKT - toaDo - viTriDauKT + 1), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), listFont[viTriDinhDang(listFont, viTriCuoiKT)]._fontSize * (1 - ((int)listFont[viTriDinhDang(listFont, viTriCuoiKT)]._offset != 0 ? 1 : 0) / 3.0), Brushes.Black);
                                var kt22 = new FormatText(item.KhoangTrang.Substring(toaDo, viTriCuoiKT - toaDo - viTriDauKT + 1), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), listFont[viTriDinhDang(listFont, viTriCuoiKT)]._fontSize * (1 - ((int)listFont[viTriDinhDang(listFont, viTriCuoiKT)]._offset != 0 ? 1 : 0) / 3.0), Brushes.Black, listFont[viTriDinhDang(listFont, viTriCuoiKT)]._offset, listFont[viTriDinhDang(listFont, viTriCuoiKT)]._line);
                                _listFormattedText.Add(kt2);
                                _listFormatText.Add(kt22);
                                sumWidth += kt2.WidthIncludingTrailingWhitespace;
                            }
                        }
                    }
                }
                else
                {/*
                   var tu = new FormattedText(item.Tu, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"),listFont[positionFontTu]._fontSize, Brushes.Black);
                   var KT = new FormattedText(item.KhoangTrang, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), listFont[positionFontKT]._fontSize, Brushes.Black);
                   if (tu.Baseline > maxBaseline) maxBaseline = tu.Baseline;
                   _listFormattedText.Add(tu);
                   _listFormattedText.Add(KT);
                   sumWidth += KT.WidthIncludingTrailingWhitespace;
                   */
                    //dinh dang va add vao list cac tu

                    int toaDo = 0;
                    int i = 0;
                    i = viTriDinhDang(listFont, viTriDauChuoi);
                    i = viTriCuoiChuoi;
                    for (i = viTriDinhDang(listFont, viTriDauChuoi); i < viTriDinhDang(listFont, viTriCuoiChuoi); i++)
                    {
                        int ch = 0;
                        ch = listFont[i]._offset != 0 ? 1 : 0;

                        var xau = new FormattedText(item.Tu.Substring(toaDo, listFont[i].viTriKetThuc - viTriDauChuoi - toaDo + 1), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), listFont[i]._fontSize * (1 - ((int)listFont[i]._offset != 0 ? 1 : 0) / 3.0), Brushes.Black);
                        var xau21 = new FormatText(item.Tu.Substring(toaDo, listFont[i].viTriKetThuc - viTriDauChuoi - toaDo + 1), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), listFont[i]._fontSize * (1 - ((int)listFont[i]._offset != 0 ? 1 : 0) / 3.0), Brushes.Black, listFont[i]._offset, listFont[i]._line);
                        toaDo = listFont[i].viTriKetThuc - viTriDauChuoi + 1;
                        if (xau.Baseline > maxBaseline) maxBaseline = xau.Baseline;
                        _listFormattedText.Add(xau);
                        _listFormatText.Add(xau21);

                    }
                    if (item.Tu.Length > 0)
                    {
                        var xau2 = new FormattedText(item.Tu.Substring(toaDo, viTriCuoiChuoi - toaDo - viTriDauChuoi + 1), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), listFont[viTriDinhDang(listFont, viTriCuoiChuoi)]._fontSize * (1 - ((int)listFont[viTriDinhDang(listFont, viTriCuoiChuoi)]._offset != 0 ? 1 : 0) / 3.0), Brushes.Black);
                        var xau22 = new FormatText(item.Tu.Substring(toaDo, viTriCuoiChuoi - toaDo - viTriDauChuoi + 1), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), listFont[viTriDinhDang(listFont, viTriCuoiChuoi)]._fontSize * (1 - ((int)listFont[viTriDinhDang(listFont, viTriCuoiChuoi)]._offset != 0 ? 1 : 0) / 3.0), Brushes.Black, listFont[viTriDinhDang(listFont, viTriCuoiChuoi)]._offset, listFont[viTriDinhDang(listFont, viTriCuoiChuoi)]._line);
                        if (xau2.Baseline > maxBaseline) maxBaseline = xau2.Baseline;
                        _listFormattedText.Add(xau2);
                        _listFormatText.Add(xau22);
                    }

                    //dinh dang va add vao list cac kt
                    toaDo = 0;
                    i = 0;
                    if (!string.IsNullOrEmpty(item.KhoangTrang))
                    {
                        for (i = viTriDinhDang(listFont, viTriDauKT); i < viTriDinhDang(listFont, viTriCuoiKT); i++)
                        {
                            string t = item.KhoangTrang;
                            var kt = new FormattedText(item.KhoangTrang.Substring(toaDo, listFont[i].viTriKetThuc - viTriDauKT - toaDo + 1), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), listFont[i]._fontSize * (1 - ((int)listFont[i]._offset != 0 ? 1 : 0) / 3.0), Brushes.Black);
                            var kt21 = new FormatText(item.KhoangTrang.Substring(toaDo, listFont[i].viTriKetThuc - viTriDauKT - toaDo + 1), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), listFont[i]._fontSize * (1 - ((int)listFont[i]._offset != 0 ? 1 : 0) / 3.0), Brushes.Black, listFont[i]._offset, listFont[i]._line);
                            toaDo = listFont[i].viTriKetThuc - viTriDauKT + 1;
                            _listFormattedText.Add(kt);
                            _listFormatText.Add(kt21);
                            sumWidth += kt.WidthIncludingTrailingWhitespace;
                        }
                        if (item.KhoangTrang.Length > 0)
                        {
                            var kt2 = new FormattedText(item.KhoangTrang.Substring(toaDo, viTriCuoiKT - toaDo - viTriDauKT + 1), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), listFont[viTriDinhDang(listFont, viTriCuoiKT)]._fontSize * (1 - ((int)listFont[viTriDinhDang(listFont, viTriCuoiKT)]._offset != 0 ? 1 : 0) / 3.0), Brushes.Black);
                            var kt22 = new FormatText(item.KhoangTrang.Substring(toaDo, viTriCuoiKT - toaDo - viTriDauKT + 1), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), listFont[viTriDinhDang(listFont, viTriCuoiKT)]._fontSize * (1 - ((int)listFont[viTriDinhDang(listFont, viTriCuoiKT)]._offset != 0 ? 1 : 0) / 3.0), Brushes.Black, listFont[viTriDinhDang(listFont, viTriCuoiKT)]._offset, listFont[viTriDinhDang(listFont, viTriCuoiKT)]._line);
                            _listFormattedText.Add(kt2);
                            _listFormatText.Add(kt22);
                            sumWidth += kt2.WidthIncludingTrailingWhitespace;
                        }
                    }
                }
            }
            x = 0;
            for (int j = 0; j < _listFormattedText.Count; j++)
            {
                Typeface typeface = new Typeface(("Arial"));
                /*Point origin = new Point(x, maxBaseline  + _height + lineSpacing);
                    GlyphRun run = CreateGlyphRun(typeface,_listFormatText[j].textToFormat, _listFormatText[j].emSize, origin);
                    drawingContext.DrawGlyphRun(Brushes.Black, run);*/
                _listText.Add(new ListText { text = _listFormatText[j].textToFormat, size = _listFormatText[j].emSize, typeface = typeface, foreground = _listFormatText[j].foreground, x = x, y = maxBaseline + _height + lineSpacing, maxBaseline = maxBaseline, offset = _listFormatText[j].offset, line = _listFormatText[j].line });
                //drawingContext.DrawText(_listFormattedText[j], new Point(x, maxBaseline - _listFormattedText[j].Baseline + _height + lineSpacing));
                x += _listFormattedText[j].WidthIncludingTrailingWhitespace;
            }
            _listFormattedText.Clear();
            _listFormatText.Clear();
            FormattedText _formattedText = new FormattedText(
                string.Format(xauHoanChinh, Environment.NewLine), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                new Typeface("Arial"), 14, Brushes.Black);
          
            //Xử lý bôi đen xâu.
            Point pTam1 = new Point(0, 0);//Xác định vị trí bắt đầu con trỏ bôi đen
            Point pTam2 = new Point(0, 0);//Xác định vị trí kết thúc con trỏ bôi đen
            int _viTriBoiDen1 = 0;
            int _viTriBoiDen2 = 0;
            //Xác định vị trí từ bôi đen bắt đầu
            for (int i = 0; i < _listText.Count; i++)
            {
                if (_listText[i].y - _listText[i].maxBaseline > p1.Y) break;
                if (_listText[i].x <= p1.X)
                {
                    _viTriBoiDen1 = i;
                }
            }
            string a = _listText[_viTriBoiDen1].text;
            //Xác định vi trí con trỏ bắt đầu
            double vitriCheck1 = _listText[_viTriBoiDen1].x;
            for (int i = 0; i < _listText[_viTriBoiDen1].text.Length; i++)
            {
                var charKiTu = new FormattedText(_listText[_viTriBoiDen1].text[i].ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), _listText[_viTriBoiDen1].size, Brushes.Black);
                if (p1.X < vitriCheck1 + charKiTu.WidthIncludingTrailingWhitespace / 2.0)
                {
                    pTam1.X = vitriCheck1;
                    pTam1.Y = _listText[_viTriBoiDen1].y;
                    break;
                }
                vitriCheck1 += charKiTu.WidthIncludingTrailingWhitespace;
                if (i == _listText[_viTriBoiDen1].text.Length - 1)
                {
                    pTam1.X = vitriCheck1;
                    pTam1.Y = _listText[_viTriBoiDen1].y;
                }
            }
            //Xác định vị trí từ bôi đen kết thúc
            for (int i = 0; i < _listText.Count; i++)
            {
                if (_listText[i].y - _listText[i].maxBaseline > p2.Y) break;
                if (_listText[i].x <= p2.X)
                {
                    _viTriBoiDen2 = i;
                }
            }
            string b = _listText[_viTriBoiDen2].text;
            //Xác định vi trí con trỏ kết thúc
            double vitriCheck2 = _listText[_viTriBoiDen2].x;

            for (int i = 0; i < _listText[_viTriBoiDen2].text.Length; i++)
            {
                var charKiTu = new FormattedText(_listText[_viTriBoiDen2].text[i].ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), _listText[_viTriBoiDen2].size, Brushes.Black);
                if (p2.X < vitriCheck2 + charKiTu.WidthIncludingTrailingWhitespace / 2.0)
                {
                    pTam2.X = vitriCheck2;
                    pTam2.Y = _listText[_viTriBoiDen2].y;
                    break;
                }
                vitriCheck2 += charKiTu.WidthIncludingTrailingWhitespace;
                if (i == _listText[_viTriBoiDen2].text.Length - 1)
                {
                    pTam2.X = vitriCheck2;
                    pTam2.Y = _listText[_viTriBoiDen2].y;
                }
            }
            //Vẽ bôi đen
             //Trường hợp vị trí chuột trên một hàng
            if (_listText[_viTriBoiDen1].y == _listText[_viTriBoiDen2].y)
            {
                Rect rect = new Rect(pTam1.X, pTam1.Y - _listText[_viTriBoiDen1].maxBaseline, Math.Abs(pTam2.X - pTam1.X), _listText[_viTriBoiDen1].maxBaseline);
                Brush brush = new SolidColorBrush(Colors.Blue);
                drawingContext.DrawRectangle(brush, null, rect);
            }
            else
            {
             //Vẽ hàng đầu tiên
                for (int i = _viTriBoiDen1; i < _viTriBoiDen2; i++)
                {
                    if (_listText[i + 1].y > _listText[i].y)
                    {
                        var charKiTu = new FormattedText(_listText[i].text.ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), _listText[i].size, Brushes.Black);
                        //Vẽ hàng
                        Rect rect2 = new Rect(pTam1.X, pTam1.Y - _listText[_viTriBoiDen1].maxBaseline, _listText[i].x + charKiTu.WidthIncludingTrailingWhitespace - pTam1.X, _listText[i].maxBaseline);
                        Brush brush2 = new SolidColorBrush(Colors.Blue);
                        drawingContext.DrawRectangle(brush2, null, rect2);
                        break;
                    }
                }
                //Vẽ các hàng tiếp theo
                if (pTam1.Y < pTam2.Y - _listText[_viTriBoiDen2].maxBaseline)
                {
                    for (int i = _viTriBoiDen1; i < _viTriBoiDen2; i++)
                    {
                        if (_listText[i].y > pTam1.Y)
                        {
                            if (_listText[i].y < pTam2.Y)
                            {
                                if (_listText[i + 1].y > _listText[i].y)
                                {
                                    var charKiTu = new FormattedText(_listText[i].text.ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), _listText[i].size, Brushes.Black);
                                    //Vẽ hàng
                                    Rect rect2 = new Rect(0, _listText[i].y - _listText[i].maxBaseline, _listText[i].x + charKiTu.WidthIncludingTrailingWhitespace, _listText[i].maxBaseline);
                                    Brush brush2 = new SolidColorBrush(Colors.Blue);
                                    drawingContext.DrawRectangle(brush2, null, rect2);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                Rect rect3 = new Rect(0, pTam2.Y - _listText[_viTriBoiDen2].maxBaseline, pTam2.X, _listText[_viTriBoiDen2].maxBaseline);
                Brush brush3 = new SolidColorBrush(Colors.Blue);
                drawingContext.DrawRectangle(brush3, null, rect3);
            }
            //Vẽ điểm kéo thả
            Brush brushc = new SolidColorBrush(Colors.Red);
            drawingContext.DrawEllipse(brushc, null, p1, 2, 2);
            drawingContext.DrawEllipse(brushc, null, p2, 2, 2);
            //VẼ VĂN BẢN
            double _xChange = 0;
            double _yChange = 0;
            int tam = 0;
            if (_canLe == 1)//Can le trai           
                for (int i = 0; i < _listText.Count; i++)
                {
                    var xau = new FormattedText(_listText[i].text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), _listText[i].size, Brushes.Black);
                    Typeface typeface = new Typeface(("Arial"));
                    double ch = xau.Baseline * _listText[i].offset * 2 / 100.0;

                    Point origin = new Point(_listText[i].x + _xChange, _listText[i].y - xau.Baseline * _listText[i].offset * 2 / 100.0 - _yChange);
                    GlyphRun run = CreateGlyphRun(typeface, _listText[i].text, _listText[i].size, origin);
                    drawingContext.DrawGlyphRun(Brushes.Red, run);
                    if (_listText[i].line == 1)
                    {
                        Pen pen = new Pen(Brushes.Black, 1);
                        Point point1 = new Point(_listText[i].x, _listText[i].y - _listText[i].size * 1 / 4);
                        Point point2 = new Point(_listText[i].x + xau.WidthIncludingTrailingWhitespace, _listText[i].y - _listText[i].size * 1 / 4);
                        drawingContext.DrawLine(pen, point1, point2);
                    }
                    if ((_listText[i].y > _listText[_listText.Count - 1].y / _soCot + _yChange) && (_listText[i].y) > _listText[tam].y)
                    {
                        tam = i;
                        for (int j = i + 1; j < _listText.Count; j++)
                        {
                            if (_listText[j].y == _listText[i].y)
                            {
                                xau = new FormattedText(_listText[j].text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), _listText[j].size, Brushes.Black);
                                typeface = new Typeface(("Arial"));
                                ch = xau.Baseline * _listText[j].offset * 2 / 100.0;

                                origin = new Point(_listText[j].x + _xChange, _listText[j].y - xau.Baseline * _listText[j].offset * 2 / 100.0 - _yChange);
                                run = CreateGlyphRun(typeface, _listText[j].text, _listText[j].size, origin);
                                drawingContext.DrawGlyphRun(Brushes.Red, run);
                                if (_listText[j].line == 1)
                                {
                                    Pen pen = new Pen(Brushes.Black, 5F);
                                    Point point1 = new Point(_listText[j].x, _listText[j].y - _listText[j].maxBaseline * 4 / 10);
                                    Point point2 = new Point(_listText[j].x + xau.WidthIncludingTrailingWhitespace, _listText[j].y - _listText[j].maxBaseline * 4 / 10);
                                    drawingContext.DrawLine(pen, point1, point2);
                                }
                            }

                            else break;
                            i = j;
                        }
                        _xChange += widthCheck;
                        _yChange = _listText[i - 1].y;
                    }

                }
            /* if (_canLe == 1)//Can le trai
                 for (int i = 0; i < _listText.Count; i++)
                 {

                     var xau = new FormattedText(_listText[i].text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), _listText[i].size, Brushes.Black);
                     Typeface typeface = new Typeface(("Arial"));
                     double ch = xau.Baseline * _listText[i].offset * 2 / 100.0;

                     Point origin = new Point(_listText[i].x , _listText[i].y - xau.Baseline * _listText[i].offset * 2 / 100.0);
                     GlyphRun run = CreateGlyphRun(typeface, _listText[i].text, _listText[i].size, origin);
                     drawingContext.DrawGlyphRun(Brushes.Black, run);
                   
                 }*/
            if (_canLe == 2)//Can le phai
            {
                double _toaDoY = 0;
                _toaDoY = _listText[0].y;
                int j = 0;
                double _viTriCuoi = 0;
                for (int i = 0; i < _listText.Count; i++)
                {
                    if ((_listText[i].y > _toaDoY) || (i == _listText.Count - 1))
                    {
                        //Gan lai toa do y
                        _toaDoY = _listText[i].y;
                        for (int t = j; t <= i - 1; t++)
                        {
                            var xau = new FormattedText(_listText[t].text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), _listText[t].size, Brushes.Black);
                            Typeface typeface = new Typeface(("Arial"));
                            Point origin = new Point(_listText[t].x + widthCheck - _viTriCuoi, _listText[t].y - xau.Baseline * _listText[t].offset * 2 / 100.0);
                            GlyphRun run = CreateGlyphRun(typeface, _listText[t].text, _listText[t].size, origin);
                            drawingContext.DrawGlyphRun(Brushes.Black, run);
                        }
                        j = i;
                    }
                    if ((_listText[i].text[0] != '_') && (_listText[i].y <= _toaDoY))
                    {
                        var xau = new FormattedText(_listText[i].text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), _listText[i].size, Brushes.Black);
                        _viTriCuoi = _listText[i].x + xau.WidthIncludingTrailingWhitespace;
                    }
                }
            }
            if (_canLe == 3)//Can giua
            {
                double _toaDoY = 0;
                _toaDoY = _listText[0].y;
                int j = 0;
                double _viTriCuoi = 0;
                for (int i = 0; i < _listText.Count; i++)
                {
                    if ((_listText[i].y > _toaDoY) || (i == _listText.Count - 1))
                    {
                        //Gan lai toa do y
                        _toaDoY = _listText[i].y;
                        for (int t = j; t <= i - 1; t++)
                        {
                            var xau = new FormattedText(_listText[t].text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), _listText[t].size, Brushes.Black);
                            Typeface typeface = new Typeface(("Arial"));
                            Point origin = new Point(_listText[t].x + (widthCheck - _viTriCuoi) / 2.0, _listText[t].y - xau.Baseline * _listText[t].offset * 2 / 100.0);
                            GlyphRun run = CreateGlyphRun(typeface, _listText[t].text, _listText[t].size, origin);
                            drawingContext.DrawGlyphRun(Brushes.Black, run);
                        }
                        j = i;
                    }
                    if ((_listText[i].text[0] != '_') && (_listText[i].y <= _toaDoY))
                    {
                        var xau = new FormattedText(_listText[i].text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), _listText[i].size, Brushes.Black);
                        _viTriCuoi = _listText[i].x + xau.WidthIncludingTrailingWhitespace;
                    }
                }
            }
            if (_canLe == 4)//Can deu
            {
                double _toaDoY = 0;
                _toaDoY = _listText[0].y;
                int j = 0;
                double _viTriCuoi = 0;
                double sumWidthKT = 0;
                for (int i = 0; i < _listText.Count; i++)
                {
                    if ((_listText[i].y > _toaDoY) || (i == _listText.Count - 1))
                    {
                        //Gan lai toa do y
                        _toaDoY = _listText[i].y;
                        if ((_listText[i - 1].text[0] == '_') && true)
                        {
                            var xau = new FormattedText(_listText[i - 1].text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), _listText[i - 1].size, Brushes.Black);
                            sumWidthKT -= xau.WidthIncludingTrailingWhitespace;
                        }
                        double xChange = 0;
                        for (int t = j; t <= i - 1; t++)
                        {
                            var xau = new FormattedText(_listText[t].text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), _listText[t].size, Brushes.Black);
                            Typeface typeface = new Typeface(("Arial"));
                            if (_listText[t].text[0] == '_' && (t != 0))
                            {
                                xChange += xau.WidthIncludingTrailingWhitespace * (widthCheck - _viTriCuoi) / sumWidthKT;
                            }

                            Point origin = new Point(_listText[t].x + xChange, _listText[t].y - xau.Baseline * _listText[t].offset * 2 / 100.0);

                            GlyphRun run = CreateGlyphRun(typeface, _listText[t].text, _listText[t].size, origin);
                            drawingContext.DrawGlyphRun(Brushes.Black, run);
                        }
                        j = i;
                        sumWidthKT = 0;
                    }
                    if ((_listText[i].text[0] != '_') && (_listText[i].y <= _toaDoY))
                    {
                        var xau = new FormattedText(_listText[i].text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), _listText[i].size, Brushes.Black);
                        _viTriCuoi = _listText[i].x + xau.WidthIncludingTrailingWhitespace;
                    }
                    if ((_listText[i].text[0] == '_') && i != 0)
                    {
                        var xau = new FormattedText(_listText[i].text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), _listText[i].size, Brushes.Black);
                        sumWidthKT += xau.WidthIncludingTrailingWhitespace;
                    }
                }
            }
        }


        //Tìm vị trí đầu tiên khác khoảng trắng của xâu s bắt đầu từ vị trí kt
        public int viTriDauTienKhacKT(string s, int kt)
        {
            for (int j = kt; j < s.Length; j++)
            {
                if (s[j] != '_') return j;
            }
            return 0;
        }

        public class TemplateFormattedText
        {
            public string Tu { get; set; }
            public string KhoangTrang { get; set; }

        }
        //Độ dài xâu
        public double widthXau(string xau, ItemFormatText _item)
        {
            double sum = 0;
            for (int i = 0; i < xau.Length; i++)
            {
                var charTu = new FormattedText(xau[i].ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                                                         new Typeface("Arial"), _item._fontSize, Brushes.Black);
                sum += charTu.WidthIncludingTrailingWhitespace;
            }
            return sum;
        }
        // Xử lý xuống hàng
        public string XuLyXuongHang(string xau, double widthXau, ItemFormatText _item)
        {
            string xauHC = "";
            double sumWidth = 0;
            for (int i = 0; i < xau.Length; i++)
            {
                var charTu = new FormattedText(xau[i].ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                                                     new Typeface("Arial"), _item._fontSize, Brushes.Black);
                sumWidth += charTu.WidthIncludingTrailingWhitespace;
                if (sumWidth > widthXau)
                {
                    sumWidth = charTu.WidthIncludingTrailingWhitespace;
                    xauHC += "\n";
                }
                xauHC += xau[i];
            }
            return xauHC;
        }
      
        public class ItemFormatText
        {
            public int viTriKetThuc { get; set; }
            public int _fontSize { get; set; }
            public string _fontType { get; set; }
            public double _offset { get; set; }
            public int _line { get; set; }


        }
        public class ItemText
        {
            public string _text { get; set; }
            public int _fontSize { get; set; }
            public string _fontType { get; set; }


        }
        //Xác định vị trí định dạng
        public int viTriDinhDang(List<ItemFormatText> _itemFMT, int _viTriCuoiXau)
        {
            int result = 0;
            for (int i = 0; i < _itemFMT.Count; i++)
            {
                if (_viTriCuoiXau <= _itemFMT[i].viTriKetThuc)
                    return i;
            }
            return result;
        }
        //Độ dài của từ có nhiều định dạng
        public double widthXau(List<ItemFormatText> _itemFMT, string tu, int _viTriDauXau, int _viTriCuoiXau)
        {
            double _widthSum = 0;
            int toaDo = 0;
            int i = 0;
            for (i = viTriDinhDang(_itemFMT, _viTriDauXau); i < viTriDinhDang(_itemFMT, _viTriCuoiXau); i++)
            {
                var xau = new FormattedText(tu.Substring(toaDo, _itemFMT[i].viTriKetThuc - _viTriDauXau - toaDo + 1), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), _itemFMT[i]._fontSize, Brushes.Black);
                toaDo = _itemFMT[i].viTriKetThuc - _viTriDauXau + 1;
                _widthSum += xau.WidthIncludingTrailingWhitespace;
            }
            if (tu.Length > 0)
            {
                var xau2 = new FormattedText(tu.Substring(toaDo, _viTriCuoiXau - toaDo - _viTriDauXau + 1), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), _itemFMT[viTriDinhDang(_itemFMT, _viTriCuoiXau)]._fontSize, Brushes.Black);
                _widthSum += xau2.WidthIncludingTrailingWhitespace;
            }

            return _widthSum;
        }
        //public double WidthXau(string _xau,ItemFormatText _item)
        //{

        //}
        public int viTriKhacKTCuoiList(string[] listCat)
        {
            int result = 0;
            for (int i = listCat.Length - 1; i >= 0; i--)
            {
                if (listCat[i] != "")
                {
                    return i;
                }
            }
            return result;
        }
        //chuyen doi chuoi thanh so
        public GlyphRun CreateGlyphRun(Typeface typeface, string text, double size, Point origin)
        {
            if (text.Length == 0)
                return null;
            GlyphTypeface glyphTypeface;

            typeface.TryGetGlyphTypeface(out glyphTypeface);

            var glyphIndexes = new ushort[text.Length];
            var advanceWidths = new double[text.Length];
            for (int n = 0; n < text.Length; n++)
            {
                if (text[n] == '\n')
                    continue;
                var glyphIndex = glyphTypeface.CharacterToGlyphMap[text[n]];
                glyphIndexes[n] = glyphIndex;
                advanceWidths[n] = glyphTypeface.AdvanceWidths[glyphIndex] * size;
            }
            var glyphRun = new GlyphRun(glyphTypeface, 0, false, size, glyphIndexes, origin, advanceWidths, null, null,
                                        null,null, null, null);
            return glyphRun;
        }

        public class FormatText
        {
            public string textToFormat { get; set; }
            public CultureInfo culture { get; set; }
            public FlowDirection flowDirection { get; set; }
            public Typeface typeface { get; set; }
            public double emSize { get; set; }
            public Brush foreground { get; set; }
            public double offset { get; set; }
            public int line { get; set; }

            public FormatText(string textToFormat, CultureInfo culture, FlowDirection flowDirection, Typeface typeface, double emSize, Brush foreground, double offset, int line)
            {
                this.textToFormat = textToFormat;
                this.culture = culture;
                this.flowDirection = flowDirection;
                this.typeface = typeface;
                this.emSize = emSize;
                this.foreground = foreground;
                this.offset = offset;
                this.line = line;

            }
        }
        public class ListText
        {
            public string text { get; set; }
            public double size { get; set; }
            public Typeface typeface { get; set; }
            public Brush foreground { get; set; }
            public double x { get; set; }
            public double y { get; set; }
            public double maxBaseline { get; set; }
            public double offset { get; set; }
            public int line { get; set; }

        }

    }
}

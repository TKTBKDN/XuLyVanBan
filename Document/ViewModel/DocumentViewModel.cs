using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Document.ViewModel
{
    class DocumentViewModel
    {

    /*    private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontFamily.SelectedItem != null)
                rtbEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
        }*/

        private string _selectionRtb;

	    public string SelectionRtb
	    {
		    get { return _selectionRtb;}
		    set { _selectionRtb = value;}
	    }

        private ObservableCollection<FontFamily> _systemFonts = new ObservableCollection<FontFamily>();
        public ObservableCollection<FontFamily> SystemFonts
        {
            get { return _systemFonts; }
        }
        public void LoadSystemFonts()
        {
            _systemFonts.Clear();
            var fonts = Fonts.SystemFontFamilies.OrderBy(f => f.ToString());
            foreach (var f in fonts)
                _systemFonts.Add(f);
        }
        /*
        public ObservableCollection<string> MyItemsFontFamily { get; set; }

        private string _mySelectedItemFontFamily;
        public string MySelectedItemFontFamily
        {
            get { return _mySelectedItemFontFamily; }
            set
            {
                /*if (_mySelectedItemFontFamily != null)
                    rtbEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, _mySelectedItemFontFamily);
                // Some logic here
                _mySelectedItemFontFamily = value;
                _mySelectedItemFontFamily=Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            }
        }*/
    }

}

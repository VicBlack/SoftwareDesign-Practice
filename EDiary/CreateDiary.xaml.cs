using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EDiary
{
    /// <summary>
    /// CreateDiary.xaml 的交互逻辑
    /// </summary>
    public partial class CreateDiary : Window
    {
        public CreateDiary()
        {
            InitializeComponent();
        }
        //声明委托 发布者
        public delegate void CreateDiaryEventHandler(object sender, CreateDiaryEventArg e);
        //定义事件
        public event CreateDiaryEventHandler CreateDiaryhdler;

        //定义事件参数，须继承EventArgs
        public class CreateDiaryEventArg : EventArgs
        {
            private string _Title;
            public string Title
            {
                get { return _Title; }
            }

            private string _Content;
            public string Content
            {
                get { return _Content; }
            }
            public CreateDiaryEventArg(string newtitle, string newcontent)
            {
                _Title = newtitle;
                _Content = newcontent;
            }
        }
        //引发事件
        public void RaiseEvent(string newname, string newremarks)
        {
            CreateDiaryEventArg e = new CreateDiaryEventArg(newname, newremarks);
            if (CreateDiaryhdler != null)
            {
                CreateDiaryhdler(this, e);
            }
        }

        private void OnSubmitCreateDiary(object sender, RoutedEventArgs e)
        {
            RaiseEvent(tbtitle.Text.Trim(), tbcontent.Text.Trim());
            this.Close();
        }
    }
}

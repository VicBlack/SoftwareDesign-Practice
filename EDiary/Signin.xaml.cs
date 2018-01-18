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
    /// Signin.xaml 的交互逻辑
    /// </summary>
    public partial class Signin : Window
    {
        public Signin()
        {
            InitializeComponent();
        }

        //声明委托 发布者
        public delegate void SigninEventHandler(object sender, SigninEventArg e);
        //定义事件
        public event SigninEventHandler Signinhdler;

        //定义事件参数，须继承EventArgs
        public class SigninEventArg : EventArgs
        {
            private int _ID;
            public int ID
            {
                get { return _ID; }
            }
            private string _PWD;
            public string PWD
            {
                get { return _PWD; }
            }
            
            public SigninEventArg(int newid, string newpwd)
            {
                _ID = newid;
                _PWD = newpwd;                
            }
        }
        //引发事件
        public void RaiseEvent(int newid, string newpwd)
        {
            SigninEventArg e = new SigninEventArg(newid, newpwd);
            if (Signinhdler != null)
            {
                Signinhdler(this, e);
            }
        }

        private void OnSubmitSignin(object sender, RoutedEventArgs e)
        {
            int newid=0;
            if (int.TryParse(tbid.Text.Trim(),out newid)==false)
            {
                MessageBox.Show("帐号必须为数字!");
                return;
            }
            if (string.IsNullOrEmpty(tbpwd.Password.Trim()))
            {
                MessageBox.Show("密码不能为空!");
                return;
            }
            RaiseEvent(Int32.Parse(tbid.Text), tbpwd.Password.Trim());
            this.Close();
        }
    }
}

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
    /// Signup.xaml 的交互逻辑
    /// </summary>
    public partial class Signup : Window
    {
        public Signup()
        {
            InitializeComponent();
        }

        //声明委托 发布者
        public delegate void SignupEventHandler(object sender, SignupEventArg e);
        //定义事件
        public event SignupEventHandler Signuphdler;

        //定义事件参数，须继承EventArgs
        public class SignupEventArg : EventArgs
        {
            private string _Name;
            public string Name
            {
                get { return _Name; }
            }
            private string _PWD;
            public string PWD
            {
                get { return _PWD; }
            }
            private string _Remarks;
            public string Remarks
            {
                get { return _Remarks; }
            }
            public SignupEventArg(string newname, string newpwd, string newremarks)
            {
                _Name = newname;
                _PWD = newpwd;
                _Remarks = newremarks;
            }
        }
        //引发事件
        public void RaiseEvent(string newname, string newpwd, string newremarks)
        {
            SignupEventArg e = new SignupEventArg(newname, newpwd, newremarks);
            if (Signuphdler != null)
            {
                Signuphdler(this, e);
            }
        }

        private void OnSubmitSignup(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbname.Text.Trim()))
            {
                MessageBox.Show("姓名不能为空!");
                return;
            }
            if (string.IsNullOrEmpty(tbpwd.Password.Trim()))
            {
                MessageBox.Show("密码不能为空!");
                return;
            }
            RaiseEvent(tbname.Text.Trim(), tbpwd.Password.Trim(), tbremarks.Text.Trim());
            this.Close();
        }
    }
}

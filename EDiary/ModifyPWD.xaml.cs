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
    /// ModifyPWD.xaml 的交互逻辑
    /// </summary>
    public partial class ModifyPWD : Window
    {
        public ModifyPWD()
        {
            InitializeComponent();
        }

        //声明委托 发布者
        public delegate void ModifyPWDEventHandler(object sender, ModifyPWDEventArg e);
        //定义事件
        public event ModifyPWDEventHandler ModifyPWDhdler;

        //定义事件参数，须继承EventArgs
        public class ModifyPWDEventArg : EventArgs
        {
            private string _PWD;
            public string PWD
            {
                get { return _PWD; }
            }
            private string _NPWD;
            public string NPWD
            {
                get { return _NPWD; }
            }

            public ModifyPWDEventArg(string pwd, string npwd)
            {
                _PWD = pwd;
                _NPWD = npwd;
            }
        }
        //引发事件
        public void RaiseEvent(string pwd, string npwd)
        {
            ModifyPWDEventArg e = new ModifyPWDEventArg(pwd, npwd);
            if (ModifyPWDhdler != null)
            {
                ModifyPWDhdler(this, e);
            }
        }

        private void OnSubmitModifyPWD(object sender, RoutedEventArgs e)
        {
            
            if (string.IsNullOrEmpty(tbpwd.Password.Trim())|| string.IsNullOrEmpty(tbnpwd.Password.Trim())|| string.IsNullOrEmpty(tbvpwd.Password.Trim()))
            {
                MessageBox.Show("密码不能为空!");
                return;
            }
            if(!tbnpwd.Password.Trim().Equals(tbvpwd.Password.Trim()))
            {
                MessageBox.Show("两次输入的新密码不同!");
                return;
            }
            RaiseEvent(tbpwd.Password.Trim(), tbnpwd.Password.Trim());
            this.Close();
        }
    }
}

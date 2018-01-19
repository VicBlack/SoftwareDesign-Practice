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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EDiary
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();            
            _Model = new MainModel();
            this.DataContext = _Model;            
        }
        private MainModel _Model;

        private void OnSignup_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _Model != null && _Model.Currentuser==null;
        }

        private void OnSignup_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Signup signup = new Signup();
            signup.Signuphdler += new Signup.SignupEventHandler(Signup);
            signup.ShowDialog();            
        }

        public void Signup(object sender, Signup.SignupEventArg e)
        {
            _Model.Signup(e.Name, e.PWD, e.Remarks);
            MessageBox.Show($"您的帐号为{_Model.Currentuser.ID}，请登录时使用帐号登录");
        }

        private void OnMainWindowClosed(object sender, EventArgs e)
        {
            _Model.Save();
            _Model.Submit();
        }

        private void OnSignin_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _Model != null && _Model.Currentuser == null;
        }

        private void OnSignin_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Signin signin = new Signin();
            signin.Signinhdler += new Signin.SigninEventHandler(Signin);
            signin.ShowDialog();
        }

        public void Signin(object sender, Signin.SigninEventArg e)
        {
            _Model.Signin(e.ID, e.PWD);
        }

        private void OnSignout_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _Model != null && _Model.Currentuser != null;
        }

        private void OnSignout_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("是否保存所有操作", "注销", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (dr == MessageBoxResult.Yes)
            {
                _Model.Signout(true);
            }
            if (dr == MessageBoxResult.No)
            {
                _Model.Signout(false);
            }
        }

        private void OnModifyPWD_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _Model != null && _Model.Currentuser != null;
        }

        private void OnModifyPWD_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ModifyPWD modifyPWD = new ModifyPWD();
            modifyPWD.ModifyPWDhdler += new ModifyPWD.ModifyPWDEventHandler(ModifyPWD);
            modifyPWD.ShowDialog();
        }

        public void ModifyPWD(object sender, ModifyPWD.ModifyPWDEventArg e)
        {
            _Model.ModifyPWD(e.PWD, e.NPWD);
        }

        private void OnGeneralInfo_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _Model != null && _Model.Currentuser != null;
        }

        private void OnGeneralInfo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show($"帐号：{_Model.Currentuser.ID}\n姓名：{_Model.Currentuser.Name}\n日记数量：{_Model.getDiaryCount()}\n个性签名：{_Model.Currentuser.Remarks}");
        }

        private void OnCreateDiary_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _Model!=null && _Model.Currentuser != null;
        }

        private void OnCreateDiary_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CreateDiary createDiary = new CreateDiary();
            createDiary.CreateDiaryhdler += new CreateDiary.CreateDiaryEventHandler(CreateDiary);
            createDiary.ShowDialog();
        }

        public void CreateDiary(object sender, CreateDiary.CreateDiaryEventArg e)
        {
            _Model.CreateDiary(e.Title,e.Content);
        }

        private void OnSearch_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _Model != null && _Model.Currentuser != null && (!string.IsNullOrWhiteSpace(_Model.SearchTitle) || _Model.SearchYear!=0 );
        }

        private void OnSearch_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _Model.Search();
        }

        private void OnMatchRE_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute=_Model!=null && _Model.CanStartMatch(content.Text);
        }

        private void OnMatchRE_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<string> list = _Model.GetMatches(content.Text);
            foreach(string text in list)
            {
                List<TextRange> textRanges = FindWordFromPosition(rtbcontent.Document.ContentStart, text);
                foreach (var range in textRanges)
                {
                    range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Red));
                    //range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                    //range.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.PowderBlue));
                }
            }
            
        }

        private void OnReplaceRE_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {            
            e.CanExecute = _Model != null && _Model.CanStartReplace(content.Text);
        }

        private void OnReplaceRE_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            content.Text = _Model.Replace(content.Text);
        }

        List<TextRange> FindWordFromPosition(TextPointer position, string word)
        {
            List<TextRange> matchingText = new List<TextRange>();
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    //带有内容的文本
                    string textRun = position.GetTextInRun(LogicalDirection.Forward);
                    //查找关键字在这文本中的位置
                    int indexInRun = textRun.IndexOf(word);
                    int indexHistory = 0;
                    while (indexInRun >= 0)
                    {
                        TextPointer start = position.GetPositionAtOffset(indexInRun + indexHistory);
                        TextPointer end = start.GetPositionAtOffset(word.Length);
                        matchingText.Add(new TextRange(start, end));

                        indexHistory = indexHistory + indexInRun + word.Length;
                        textRun = textRun.Substring(indexInRun + word.Length);//去掉已经采集过的内容
                        indexInRun = textRun.IndexOf(word);//重新判断新的字符串是否还有关键字
                    }
                }
                position = position.GetNextContextPosition(LogicalDirection.Forward);
            }
            return matchingText;
        }
    }
}

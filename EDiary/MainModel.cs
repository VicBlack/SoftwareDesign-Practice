using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EDiary
{
    class MainModel: INotifyPropertyChanged
    {
        const string ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=EDiaryDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public MainModel()
        {
            DataContext = new EDiaryContentDataContext(ConnectionString);
            Status = "连接数据库中……";
            if (!DataContext.DatabaseExists())
            {
                DataContext.CreateDatabase();
                Status="数据库创建成功！";
            }
            else
            {
                Status="数据库已经存在！";                
            }
            Status = $"请登录或注册。";
            Diaries = new ObservableCollection<Diary>();
            AllDiaries = new ObservableCollection<Diary>();

        }

        #region 成员
        public string Status { get { return _Status; } set { if (_Status == value) return; _Status = value; OnPropertyChanged(nameof(Status)); } }
        private string _Status;

        public User Currentuser { get { return _Currentuser; } set { if (_Currentuser == value) return; _Currentuser = value; OnPropertyChanged(nameof(Currentuser)); } }
        private User _Currentuser=null;

        public bool DiaryVisible { get { return _DiaryVisible; } set { if (_DiaryVisible == value) return; _DiaryVisible = value; OnPropertyChanged(nameof(DiaryVisible)); } }
        private bool _DiaryVisible;

        public int SearchYear { get { return _SearchYear; } set { if (_SearchYear == value) return; _SearchYear = value; OnPropertyChanged(nameof(SearchYear)); } }
        private int _SearchYear;
        
        public int SearchMonth { get { return _SearchMonth; } set { if (_SearchMonth == value) return; _SearchMonth = value; OnPropertyChanged(nameof(SearchMonth)); } }
        private int _SearchMonth;

        public int SearchDay { get { return _SearchDay; } set { if (_SearchDay == value) return; _SearchDay = value; OnPropertyChanged(nameof(SearchDay)); } }
        private int _SearchDay;

        public string SearchTitle { get { return _SearchTitle; } set { if (_SearchTitle == value) return; _SearchTitle = value; OnPropertyChanged(nameof(SearchTitle)); } }
        private string _SearchTitle;

        public string Pattern { get { return _Pattern; } set { if (_Pattern == value) return; _Pattern = value; OnPropertyChanged(nameof(Pattern)); } }
        private string _Pattern;

        public string ReplacePattern { get { return _ReplacePattern; } set { if (_ReplacePattern == value) return; _ReplacePattern = value; OnPropertyChanged(nameof(ReplacePattern)); } }
        private string _ReplacePattern;

        public ObservableCollection<Diary> Diaries { get { return _Diaries; } set { if (_Diaries == value) return; _Diaries = value; OnPropertyChanged(nameof(Diaries)); } }
        private ObservableCollection<Diary> _Diaries;

        public ObservableCollection<Diary> AllDiaries { get { return _AllDiaries; } set { if (_AllDiaries == value) return; _AllDiaries = value; OnPropertyChanged(nameof(AllDiaries)); } }
        private ObservableCollection<Diary> _AllDiaries;
        //public Table<Diary> Diaries
        //{
        //    get{
        //        return DataContext.Diary;
        //    }
        //}

        public EDiaryContentDataContext DataContext { get; }
        #endregion

        public void Submit()
        {
            DataContext.SubmitChanges();
        }

        #region 用户管理
        public void Signup(string newname,string newpwd,string newremarks)
        {
            User newuser = new User { Name = newname, PWD = newpwd, Remarks = newremarks };
            DataContext.User.InsertOnSubmit(newuser);
            Submit();
            Currentuser = DataContext.ExecuteQuery<User>("Select top 1 * from [dbo].[User] order by ID desc").First();
            Status = $"您的帐号为{Currentuser.ID}，请登录时使用帐号登录";
            InitializeDiary();
        }

        public bool Signin(int id,string pwd)
        {
            var sign= from r in DataContext.User where r.ID == id && r.PWD.Equals(pwd) select r;
            if (sign.Count() == 0)
            {
                Status = $"登录失败，请重试！";
                return false;
            }
            Currentuser = sign.First();
            InitializeDiary();
            Status = $"欢迎登录，用户{Currentuser.Name}";
            return true;
        }

        public void InitializeDiary()
        {
            AllDiaries.Clear();
            Diaries.Clear();
            var QDiaries = from r in DataContext.Diary where r.UserID==Currentuser.ID select r;
            foreach(var QD in QDiaries)
            {
                AllDiaries.Add(QD);
                Diaries.Add(QD);
            }
        }
        
        public void Save()
        {
            foreach(Diary d in Diaries)
            {
                foreach(Diary a in AllDiaries)
                {
                    if (d.WritenDT.Equals(a.WritenDT))
                    {
                        AllDiaries.Remove(a);
                        AllDiaries.Add(d);
                        break;
                    }
                }
            }
            var QDiaries = from r in DataContext.Diary where r.UserID == Currentuser.ID select r;
            foreach (var QD in QDiaries)
            {
                DataContext.Diary.DeleteOnSubmit(QD);
            }
            foreach (Diary d in AllDiaries)
            {
                DataContext.Diary.InsertOnSubmit(d);
            }
            Submit();
        }

        public void Signout(bool needsave)
        {
            Status = $"感谢使用！";
            if (needsave)
            {
                Save();
            }
            Currentuser = null;
            Diaries.Clear();
            AllDiaries.Clear();
        }

        public void ModifyPWD(string pwd,string npwd)
        {
            if(!Currentuser.PWD.Equals(pwd))
            {
                Status = "修改密码失败，原密码错误！";
            }
            Currentuser.PWD = npwd;
            Submit();
            Status = "修改密码成功！";
        }

        public int getDiaryCount()
        {
            var QDiaries = from r in DataContext.Diary where r.UserID == Currentuser.ID select r;
            return QDiaries.Count();
        }

        public void CreateDiary(string ntitle,string ncontent)
        {
            if (string.IsNullOrWhiteSpace(ntitle))
                ntitle = "Untitled";
            Diary diary = new Diary {
                UserID = Currentuser.ID, 
                Title = ntitle,
                Content = ncontent,
                WritenDT = DateTime.Now 
            };
            Diaries.Add(diary);
            AllDiaries.Add(diary);
            Status = "创建日记成功！";            
        }

        public void Search()
        {
            Diaries.Clear();
            var NQDiaries = from r in DataContext.Diary where r.UserID == Currentuser.ID select r;
            foreach (var QD in NQDiaries)
            {
                if (!string.IsNullOrWhiteSpace(SearchTitle))
                    if (!QD.Title.Contains(SearchTitle)) continue;
                if (SearchYear != 0)
                {
                    if (QD.WritenDT.Year != SearchYear) continue;
                    if (SearchMonth != 0)
                    {   if (QD.WritenDT.Month != SearchMonth) continue;
                        if (SearchDay != 0)
                            if (QD.WritenDT.Day != SearchDay) continue;
                    }
                }
                Diaries.Add(QD);
            }
            Status = $"查询中！";
        }

        public bool CanStartMatch(string TargetText)
        {
            return !string.IsNullOrWhiteSpace(Pattern) && !string.IsNullOrWhiteSpace(TargetText);
        }

        public bool CanStartReplace(string TargetText)
        {
            return !string.IsNullOrWhiteSpace(Pattern) && !string.IsNullOrWhiteSpace(ReplacePattern) && !string.IsNullOrWhiteSpace(TargetText);
        }

        public List<string> GetMatches(string TargetText)
        {
            Regex aRegex = new Regex(Pattern);
            MatchCollection aMatches = aRegex.Matches(TargetText);
            List<string> list = new List<string>();
            foreach (Match aMatch in aMatches)
            {
                list.Add(aMatch.Value);
            }
            Status = $"匹配中！";
            return list;            
        }

        public string Replace(string TargetText)
        {
            Regex aRegex = new Regex(Pattern);
            Status = $"替换中！";
            return aRegex.Replace(TargetText, ReplacePattern);            
        }

        #endregion
        #region INotifyPropertyChanged接口便利实现
        private void OnPropertyChanged(string aPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(aPropertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}

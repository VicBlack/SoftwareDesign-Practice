using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
        }

        #region 成员
        public string Status { get { return _Status; } set { if (_Status == value) return; _Status = value; OnPropertyChanged(nameof(Status)); } }
        private string _Status;

        public EDiaryContentDataContext DataContext { get; }
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

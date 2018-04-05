using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Web.Configuration;
using Dapper;

namespace WebApplication_Sample.DbAccess
{
    public class DbAccessor : IDisposable
    {
        private OdbcConnection _ocn = null;
        private OdbcTransaction _otn = null;

        public DbAccessor()
        {
            _ocn = new OdbcConnection(WebConfigurationManager.ConnectionStrings["OdbcConnectionString"].ConnectionString);
            Open();
        }

        public void Open()
        {
            if (_ocn.State != System.Data.ConnectionState.Open) _ocn.Open();
        }

        public void Close() => _ocn.Close();

        public void BeginTransaction() => _otn = _ocn.BeginTransaction();

        public bool IsBeginTrans() => (_otn != null) ? true : false;

        public void Commit()
        {
            if (_otn != null) _otn.Commit();
            _otn = null;
        }

        public void Rollback()
        {
            if (_otn != null) _otn.Rollback();
            _otn = null;
        }

        public IEnumerable<T> ExecuteQuery<T>(string sql)
        {
            return _ocn.Query<T>(sql);
        }

        public IEnumerable<T> ExecuteQuery<T>(string sql, object param = null) => _ocn.Query<T>(sql, param, _otn);

        public IEnumerable<dynamic> ExecuteQuery(string sql, object param = null) => _ocn.Query(sql, param, _otn);

        public object ExecuteScalar(string sql, object param = null) => _ocn.ExecuteScalar(sql, param);

        public int ExecuteNonQuery(string sql, object param = null)
        {
            if (IsBeginTrans() == false) BeginTransaction();
            return _ocn.Execute(sql, param, _otn);
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                    if(_otn != null)
                    {
                        _otn.Rollback();
                        _otn = null;
                    }
                    if(_ocn != null)
                    {
                        _ocn.Close();
                        _ocn.Dispose();
                        _ocn = null;
                    }
                }

                // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~DbAccessor() {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        public void Dispose() => Dispose(true); // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。// GC.SuppressFinalize(this);
        #endregion
    }
}
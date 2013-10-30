using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using Iesi.Collections.Generic;
using DigitizingDataBizLayer.Helpers;

namespace DigitizingDataBizLayer.Repositories
{
    public abstract class RepositoryBase<T>
    {
        //The Proxy to connect to the Service Layer
        private ISession sessionProxy = null;

        public RepositoryBase()
        {
            sessionProxy = AppGlobals.SessionProxy;
        }

        protected ISession SessionProxy
        {
            get
            {
                //If the session is NULL or Not Open, the Recreate the session
                if (sessionProxy == null || !sessionProxy.IsOpen)
                {
                    sessionProxy = sessionProxy = AppGlobals.SessionProxy;
                }
                return sessionProxy;
            }
            set
            {
                sessionProxy = AppGlobals.SessionProxy;
            }
        }

        //TODO: I think these Repository methods should be static
        public virtual IList<T> FindAll()
        {
            try
            {
                //ISession session = DigitizingDataDomain.Helpers.NHibernateHelper.OpenSessionForDdl();
                //session = null;
                
                var query =
                    from u in SessionProxy.Query<T>()
                    select u;

                return query.ToList();
                
            }
            catch (Exception ex)
            {
                DiscardCurrentSession();
                string errorMessage = BuildErrorMessage(ex);                
                AppGlobals.LogToFileServer(AppGlobals.LOG_FOLDER_SERVER, errorMessage + Environment.NewLine + ex.StackTrace);
                
                return new List<T>();
            }
        }

        //TODO: I think these Repository methods should be static
        public virtual IQueryable<T> FindAll(bool returnsIQueryable)
        {
            try
            {
                var query =
                    from u in SessionProxy.Query<T>()
                    select u;

                return query;

            }
            catch (Exception ex)
            {
                DiscardCurrentSession();
                string errorMessage = BuildErrorMessage(ex);
                AppGlobals.LogToFileServer(AppGlobals.LOG_FOLDER_SERVER, errorMessage + Environment.NewLine + ex.StackTrace);
                return null;
            }
        }

        //Validate Uniqueness of the object, will be overriden by the sub-class
        public virtual bool IsUnique(T theEntity, ref string failureMessage)
        {
            return true;
        }

        public virtual bool Insert(T theEntity)
        {
            using (var transaction = SessionProxy.BeginTransaction())
            {
                try
                {
                    SessionProxy.Save(theEntity);
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    DiscardCurrentSession();
                    string errorMessage = BuildErrorMessage(ex);
                    AppGlobals.LogToFileServer(AppGlobals.LOG_FOLDER_SERVER, errorMessage + Environment.NewLine + ex.StackTrace);
                    return false;
                }                
            }
               
        }

        public virtual bool Update(T theEntity)
        {
            using (var transaction = sessionProxy.BeginTransaction())
            {
                try
                {
                    sessionProxy.Update(theEntity);
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    DiscardCurrentSession();
                    string errorMessage = BuildErrorMessage(ex);
                    AppGlobals.LogToFileServer(AppGlobals.LOG_FOLDER_SERVER, errorMessage + Environment.NewLine + ex.StackTrace);
                    return false;
                }
            }
            
        }

        public virtual bool Delete(T theEntity)
        {
            using (var transaction = sessionProxy.BeginTransaction())
            {
                try
                {
                    sessionProxy.Delete(theEntity);
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    DiscardCurrentSession();
                    string errorMessage = BuildErrorMessage(ex);
                    AppGlobals.LogToFileServer(AppGlobals.LOG_FOLDER_SERVER, errorMessage + Environment.NewLine + ex.StackTrace);
                    return false;
                }
            }            
        }

        protected virtual void DiscardCurrentSession()
        {
            if (sessionProxy != null)
            {
                //Try flushing the session
                try
                {
                    sessionProxy.Flush();
                }
                catch (Exception ex)
                {
                    string errorMessage = BuildErrorMessage(ex);
                    AppGlobals.LogToFileServer(AppGlobals.LOG_FOLDER_SERVER, errorMessage + Environment.NewLine + ex.StackTrace);
                }
                finally
                {
                    sessionProxy.Close();
                    sessionProxy = null;
                }   
            }
        }

        protected string BuildErrorMessage(Exception exception)
        {
            string errorMessage = string.Empty;
            try
            {
                if (exception == null)
                {
                    return errorMessage;                    
                }

                errorMessage = exception.Message;
                Exception innerException = exception.InnerException;
                
                while (innerException != null)
                {
                    errorMessage += (Environment.NewLine + innerException.Message);

                    //do some recursion
                    innerException = innerException.InnerException;
                }

                return errorMessage;
            }
            catch
            {
                return errorMessage;
            }
        }
    }
}

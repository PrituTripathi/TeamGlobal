using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using TeamGlobal.Infrastructure.BaseRepository;

namespace TeamGlobal.Infrastructure
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IDictionary<Type, object> repositories;
        private TeamGlobalContext context;
        private bool disposed = false;

        public UnitOfWork()
        {
            this.context = new TeamGlobalContext();
            repositories = new Dictionary<Type, object>();
        }

        public IRepository<T> Repository<T>() where T : class
        {
            if (repositories.Keys.Contains(typeof(T)) == true)
            {
                return repositories[typeof(T)] as Repository<T>;
            }
            IRepository<T> repo = new Repository<T>(context);
            repositories.Add(typeof(T), repo);
            return repo;
        }

        public bool Commit()
        {
            using (var dbContextTransaction = context.Database.BeginTransaction())
            {
                try
                {
                    int affectedRows = context.SaveChanges();
                    dbContextTransaction.Commit();
                    return affectedRows > 0;
                }
                catch (DbEntityValidationException ex)
                {
                    // Retrieve the error messages as a list of strings.
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);

                    // Join the list to a single string.
                    var fullErrorMessage = string.Join("; ", errorMessages);

                    // Combine the original exception message with the new one.
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                    // Throw a new DbEntityValidationException with the improved exception message.
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }
                catch (Exception exception)
                {
                    dbContextTransaction.Rollback();
                    log.Debug(exception.Message);
                    log.Debug(exception.StackTrace);
                    throw exception;
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
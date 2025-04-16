using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public abstract class Manager<T> where T : class, IEntityWithId
    {

        protected readonly AppDbContext context;
        protected Manager(AppDbContext context) => this.context = context;

        public virtual void Add(T t)
        {
            context.Set<T>().Add(t);
            context.SaveChanges();
        }

        public virtual void RemoveById(int id)
        {
            var toRmv = context.Set<T>()
                .FirstOrDefault(o => o.Id == id);
            if (toRmv != null)
            {
                context.Set<T>().Remove(toRmv);
                context.SaveChanges();
            }
        }

        public virtual void Modify(T t)
        {
            context.Set<T>().Attach(t);
            context.Entry(t).State = EntityState.Modified;
            context.SaveChanges();
        }

        public virtual T? FindById(int id)
        {
            return context.Set<T>().SingleOrDefault(o => o.Id == id);
        }

        public virtual List<T> Query(Func<T, bool> predicate)
        {
            return [.. context.Set<T>().Where(predicate)];
        }
    }
}

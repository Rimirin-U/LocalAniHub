using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class EpisodeManager
    {
        public void Add(Episode episode)
        {
            using (var context = new AppDbContext())
            {
                context.Episodes.Add(episode);
                context.SaveChanges();
            }
        }

        public Episode? FindById(int id)
        {
            using (var context = new AppDbContext())
            {
                return context.Episodes
                    .SingleOrDefault(ep => ep.Id == id);
            }
        }

        public List<Episode> FindByEntryId(int entryId)
        {
            using (var context = new AppDbContext())
            {
                return context.Episodes
                    .Where(ep => ep.EntryId == entryId)
                    .OrderBy(ep => ep.Id)
                    .ToList();
            }
        }

        // not found ? do nothing
        public void RemoveById(int id)
        {
            using (var context = new AppDbContext())
            {
                var toRmv = context.Episodes
                    .FirstOrDefault(ep => ep.Id == id);
                if(toRmv != null)
                {
                    context.Episodes.Remove(toRmv);
                    context.SaveChanges();
                }
            }
        }

        public void Modify(Episode episode)
        {
            using (var context = new AppDbContext())
            {
                context.Episodes.Attach(episode);
                context.Entry(episode).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}

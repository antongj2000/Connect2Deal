using Connect2Deal.Data;
using Connect2Deal.Models;
using Microsoft.EntityFrameworkCore;

namespace Connect2Deal.Services
{
    public class ListingService
    {

        private readonly AppDbContext mycontext;

        public ListingService(AppDbContext _mycontext)
        {
            mycontext = _mycontext;
        }

        public async Task<List<category>> ParentCategoryFetch ()
        {
            var parentCat = await mycontext.categories.Where(u => u.parent_id == null).OrderBy(c => c.name).ToListAsync();
            return parentCat;
        }

        public async Task<List<category>> ChildCategoryFetch (int parentId)
        {
            var childCat = await mycontext.categories.Where(u =>u.parent_id == parentId).OrderBy(c => c.name).ToListAsync();
            return childCat;
        }

        //public async Task<bool> IsCategoryValid (int parentId, int childId)
        //{
        //    var parentCat = ParentCategoryFetch().Where(u => u.parentId ==  parentId);

        //    if ()
        //    {

        //    }
        }



    }
}

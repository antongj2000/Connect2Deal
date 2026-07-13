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

        public async Task<List<Category>> ParentCategoryFetch()
        {
            return await mycontext.Categories
                .Where(c => c.ParentId == null)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<List<Category>> ChildCategoryFetch(int parentId)
        {
            return await mycontext.Categories
                .Where(c => c.ParentId == parentId)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<bool> IsCategoryValid(int parentId, int childId)
        {
            return await mycontext.Categories
                .AnyAsync(c => c.Id == childId && c.ParentId == parentId);
        }





    }



    }


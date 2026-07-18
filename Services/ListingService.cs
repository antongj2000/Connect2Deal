using Connect2Deal.Data;
using Connect2Deal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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


        #region Location

        public async Task<List<Location>> CountryFetch()
        {
            return await mycontext.Locations
                .Where(c => c.ParentId == null)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<List<Location>> CityFetch(int parentId)
        {
            return await mycontext.Locations
                .Where(c => c.ParentId == parentId)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<bool> IsLocationValid(int parentId, int childId)
        {
            return await mycontext.Locations
                .AnyAsync(c => c.Id == childId && c.ParentId == parentId);
        }


        public async Task<Listing> CreateListing(int userId, int categoryId, int locationId,
                                         string title, string description, decimal price)
        {
            var newListing = new Listing
            {
                UserId = userId,
                CategoryId = categoryId,
                LocationId = locationId,
                Title = title,
                Description = description,
                Price = price
            };

            mycontext.Listings.Add(newListing);
            await mycontext.SaveChangesAsync();
            return newListing;
        }


        #endregion






        #region Listing feed for users

        public async Task<List<Listing>> GetAllListings ()
        {
            return await mycontext.Listings.Where(u => u.Status == "Active").
                Include(l => l.Location).Include(c => c.Category).Include(u => u.User).OrderByDescending(u => u.CreatedAt).ToListAsync();
        }


        #endregion


        #region Listing details
        public async Task<Listing?> GetListingById(int id)
        {
            return await mycontext.Listings
                .Include(l => l.Category)
                .Include(l => l.Location)
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task SaveListingImages(int listingId, List<IFormFile> images, string webRootPath)
        {
            var folder = Path.Combine(webRootPath, "uploads", "listings");

            if (!Directory.Exists(folder)) 
            {
                Directory.CreateDirectory(folder);
            }

            bool isFirst = true;

            foreach (var file in images)
            {
                if (file.Length == 0) continue;

                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var diskPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(diskPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var image = new ListingImage
                {
                    ListingId = listingId,
                    ImagePath = "/uploads/listings/" + fileName,
                    IsPrimary = isFirst
                };

                mycontext.ListingImages.Add(image);
                isFirst = false;
            }

            await mycontext.SaveChangesAsync();
        }





        #endregion


    }



}


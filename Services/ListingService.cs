using Connect2Deal.Constants;
using Connect2Deal.Data;
using Connect2Deal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace Connect2Deal.Services
{
    public class ListingService
    {

        private readonly AppDbContext mycontext;

        public ListingService(AppDbContext _mycontext)
        {
            mycontext = _mycontext;
        }

        #region Category

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

        #endregion

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

        #endregion

        #region AddListing

        public async Task<Listing> CreateListing(int userId, int categoryId, int locationId,
                                         string title, string description, decimal price,
                                         List<IFormFile> images, string rootPath)
        {
            var newListing = new Listing
            {
                UserId = userId,
                CategoryId = categoryId,
                LocationId = locationId,
                Title = title,
                Description = description,
                Price = price,
            };

            mycontext.Listings.Add(newListing);
            await mycontext.SaveChangesAsync();

            await SaveListingImages(newListing.Id, images, rootPath);

            return newListing;
        }


        #endregion

        #region Edit Listing

        public async Task<bool> UpdateListing(int listingId, int userId, int categoryId, int locationId,
                                  string title, string description, decimal price)
        {
            var listing = await mycontext.Listings.FindAsync(listingId);

            if (listing == null || listing.UserId != userId)
            {
                return false;
            }

            listing.CategoryId = categoryId;
            listing.LocationId = locationId;
            listing.Title = title;
            listing.Description = description;
            listing.Price = price;
            listing.UpdatedAt = DateTime.UtcNow;

            await mycontext.SaveChangesAsync();
            return true;
        }


        #endregion




        #region Listing feed for users

        public async Task<List<Listing>> GetAllListings ()
        {
            return await mycontext.Listings.Where(u => u.Status == "Active" ).
                Include(l => l.Location).
                Include(c => c.Category).
                Include(u => u.User).
                Include(i => i.ListingImages).
                OrderByDescending(u => u.CreatedAt).ToListAsync();
        }


        #endregion


        #region Listing details
        public async Task<Listing?> GetListingById(int id)  
        {
            return await mycontext.Listings
                .Include(l => l.Category)
                .Include(l => l.Location)
                .Include(l => l.User)
                .Include(i => i.ListingImages)
                .FirstOrDefaultAsync(l => l.Id == id);
        }


        public async Task SaveListingImages(int idListing, List<IFormFile> images,string rootPath )
        {
            var pathFolder = Path.Combine(rootPath, "uploads", "listings", idListing.ToString());

            if (!Directory.Exists(pathFolder))
            {
                Directory.CreateDirectory(pathFolder);  
            }

            bool firstPicture = true;
              
            foreach (var file in images)
            {
                if (file.Length == 0)
                {
                    continue;
                }

                var extension = Path.GetExtension(file.FileName);
                var fileName = Guid.NewGuid() + extension;
                var fullPath = Path.Combine(pathFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var image = new ListingImage()
                {
                    ListingId = idListing,
                    ImagePath = $"/uploads/listings/{idListing}/{fileName}",
                    IsPrimary = firstPicture
                };

                mycontext.ListingImages.Add(image);
                firstPicture = false;

            }
            await mycontext.SaveChangesAsync();

        }




        #endregion


        #region Favorites

        public async Task<bool> FavoritesExists (int userId, int listingId)
        {
            var listing = await mycontext.Favorites.FirstOrDefaultAsync(x => x.UserId == userId && x.ListingId == listingId);

            if (listing == null) 
            {
                return false;
            }
            return true;
        }


        public async Task RemoveFavorite (int userId, int listingId)
        {
            var listing = await mycontext.Favorites.FirstOrDefaultAsync(x => x.UserId == userId && x.ListingId == listingId);

            if (listing == null)
            {
                return; 
            }

            mycontext.Favorites.Remove(listing);
            await mycontext.SaveChangesAsync();
        }



        public async Task<Favorite> CreateFavorite(int userId, int listingId)
        {
            var newFavorite = new Favorite
            {
                UserId = userId,
                ListingId = listingId
            };

            mycontext.Favorites.Add(newFavorite);
            await mycontext.SaveChangesAsync();


            return newFavorite;
        }


        public async Task<List<Listing>> GetUserFavorites (int userId)
        {
            var listings = await mycontext.Listings
                .Include(x => x.Category)
                .Include(x => x.Location)
                .Include(x => x.ListingImages)
                .Include(x => x.Favorites)
                .Where(x => x.Favorites.Any(f => f.UserId == userId)).ToListAsync();

            return listings;
        }

        public async Task<List<int>> GetUserFavoriteIds(int userId)
        {
            return await mycontext.Favorites
                .Where(x => x.UserId == userId)
                .Select(x => x.ListingId)
                .ToListAsync();
        }


        #endregion





        #region MyListings
        public async Task<List<Listing>> GetMyListings(int userId)
        {
            return await mycontext.Listings.Where(l => l.UserId == userId).
                Include(l => l.Location).
                Include(c => c.Category).
                Include(u => u.User).
                Include(i => i.ListingImages).
                OrderByDescending(u => u.CreatedAt).ToListAsync();
        }




        #endregion



        public async Task CloseTransaction(int listingId, int buyerId, int sellerId)
        {
            var listing = await mycontext.Listings.FindAsync(listingId);
            if (listing == null)
            {
                return;
            }

            bool isService = await IsServiceListing(listing.CategoryId);

            bool alreadyClosed = isService
                ? await mycontext.Transactions.AnyAsync(x => x.ListingId == listingId && x.BuyerId == buyerId)
                : await mycontext.Transactions.AnyAsync(x => x.ListingId == listingId);

            if (alreadyClosed)
            {
                return;
            }

            var transaction = new Transaction()
            {
                ListingId = listingId,
                SellerId = sellerId,
                BuyerId = buyerId
            };
            mycontext.Transactions.Add(transaction);

            if (!isService)
            {
                listing.Status = "Sold";
            }

            await mycontext.SaveChangesAsync();

            var message = isService
                ? $"Your booking for \"{listing.Title}\" has been confirmed. Please rate the provider to help build a trustworthy community."
                : $"Congratulations on your new \"{listing.Title}\"! We wish you the best using it. Please rate your seller to help build a trustworthy community.";

            var notification = new Notification()
            {
                UserId = buyerId,
                Type = NotificationTypes.RateSeller,
                Message = message,
                RelatedId = transaction.Id
            };

            mycontext.Notifications.Add(notification);
            await mycontext.SaveChangesAsync();
        }


        public async Task<List<int>> GetExistingBuyerIds(int listingId)
        {
            return await mycontext.Transactions
                .Where(t => t.ListingId == listingId)
                .Select(t => t.BuyerId)
                .ToListAsync();
        }

        public async Task<bool> IsServiceListing(int categoryId)
        {
            var category = await mycontext.Categories.FindAsync(categoryId);
            if (category?.ParentId == null)
            {
                return false;
            }

            return await mycontext.Categories
                .AnyAsync(p => p.Id == category.ParentId && p.Slug == CategorySlugs.Services);
        }



        #region Filtriranje oglasa


        public async Task<List<Listing>> GetFilteredListings(string? search = null,int? countryId = null,int? cityId = null,int? categoryId = null, int? subcategoryId = null)
        {
            var query = mycontext.Listings
                .Where(l => l.Status == "Active");

            if (!string.IsNullOrWhiteSpace(search))
            {
                var pattern = $"%{search.Trim()}%";
                query = query.Where(l => EF.Functions.ILike(l.Title, pattern)
                                      || EF.Functions.ILike(l.Description, pattern));
            }
            if (cityId != null)
            {
                query = query.Where(l => l.LocationId == cityId);
            }
            else if (countryId != null)
            {
                query = query.Where(l => l.Location.ParentId == countryId);
            }

            if (subcategoryId != null)
            {
                query = query.Where(l => l.CategoryId == subcategoryId);
            }
            else if (categoryId != null)
            {
                query = query.Where(l => l.Category.ParentId == categoryId);
            }
            return await query
                .Include(l => l.Category)
                .Include(l => l.Location)
                .Include(l => l.ListingImages)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }


        #endregion


        #region Report listings

        public async Task<bool> CreateReport(int listingId, int reporterId, string reason)
        {
            bool listingExists = await mycontext.Listings
                .AnyAsync(l => l.Id == listingId && l.UserId != reporterId);

            if (!listingExists)
            {
                return false;
            }

            bool alreadyReported = await mycontext.Reports
                .AnyAsync(r => r.ListingId == listingId && r.ReporterId == reporterId);

            if (alreadyReported)
            {
                return false;
            }

            var report = new Report
            {
                ListingId = listingId,
                ReporterId = reporterId,
                Reason = reason
            };

            mycontext.Reports.Add(report);
            await mycontext.SaveChangesAsync();

            return true;
        }

        #endregion

    }



}


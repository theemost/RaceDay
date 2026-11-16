using Microsoft.Data.SqlClient;
using RaceDay.Models;
using RaceDay.Data;
using Microsoft.EntityFrameworkCore;

namespace RaceDay.DbConnection
{
    public class CategoriesConnection
    {
        private readonly RaceDayContext _context;

        public CategoriesConnection(RaceDayContext context)
        {
            _context = context;
        }

        public List<Category> GetCategories()
        {
            try
            {
                List<Category> categories = new List<Category>();
                categories = (
                    from category in _context.Categories.AsNoTracking()
                    join _event in _context.Events.AsNoTracking()
                    on category.EventId equals _event.EventId
                    select new Category
                    {
                        CategoryId = category.CategoryId,
                        CategoryName = category.CategoryName,
                        EventId = category.EventId,
                        EventName = _event.EventName
                    }).ToList();
                return categories;
            }
            catch (SqlException error)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve the categories from the database.", error);
            }
        }

        // Get categories belonging to a specific event
        public List<Category> GetCategoriesByEventId(int eventId)
        {
            try
            {
                var categories = (
                    from category in _context.Categories.AsNoTracking()

                    join selectedEvent in _context.Events.AsNoTracking()
                        on category.EventId equals selectedEvent.EventId

                    where category.EventId == eventId

                    select new Category
                    {
                        CategoryId = category.CategoryId,
                        CategoryName = category.CategoryName,
                        EventId = category.EventId,
                        EventName = selectedEvent.EventName
                    }).ToList();

                return categories;
            }
            catch (Exception error)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve categories for this event.", error);
            }
        }
        public Category? GetCategory(int categoryId)
        {
            try
            {
                var category = (from _category in _context.Categories.AsNoTracking()
                                join _event in _context.Events.AsNoTracking()
                                on _category.EventId equals _event.EventId
                                where _category.CategoryId == categoryId
                                select new Category
                                {
                                    CategoryId = _category.CategoryId,
                                    CategoryName = _category.CategoryName,
                                    EventId = _category.EventId,
                                    EventName = _event.EventName
                                }).FirstOrDefault();
                return category;
            }
            catch (SqlException error)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve the category from the database.", error);
            }
        }

        public Category? AddCategory(string categoryName, int eventId, int organiserId)
        {
            try
            {
                var existingEvent = _context.Events
                    .FirstOrDefault(e => e.EventId == eventId);

                if (existingEvent == null ||
                    existingEvent.OrganiserId != organiserId)
                {
                    return null;
                }

                var newCategory = new Category
                {
                    CategoryName = categoryName,
                    EventId = eventId
                };

                _context.Categories.Add(newCategory);
                _context.SaveChanges();

                return GetCategory(newCategory.CategoryId);
            }
            catch (DbUpdateException error)
            {
                throw new InvalidOperationException(
                    "Unable to add the category.", error);
            }
        }

        public Category? EditCategory(
    int categoryId, string categoryName, int eventId, int organiserId)
        {
            try
            {
                var existingCategory = _context.Categories
                    .FirstOrDefault(c => c.CategoryId == categoryId);

                if (existingCategory == null)
                {
                    return null;
                }

                var existingEvent = _context.Events
                    .FirstOrDefault(e => e.EventId == existingCategory.EventId);

                if (existingEvent == null ||
                    existingEvent.OrganiserId != organiserId)
                {
                    return null;
                }

                var newEvent = _context.Events
                    .FirstOrDefault(e => e.EventId == eventId);

                if (newEvent == null || newEvent.OrganiserId != organiserId)
                {
                    return null;
                }

                existingCategory.CategoryName = categoryName;
                existingCategory.EventId = eventId;

                _context.SaveChanges();

                return GetCategory(categoryId);
            }
            catch (DbUpdateException error)
            {
                throw new InvalidOperationException(
                    "Unable to update the category.", error);
            }
        }

        public bool DeleteCategory(int categoryId, int organiserId)
        {
            try
            {
                var existingCategory = _context.Categories
                    .FirstOrDefault(c => c.CategoryId == categoryId);

                if (existingCategory == null)
                {
                    return false;
                }

                var existingEvent = _context.Events
                    .FirstOrDefault(e => e.EventId == existingCategory.EventId);

                if (existingEvent == null ||
                    existingEvent.OrganiserId != organiserId)
                {
                    return false;
                }

                _context.Categories.Remove(existingCategory);
                _context.SaveChanges();

                return true;
            }
            catch (DbUpdateException error)
            {
                throw new InvalidOperationException(
                    "Unable to delete the category because it is referenced by other records.",
                    error);
            }
        }
    }
}

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
    }
}

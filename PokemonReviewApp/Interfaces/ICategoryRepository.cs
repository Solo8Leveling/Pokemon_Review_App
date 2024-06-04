using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        Category GetCategory(int id);
        ICollection<Pokemon> GetPokemonByCategory(int categoryId);
        bool CategoryExists(int id);
        //asagidaki ikisini sonradan post&create etmek ucun yazdim
        bool CreateCategory(Category category);
        bool Save();
        //asagidakini update etmek ucun istifade edeceyik
        bool UpdateCategory(Category category);
        //sadece category delete etmek ucun bir method'du, isleyis terzini repository'de yaziriq
        bool DeleteCategory(Category category);
    }
}

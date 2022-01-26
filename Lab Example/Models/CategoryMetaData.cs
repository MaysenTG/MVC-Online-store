using System.ComponentModel.DataAnnotations;

namespace Lab_Example.Models
{
    [MetadataType(typeof(CategoryMetaData))]
    public partial class Category
    {
    }
    public class CategoryMetaData
    {
        [Display(Name = "Category Name")]
        public string Name;
    }
}
using FixDesk.Domain.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace FixDesk.Domain.Categories
{
    // Lookup: الاسم فقط، IsActive بدل الحذف، وUnique على الاسم (يُفرض في الـ Infrastructure).
    public class Category : AggregateRoot<CategoryId>
    {
        public string Name { get; private set; }
        public bool IsActive { get; private set; }

        [SetsRequiredMembers]
        private Category(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be null or empty.", nameof(name));

            Id = CategoryId.New();
            Name = name.Trim();
            IsActive = true;
        }

        public static Category Create(string name)
        {
            return new Category(name);
        }

        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be null or empty.", nameof(name));
            Name = name.Trim();
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}

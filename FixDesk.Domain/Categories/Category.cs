using FixDesk.Domain.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace FixDesk.Domain.Categories
{
    public class Category : AggregateRoot<CategoryId>
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; }

        [SetsRequiredMembers]
        private Category(string name, string description)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Category name cannot be null or empty.", nameof(name));
            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("Category description cannot be null or empty.", nameof(description));

            Id = CategoryId.New();
            Name = name;
            Description = description;
            IsActive = true;
        }

        public static Category Create(string name, string description)
        {
            return new Category(name, description);
        }

        public void Rename(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Category name cannot be null or empty.", nameof(name));
            Name = name;
        }

        public void UpdateDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("Category description cannot be null or empty.", nameof(description));
            Description = description;
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